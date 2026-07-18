using System.Collections.Generic;
using NUnit.Framework;
using Godot;
using Systems.Dungeon.Data;
using Systems.Dungeon.Optimization;

namespace Tests.Core.Dungeon.Optimization
{
    public class RoomVisibilityManagerTests
    {
        private static readonly Vector2I PositionA = new(0, 0);
        private static readonly Vector2I PositionB = new(16, 0);
        private static readonly Vector2I PositionC = new(32, 0);
        private static readonly Vector2I PositionD = new(0, 16);

        /// <summary>
        /// A-B-C が直列に接続され、D はどの部屋とも接続されていない部屋集合を作成する
        /// </summary>
        private static Dictionary<Vector2I, RoomData> CreateChainWithIsolatedRoom()
        {
            var roomA = new RoomData { Position = PositionA, Type = RoomType.Combat };
            var roomB = new RoomData { Position = PositionB, Type = RoomType.Combat };
            var roomC = new RoomData { Position = PositionC, Type = RoomType.Combat };
            var roomD = new RoomData { Position = PositionD, Type = RoomType.Combat };

            roomA.AddDoor(new DoorData { Position = PositionA, ConnectedRoomPosition = PositionB, Type = DoorType.Normal });
            roomB.AddDoor(new DoorData { Position = PositionB, ConnectedRoomPosition = PositionA, Type = DoorType.Normal });
            roomB.AddDoor(new DoorData { Position = PositionB, ConnectedRoomPosition = PositionC, Type = DoorType.Normal });
            roomC.AddDoor(new DoorData { Position = PositionC, ConnectedRoomPosition = PositionB, Type = DoorType.Normal });

            return new Dictionary<Vector2I, RoomData>
            {
                [PositionA] = roomA,
                [PositionB] = roomB,
                [PositionC] = roomC,
                [PositionD] = roomD
            };
        }

        [Test]
        public void GetActiveRooms_Radius1_ReturnsCurrentAndDirectNeighborsOnly()
        {
            var rooms = CreateChainWithIsolatedRoom();
            var manager = new RoomVisibilityManager();

            var active = manager.GetActiveRooms(PositionB, rooms, radius: 1);

            Assert.That(active, Is.EquivalentTo(new[] { PositionA, PositionB, PositionC }));
        }

        [Test]
        public void GetActiveRooms_Radius0_ReturnsOnlyCurrentRoom()
        {
            var rooms = CreateChainWithIsolatedRoom();
            var manager = new RoomVisibilityManager();

            var active = manager.GetActiveRooms(PositionB, rooms, radius: 0);

            Assert.That(active, Is.EquivalentTo(new[] { PositionB }));
        }

        [Test]
        public void GetActiveRooms_UnknownCurrentRoom_ReturnsEmptySet()
        {
            var rooms = CreateChainWithIsolatedRoom();
            var manager = new RoomVisibilityManager();

            var active = manager.GetActiveRooms(new Vector2I(999, 999), rooms, radius: 1);

            Assert.IsEmpty(active);
        }

        [Test]
        public void GetActiveRooms_DisconnectedRoom_IsNeverIncludedEvenWithLargeRadius()
        {
            var rooms = CreateChainWithIsolatedRoom();
            var manager = new RoomVisibilityManager();

            var active = manager.GetActiveRooms(PositionA, rooms, radius: 10);

            Assert.That(active, Does.Not.Contain(PositionD));
        }

        [Test]
        public void GetActiveRooms_RadiusCoversFullChain_ReturnsAllConnectedRooms()
        {
            var rooms = CreateChainWithIsolatedRoom();
            var manager = new RoomVisibilityManager();

            var active = manager.GetActiveRooms(PositionA, rooms, radius: 2);

            Assert.That(active, Is.EquivalentTo(new[] { PositionA, PositionB, PositionC }));
        }
    }
}
