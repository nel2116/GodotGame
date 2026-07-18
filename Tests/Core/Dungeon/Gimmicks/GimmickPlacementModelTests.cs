using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Godot;
using Systems.Dungeon.Data;
using Systems.Dungeon.Gimmicks;
using Systems.Dungeon.Models;

namespace Tests.Core.Dungeon.Gimmicks
{
    public class GimmickPlacementModelTests
    {
        /// <summary>
        /// 性質ベーステストで使用するシード数
        /// </summary>
        private const int SEED_COUNT = 10;

        /// <summary>
        /// 部屋 A・B を扉で接続した最小限の部屋辞書を作成する
        /// </summary>
        private static Dictionary<Vector2I, RoomData> CreateConnectedRoomPair(
            Vector2I positionA, RoomType typeA, Vector2I doorPositionA,
            Vector2I positionB, RoomType typeB, Vector2I doorPositionB)
        {
            var roomA = new RoomData { Position = positionA, Type = typeA };
            var roomB = new RoomData { Position = positionB, Type = typeB };

            roomA.AddDoor(new DoorData
            {
                Position = doorPositionA,
                Type = DoorType.Normal,
                IsLocked = false,
                ConnectedRoomPosition = positionB
            });
            roomB.AddDoor(new DoorData
            {
                Position = doorPositionB,
                Type = DoorType.Normal,
                IsLocked = false,
                ConnectedRoomPosition = positionA
            });

            return new Dictionary<Vector2I, RoomData> { [positionA] = roomA, [positionB] = roomB };
        }

        [Test]
        public void Constructor_NullRandom_ThrowsArgumentNullException()
        {
            // 乱数の注入が必須であること
            Assert.Throws<ArgumentNullException>(() => _ = new GimmickPlacementModel(null!));
        }

        [Test]
        public void PlaceGimmicks_NullRooms_ThrowsArgumentNullException()
        {
            var model = new GimmickPlacementModel(new Random(0));

            Assert.Throws<ArgumentNullException>(() => model.PlaceGimmicks(null!));
        }

        [Test]
        public void PlaceGimmicks_SecretRoomDoor_BecomesHiddenPassageOnBothSides()
        {
            var positionA = new Vector2I(0, 0);
            var positionB = new Vector2I(16, 0);
            var doorA = new Vector2I(15, 8);
            var doorB = new Vector2I(16, 8);
            var rooms = CreateConnectedRoomPair(positionA, RoomType.Combat, doorA, positionB, RoomType.Secret, doorB);

            new GimmickPlacementModel(new Random(0)).PlaceGimmicks(rooms);

            // 両側の扉が隠し扉（Secret）に変わり、施錠はされていないこと
            Assert.AreEqual(DoorType.Secret, rooms[positionA].Doors[0].Type);
            Assert.IsFalse(rooms[positionA].Doors[0].IsLocked);
            Assert.AreEqual(DoorType.Secret, rooms[positionB].Doors[0].Type);
            Assert.IsFalse(rooms[positionB].Doors[0].IsLocked);

            // 両側の部屋に未発動の隠し通路ギミックが追加されていること
            var gimmickA = rooms[positionA].Gimmicks.Single();
            Assert.AreEqual(GimmickType.HiddenPassage, gimmickA.Type);
            Assert.AreEqual(doorA, gimmickA.Position);
            Assert.IsFalse(gimmickA.IsActive);

            var gimmickB = rooms[positionB].Gimmicks.Single();
            Assert.AreEqual(GimmickType.HiddenPassage, gimmickB.Type);
            Assert.AreEqual(doorB, gimmickB.Position);
            Assert.IsFalse(gimmickB.IsActive);
        }

        [Test]
        public void PlaceGimmicks_TreasureDoorNotAdjacentToStart_BecomesLockedDoor()
        {
            // Start - Combat - Treasure の一直線構成（Combat-Treasure の扉のみが候補になる）
            var start = new Vector2I(0, 0);
            var combat = new Vector2I(16, 0);
            var treasure = new Vector2I(32, 0);

            var rooms = CreateConnectedRoomPair(start, RoomType.Start, new Vector2I(15, 8), combat, RoomType.Combat, new Vector2I(16, 8));
            var combatTreasureDoorCombat = new Vector2I(31, 8);
            var combatTreasureDoorTreasure = new Vector2I(32, 8);
            rooms[combat].AddDoor(new DoorData
            {
                Position = combatTreasureDoorCombat,
                Type = DoorType.Normal,
                IsLocked = false,
                ConnectedRoomPosition = treasure
            });
            rooms[treasure] = new RoomData { Position = treasure, Type = RoomType.Treasure };
            rooms[treasure].AddDoor(new DoorData
            {
                Position = combatTreasureDoorTreasure,
                Type = DoorType.Normal,
                IsLocked = false,
                ConnectedRoomPosition = combat
            });

            new GimmickPlacementModel(new Random(0)).PlaceGimmicks(rooms);

            // Start-Combat の扉は変更されないこと
            var startDoor = rooms[start].Doors.Single();
            Assert.AreEqual(DoorType.Normal, startDoor.Type);
            Assert.IsFalse(startDoor.IsLocked);
            Assert.IsEmpty(rooms[start].Gimmicks);

            // Combat-Treasure の扉が鍵扉になること
            var combatDoorToTreasure = rooms[combat].Doors.Single(d => d.ConnectedRoomPosition == treasure);
            Assert.AreEqual(DoorType.Locked, combatDoorToTreasure.Type);
            Assert.IsTrue(combatDoorToTreasure.IsLocked);

            var treasureDoor = rooms[treasure].Doors.Single();
            Assert.AreEqual(DoorType.Locked, treasureDoor.Type);
            Assert.IsTrue(treasureDoor.IsLocked);

            // 両側の部屋に未発動の鍵扉ギミックが追加されていること
            var combatGimmick = rooms[combat].Gimmicks.Single();
            Assert.AreEqual(GimmickType.LockedDoor, combatGimmick.Type);
            Assert.AreEqual(combatTreasureDoorCombat, combatGimmick.Position);
            Assert.IsFalse(combatGimmick.IsActive);

            var treasureGimmick = rooms[treasure].Gimmicks.Single();
            Assert.AreEqual(GimmickType.LockedDoor, treasureGimmick.Type);
            Assert.AreEqual(combatTreasureDoorTreasure, treasureGimmick.Position);
            Assert.IsFalse(treasureGimmick.IsActive);
        }

        [Test]
        public void PlaceGimmicks_TreasureAdjacentToStart_NoLockedDoorPlaced()
        {
            // Start に直接隣接する Treasure しか存在しない場合、候補が 0 件になり鍵扉は配置されない
            var start = new Vector2I(0, 0);
            var treasure = new Vector2I(16, 0);
            var rooms = CreateConnectedRoomPair(start, RoomType.Start, new Vector2I(15, 8), treasure, RoomType.Treasure, new Vector2I(16, 8));

            new GimmickPlacementModel(new Random(0)).PlaceGimmicks(rooms);

            Assert.AreEqual(DoorType.Normal, rooms[start].Doors[0].Type);
            Assert.AreEqual(DoorType.Normal, rooms[treasure].Doors[0].Type);
            Assert.IsEmpty(rooms[start].Gimmicks);
            Assert.IsEmpty(rooms[treasure].Gimmicks);
        }

        [Test]
        public async Task PlaceGimmicks_MultipleSeeds_AllSecretRoomDoorsBecomeHiddenPassage()
        {
            // 複数シードで生成した実際のレベルに対し、Secret 部屋の全扉が隠し通路化されること
            for (int seed = 0; seed < SEED_COUNT; seed++)
            {
                var levelModel = new LevelGenerationModel(seed);
                var rooms = await levelModel.GenerateLevelAsync();

                new GimmickPlacementModel(new Random(seed)).PlaceGimmicks(rooms);

                foreach (var room in rooms.Values.Where(r => r.Type == RoomType.Secret))
                {
                    foreach (var door in room.Doors)
                    {
                        Assert.AreEqual(DoorType.Secret, door.Type, $"シード {seed}: Secret 部屋の扉が隠し扉化されていない");
                        Assert.IsFalse(door.IsLocked, $"シード {seed}: 隠し扉が施錠されている");

                        var connectedRoom = rooms[door.ConnectedRoomPosition];
                        var connectedDoor = connectedRoom.Doors.Single(d => d.ConnectedRoomPosition == room.Position);
                        Assert.AreEqual(DoorType.Secret, connectedDoor.Type, $"シード {seed}: 接続先の扉が隠し扉化されていない");

                        Assert.IsTrue(room.Gimmicks.Any(g => g.Type == GimmickType.HiddenPassage && g.Position == door.Position && !g.IsActive),
                            $"シード {seed}: Secret 側に隠し通路ギミックがない");
                        Assert.IsTrue(connectedRoom.Gimmicks.Any(g => g.Type == GimmickType.HiddenPassage && g.Position == connectedDoor.Position && !g.IsActive),
                            $"シード {seed}: 接続先に隠し通路ギミックがない");
                    }
                }
            }
        }

        [Test]
        public async Task PlaceGimmicks_MultipleSeeds_LockedDoorInvariantsHold()
        {
            // 複数シードで、鍵扉は 0 個か対になる 2 個のみ存在し、Start に隣接せず Treasure/Boss に接続すること
            for (int seed = 0; seed < SEED_COUNT; seed++)
            {
                var levelModel = new LevelGenerationModel(seed);
                var rooms = await levelModel.GenerateLevelAsync();

                new GimmickPlacementModel(new Random(seed)).PlaceGimmicks(rooms);

                var lockedDoors = rooms.Values.SelectMany(r => r.Doors.Select(d => (Room: r, Door: d)))
                    .Where(x => x.Door.Type == DoorType.Locked)
                    .ToList();

                Assert.That(lockedDoors.Count, Is.EqualTo(0).Or.EqualTo(2), $"シード {seed}: 鍵扉の数が 0 または 2 でない");

                foreach (var (room, door) in lockedDoors)
                {
                    Assert.IsTrue(door.IsLocked, $"シード {seed}: 鍵扉が施錠されていない");
                    Assert.AreNotEqual(RoomType.Start, room.Type, $"シード {seed}: Start 部屋の扉が鍵扉化された");

                    var connectedRoom = rooms[door.ConnectedRoomPosition];
                    Assert.AreNotEqual(RoomType.Start, connectedRoom.Type, $"シード {seed}: Start に隣接する扉が鍵扉化された");
                    Assert.IsTrue(room.Type is RoomType.Treasure or RoomType.Boss || connectedRoom.Type is RoomType.Treasure or RoomType.Boss,
                        $"シード {seed}: Treasure/Boss に接続しない扉が鍵扉化された");

                    Assert.IsTrue(room.Gimmicks.Any(g => g.Type == GimmickType.LockedDoor && g.Position == door.Position && !g.IsActive),
                        $"シード {seed}: 鍵扉ギミックが追加されていない");
                }
            }
        }
    }
}
