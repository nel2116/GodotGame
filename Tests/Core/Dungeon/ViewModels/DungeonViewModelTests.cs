using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using Godot;
using Core.Events;
using Systems.Dungeon.Data;
using Systems.Dungeon.Events;
using Systems.Dungeon.Gimmicks;
using Systems.Dungeon.Models;
using Systems.Dungeon.Navigation;
using Systems.Dungeon.ViewModels;

namespace Tests.Core.Dungeon.ViewModels
{
    public class DungeonViewModelTests
    {
        private static readonly Vector2I RoomAPosition = new(0, 0);
        private static readonly Vector2I RoomBPosition = new(16, 0);
        private static readonly Vector2I DoorAPosition = new(15, 8);
        private static readonly Vector2I DoorBPosition = new(16, 8);

        /// <summary>
        /// テスト対象の DungeonViewModel を、実体のモデル群を注入して生成する
        /// </summary>
        private static DungeonViewModel CreateViewModel(GameEventBus bus)
        {
            return new DungeonViewModel(
                new LevelGenerationModel(0),
                new GimmickPlacementModel(new Random(0)),
                new GimmickActivator(),
                new NavigationManager(),
                bus);
        }

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
        public void Constructor_NullLevelGenerationModel_ThrowsArgumentNullException()
        {
            var bus = new GameEventBus();
            Assert.Throws<ArgumentNullException>(() => _ = new DungeonViewModel(
                null!, new GimmickPlacementModel(new Random(0)), new GimmickActivator(), new NavigationManager(), bus));
        }

        [Test]
        public void Constructor_NullEventBus_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new DungeonViewModel(
                new LevelGenerationModel(0), new GimmickPlacementModel(new Random(0)), new GimmickActivator(), new NavigationManager(), null!));
        }

        [Test]
        public async Task GenerateLevelAsync_ValidSeed_UpdatesRoomsAndCurrentRoomPositionAndPublishesEvent()
        {
            var bus = new GameEventBus();
            var viewModel = CreateViewModel(bus);
            LevelGeneratedEvent? received = null;
            bus.GetEventStream<LevelGeneratedEvent>().Subscribe(e => received = e);

            await viewModel.GenerateLevelAsync(42);

            Assert.That(viewModel.Rooms.Value, Is.Not.Empty);
            Assert.That(viewModel.CurrentRoomPosition.Value, Is.EqualTo(Vector2I.Zero));
            Assert.That(viewModel.Rooms.Value.ContainsKey(Vector2I.Zero), Is.True);
            Assert.That(received, Is.Not.Null);
            Assert.That(received!.RoomCount, Is.EqualTo(viewModel.Rooms.Value.Count));
        }

        [Test]
        public void EnterRoom_ExistingRoom_ReturnsTrueUpdatesCurrentRoomAndPublishesEvent()
        {
            var bus = new GameEventBus();
            var viewModel = CreateViewModel(bus);
            viewModel.Rooms.Value = CreateRoomsWithHiddenPassage();
            RoomEnteredEvent? received = null;
            bus.GetEventStream<RoomEnteredEvent>().Subscribe(e => received = e);

            bool result = viewModel.EnterRoom(RoomBPosition);

            Assert.IsTrue(result);
            Assert.AreEqual(RoomBPosition, viewModel.CurrentRoomPosition.Value);
            Assert.That(received, Is.Not.Null);
            Assert.AreEqual(RoomBPosition, received!.RoomPosition);
            Assert.AreEqual(RoomType.Secret, received!.RoomType);
        }

        [Test]
        public void EnterRoom_NonExistingRoom_ReturnsFalseAndDoesNotPublishEvent()
        {
            var bus = new GameEventBus();
            var viewModel = CreateViewModel(bus);
            viewModel.Rooms.Value = CreateRoomsWithHiddenPassage();
            bool received = false;
            bus.GetEventStream<RoomEnteredEvent>().Subscribe(_ => received = true);

            bool result = viewModel.EnterRoom(new Vector2I(999, 999));

            Assert.IsFalse(result);
            Assert.IsFalse(received);
        }

        [Test]
        public void TryActivateHiddenPassage_ValidGimmick_ReturnsTrueAndPublishesRevealedEvent()
        {
            var bus = new GameEventBus();
            var viewModel = CreateViewModel(bus);
            viewModel.Rooms.Value = CreateRoomsWithHiddenPassage();
            HiddenPassageRevealedEvent? received = null;
            bus.GetEventStream<HiddenPassageRevealedEvent>().Subscribe(e => received = e);

            bool result = viewModel.TryActivateHiddenPassage(RoomAPosition, DoorAPosition);

            Assert.IsTrue(result);
            Assert.That(received, Is.Not.Null);
            Assert.AreEqual(RoomAPosition, received!.RoomPosition);
            Assert.AreEqual(DoorAPosition, received!.GimmickPosition);
            Assert.IsTrue(viewModel.Rooms.Value[RoomAPosition].Gimmicks[0].IsActive);
        }

        [Test]
        public void TryActivateHiddenPassage_ValidGimmick_NotifiesRoomsSubscribers()
        {
            var bus = new GameEventBus();
            var viewModel = CreateViewModel(bus);
            viewModel.Rooms.Value = CreateRoomsWithHiddenPassage();
            bool notified = false;
            viewModel.Rooms.Subscribe(_ => notified = true);

            // ギミック発動は RoomData を in-place で書き換えるため、Rooms の変更通知が確実に発火することを確認する
            bool result = viewModel.TryActivateHiddenPassage(RoomAPosition, DoorAPosition);

            Assert.IsTrue(result);
            Assert.IsTrue(notified);
        }

        [Test]
        public void TryActivateHiddenPassage_AlreadyActive_ReturnsFalseAndPublishesFailedEvent()
        {
            var bus = new GameEventBus();
            var viewModel = CreateViewModel(bus);
            viewModel.Rooms.Value = CreateRoomsWithHiddenPassage();
            Assert.IsTrue(viewModel.TryActivateHiddenPassage(RoomAPosition, DoorAPosition));

            GimmickActivationFailedEvent? received = null;
            bus.GetEventStream<GimmickActivationFailedEvent>().Subscribe(e => received = e);

            bool result = viewModel.TryActivateHiddenPassage(RoomAPosition, DoorAPosition);

            Assert.IsFalse(result);
            Assert.That(received, Is.Not.Null);
            Assert.AreEqual(GimmickType.HiddenPassage, received!.GimmickType);
            Assert.AreEqual(RoomAPosition, received!.RoomPosition);
            Assert.AreEqual(DoorAPosition, received!.GimmickPosition);
        }

        [Test]
        public void TryActivateLockedDoor_HasKey_ReturnsTrueAndPublishesUnlockedEvent()
        {
            var bus = new GameEventBus();
            var viewModel = CreateViewModel(bus);
            viewModel.Rooms.Value = CreateRoomsWithLockedDoor();
            LockedDoorUnlockedEvent? received = null;
            bus.GetEventStream<LockedDoorUnlockedEvent>().Subscribe(e => received = e);

            bool result = viewModel.TryActivateLockedDoor(RoomAPosition, DoorAPosition, hasKey: true);

            Assert.IsTrue(result);
            Assert.That(received, Is.Not.Null);
            Assert.AreEqual(RoomAPosition, received!.RoomPosition);
            Assert.AreEqual(DoorAPosition, received!.GimmickPosition);
            Assert.IsFalse(viewModel.Rooms.Value[RoomAPosition].Doors[0].IsLocked);
        }

        [Test]
        public void TryActivateLockedDoor_NoKey_ReturnsFalseAndPublishesFailedEvent()
        {
            var bus = new GameEventBus();
            var viewModel = CreateViewModel(bus);
            viewModel.Rooms.Value = CreateRoomsWithLockedDoor();
            GimmickActivationFailedEvent? received = null;
            bus.GetEventStream<GimmickActivationFailedEvent>().Subscribe(e => received = e);

            bool result = viewModel.TryActivateLockedDoor(RoomAPosition, DoorAPosition, hasKey: false);

            Assert.IsFalse(result);
            Assert.That(received, Is.Not.Null);
            Assert.AreEqual(GimmickType.LockedDoor, received!.GimmickType);
            Assert.AreEqual(RoomAPosition, received!.RoomPosition);
            Assert.AreEqual(DoorAPosition, received!.GimmickPosition);
            Assert.IsTrue(viewModel.Rooms.Value[RoomAPosition].Doors[0].IsLocked);
        }
    }
}
