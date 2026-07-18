using System.Collections.Generic;
using NUnit.Framework;
using Godot;
using Systems.Dungeon.Data;
using Systems.Dungeon.Models;
using Systems.Dungeon.Optimization;

namespace Tests.Core.Dungeon.Optimization
{
    public class RoomLifecycleManagerTests
    {
        private static readonly Vector2I PositionA = new(0, 0);
        private static readonly Vector2I PositionB = new(16, 0);
        private static readonly Vector2I PositionC = new(32, 0);

        private static (Dictionary<Vector2I, RoomData> Rooms, Dictionary<Vector2I, RoomTemplate> Templates) CreateRooms()
        {
            var rooms = new Dictionary<Vector2I, RoomData>
            {
                [PositionA] = new RoomData { Position = PositionA, Type = RoomType.Combat },
                [PositionB] = new RoomData { Position = PositionB, Type = RoomType.Combat },
                [PositionC] = new RoomData { Position = PositionC, Type = RoomType.Combat }
            };
            var templates = new Dictionary<Vector2I, RoomTemplate>
            {
                [PositionA] = new RoomTemplate { Type = RoomType.Combat },
                [PositionB] = new RoomTemplate { Type = RoomType.Combat },
                [PositionC] = new RoomTemplate { Type = RoomType.Combat }
            };
            return (rooms, templates);
        }

        [Test]
        public void SyncActiveRooms_NewlyActiveRooms_AppliesAndTracksAsLoaded()
        {
            var (rooms, templates) = CreateRooms();
            var manager = new RoomLifecycleManager();
            var renderer = new FakeRoomTileRenderer();

            var result = manager.SyncActiveRooms(new HashSet<Vector2I> { PositionA, PositionB }, rooms, templates, renderer);

            Assert.That(result.Loaded, Is.EquivalentTo(new[] { PositionA, PositionB }));
            Assert.IsEmpty(result.Unloaded);
            Assert.That(renderer.AppliedRoomPositions, Is.EquivalentTo(new[] { PositionA, PositionB }));
            Assert.That(manager.LoadedRooms, Is.EquivalentTo(new[] { PositionA, PositionB }));
        }

        [Test]
        public void SyncActiveRooms_RoomLeavesActiveSet_ClearsAndRemovesFromLoaded()
        {
            var (rooms, templates) = CreateRooms();
            var manager = new RoomLifecycleManager();
            var renderer = new FakeRoomTileRenderer();
            manager.SyncActiveRooms(new HashSet<Vector2I> { PositionA, PositionB }, rooms, templates, renderer);

            var result = manager.SyncActiveRooms(new HashSet<Vector2I> { PositionB, PositionC }, rooms, templates, renderer);

            Assert.That(result.Loaded, Is.EquivalentTo(new[] { PositionC }));
            Assert.That(result.Unloaded, Is.EquivalentTo(new[] { PositionA }));
            Assert.That(renderer.ClearedRoomPositions, Is.EquivalentTo(new[] { PositionA }));
            Assert.That(manager.LoadedRooms, Is.EquivalentTo(new[] { PositionB, PositionC }));
        }

        [Test]
        public void SyncActiveRooms_AlreadyLoadedRoomStaysActive_DoesNotReapplyOrClear()
        {
            var (rooms, templates) = CreateRooms();
            var manager = new RoomLifecycleManager();
            var renderer = new FakeRoomTileRenderer();
            manager.SyncActiveRooms(new HashSet<Vector2I> { PositionA }, rooms, templates, renderer);

            var result = manager.SyncActiveRooms(new HashSet<Vector2I> { PositionA }, rooms, templates, renderer);

            Assert.IsEmpty(result.Loaded);
            Assert.IsEmpty(result.Unloaded);
            Assert.AreEqual(1, renderer.AppliedRoomPositions.Count);
        }

        [Test]
        public void SyncActiveRooms_ActiveRoomMissingTemplate_IsSkipped()
        {
            var (rooms, templates) = CreateRooms();
            templates.Remove(PositionA);
            var manager = new RoomLifecycleManager();
            var renderer = new FakeRoomTileRenderer();

            var result = manager.SyncActiveRooms(new HashSet<Vector2I> { PositionA }, rooms, templates, renderer);

            Assert.IsEmpty(result.Loaded);
            Assert.IsEmpty(renderer.AppliedRoomPositions);
        }
    }
}
