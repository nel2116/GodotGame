using System;
using System.Collections.Generic;
using NUnit.Framework;
using Godot;
using Systems.Dungeon.Data;
using Systems.Dungeon.Models;
using Systems.Dungeon.Navigation;
using NavigationMesh = Systems.Dungeon.Navigation.NavigationMesh;

namespace Tests.Core.Dungeon.Navigation
{
    public class PathFinderTests
    {
        private static readonly Vector2I RoomPosition = new(0, 0);

        /// <summary>
        /// 指定した障害物を持つ単一の部屋（扉なし）のナビゲーションメッシュを作成する
        /// </summary>
        private static NavigationMesh CreateSingleRoomMesh(List<Vector2I> obstacles)
        {
            var room = new RoomData { Position = RoomPosition, Type = RoomType.Combat };
            var rooms = new Dictionary<Vector2I, RoomData> { [RoomPosition] = room };
            var templates = new Dictionary<Vector2I, RoomTemplate>
            {
                [RoomPosition] = new RoomTemplate { Type = RoomType.Combat, ObstaclePositions = obstacles }
            };

            var mesh = new NavigationMesh();
            mesh.Build(rooms, templates);
            return mesh;
        }

        [Test]
        public void Constructor_NullMesh_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => _ = new PathFinder(null!));
        }

        [Test]
        public void FindPath_StartEqualsGoal_ReturnsSingleElementList()
        {
            var mesh = CreateSingleRoomMesh(new List<Vector2I>());
            var pathFinder = new PathFinder(mesh);
            var point = RoomPosition + new Vector2I(3, 3);

            var path = pathFinder.FindPath(point, point);

            Assert.AreEqual(new List<Vector2I> { point }, path);
        }

        [Test]
        public void FindPath_StartNotWalkable_ReturnsEmptyList()
        {
            var mesh = CreateSingleRoomMesh(new List<Vector2I>());
            var pathFinder = new PathFinder(mesh);

            // 壁（外周）は通行不可なので開始地点として無効
            var path = pathFinder.FindPath(RoomPosition, RoomPosition + new Vector2I(3, 3));

            Assert.IsEmpty(path);
        }

        [Test]
        public void FindPath_StraightLine_ReturnsDirectPath()
        {
            var mesh = CreateSingleRoomMesh(new List<Vector2I>());
            var pathFinder = new PathFinder(mesh);
            var start = RoomPosition + new Vector2I(1, 1);
            var goal = RoomPosition + new Vector2I(1, 5);

            var path = pathFinder.FindPath(start, goal);

            // 障害物がない直線区間では、マンハッタン距離と同じ長さの最短経路になること
            Assert.AreEqual(start, path[0]);
            Assert.AreEqual(goal, path[^1]);
            Assert.AreEqual(5, path.Count);
        }

        [Test]
        public void FindPath_ObstacleWithGap_DetoursThroughGap()
        {
            // y = 5 の行を x = 1..13 まで障害物で塞ぎ、x = 14 のみ通行可能な隙間を作る
            var obstacles = new List<Vector2I>();
            for (int x = 1; x <= 13; x++)
            {
                obstacles.Add(new Vector2I(x, 5));
            }

            var mesh = CreateSingleRoomMesh(obstacles);
            var pathFinder = new PathFinder(mesh);
            var start = RoomPosition + new Vector2I(1, 1);
            var goal = RoomPosition + new Vector2I(1, 9);

            var path = pathFinder.FindPath(start, goal);

            Assert.IsNotEmpty(path);
            Assert.AreEqual(start, path[0]);
            Assert.AreEqual(goal, path[^1]);

            // 経路は隙間（x = 14, y = 5）を通ること
            Assert.Contains(RoomPosition + new Vector2I(14, 5), path);

            // 障害物の上は通らないこと
            foreach (var obstacle in obstacles)
            {
                Assert.IsFalse(path.Contains(RoomPosition + obstacle));
            }
        }

        [Test]
        public void FindPath_NoPathExists_ReturnsEmptyList()
        {
            // y = 5 の行を全幅（x = 1..14）塞ぎ、隙間なしの壁にする
            var obstacles = new List<Vector2I>();
            for (int x = 1; x <= 14; x++)
            {
                obstacles.Add(new Vector2I(x, 5));
            }

            var mesh = CreateSingleRoomMesh(obstacles);
            var pathFinder = new PathFinder(mesh);
            var start = RoomPosition + new Vector2I(1, 1);
            var goal = RoomPosition + new Vector2I(1, 9);

            var path = pathFinder.FindPath(start, goal);

            Assert.IsEmpty(path);
        }
    }
}
