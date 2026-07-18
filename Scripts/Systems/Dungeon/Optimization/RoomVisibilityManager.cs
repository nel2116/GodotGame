using System.Collections.Generic;
using Godot;
using Systems.Dungeon.Data;

namespace Systems.Dungeon.Optimization
{
    /// <summary>
    /// 部屋可視性管理
    /// 現在部屋を起点に、扉のグラフ上の幅優先探索（BFS）で一定ホップ数以内の部屋を「アクティブな部屋集合」として算出する
    /// 3Dカメラのフラスタムではなく部屋の接続グラフを基準にすることで、2Dタイル・部屋グラフ構造の本システムに合致させる
    /// （<see cref="Models.RoomConnectionModel"/> の到達可能部屋探索と同じBFSパターンを踏襲）
    /// </summary>
    public class RoomVisibilityManager
    {
        /// <summary>
        /// 現在部屋から radius ホップ以内の部屋位置の集合を求める（現在部屋自身を含む）
        /// </summary>
        /// <param name="currentRoom">現在の部屋の位置</param>
        /// <param name="rooms">部屋データの辞書（部屋位置がキー）</param>
        /// <param name="radius">現在部屋から辿るホップ数（0以下の場合は現在部屋のみを返す）</param>
        /// <returns>アクティブな部屋位置の集合。currentRoom が rooms に存在しない場合は空集合</returns>
        public HashSet<Vector2I> GetActiveRooms(Vector2I currentRoom, Dictionary<Vector2I, RoomData> rooms, int radius = 1)
        {
            var active = new HashSet<Vector2I>();
            if (!rooms.ContainsKey(currentRoom))
            {
                return active;
            }

            active.Add(currentRoom);
            if (radius <= 0)
            {
                return active;
            }

            var queue = new Queue<(Vector2I Position, int Depth)>();
            queue.Enqueue((currentRoom, 0));

            while (queue.Count > 0)
            {
                var (position, depth) = queue.Dequeue();
                if (depth >= radius)
                {
                    continue;
                }

                foreach (var door in rooms[position].Doors)
                {
                    var next = door.ConnectedRoomPosition;
                    if (rooms.ContainsKey(next) && active.Add(next))
                    {
                        queue.Enqueue((next, depth + 1));
                    }
                }
            }

            return active;
        }
    }
}
