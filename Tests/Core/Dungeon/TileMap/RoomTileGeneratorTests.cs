using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Godot;
using Systems.Dungeon.Data;
using Systems.Dungeon.Models;
using Systems.Dungeon.TileMap;
using Systems.Dungeon.Utilities;
using TileType = Systems.Dungeon.TileMap.TileType;

namespace Tests.Core.Dungeon.TileMap
{
    public class RoomTileGeneratorTests
    {
        private static readonly Vector2I RoomPosition = new(32, 16);

        /// <summary>
        /// 扉を 1 つ持つ部屋データと、対応する部屋テンプレートを作成する
        /// </summary>
        private static (RoomData Room, RoomTemplate Template) CreateRoomWithDoor(
            DoorType doorType, bool isLocked, Vector2I doorLocal, List<Vector2I>? obstacles = null)
        {
            var room = new RoomData { Position = RoomPosition, Type = RoomType.Combat };
            room.AddDoor(new DoorData
            {
                Position = RoomPosition + doorLocal,
                Type = doorType,
                IsLocked = isLocked,
                ConnectedRoomPosition = RoomPosition + new Vector2I(16, 0)
            });

            var template = new RoomTemplate
            {
                Type = RoomType.Combat,
                ObstaclePositions = obstacles ?? new List<Vector2I>(),
                DoorPositions = new List<Vector2I> { doorLocal }
            };

            return (room, template);
        }

        private static TileType FindType(List<(Vector2I WorldPosition, TileType Type)> tiles, Vector2I worldPosition)
        {
            return tiles.First(t => t.WorldPosition == worldPosition).Type;
        }

        [Test]
        public void GenerateTiles_NullRoom_Throws()
        {
            var generator = new RoomTileGenerator();
            var template = new RoomTemplate();

            Assert.Throws<ArgumentNullException>(() => generator.GenerateTiles(null!, template));
        }

        [Test]
        public void GenerateTiles_NullTemplate_Throws()
        {
            var generator = new RoomTileGenerator();
            var room = new RoomData { Position = RoomPosition, Type = RoomType.Combat };

            Assert.Throws<ArgumentNullException>(() => generator.GenerateTiles(room, null!));
        }

        [Test]
        public void GenerateTiles_ReturnsAllCellsInRoom()
        {
            var (room, template) = CreateRoomWithDoor(DoorType.Normal, isLocked: false, doorLocal: new Vector2I(15, 8));
            var generator = new RoomTileGenerator();

            var tiles = generator.GenerateTiles(room, template);

            Assert.AreEqual(DungeonConstants.ROOM_SIZE * DungeonConstants.ROOM_SIZE, tiles.Count);
        }

        [Test]
        public void GenerateTiles_InteriorWithoutObstacle_IsFloor()
        {
            var (room, template) = CreateRoomWithDoor(DoorType.Normal, isLocked: false, doorLocal: new Vector2I(15, 8));
            var generator = new RoomTileGenerator();

            var tiles = generator.GenerateTiles(room, template);

            Assert.AreEqual(TileType.Floor, FindType(tiles, RoomPosition + new Vector2I(1, 1)));
            Assert.AreEqual(TileType.Floor, FindType(tiles, RoomPosition + new Vector2I(14, 14)));
        }

        [Test]
        public void GenerateTiles_InteriorObstaclePosition_IsObstacle()
        {
            var obstacle = new Vector2I(5, 5);
            var (room, template) = CreateRoomWithDoor(
                DoorType.Normal, isLocked: false, doorLocal: new Vector2I(15, 8), obstacles: new List<Vector2I> { obstacle });
            var generator = new RoomTileGenerator();

            var tiles = generator.GenerateTiles(room, template);

            Assert.AreEqual(TileType.Obstacle, FindType(tiles, RoomPosition + obstacle));
        }

        [Test]
        public void GenerateTiles_BoundaryWithoutDoor_IsWall()
        {
            var (room, template) = CreateRoomWithDoor(DoorType.Normal, isLocked: false, doorLocal: new Vector2I(15, 8));
            var generator = new RoomTileGenerator();

            var tiles = generator.GenerateTiles(room, template);

            Assert.AreEqual(TileType.Wall, FindType(tiles, RoomPosition + new Vector2I(0, 0)));
            Assert.AreEqual(TileType.Wall, FindType(tiles, RoomPosition + new Vector2I(0, 8)));
            Assert.AreEqual(TileType.Wall, FindType(tiles, RoomPosition + new Vector2I(15, 0)));
            Assert.AreEqual(TileType.Wall, FindType(tiles, RoomPosition + new Vector2I(15, 15)));
        }

        [Test]
        public void GenerateTiles_NormalDoor_IsDoor()
        {
            var doorLocal = new Vector2I(15, 8);
            var (room, template) = CreateRoomWithDoor(DoorType.Normal, isLocked: false, doorLocal: doorLocal);
            var generator = new RoomTileGenerator();

            var tiles = generator.GenerateTiles(room, template);

            Assert.AreEqual(TileType.Door, FindType(tiles, RoomPosition + doorLocal));
        }

        [Test]
        public void GenerateTiles_LockedDoor_IsLockedDoor()
        {
            var doorLocal = new Vector2I(15, 8);
            var (room, template) = CreateRoomWithDoor(DoorType.Locked, isLocked: true, doorLocal: doorLocal);
            var generator = new RoomTileGenerator();

            var tiles = generator.GenerateTiles(room, template);

            Assert.AreEqual(TileType.LockedDoor, FindType(tiles, RoomPosition + doorLocal));
        }

        [Test]
        public void GenerateTiles_UndiscoveredSecretDoor_IsSecretWall()
        {
            var doorLocal = new Vector2I(15, 8);
            var (room, template) = CreateRoomWithDoor(DoorType.Secret, isLocked: false, doorLocal: doorLocal);
            var generator = new RoomTileGenerator();

            var tiles = generator.GenerateTiles(room, template);

            Assert.AreEqual(TileType.SecretWall, FindType(tiles, RoomPosition + doorLocal));
        }

        [Test]
        public void GenerateTiles_WorldPositionOffsetByRoomPosition()
        {
            var (room, template) = CreateRoomWithDoor(DoorType.Normal, isLocked: false, doorLocal: new Vector2I(15, 8));
            var generator = new RoomTileGenerator();

            var tiles = generator.GenerateTiles(room, template);

            // 全セルが room.Position を基準としたワールド座標範囲に収まっていること
            Assert.IsTrue(tiles.All(t =>
                t.WorldPosition.X >= RoomPosition.X && t.WorldPosition.X < RoomPosition.X + DungeonConstants.ROOM_SIZE &&
                t.WorldPosition.Y >= RoomPosition.Y && t.WorldPosition.Y < RoomPosition.Y + DungeonConstants.ROOM_SIZE));
        }

        [Test]
        public void GenerateTiles_DoorStateChangesAfterGimmickActivation_ReflectsNewType()
        {
            // 鍵扉解錠後（Type は Locked のまま、IsLocked のみ false に変化）の再生成では
            // IsLocked も判定に使うため Door（通行可能な扉）になること
            var doorLocal = new Vector2I(15, 8);
            var (room, template) = CreateRoomWithDoor(DoorType.Locked, isLocked: true, doorLocal: doorLocal);
            var generator = new RoomTileGenerator();

            room.Doors[0].IsLocked = false;
            var tiles = generator.GenerateTiles(room, template);

            Assert.AreEqual(TileType.Door, FindType(tiles, RoomPosition + doorLocal));

            // 隠し通路発動後（Type が Secret から Normal へ変化）も Door になること
            var (secretRoom, secretTemplate) = CreateRoomWithDoor(DoorType.Secret, isLocked: false, doorLocal: doorLocal);
            secretRoom.Doors[0].Type = DoorType.Normal;
            var secretTiles = generator.GenerateTiles(secretRoom, secretTemplate);

            Assert.AreEqual(TileType.Door, FindType(secretTiles, RoomPosition + doorLocal));
        }

        [Test]
        public void GenerateTiles_LockedDoorStillLocked_IsLockedDoor()
        {
            // 施錠中（IsLocked = true）の間は LockedDoor のままであること
            var doorLocal = new Vector2I(15, 8);
            var (room, template) = CreateRoomWithDoor(DoorType.Locked, isLocked: true, doorLocal: doorLocal);
            var generator = new RoomTileGenerator();

            var tiles = generator.GenerateTiles(room, template);

            Assert.AreEqual(TileType.LockedDoor, FindType(tiles, RoomPosition + doorLocal));
        }

        [Test]
        public void GenerateTiles_DoorCountMismatch_Throws()
        {
            // RoomTemplate.DoorPositions と RoomData.Doors の件数が一致しない場合は例外を投げること
            var room = new RoomData { Position = RoomPosition, Type = RoomType.Combat };
            room.AddDoor(new DoorData
            {
                Position = RoomPosition + new Vector2I(15, 8),
                Type = DoorType.Normal,
                IsLocked = false,
                ConnectedRoomPosition = RoomPosition + new Vector2I(16, 0)
            });

            var template = new RoomTemplate
            {
                Type = RoomType.Combat,
                ObstaclePositions = new List<Vector2I>(),
                DoorPositions = new List<Vector2I>()
            };

            var generator = new RoomTileGenerator();

            Assert.Throws<InvalidOperationException>(() => generator.GenerateTiles(room, template));
        }
    }
}
