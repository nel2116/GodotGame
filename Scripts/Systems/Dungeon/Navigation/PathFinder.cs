using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Systems.Dungeon.Navigation
{
    /// <summary>
    /// 経路探索器
    /// <see cref="NavigationMesh"/> 上で A* アルゴリズムによる最短経路探索を行う
    /// ヒューリスティックはマンハッタン距離、1 タイルの移動コストは 1 とする
    /// </summary>
    public class PathFinder
    {
        /// <summary>
        /// 探索対象のナビゲーションメッシュ
        /// </summary>
        private readonly NavigationMesh navigationMesh;

        /// <summary>
        /// 1 タイル移動あたりのコスト
        /// </summary>
        private const float MOVE_COST = 1f;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="navigationMesh">経路探索に使用するナビゲーションメッシュ</param>
        /// <exception cref="ArgumentNullException">navigationMesh が null の場合</exception>
        public PathFinder(NavigationMesh navigationMesh)
        {
            this.navigationMesh = navigationMesh ?? throw new ArgumentNullException(nameof(navigationMesh));
        }

        /// <summary>
        /// 開始地点から目標地点までの最短経路を A* アルゴリズムで探索する
        /// </summary>
        /// <param name="start">開始地点のワールドタイル座標</param>
        /// <param name="goal">目標地点のワールドタイル座標</param>
        /// <returns>
        /// 開始地点から目標地点までの経路（両端を含み、開始 → 目標の順）
        /// 開始・目標のいずれかが通行不可、または経路が存在しない場合は空リスト
        /// </returns>
        public List<Vector2I> FindPath(Vector2I start, Vector2I goal)
        {
            if (!navigationMesh.IsWalkable(start) || !navigationMesh.IsWalkable(goal))
            {
                return new List<Vector2I>();
            }

            if (start == goal)
            {
                return new List<Vector2I> { start };
            }

            var openNodes = new Dictionary<Vector2I, NavigationNode>();
            var closedPositions = new HashSet<Vector2I>();

            var startNode = new NavigationNode
            {
                Position = start,
                GCost = 0f,
                HCost = CalculateManhattanDistance(start, goal),
                Parent = null
            };
            openNodes[start] = startNode;

            while (openNodes.Count > 0)
            {
                var current = SelectLowestFCostNode(openNodes);

                if (current.Position == goal)
                {
                    return BuildPath(current);
                }

                openNodes.Remove(current.Position);
                closedPositions.Add(current.Position);

                foreach (var neighborPosition in navigationMesh.GetWalkableNeighbors(current.Position))
                {
                    if (closedPositions.Contains(neighborPosition))
                    {
                        continue;
                    }

                    float tentativeGCost = current.GCost + MOVE_COST;

                    if (openNodes.TryGetValue(neighborPosition, out var existingNode))
                    {
                        if (tentativeGCost < existingNode.GCost)
                        {
                            existingNode.GCost = tentativeGCost;
                            existingNode.Parent = current;
                        }
                    }
                    else
                    {
                        openNodes[neighborPosition] = new NavigationNode
                        {
                            Position = neighborPosition,
                            GCost = tentativeGCost,
                            HCost = CalculateManhattanDistance(neighborPosition, goal),
                            Parent = current
                        };
                    }
                }
            }

            return new List<Vector2I>();
        }

        /// <summary>
        /// オープンリストから FCost が最小のノードを選ぶ（同値の場合は HCost が小さい方を優先する）
        /// </summary>
        /// <param name="openNodes">探索中のオープンリスト（座標をキーとする）</param>
        /// <returns>FCost が最小のノード</returns>
        private static NavigationNode SelectLowestFCostNode(Dictionary<Vector2I, NavigationNode> openNodes)
        {
            return openNodes.Values
                .OrderBy(node => node.FCost)
                .ThenBy(node => node.HCost)
                .First();
        }

        /// <summary>
        /// 2 点間のマンハッタン距離を計算する
        /// </summary>
        /// <param name="from">開始座標</param>
        /// <param name="to">目標座標</param>
        /// <returns>マンハッタン距離</returns>
        private static float CalculateManhattanDistance(Vector2I from, Vector2I to)
        {
            return Math.Abs(from.X - to.X) + Math.Abs(from.Y - to.Y);
        }

        /// <summary>
        /// 目標ノードから Parent を辿って開始 → 目標の順の経路リストを復元する
        /// </summary>
        /// <param name="goalNode">目標地点に到達したノード</param>
        /// <returns>開始地点から目標地点までの座標のリスト</returns>
        private static List<Vector2I> BuildPath(NavigationNode goalNode)
        {
            var path = new List<Vector2I>();
            NavigationNode? current = goalNode;

            while (current != null)
            {
                path.Add(current.Position);
                current = current.Parent;
            }

            path.Reverse();
            return path;
        }
    }
}
