using System;
using System.Collections.Generic;
using NUnit.Framework;
using Godot;
using Core.Events;
using Systems.Dungeon.Data;
using Systems.Dungeon.ViewModels;

namespace Tests.Core.Dungeon.ViewModels
{
    public class RoomViewModelTests
    {
        private static RoomData CreateRoom()
        {
            var room = new RoomData { Position = Vector2I.Zero, Type = RoomType.Treasure };
            room.AddDoor(new DoorData { Position = new Vector2I(1, 1), Type = DoorType.Locked, IsLocked = true, ConnectedRoomPosition = new Vector2I(16, 0) });
            room.AddGimmick(new GimmickData { Position = new Vector2I(1, 1), Type = GimmickType.LockedDoor, IsActive = false });
            return room;
        }

        [Test]
        public void Constructor_NullRoom_ThrowsArgumentNullException()
        {
            var bus = new GameEventBus();
            Assert.Throws<ArgumentNullException>(() => _ = new RoomViewModel(null!, bus));
        }

        [Test]
        public void Constructor_InitialState_ReflectsRoomData()
        {
            var bus = new GameEventBus();
            var room = CreateRoom();

            var viewModel = new RoomViewModel(room, bus);

            Assert.AreEqual(RoomType.Treasure, viewModel.Type);
            Assert.IsFalse(viewModel.IsVisited.Value);
            Assert.AreEqual(1, viewModel.Doors.Value.Count);
            Assert.AreEqual(1, viewModel.Gimmicks.Value.Count);
        }

        [Test]
        public void MarkVisited_SetsIsVisitedTrue()
        {
            var bus = new GameEventBus();
            var viewModel = new RoomViewModel(CreateRoom(), bus);

            viewModel.MarkVisited();

            Assert.IsTrue(viewModel.IsVisited.Value);
        }

        [Test]
        public void Refresh_AfterRoomDataChanges_NotifiesSubscribersWithUpdatedSnapshot()
        {
            var bus = new GameEventBus();
            var room = CreateRoom();
            var viewModel = new RoomViewModel(room, bus);
            IReadOnlyList<DoorData>? notifiedDoors = null;
            viewModel.Doors.Subscribe(doors => notifiedDoors = doors);

            // RoomData.Doors の要素は同一インスタンス参照のため、Refresh を呼ぶ前でも
            // ReactiveProperty のスナップショット経由で最新のフィールド値自体は見える
            room.Doors[0].IsLocked = false;
            Assert.IsFalse(viewModel.Doors.Value[0].IsLocked);

            // Refresh を呼び出すことで初めて、購読者への変更通知（Subscribe）が発火すること
            Assert.IsNull(notifiedDoors);
            viewModel.Refresh();

            Assert.IsNotNull(notifiedDoors);
            Assert.IsFalse(notifiedDoors![0].IsLocked);
        }
    }
}
