using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Godot;
using Systems.Dungeon.Data;
using Systems.Dungeon.Models;
using Systems.Dungeon.Utilities;

namespace Tests.Core.Dungeon
{
    public class RoomConnectionModelTests
    {
        /// <summary>
        /// 性質ベーステストで使用するシード数
        /// </summary>
        private const int SEED_COUNT = 10;

        /// <summary>
        /// テスト用の部屋辞書を作成する（グリッド整列したランダムな重複なし配置）
        /// </summary>
        private static Dictionary<Vector2I, RoomData> CreateRooms(int seed, int count = DungeonConstants.ROOM_COUNT)
        {
            var random = new Random(seed);
            var rooms = new Dictionary<Vector2I, RoomData>();

            while (rooms.Count < count)
            {
                var position = new Vector2I(
                    random.Next(-DungeonConstants.GENERATION_GRID_RANGE, DungeonConstants.GENERATION_GRID_RANGE + 1) * DungeonConstants.ROOM_SIZE,
                    random.Next(-DungeonConstants.GENERATION_GRID_RANGE, DungeonConstants.GENERATION_GRID_RANGE + 1) * DungeonConstants.ROOM_SIZE);

                if (!rooms.ContainsKey(position))
                {
                    rooms[position] = new RoomData
                    {
                        Position = position,
                        Size = new Vector2I(DungeonConstants.ROOM_SIZE, DungeonConstants.ROOM_SIZE),
                        Type = RoomType.Combat
                    };
                }
            }

            return rooms;
        }

        /// <summary>
        /// 扉のグラフを辿って開始部屋から到達可能な部屋数を数える
        /// </summary>
        private static int CountReachableRooms(Dictionary<Vector2I, RoomData> rooms)
        {
            var start = rooms.Keys.First();
            var visited = new HashSet<Vector2I> { start };
            var queue = new Queue<Vector2I>();
            queue.Enqueue(start);

            while (queue.Count > 0)
            {
                foreach (var door in rooms[queue.Dequeue()].Doors)
                {
                    if (rooms.ContainsKey(door.ConnectedRoomPosition) && visited.Add(door.ConnectedRoomPosition))
                    {
                        queue.Enqueue(door.ConnectedRoomPosition);
                    }
                }
            }

            return visited.Count;
        }

        [Test]
        public void ConnectRooms_MultipleSeeds_AllRoomsAreConnected()
        {
            // 複数シードで全部屋が連結になること
            for (int seed = 0; seed < SEED_COUNT; seed++)
            {
                var rooms = CreateRooms(seed);
                new RoomConnectionModel(new Random(seed)).ConnectRooms(rooms);

                Assert.AreEqual(rooms.Count, CountReachableRooms(rooms), $"シード {seed}: 全部屋が連結でない");
            }
        }

        [Test]
        public void ConnectRooms_MultipleSeeds_DoorsArePaired()
        {
            // 複数シードで扉が必ず対になる（対称性）こと
            for (int seed = 0; seed < SEED_COUNT; seed++)
            {
                var rooms = CreateRooms(seed);
                new RoomConnectionModel(new Random(seed)).ConnectRooms(rooms);

                foreach (var (position, room) in rooms)
                {
                    foreach (var door in room.Doors)
                    {
                        // 接続先の部屋が存在すること
                        Assert.IsTrue(rooms.ContainsKey(door.ConnectedRoomPosition),
                            $"シード {seed}: 扉の接続先 {door.ConnectedRoomPosition} が存在しない");

                        // 接続先の部屋に自分を指す扉があること
                        var pairedDoors = rooms[door.ConnectedRoomPosition].Doors
                            .Count(d => d.ConnectedRoomPosition == position);
                        Assert.GreaterOrEqual(pairedDoors, 1,
                            $"シード {seed}: 部屋 {position} → {door.ConnectedRoomPosition} の対の扉がない");
                    }
                }
            }
        }

        [Test]
        public void ConnectRooms_MultipleSeeds_ValidateConnectionsReturnsTrue()
        {
            // 複数シードで接続後の検証が必ず成功すること
            for (int seed = 0; seed < SEED_COUNT; seed++)
            {
                var rooms = CreateRooms(seed);
                var model = new RoomConnectionModel(new Random(seed));
                model.ConnectRooms(rooms);

                Assert.IsTrue(model.ValidateConnections(rooms), $"シード {seed}: ValidateConnections が false");
            }
        }

        [Test]
        public void FindPath_MultipleSeeds_PathExistsBetweenAllRoomPairs()
        {
            // 複数シードで任意の 2 部屋間に経路が存在すること
            for (int seed = 0; seed < SEED_COUNT; seed++)
            {
                var rooms = CreateRooms(seed);
                var model = new RoomConnectionModel(new Random(seed));
                model.ConnectRooms(rooms);

                var positions = rooms.Keys.ToList();
                foreach (var start in positions)
                {
                    foreach (var end in positions)
                    {
                        var path = model.FindPath(start, end);

                        Assert.IsNotEmpty(path, $"シード {seed}: {start} → {end} の経路がない");
                        Assert.AreEqual(start, path.First(), $"シード {seed}: 経路の始点が不正");
                        Assert.AreEqual(end, path.Last(), $"シード {seed}: 経路の終点が不正");
                    }
                }
            }
        }

        [Test]
        public void ConnectRooms_MultipleSeeds_DoorsAreOnOwnRoomPerimeter()
        {
            // 扉が自室の外周上（ワールドタイル座標）に配置されること
            for (int seed = 0; seed < SEED_COUNT; seed++)
            {
                var rooms = CreateRooms(seed);
                new RoomConnectionModel(new Random(seed)).ConnectRooms(rooms);

                foreach (var (position, room) in rooms)
                {
                    foreach (var door in room.Doors)
                    {
                        var local = door.Position - position;
                        bool onPerimeter =
                            local.X == 0 || local.X == DungeonConstants.ROOM_SIZE - 1 ||
                            local.Y == 0 || local.Y == DungeonConstants.ROOM_SIZE - 1;

                        Assert.That(local.X, Is.InRange(0, DungeonConstants.ROOM_SIZE - 1), $"シード {seed}: 扉が部屋外");
                        Assert.That(local.Y, Is.InRange(0, DungeonConstants.ROOM_SIZE - 1), $"シード {seed}: 扉が部屋外");
                        Assert.IsTrue(onPerimeter, $"シード {seed}: 扉 {door.Position} が部屋 {position} の外周上にない");
                    }
                }
            }
        }

        [Test]
        public void ConnectRooms_EmptyDictionary_DoesNotThrow()
        {
            // 空の辞書では何も起こらないこと
            var model = new RoomConnectionModel(new Random(0));
            var rooms = new Dictionary<Vector2I, RoomData>();

            Assert.DoesNotThrow(() => model.ConnectRooms(rooms));
            Assert.IsFalse(model.ValidateConnections(rooms));
        }

        [Test]
        public void ConnectRooms_SingleRoom_NoDoorsAndValid()
        {
            // 部屋 1 個では扉は追加されず、検証は成功すること
            var model = new RoomConnectionModel(new Random(0));
            var rooms = CreateRooms(0, count: 1);

            model.ConnectRooms(rooms);

            Assert.AreEqual(0, rooms.Values.First().Doors.Count);
            Assert.IsTrue(model.ValidateConnections(rooms));

            // 同一部屋への経路は自分自身のみ
            var position = rooms.Keys.First();
            CollectionAssert.AreEqual(new List<Vector2I> { position }, model.FindPath(position, position));
        }

        [Test]
        public void FindPath_BeforeConnectRooms_ReturnsEmptyList()
        {
            // ConnectRooms 前は経路が存在しないこと
            var model = new RoomConnectionModel(new Random(0));

            Assert.IsEmpty(model.FindPath(Vector2I.Zero, new Vector2I(16, 0)));
        }

        [Test]
        public void ConnectRooms_NullDictionary_ThrowsArgumentNullException()
        {
            var model = new RoomConnectionModel(new Random(0));

            Assert.Throws<ArgumentNullException>(() => model.ConnectRooms(null!));
        }

        [Test]
        public void Constructor_NullRandom_ThrowsArgumentNullException()
        {
            // 乱数の注入が必須であること
            Assert.Throws<ArgumentNullException>(() => _ = new RoomConnectionModel(null!));
        }
    }
}
