using System.Collections.Generic;
using NUnit.Framework;
using Godot;
using Systems.Dungeon.Data;
using Systems.Dungeon.Models;
using Systems.Dungeon.Navigation;
using Systems.Dungeon.Optimization;

namespace Tests.Core.Dungeon.Optimization
{
    public class DungeonOptimizationCoordinatorTests
    {
        private static readonly Vector2I PositionA = new(0, 0);
        private static readonly Vector2I PositionB = new(16, 0);
        private static readonly Vector2I DoorAPosition = new(15, 8);
        private static readonly Vector2I DoorBPosition = new(16, 8);

        private static (Dictionary<Vector2I, RoomData> Rooms, Dictionary<Vector2I, RoomTemplate> Templates) CreateConnectedRooms()
        {
            var roomA = new RoomData { Position = PositionA, Type = RoomType.Combat };
            var roomB = new RoomData { Position = PositionB, Type = RoomType.Combat };
            roomA.AddDoor(new DoorData { Position = DoorAPosition, Type = DoorType.Normal, ConnectedRoomPosition = PositionB });
            roomB.AddDoor(new DoorData { Position = DoorBPosition, Type = DoorType.Normal, ConnectedRoomPosition = PositionA });

            var rooms = new Dictionary<Vector2I, RoomData> { [PositionA] = roomA, [PositionB] = roomB };
            var templates = new Dictionary<Vector2I, RoomTemplate>
            {
                [PositionA] = new RoomTemplate { Type = RoomType.Combat },
                [PositionB] = new RoomTemplate { Type = RoomType.Combat }
            };
            return (rooms, templates);
        }

        [Test]
        public void OnRoomEntered_ComputesVisibilityAndSyncsLifecycle()
        {
            var (rooms, templates) = CreateConnectedRooms();
            var coordinator = new DungeonOptimizationCoordinator(new RoomVisibilityManager(), new RoomLifecycleManager(), new NavigationManager());
            var renderer = new FakeRoomTileRenderer();

            var result = coordinator.OnRoomEntered(PositionA, rooms, templates, renderer);

            Assert.That(result.Loaded, Is.EquivalentTo(new[] { PositionA, PositionB }));
            Assert.That(renderer.AppliedRoomPositions, Is.EquivalentTo(new[] { PositionA, PositionB }));
        }

        [Test]
        public void OnRoomEntered_RoomAlreadyActive_ReturnsEmptyDiffOnSecondCall()
        {
            var (rooms, templates) = CreateConnectedRooms();
            var coordinator = new DungeonOptimizationCoordinator(new RoomVisibilityManager(), new RoomLifecycleManager(), new NavigationManager());
            var renderer = new FakeRoomTileRenderer();
            coordinator.OnRoomEntered(PositionA, rooms, templates, renderer);

            var result = coordinator.OnRoomEntered(PositionA, rooms, templates, renderer);

            Assert.IsEmpty(result.Loaded);
            Assert.IsEmpty(result.Unloaded);
        }

        [Test]
        public void OnDoorStateChanged_AfterUnlockingDoor_PathBecomesAvailableWithoutFullRebuild()
        {
            var (rooms, templates) = CreateConnectedRooms();
            rooms[PositionA].Doors[0].Type = DoorType.Locked;
            rooms[PositionA].Doors[0].IsLocked = true;
            rooms[PositionB].Doors[0].Type = DoorType.Locked;
            rooms[PositionB].Doors[0].IsLocked = true;

            var navigationManager = new NavigationManager();
            navigationManager.BuildMesh(rooms, templates);

            var start = PositionA + new Vector2I(1, 1);
            var goal = PositionB + new Vector2I(1, 1);
            Assert.IsEmpty(navigationManager.FindPath(start, goal));

            rooms[PositionA].Doors[0].IsLocked = false;
            rooms[PositionB].Doors[0].IsLocked = false;

            var coordinator = new DungeonOptimizationCoordinator(new RoomVisibilityManager(), new RoomLifecycleManager(), navigationManager);
            coordinator.OnDoorStateChanged(PositionA, PositionB, rooms, templates);

            Assert.IsNotEmpty(navigationManager.FindPath(start, goal));
        }
    }
}
