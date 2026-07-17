using System;
using NUnit.Framework;
using Godot;
using Systems.Dungeon.Data;
using Systems.Dungeon.Models;
using Systems.Dungeon.Utilities;

namespace Tests.Core.Dungeon
{
    public class RoomLayoutGeneratorTests
    {
        /// <summary>
        /// テスト用の部屋データ（扉 2 つ付き）を作成する
        /// </summary>
        private static RoomData CreateRoomWithDoors(RoomType type)
        {
            var room = new RoomData
            {
                Position = new Vector2I(16, 16),
                Size = new Vector2I(DungeonConstants.ROOM_SIZE, DungeonConstants.ROOM_SIZE),
                Type = type
            };

            // 右辺と上辺に扉を配置（ワールドタイル座標）
            room.AddDoor(new DoorData
            {
                Position = new Vector2I(31, 24),
                ConnectedRoomPosition = new Vector2I(32, 16)
            });
            room.AddDoor(new DoorData
            {
                Position = new Vector2I(24, 16),
                ConnectedRoomPosition = new Vector2I(16, 0)
            });

            return room;
        }

        [Test]
        public void GenerateLayout_SameSeed_GeneratesSameLayout()
        {
            // 同一シードなら同一レイアウト（障害物・扉位置）になること
            var template1 = new RoomLayoutGenerator(new Random(42)).GenerateLayout(CreateRoomWithDoors(RoomType.Combat));
            var template2 = new RoomLayoutGenerator(new Random(42)).GenerateLayout(CreateRoomWithDoors(RoomType.Combat));

            CollectionAssert.AreEqual(template1.ObstaclePositions, template2.ObstaclePositions);
            CollectionAssert.AreEqual(template1.DoorPositions, template2.DoorPositions);
        }

        [Test]
        public void GenerateLayout_ObstaclesAreInsideRoomBounds()
        {
            // 複数シードで障害物が部屋境界（0..15）内に収まること
            for (int seed = 0; seed < 10; seed++)
            {
                var template = new RoomLayoutGenerator(new Random(seed)).GenerateLayout(CreateRoomWithDoors(RoomType.Combat));

                foreach (var obstacle in template.ObstaclePositions)
                {
                    Assert.That(obstacle.X, Is.InRange(0, DungeonConstants.ROOM_SIZE - 1),
                        $"シード {seed}: 障害物 X が部屋境界外");
                    Assert.That(obstacle.Y, Is.InRange(0, DungeonConstants.ROOM_SIZE - 1),
                        $"シード {seed}: 障害物 Y が部屋境界外");
                }
            }
        }

        [Test]
        public void GenerateLayout_DoorPositionsAreOnPerimeter()
        {
            // 扉のローカル座標が外周（0 または ROOM_SIZE-1）上にあること
            var template = new RoomLayoutGenerator(new Random(0)).GenerateLayout(CreateRoomWithDoors(RoomType.Combat));

            Assert.AreEqual(2, template.DoorPositions.Count);
            foreach (var door in template.DoorPositions)
            {
                bool onPerimeter =
                    door.X == 0 || door.X == DungeonConstants.ROOM_SIZE - 1 ||
                    door.Y == 0 || door.Y == DungeonConstants.ROOM_SIZE - 1;
                Assert.IsTrue(onPerimeter, $"扉 {door} が外周上にない");
            }
        }

        [Test]
        public void GenerateLayout_ObstaclesDoNotOverlapDoors()
        {
            // 障害物が扉位置と重ならないこと
            for (int seed = 0; seed < 10; seed++)
            {
                var template = new RoomLayoutGenerator(new Random(seed)).GenerateLayout(CreateRoomWithDoors(RoomType.Combat));

                foreach (var obstacle in template.ObstaclePositions)
                {
                    CollectionAssert.DoesNotContain(template.DoorPositions, obstacle);
                }
            }
        }

        [Test]
        public void GenerateLayout_StartRoom_HasNoObstacles()
        {
            // 開始部屋は安全地帯のため障害物が置かれないこと
            var template = new RoomLayoutGenerator(new Random(0)).GenerateLayout(CreateRoomWithDoors(RoomType.Start));

            Assert.AreEqual(0, template.ObstaclePositions.Count);
        }

        [Test]
        public void GenerateLayout_SetsIsGeneratedAndType()
        {
            // 生成完了フラグと部屋タイプが正しく設定されること
            var room = CreateRoomWithDoors(RoomType.Boss);
            var template = new RoomLayoutGenerator(new Random(0)).GenerateLayout(room);

            Assert.IsTrue(room.IsGenerated);
            Assert.AreEqual(RoomType.Boss, template.Type);
        }

        [Test]
        public void Constructor_NullRandom_ThrowsArgumentNullException()
        {
            // 乱数の注入が必須であること
            Assert.Throws<ArgumentNullException>(() => _ = new RoomLayoutGenerator(null!));
        }

        [Test]
        public void GenerateLayout_NullRoom_ThrowsArgumentNullException()
        {
            var generator = new RoomLayoutGenerator(new Random(0));

            Assert.Throws<ArgumentNullException>(() => generator.GenerateLayout(null!));
        }
    }
}
