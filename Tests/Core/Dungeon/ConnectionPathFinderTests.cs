using System;
using System.Collections.Generic;
using NUnit.Framework;
using Godot;
using Systems.Dungeon.Models;
using Systems.Dungeon.Utilities;

namespace Tests.Core.Dungeon
{
    public class ConnectionPathFinderTests
    {
        private readonly ConnectionPathFinder pathFinder = new();

        [Test]
        public void FindDoorPosition_HorizontallyAdjacentRooms_DoorsFaceEachOtherOnSharedBoundary()
        {
            // 左右に隣接する 2 部屋の扉が共有境界を挟んで向かい合うこと
            var roomA = new Vector2I(0, 0);
            var roomB = new Vector2I(16, 0);

            var doorA = pathFinder.FindDoorPosition(roomA, roomB);
            var doorB = pathFinder.FindDoorPosition(roomB, roomA);

            // 部屋 A の扉は右辺（X = 15）、部屋 B の扉は左辺（X = 16）
            Assert.AreEqual(15, doorA.X);
            Assert.AreEqual(16, doorB.X);

            // 同じ Y 座標で向かい合い、共有境界の範囲（両部屋の Y 範囲）内にあること
            Assert.AreEqual(doorA.Y, doorB.Y);
            Assert.That(doorA.Y, Is.InRange(0, DungeonConstants.ROOM_SIZE - 1));
        }

        [Test]
        public void FindDoorPosition_VerticallyAdjacentRooms_DoorsFaceEachOtherOnSharedBoundary()
        {
            // 上下に隣接する 2 部屋の扉が共有境界を挟んで向かい合うこと
            var roomA = new Vector2I(-16, 16);
            var roomB = new Vector2I(-16, 32);

            var doorA = pathFinder.FindDoorPosition(roomA, roomB);
            var doorB = pathFinder.FindDoorPosition(roomB, roomA);

            // 部屋 A の扉は下辺（Y = 31）、部屋 B の扉は上辺（Y = 32）
            Assert.AreEqual(31, doorA.Y);
            Assert.AreEqual(32, doorB.Y);

            // 同じ X 座標で向かい合い、共有境界の範囲内にあること
            Assert.AreEqual(doorA.X, doorB.X);
            Assert.That(doorA.X - roomA.X, Is.InRange(0, DungeonConstants.ROOM_SIZE - 1));
        }

        [Test]
        public void FindDoorPosition_NonAdjacentRooms_DoorIsOnOwnRoomPerimeter()
        {
            // 隣接しない部屋同士でも扉が自室の外周上に収まること
            var roomA = new Vector2I(0, 0);
            var roomB = new Vector2I(48, 32);

            var doorA = pathFinder.FindDoorPosition(roomA, roomB);
            var localA = doorA - roomA;

            bool onPerimeter =
                localA.X == 0 || localA.X == DungeonConstants.ROOM_SIZE - 1 ||
                localA.Y == 0 || localA.Y == DungeonConstants.ROOM_SIZE - 1;

            Assert.That(localA.X, Is.InRange(0, DungeonConstants.ROOM_SIZE - 1));
            Assert.That(localA.Y, Is.InRange(0, DungeonConstants.ROOM_SIZE - 1));
            Assert.IsTrue(onPerimeter, $"扉 {doorA} が部屋 {roomA} の外周上にない");
        }

        [Test]
        public void FindDoorPosition_SamePosition_ThrowsArgumentException()
        {
            // 同一位置の部屋同士は扉を配置できないこと
            Assert.Throws<ArgumentException>(() => pathFinder.FindDoorPosition(Vector2I.Zero, Vector2I.Zero));
        }

        [Test]
        public void FindOptimalPath_GridAlignedRooms_ReturnsContinuousPath()
        {
            // L 字経路が ROOM_SIZE 刻みで連続し、両端が正しいこと
            var start = new Vector2I(0, 0);
            var end = new Vector2I(32, 16);

            var path = pathFinder.FindOptimalPath(start, end);

            Assert.AreEqual(start, path[0]);
            Assert.AreEqual(end, path[^1]);
            Assert.AreEqual(4, path.Count);

            for (int i = 1; i < path.Count; i++)
            {
                var step = path[i] - path[i - 1];
                Assert.AreEqual(DungeonConstants.ROOM_SIZE, Math.Abs(step.X) + Math.Abs(step.Y),
                    "経路が 1 部屋ずつ進んでいない");
            }
        }

        [Test]
        public void FindOptimalPath_SamePosition_ReturnsSingleElement()
        {
            // 同一位置なら自分自身のみの経路になること
            var path = pathFinder.FindOptimalPath(new Vector2I(16, -16), new Vector2I(16, -16));

            CollectionAssert.AreEqual(new List<Vector2I> { new(16, -16) }, path);
        }

        [Test]
        public void FindOptimalPath_MisalignedPositions_ThrowsArgumentException()
        {
            // グリッドに整列していない差分は例外になること
            Assert.Throws<ArgumentException>(
                () => pathFinder.FindOptimalPath(Vector2I.Zero, new Vector2I(8, 0)));
        }
    }
}
