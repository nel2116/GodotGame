using System;
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

            Assert.AreEqual(RoomType.Treasure, viewModel.Type.Value);
            Assert.IsFalse(viewModel.IsVisited.Value);
            Assert.AreEqual(1, viewModel.Doors.Count);
            Assert.AreEqual(1, viewModel.Gimmicks.Count);
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
        public void Doors_ReflectsLatestRoomDataState()
        {
            var bus = new GameEventBus();
            var room = CreateRoom();
            var viewModel = new RoomViewModel(room, bus);

            // RoomData 側の扉状態が書き換わった場合、ViewModel 経由でも最新状態が見えること
            room.Doors[0].IsLocked = false;

            Assert.IsFalse(viewModel.Doors[0].IsLocked);
        }
    }
}
