using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Systems.Dungeon.Data;

namespace Systems.Dungeon.Gimmicks
{
    /// <summary>
    /// ギミック配置モデル
    /// 生成・接続済みの部屋データに対し、隠し通路（HiddenPassage）と鍵扉（LockedDoor）ギミックを配置する
    /// </summary>
    public class GimmickPlacementModel
    {
        /// <summary>
        /// 鍵扉候補の選定・決定に使用する乱数（テスト再現性のため注入する）
        /// </summary>
        private readonly Random random;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="random">ギミック配置処理に使用する乱数生成器</param>
        /// <exception cref="ArgumentNullException">random が null の場合</exception>
        public GimmickPlacementModel(Random random)
        {
            this.random = random ?? throw new ArgumentNullException(nameof(random));
        }

        /// <summary>
        /// 部屋データにギミックを配置する
        /// 隠し部屋（Secret）に接続する扉を隠し通路化し、続けて宝物部屋・ボス部屋（Treasure/Boss）に接続する扉のうち
        /// 開始部屋（Start）に隣接せず隠し扉化されていないものから 1 つを選んで鍵扉化する
        /// </summary>
        /// <param name="rooms">配置対象の部屋データの辞書（部屋位置がキー、接続済みであること）</param>
        /// <exception cref="ArgumentNullException">rooms が null の場合</exception>
        public void PlaceGimmicks(Dictionary<Vector2I, RoomData> rooms)
        {
            if (rooms == null)
            {
                throw new ArgumentNullException(nameof(rooms));
            }

            PlaceHiddenPassages(rooms);
            PlaceLockedDoor(rooms);
        }

        /// <summary>
        /// 隠し部屋（Secret）に接続する扉をすべて隠し扉化し、両側の部屋に隠し通路ギミックを追加する
        /// </summary>
        /// <param name="rooms">配置対象の部屋データの辞書</param>
        private static void PlaceHiddenPassages(Dictionary<Vector2I, RoomData> rooms)
        {
            // 固定順（座標順）で処理し、シード再現性を確保する
            foreach (var position in rooms.Keys.OrderBy(p => p.X).ThenBy(p => p.Y))
            {
                var room = rooms[position];
                if (room.Type != RoomType.Secret)
                {
                    continue;
                }

                foreach (var door in room.Doors.OrderBy(d => d.Position.X).ThenBy(d => d.Position.Y))
                {
                    if (!rooms.TryGetValue(door.ConnectedRoomPosition, out var connectedRoom))
                    {
                        continue;
                    }

                    var connectedDoor = connectedRoom.GetDoorTo(position);
                    if (connectedDoor == null)
                    {
                        continue;
                    }

                    door.Type = DoorType.Secret;
                    door.IsLocked = false;
                    connectedDoor.Type = DoorType.Secret;
                    connectedDoor.IsLocked = false;

                    room.AddGimmick(new GimmickData
                    {
                        Position = door.Position,
                        Type = GimmickType.HiddenPassage,
                        IsActive = false
                    });

                    connectedRoom.AddGimmick(new GimmickData
                    {
                        Position = connectedDoor.Position,
                        Type = GimmickType.HiddenPassage,
                        IsActive = false
                    });
                }
            }
        }

        /// <summary>
        /// 宝物部屋・ボス部屋（Treasure/Boss）に接続する扉から候補を集め、乱数で 1 つ選んで鍵扉化する
        /// 開始部屋（Start）に隣接する扉、および既に隠し扉化された扉は候補から除外する
        /// 候補が存在しない場合は何もしない
        /// </summary>
        /// <param name="rooms">配置対象の部屋データの辞書</param>
        private void PlaceLockedDoor(Dictionary<Vector2I, RoomData> rooms)
        {
            var candidates = CollectLockedDoorCandidates(rooms);
            if (candidates.Count == 0)
            {
                return;
            }

            var (room, door, connectedRoom, connectedDoor) = candidates[random.Next(candidates.Count)];

            door.Type = DoorType.Locked;
            door.IsLocked = true;
            connectedDoor.Type = DoorType.Locked;
            connectedDoor.IsLocked = true;

            room.AddGimmick(new GimmickData
            {
                Position = door.Position,
                Type = GimmickType.LockedDoor,
                IsActive = false
            });

            connectedRoom.AddGimmick(new GimmickData
            {
                Position = connectedDoor.Position,
                Type = GimmickType.LockedDoor,
                IsActive = false
            });
        }

        /// <summary>
        /// 鍵扉候補となる扉ペア（両側の部屋・扉）の一覧を座標順で固定して収集する
        /// 各辺（部屋間の接続）は 1 度だけ候補に含める
        /// </summary>
        /// <param name="rooms">探索対象の部屋データの辞書</param>
        /// <returns>候補となる (部屋, 扉, 接続先部屋, 接続先扉) のタプルの一覧（座標順）</returns>
        private static List<(RoomData Room, DoorData Door, RoomData ConnectedRoom, DoorData ConnectedDoor)> CollectLockedDoorCandidates(
            Dictionary<Vector2I, RoomData> rooms)
        {
            var candidates = new List<(RoomData, DoorData, RoomData, DoorData)>();

            // 固定順（座標順）で処理し、シード再現性を確保する
            foreach (var position in rooms.Keys.OrderBy(p => p.X).ThenBy(p => p.Y))
            {
                var room = rooms[position];

                foreach (var door in room.Doors.OrderBy(d => d.Position.X).ThenBy(d => d.Position.Y))
                {
                    if (door.Type == DoorType.Secret)
                    {
                        continue;
                    }

                    if (!rooms.TryGetValue(door.ConnectedRoomPosition, out var connectedRoom))
                    {
                        continue;
                    }

                    // 各辺を 1 度だけ候補にするため、座標順で自分が先（小さい側）の場合のみ処理する
                    if (!IsBefore(position, door.ConnectedRoomPosition))
                    {
                        continue;
                    }

                    if (room.Type == RoomType.Start || connectedRoom.Type == RoomType.Start)
                    {
                        continue;
                    }

                    bool leadsToTreasureOrBoss =
                        room.Type == RoomType.Treasure || room.Type == RoomType.Boss ||
                        connectedRoom.Type == RoomType.Treasure || connectedRoom.Type == RoomType.Boss;
                    if (!leadsToTreasureOrBoss)
                    {
                        continue;
                    }

                    var connectedDoor = connectedRoom.GetDoorTo(position);
                    if (connectedDoor == null || connectedDoor.Type == DoorType.Secret)
                    {
                        continue;
                    }

                    candidates.Add((room, door, connectedRoom, connectedDoor));
                }
            }

            return candidates;
        }

        /// <summary>
        /// 座標順（X 優先、次に Y）で a が b より前かどうかを判定する
        /// </summary>
        /// <param name="a">比較対象の位置 A</param>
        /// <param name="b">比較対象の位置 B</param>
        /// <returns>a が b より座標順で前であれば true</returns>
        private static bool IsBefore(Vector2I a, Vector2I b)
        {
            return a.X != b.X ? a.X < b.X : a.Y < b.Y;
        }
    }
}
