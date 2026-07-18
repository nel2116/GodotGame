using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Godot;
using Systems.Dungeon.Data;
using Systems.Dungeon.Models;
using NavigationMesh = Systems.Dungeon.Navigation.NavigationMesh;

namespace Tests.Core.Dungeon.Navigation
{
    public class NavigationMeshTests
    {
        private static readonly Vector2I RoomAPosition = new(0, 0);
        private static readonly Vector2I RoomBPosition = new(16, 0);
        private static readonly Vector2I DoorAPosition = new(15, 8);
        private static readonly Vector2I DoorBPosition = new(16, 8);

        /// <summary>
        /// 扉で接続された 2 部屋（部屋テンプレート付き）を作成する
        /// </summary>
        private static (Dictionary<Vector2I, RoomData> Rooms, Dictionary<Vector2I, RoomTemplate> Templates) CreateConnectedRoomPair(
            DoorType doorType, bool isLocked, List<Vector2I>? obstaclesA = null)
        {
            var roomA = new RoomData { Position = RoomAPosition, Type = RoomType.Combat };
            var roomB = new RoomData { Position = RoomBPosition, Type = RoomType.Combat };

            roomA.AddDoor(new DoorData { Position = DoorAPosition, Type = doorType, IsLocked = isLocked, ConnectedRoomPosition = RoomBPosition });
            roomB.AddDoor(new DoorData { Position = DoorBPosition, Type = doorType, IsLocked = isLocked, ConnectedRoomPosition = RoomAPosition });

            var rooms = new Dictionary<Vector2I, RoomData> { [RoomAPosition] = roomA, [RoomBPosition] = roomB };
            var templates = new Dictionary<Vector2I, RoomTemplate>
            {
                [RoomAPosition] = new RoomTemplate { Type = RoomType.Combat, ObstaclePositions = obstaclesA ?? new List<Vector2I>() },
                [RoomBPosition] = new RoomTemplate { Type = RoomType.Combat, ObstaclePositions = new List<Vector2I>() }
            };

            return (rooms, templates);
        }

        [Test]
        public void Build_RoomInterior_IsWalkableExceptObstacles()
        {
            var obstacle = new Vector2I(5, 5);
            var (rooms, templates) = CreateConnectedRoomPair(DoorType.Normal, isLocked: false, obstaclesA: new List<Vector2I> { obstacle });

            var mesh = new NavigationMesh();
            mesh.Build(rooms, templates);

            // 内部領域（障害物以外）は通行可能であること
            Assert.IsTrue(mesh.IsWalkable(RoomAPosition + new Vector2I(1, 1)));
            Assert.IsTrue(mesh.IsWalkable(RoomAPosition + new Vector2I(14, 14)));

            // 障害物の位置は通行不可であること
            Assert.IsFalse(mesh.IsWalkable(RoomAPosition + obstacle));

            // 外周（壁）は通行不可であること
            Assert.IsFalse(mesh.IsWalkable(RoomAPosition + new Vector2I(0, 0)));
        }

        [Test]
        public void Build_NormalDoor_IsWalkable()
        {
            var (rooms, templates) = CreateConnectedRoomPair(DoorType.Normal, isLocked: false);

            var mesh = new NavigationMesh();
            mesh.Build(rooms, templates);

            Assert.IsTrue(mesh.IsWalkable(DoorAPosition));
            Assert.IsTrue(mesh.IsWalkable(DoorBPosition));
        }

        [Test]
        public void Build_LockedDoor_IsNotWalkable()
        {
            var (rooms, templates) = CreateConnectedRoomPair(DoorType.Locked, isLocked: true);

            var mesh = new NavigationMesh();
            mesh.Build(rooms, templates);

            Assert.IsFalse(mesh.IsWalkable(DoorAPosition));
            Assert.IsFalse(mesh.IsWalkable(DoorBPosition));
        }

        [Test]
        public void Build_UndiscoveredSecretDoor_IsNotWalkable()
        {
            var (rooms, templates) = CreateConnectedRoomPair(DoorType.Secret, isLocked: false);

            var mesh = new NavigationMesh();
            mesh.Build(rooms, templates);

            Assert.IsFalse(mesh.IsWalkable(DoorAPosition));
            Assert.IsFalse(mesh.IsWalkable(DoorBPosition));
        }

        [Test]
        public void Build_CalledAgainAfterDoorStateChanges_ReflectsNewState()
        {
            // 鍵扉が解錠された（ギミック発動を模した）状態変化後、再度 Build すると通行可能になること
            var (rooms, templates) = CreateConnectedRoomPair(DoorType.Locked, isLocked: true);
            var mesh = new NavigationMesh();
            mesh.Build(rooms, templates);
            Assert.IsFalse(mesh.IsWalkable(DoorAPosition));

            rooms[RoomAPosition].Doors[0].IsLocked = false;
            rooms[RoomBPosition].Doors[0].IsLocked = false;
            mesh.Build(rooms, templates);

            Assert.IsTrue(mesh.IsWalkable(DoorAPosition));
            Assert.IsTrue(mesh.IsWalkable(DoorBPosition));
        }

        [Test]
        public void GetWalkableNeighbors_ReturnsOnlyOrthogonalWalkableTiles()
        {
            var (rooms, templates) = CreateConnectedRoomPair(DoorType.Normal, isLocked: false, obstaclesA: new List<Vector2I> { new(2, 1) });

            var mesh = new NavigationMesh();
            mesh.Build(rooms, templates);

            var center = RoomAPosition + new Vector2I(1, 1);
            var neighbors = mesh.GetWalkableNeighbors(center).ToList();

            // 上方向（障害物）は含まれず、右・下は通行可能なので含まれること。斜めは含まれないこと
            Assert.IsFalse(neighbors.Contains(RoomAPosition + new Vector2I(2, 1)));
            Assert.IsTrue(neighbors.Contains(RoomAPosition + new Vector2I(1, 2)));
            CollectionAssert.DoesNotContain(neighbors, RoomAPosition + new Vector2I(2, 2));
        }
    }
}
