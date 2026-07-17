using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Systems.Dungeon.Data;
using Systems.Dungeon.Interfaces;
using Systems.Dungeon.Utilities;

namespace Systems.Dungeon.Models
{
    /// <summary>
    /// 部屋接続モデル
    /// 最小全域木（MST、Prim 法）で全部屋を接続し、接続する部屋の境界に対となる扉を配置する
    /// 辺の重みはユークリッド距離のため、グリッド隣接する部屋（距離 = ROOM_SIZE）が自然に優先される
    /// </summary>
    public class RoomConnectionModel : IRoomConnector
    {
        /// <summary>
        /// 距離の同値比較に使用する許容誤差
        /// </summary>
        private const float DISTANCE_EPSILON = 0.0001f;

        /// <summary>
        /// 同重みの辺のタイブレークに使用する乱数（テスト再現性のため注入する）
        /// </summary>
        private readonly Random random;

        /// <summary>
        /// 扉位置の計算に使用する経路探索器
        /// </summary>
        private readonly ConnectionPathFinder pathFinder = new();

        /// <summary>
        /// 直近の ConnectRooms で接続した部屋データ（FindPath の探索対象）
        /// </summary>
        private Dictionary<Vector2I, RoomData>? connectedRooms;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="random">接続処理に使用する乱数生成器</param>
        /// <exception cref="ArgumentNullException">random が null の場合</exception>
        public RoomConnectionModel(Random random)
        {
            this.random = random ?? throw new ArgumentNullException(nameof(random));
        }

        /// <summary>
        /// 部屋同士を接続する
        /// 最小全域木で全部屋が連結になる辺を選び、各辺の両側の部屋に対となる扉を追加する
        /// （部屋 A の扉の ConnectedRoomPosition は部屋 B、部屋 B の扉は部屋 A を指す）
        /// </summary>
        /// <param name="rooms">接続対象の部屋データの辞書（部屋位置がキー）</param>
        /// <exception cref="ArgumentNullException">rooms が null の場合</exception>
        public void ConnectRooms(Dictionary<Vector2I, RoomData> rooms)
        {
            if (rooms == null)
            {
                throw new ArgumentNullException(nameof(rooms));
            }

            connectedRooms = rooms;

            // 部屋が 1 個以下なら接続する辺は存在しない
            if (rooms.Count <= 1)
            {
                return;
            }

            foreach (var (roomA, roomB) in CalculateMinimumSpanningTree(rooms))
            {
                AddDoorPair(rooms[roomA], rooms[roomB]);
            }
        }

        /// <summary>
        /// 全部屋が到達可能に接続されているか検証する
        /// 扉の対称性（接続先の部屋が存在し、相手側にも自分を指す扉があること）と
        /// 全部屋の連結性（扉のグラフ上で任意の部屋から全部屋に到達できること）を確認する
        /// </summary>
        /// <param name="rooms">検証対象の部屋データの辞書</param>
        /// <returns>接続が妥当な場合は true（空の辞書は false、部屋 1 個は true）</returns>
        public bool ValidateConnections(Dictionary<Vector2I, RoomData> rooms)
        {
            if (rooms == null || rooms.Count == 0)
            {
                return false;
            }

            if (rooms.Count == 1)
            {
                return true;
            }

            // 扉の対称性を検証する
            foreach (var (position, room) in rooms)
            {
                foreach (var door in room.Doors)
                {
                    if (!rooms.TryGetValue(door.ConnectedRoomPosition, out var connectedRoom))
                    {
                        return false;
                    }

                    if (!connectedRoom.Doors.Any(d => d.ConnectedRoomPosition == position))
                    {
                        return false;
                    }
                }
            }

            // 全部屋の連結性を検証する
            var reachable = CollectReachableRooms(rooms, rooms.Keys.First());
            return reachable.Count == rooms.Count;
        }

        /// <summary>
        /// 開始位置から終了位置までの経路を、接続グラフ上の幅優先探索（BFS）で探索する
        /// 事前に ConnectRooms を呼び出しておく必要がある
        /// </summary>
        /// <param name="start">開始部屋の位置</param>
        /// <param name="end">終了部屋の位置</param>
        /// <returns>経路上の部屋位置のリスト（開始・終了を含む。経路が存在しない場合は空リスト）</returns>
        public List<Vector2I> FindPath(Vector2I start, Vector2I end)
        {
            if (connectedRooms == null ||
                !connectedRooms.ContainsKey(start) ||
                !connectedRooms.ContainsKey(end))
            {
                return new List<Vector2I>();
            }

            if (start == end)
            {
                return new List<Vector2I> { start };
            }

            // BFS で最短ホップの経路を探索する
            var previous = new Dictionary<Vector2I, Vector2I>();
            var visited = new HashSet<Vector2I> { start };
            var queue = new Queue<Vector2I>();
            queue.Enqueue(start);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                foreach (var door in connectedRooms[current].Doors)
                {
                    var next = door.ConnectedRoomPosition;
                    if (visited.Contains(next) || !connectedRooms.ContainsKey(next))
                    {
                        continue;
                    }

                    visited.Add(next);
                    previous[next] = current;

                    if (next == end)
                    {
                        return BuildPath(previous, start, end);
                    }

                    queue.Enqueue(next);
                }
            }

            return new List<Vector2I>();
        }

        /// <summary>
        /// 最小全域木（Prim 法）で全部屋を連結する辺の一覧を計算する
        /// 同一の最小重みを持つ候補辺は乱数でタイブレークする
        /// </summary>
        /// <param name="rooms">接続対象の部屋データの辞書</param>
        /// <returns>接続する部屋位置ペアのリスト</returns>
        private List<(Vector2I, Vector2I)> CalculateMinimumSpanningTree(Dictionary<Vector2I, RoomData> rooms)
        {
            // Dictionary の列挙順に依存しないよう固定順に並べる（シード再現性のため）
            var positions = rooms.Keys.OrderBy(p => p.X).ThenBy(p => p.Y).ToList();
            var visited = new HashSet<Vector2I> { positions[0] };
            var edges = new List<(Vector2I, Vector2I)>();

            while (visited.Count < positions.Count)
            {
                // 訪問済み集合と未訪問集合を結ぶ最小重みの辺候補を集める
                float bestDistance = float.MaxValue;
                var candidates = new List<(Vector2I From, Vector2I To)>();

                foreach (var from in positions.Where(visited.Contains))
                {
                    foreach (var to in positions.Where(p => !visited.Contains(p)))
                    {
                        float distance = DungeonUtils.CalculateDistance(from, to);
                        if (distance < bestDistance - DISTANCE_EPSILON)
                        {
                            bestDistance = distance;
                            candidates.Clear();
                            candidates.Add((from, to));
                        }
                        else if (Math.Abs(distance - bestDistance) <= DISTANCE_EPSILON)
                        {
                            candidates.Add((from, to));
                        }
                    }
                }

                var chosen = candidates[random.Next(candidates.Count)];
                visited.Add(chosen.To);
                edges.Add((chosen.From, chosen.To));
            }

            return edges;
        }

        /// <summary>
        /// 2 部屋の境界に対となる扉を追加する
        /// 各扉は自室の外周上に配置され、ConnectedRoomPosition は互いの部屋を指す
        /// </summary>
        /// <param name="roomA">接続する部屋 A</param>
        /// <param name="roomB">接続する部屋 B</param>
        private void AddDoorPair(RoomData roomA, RoomData roomB)
        {
            roomA.AddDoor(new DoorData
            {
                Position = pathFinder.FindDoorPosition(roomA.Position, roomB.Position),
                Type = DoorType.Normal,
                IsLocked = false,
                ConnectedRoomPosition = roomB.Position
            });

            roomB.AddDoor(new DoorData
            {
                Position = pathFinder.FindDoorPosition(roomB.Position, roomA.Position),
                Type = DoorType.Normal,
                IsLocked = false,
                ConnectedRoomPosition = roomA.Position
            });
        }

        /// <summary>
        /// 指定した部屋から扉のグラフを辿って到達可能な部屋の集合を収集する
        /// </summary>
        /// <param name="rooms">部屋データの辞書</param>
        /// <param name="start">探索の開始部屋の位置</param>
        /// <returns>到達可能な部屋位置の集合（開始部屋を含む）</returns>
        private static HashSet<Vector2I> CollectReachableRooms(Dictionary<Vector2I, RoomData> rooms, Vector2I start)
        {
            var reachable = new HashSet<Vector2I> { start };
            var queue = new Queue<Vector2I>();
            queue.Enqueue(start);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                foreach (var door in rooms[current].Doors)
                {
                    var next = door.ConnectedRoomPosition;
                    if (rooms.ContainsKey(next) && reachable.Add(next))
                    {
                        queue.Enqueue(next);
                    }
                }
            }

            return reachable;
        }

        /// <summary>
        /// BFS の親情報から開始位置 → 終了位置の経路リストを復元する
        /// </summary>
        /// <param name="previous">各部屋の直前の部屋を記録した辞書</param>
        /// <param name="start">開始部屋の位置</param>
        /// <param name="end">終了部屋の位置</param>
        /// <returns>開始位置から終了位置までの部屋位置のリスト</returns>
        private static List<Vector2I> BuildPath(Dictionary<Vector2I, Vector2I> previous, Vector2I start, Vector2I end)
        {
            var path = new List<Vector2I> { end };
            var current = end;

            while (current != start)
            {
                current = previous[current];
                path.Add(current);
            }

            path.Reverse();
            return path;
        }
    }
}
