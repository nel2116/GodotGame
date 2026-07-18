using NUnit.Framework;
using Godot;
using Systems.Dungeon.Data;

namespace Tests.Core.Dungeon
{
    public class RoomDataTests
    {
        [Test]
        public void InitialState_ListsAreEmptyAndNotNull()
        {
            var room = new RoomData();

            // 初期状態でリストが null でなく空であること
            Assert.IsNotNull(room.Doors);
            Assert.IsNotNull(room.Gimmicks);
            Assert.AreEqual(0, room.Doors.Count);
            Assert.AreEqual(0, room.Gimmicks.Count);
        }

        [Test]
        public void Properties_SetAndGet_ReturnsSameValues()
        {
            var room = new RoomData
            {
                Position = new Vector2I(16, -32),
                Size = new Vector2I(16, 16),
                Type = RoomType.Boss,
                IsGenerated = true
            };

            Assert.AreEqual(new Vector2I(16, -32), room.Position);
            Assert.AreEqual(new Vector2I(16, 16), room.Size);
            Assert.AreEqual(RoomType.Boss, room.Type);
            Assert.IsTrue(room.IsGenerated);
        }

        [Test]
        public void AddDoor_AddsToDoorsList()
        {
            var room = new RoomData();
            var door = new DoorData
            {
                Position = new Vector2I(8, 0),
                Type = DoorType.Locked,
                IsLocked = true,
                ConnectedRoomPosition = new Vector2I(0, -16)
            };

            room.AddDoor(door);

            Assert.AreEqual(1, room.Doors.Count);
            Assert.AreSame(door, room.Doors[0]);
        }

        [Test]
        public void AddGimmick_AddsToGimmicksList()
        {
            var room = new RoomData();
            var gimmick = new GimmickData
            {
                Position = new Vector2I(4, 4),
                Type = GimmickType.TreasureChest,
                IsActive = true
            };

            room.AddGimmick(gimmick);

            Assert.AreEqual(1, room.Gimmicks.Count);
            Assert.AreSame(gimmick, room.Gimmicks[0]);
        }

        [Test]
        public void AddDoor_MultipleTimes_KeepsAllDoors()
        {
            var room = new RoomData();

            // 複数の扉を追加しても全て保持されること
            room.AddDoor(new DoorData { Type = DoorType.Normal });
            room.AddDoor(new DoorData { Type = DoorType.Secret });

            Assert.AreEqual(2, room.Doors.Count);
        }
    }
}
