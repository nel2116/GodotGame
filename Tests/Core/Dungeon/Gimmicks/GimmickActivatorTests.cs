using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Godot;
using Systems.Dungeon.Data;
using Systems.Dungeon.Gimmicks;

namespace Tests.Core.Dungeon.Gimmicks
{
    public class GimmickActivatorTests
    {
        private static readonly Vector2I RoomAPosition = new(0, 0);
        private static readonly Vector2I RoomBPosition = new(16, 0);
        private static readonly Vector2I DoorAPosition = new(15, 8);
        private static readonly Vector2I DoorBPosition = new(16, 8);

        /// <summary>
        /// 隠し通路ギミックが両側に配置された部屋ペアを作成する
        /// </summary>
        private static Dictionary<Vector2I, RoomData> CreateRoomsWithHiddenPassage()
        {
            var roomA = new RoomData { Position = RoomAPosition, Type = RoomType.Combat };
            var roomB = new RoomData { Position = RoomBPosition, Type = RoomType.Secret };

            roomA.AddDoor(new DoorData { Position = DoorAPosition, Type = DoorType.Secret, IsLocked = false, ConnectedRoomPosition = RoomBPosition });
            roomB.AddDoor(new DoorData { Position = DoorBPosition, Type = DoorType.Secret, IsLocked = false, ConnectedRoomPosition = RoomAPosition });

            roomA.AddGimmick(new GimmickData { Position = DoorAPosition, Type = GimmickType.HiddenPassage, IsActive = false });
            roomB.AddGimmick(new GimmickData { Position = DoorBPosition, Type = GimmickType.HiddenPassage, IsActive = false });

            return new Dictionary<Vector2I, RoomData> { [RoomAPosition] = roomA, [RoomBPosition] = roomB };
        }

        /// <summary>
        /// 鍵扉ギミックが両側に配置された部屋ペアを作成する
        /// </summary>
        private static Dictionary<Vector2I, RoomData> CreateRoomsWithLockedDoor()
        {
            var roomA = new RoomData { Position = RoomAPosition, Type = RoomType.Combat };
            var roomB = new RoomData { Position = RoomBPosition, Type = RoomType.Treasure };

            roomA.AddDoor(new DoorData { Position = DoorAPosition, Type = DoorType.Locked, IsLocked = true, ConnectedRoomPosition = RoomBPosition });
            roomB.AddDoor(new DoorData { Position = DoorBPosition, Type = DoorType.Locked, IsLocked = true, ConnectedRoomPosition = RoomAPosition });

            roomA.AddGimmick(new GimmickData { Position = DoorAPosition, Type = GimmickType.LockedDoor, IsActive = false });
            roomB.AddGimmick(new GimmickData { Position = DoorBPosition, Type = GimmickType.LockedDoor, IsActive = false });

            return new Dictionary<Vector2I, RoomData> { [RoomAPosition] = roomA, [RoomBPosition] = roomB };
        }

        [Test]
        public void TryActivateHiddenPassage_ValidGimmick_ActivatesBothSidesAndOpensDoors()
        {
            var rooms = CreateRoomsWithHiddenPassage();
            var activator = new GimmickActivator();

            bool result = activator.TryActivateHiddenPassage(rooms, RoomAPosition, DoorAPosition);

            Assert.IsTrue(result);

            // 発動した側・接続先の両方のギミックが有効化されること
            Assert.IsTrue(rooms[RoomAPosition].Gimmicks.Single().IsActive);
            Assert.IsTrue(rooms[RoomBPosition].Gimmicks.Single().IsActive);

            // 発動した側・接続先の両方の扉が通常の扉に変わること
            Assert.AreEqual(DoorType.Normal, rooms[RoomAPosition].Doors.Single().Type);
            Assert.AreEqual(DoorType.Normal, rooms[RoomBPosition].Doors.Single().Type);
        }

        [Test]
        public void TryActivateHiddenPassage_AlreadyActive_ReturnsFalseAndDoesNotChangeState()
        {
            var rooms = CreateRoomsWithHiddenPassage();
            var activator = new GimmickActivator();
            Assert.IsTrue(activator.TryActivateHiddenPassage(rooms, RoomAPosition, DoorAPosition));

            // 二重発動は失敗すること
            bool result = activator.TryActivateHiddenPassage(rooms, RoomAPosition, DoorAPosition);

            Assert.IsFalse(result);
        }

        [Test]
        public void TryActivateHiddenPassage_GimmickNotFound_ReturnsFalse()
        {
            var rooms = CreateRoomsWithHiddenPassage();
            var activator = new GimmickActivator();

            // 存在しない位置を指定した場合は失敗すること
            bool result = activator.TryActivateHiddenPassage(rooms, RoomAPosition, new Vector2I(99, 99));

            Assert.IsFalse(result);
        }

        [Test]
        public void TryActivateHiddenPassage_WrongGimmickType_ReturnsFalse()
        {
            var rooms = CreateRoomsWithLockedDoor();
            var activator = new GimmickActivator();

            // 鍵扉ギミックを隠し通路として発動しようとすると失敗すること
            bool result = activator.TryActivateHiddenPassage(rooms, RoomAPosition, DoorAPosition);

            Assert.IsFalse(result);
        }

        [Test]
        public void TryActivateLockedDoor_NoKey_ReturnsFalse()
        {
            var rooms = CreateRoomsWithLockedDoor();
            var activator = new GimmickActivator();

            // 鍵を所持していない場合は常に失敗すること
            bool result = activator.TryActivateLockedDoor(rooms, RoomAPosition, DoorAPosition, hasKey: false);

            Assert.IsFalse(result);
            Assert.IsFalse(rooms[RoomAPosition].Gimmicks.Single().IsActive);
            Assert.IsTrue(rooms[RoomAPosition].Doors.Single().IsLocked);
        }

        [Test]
        public void TryActivateLockedDoor_HasKey_ActivatesBothSidesAndUnlocksDoors()
        {
            var rooms = CreateRoomsWithLockedDoor();
            var activator = new GimmickActivator();

            bool result = activator.TryActivateLockedDoor(rooms, RoomAPosition, DoorAPosition, hasKey: true);

            Assert.IsTrue(result);

            // 発動した側・接続先の両方のギミックが有効化されること
            Assert.IsTrue(rooms[RoomAPosition].Gimmicks.Single().IsActive);
            Assert.IsTrue(rooms[RoomBPosition].Gimmicks.Single().IsActive);

            // 両側の扉が解錠されること（種類は Locked のまま据え置き）
            Assert.IsFalse(rooms[RoomAPosition].Doors.Single().IsLocked);
            Assert.IsFalse(rooms[RoomBPosition].Doors.Single().IsLocked);
            Assert.AreEqual(DoorType.Locked, rooms[RoomAPosition].Doors.Single().Type);
            Assert.AreEqual(DoorType.Locked, rooms[RoomBPosition].Doors.Single().Type);
        }

        [Test]
        public void TryActivateLockedDoor_AlreadyActive_ReturnsFalse()
        {
            var rooms = CreateRoomsWithLockedDoor();
            var activator = new GimmickActivator();
            Assert.IsTrue(activator.TryActivateLockedDoor(rooms, RoomAPosition, DoorAPosition, hasKey: true));

            // 二重発動は失敗すること
            bool result = activator.TryActivateLockedDoor(rooms, RoomAPosition, DoorAPosition, hasKey: true);

            Assert.IsFalse(result);
        }

        [Test]
        public void TryActivateLockedDoor_GimmickNotFound_ReturnsFalse()
        {
            var rooms = CreateRoomsWithLockedDoor();
            var activator = new GimmickActivator();

            // 存在しない位置を指定した場合は失敗すること
            bool result = activator.TryActivateLockedDoor(rooms, RoomAPosition, new Vector2I(99, 99), hasKey: true);

            Assert.IsFalse(result);
        }
    }
}
