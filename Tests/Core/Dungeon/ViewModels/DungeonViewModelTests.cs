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
using Systems.Dungeon.Optimization;
using Systems.Dungeon.ViewModels;
using Tests.Core.Dungeon.Optimization;

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
            var navigationManager = new NavigationManager();
            var coordinator = new DungeonOptimizationCoordinator(
                new RoomVisibilityManager(), new RoomLifecycleManager(), navigationManager);

            return new DungeonViewModel(
                new LevelGenerationModel(0),
                new GimmickPlacementModel(new Random(0)),
                new GimmickActivator(),
                navigationManager,
                coordinator,
                new FakeRoomTileRenderer(),
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

        /// <summary>
        /// コンストラクタのnullチェックテスト用に、正常な最適化ファサードを1つ生成する
        /// </summary>
        private static DungeonOptimizationCoordinator CreateCoordinator()
        {
            return new DungeonOptimizationCoordinator(
                new RoomVisibilityManager(), new RoomLifecycleManager(), new NavigationManager());
        }

        [Test]
        public void Constructor_NullLevelGenerationModel_ThrowsArgumentNullException()
        {
            var bus = new GameEventBus();
            Assert.Throws<ArgumentNullException>(() => _ = new DungeonViewModel(
                null!, new GimmickPlacementModel(new Random(0)), new GimmickActivator(), new NavigationManager(),
                CreateCoordinator(), new FakeRoomTileRenderer(), bus));
        }

        [Test]
        public void Constructor_NullOptimizationCoordinator_ThrowsArgumentNullException()
        {
            var bus = new GameEventBus();
            Assert.Throws<ArgumentNullException>(() => _ = new DungeonViewModel(
                new LevelGenerationModel(0), new GimmickPlacementModel(new Random(0)), new GimmickActivator(), new NavigationManager(),
                null!, new FakeRoomTileRenderer(), bus));
        }

        [Test]
        public void Constructor_NullRoomTileRenderer_ThrowsArgumentNullException()
        {
            var bus = new GameEventBus();
            Assert.Throws<ArgumentNullException>(() => _ = new DungeonViewModel(
                new LevelGenerationModel(0), new GimmickPlacementModel(new Random(0)), new GimmickActivator(), new NavigationManager(),
                CreateCoordinator(), null!, bus));
        }

        [Test]
        public void Constructor_NullEventBus_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new DungeonViewModel(
                new LevelGenerationModel(0), new GimmickPlacementModel(new Random(0)), new GimmickActivator(), new NavigationManager(),
                CreateCoordinator(), new FakeRoomTileRenderer(), null!));
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
        public async Task GenerateLevelAsync_PublishesRoomsVisibilityChangedEventForInitialActiveRooms()
        {
            var bus = new GameEventBus();
            var viewModel = CreateViewModel(bus);
            RoomsVisibilityChangedEvent? received = null;
            bus.GetEventStream<RoomsVisibilityChangedEvent>().Subscribe(e => received = e);

            await viewModel.GenerateLevelAsync(42);

            // 全部屋を即時読み込みするのではなく、開始部屋周辺のアクティブな部屋集合のみが1回のイベントで読み込まれること
            Assert.That(received, Is.Not.Null);
            Assert.That(received!.LoadedRooms, Does.Contain(Vector2I.Zero));
            Assert.IsEmpty(received.UnloadedRooms);
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

        [Test]
        public void TryActivateHiddenPassage_ValidGimmick_NavigationBecomesWalkableAcrossDoor()
        {
            var bus = new GameEventBus();
            var viewModel = CreateViewModel(bus);
            viewModel.Rooms.Value = CreateRoomsWithHiddenPassage();

            var start = RoomAPosition + new Vector2I(1, 1);
            var goal = RoomBPosition + new Vector2I(1, 1);
            Assert.IsEmpty(viewModel.FindPath(start, goal));

            bool result = viewModel.TryActivateHiddenPassage(RoomAPosition, DoorAPosition);

            // 発動元・接続先の2部屋のみの部分再構築（BuildMeshの全体再構築は行わない）でも経路が通ること
            Assert.IsTrue(result);
            Assert.IsNotEmpty(viewModel.FindPath(start, goal));
        }

        [Test]
        public void TryActivateLockedDoor_HasKey_NavigationBecomesWalkableAcrossDoor()
        {
            var bus = new GameEventBus();
            var viewModel = CreateViewModel(bus);
            viewModel.Rooms.Value = CreateRoomsWithLockedDoor();

            var start = RoomAPosition + new Vector2I(1, 1);
            var goal = RoomBPosition + new Vector2I(1, 1);
            Assert.IsEmpty(viewModel.FindPath(start, goal));

            bool result = viewModel.TryActivateLockedDoor(RoomAPosition, DoorAPosition, hasKey: true);

            Assert.IsTrue(result);
            Assert.IsNotEmpty(viewModel.FindPath(start, goal));
        }
    }
}
