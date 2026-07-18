using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Godot;
using Systems.Dungeon.Data;
using Systems.Dungeon.Models;
using Systems.Dungeon.Utilities;

namespace Tests.Core.Dungeon
{
    public class LevelGenerationModelTests
    {
        /// <summary>
        /// 性質ベーステストで使用するシード数
        /// </summary>
        private const int SEED_COUNT = 10;

        /// <summary>
        /// 部屋 1 個分の比較用シグネチャ（位置・タイプ・扉情報を固定順に並べたもの）
        /// </summary>
        private static (Vector2I Position, RoomType Type, List<(Vector2I DoorPosition, DoorType DoorType, bool IsLocked, Vector2I ConnectedTo)> Doors)
            BuildRoomSignature(Vector2I position, RoomData room)
        {
            var doors = room.Doors
                .Select(d => (d.Position, d.Type, d.IsLocked, d.ConnectedRoomPosition))
                .OrderBy(d => d.Position.X).ThenBy(d => d.Position.Y)
                .ToList();

            return (position, room.Type, doors);
        }

        /// <summary>
        /// 生成結果全体（部屋データ + 部屋テンプレート）を座標順に並べた比較用シグネチャ一覧に変換する
        /// </summary>
        private static object BuildLevelSignature(Dictionary<Vector2I, RoomData> rooms, LevelGenerationModel model)
        {
            var roomSignatures = rooms
                .OrderBy(kvp => kvp.Key.X).ThenBy(kvp => kvp.Key.Y)
                .Select(kvp => BuildRoomSignature(kvp.Key, kvp.Value))
                .ToList();

            var templateSignatures = model.RoomTemplates
                .OrderBy(kvp => kvp.Key.X).ThenBy(kvp => kvp.Key.Y)
                .Select(kvp => (
                    kvp.Key,
                    kvp.Value.Type,
                    Obstacles: kvp.Value.ObstaclePositions.OrderBy(p => p.X).ThenBy(p => p.Y).ToList(),
                    Doors: kvp.Value.DoorPositions.OrderBy(p => p.X).ThenBy(p => p.Y).ToList()))
                .ToList();

            return (roomSignatures, templateSignatures);
        }

        [Test]
        public async Task GenerateLevel_CreatesCorrectNumberOfRooms()
        {
            // 8 部屋が生成されること
            var model = new LevelGenerationModel(0);
            var rooms = await model.GenerateLevelAsync();

            Assert.AreEqual(DungeonConstants.ROOM_COUNT, rooms.Count);
        }

        [Test]
        public async Task GenerateLevel_StartRoomIsAtOrigin()
        {
            // 開始部屋が原点 (0, 0) に存在すること
            var model = new LevelGenerationModel(0);
            var rooms = await model.GenerateLevelAsync();

            Assert.IsTrue(rooms.ContainsKey(Vector2I.Zero));
            Assert.AreEqual(RoomType.Start, rooms[Vector2I.Zero].Type);
        }

        [Test]
        public async Task GenerateLevel_AllRoomsHaveValidTypes()
        {
            // Start がちょうど 1 つ、Boss がちょうど 1 つ、残りが Combat×4・Treasure×1・Secret×1 の分布になること
            var model = new LevelGenerationModel(0);
            var rooms = await model.GenerateLevelAsync();

            Assert.AreEqual(1, rooms.Values.Count(r => r.Type == RoomType.Start));
            Assert.AreEqual(1, rooms.Values.Count(r => r.Type == RoomType.Boss));
            Assert.AreEqual(4, rooms.Values.Count(r => r.Type == RoomType.Combat));
            Assert.AreEqual(1, rooms.Values.Count(r => r.Type == RoomType.Treasure));
            Assert.AreEqual(1, rooms.Values.Count(r => r.Type == RoomType.Secret));
        }

        [Test]
        public async Task GenerateLevel_WithSameSeed_GeneratesSameLayout()
        {
            // 同一シードなら部屋位置・タイプ・扉・レイアウトまで完全に一致すること
            var model1 = new LevelGenerationModel(123);
            var rooms1 = await model1.GenerateLevelAsync();

            var model2 = new LevelGenerationModel(123);
            var rooms2 = await model2.GenerateLevelAsync();

            var signature1 = BuildLevelSignature(rooms1, model1);
            var signature2 = BuildLevelSignature(rooms2, model2);

            Assert.AreEqual(signature1, signature2);
        }

        [Test]
        public async Task GenerateLevel_WithDifferentSeed_GeneratesDifferentLayout()
        {
            // 異なるシードなら少なくとも 1 要素が異なること
            var model1 = new LevelGenerationModel(1);
            var rooms1 = await model1.GenerateLevelAsync();

            var model2 = new LevelGenerationModel(2);
            var rooms2 = await model2.GenerateLevelAsync();

            var signature1 = BuildLevelSignature(rooms1, model1);
            var signature2 = BuildLevelSignature(rooms2, model2);

            Assert.AreNotEqual(signature1, signature2);
        }

        [Test]
        public async Task GenerateLevel_MultipleSeeds_ValidateLevelReturnsTrue()
        {
            // 複数シードで ValidateLevel が true になること
            for (int seed = 0; seed < SEED_COUNT; seed++)
            {
                var model = new LevelGenerationModel(seed);
                var rooms = await model.GenerateLevelAsync();

                Assert.IsTrue(model.ValidateLevel(rooms), $"シード {seed}: ValidateLevel が false");
            }
        }

        [Test]
        public async Task GenerateLevel_MultipleSeeds_AllRoomsAreConnected()
        {
            // 複数シードで全部屋が扉のグラフ上で連結になること
            for (int seed = 0; seed < SEED_COUNT; seed++)
            {
                var model = new LevelGenerationModel(seed);
                var rooms = await model.GenerateLevelAsync();

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

                Assert.AreEqual(rooms.Count, visited.Count, $"シード {seed}: 全部屋が連結でない");
            }
        }

        [Test]
        public async Task GenerateLevel_MultipleSeeds_PathExistsFromStartToBoss()
        {
            // 複数シードで Start → Boss の経路が存在すること
            for (int seed = 0; seed < SEED_COUNT; seed++)
            {
                var model = new LevelGenerationModel(seed);
                var rooms = await model.GenerateLevelAsync();
                var bossPosition = rooms.First(kvp => kvp.Value.Type == RoomType.Boss).Key;

                // ConnectRooms 済みの内部状態を利用するため IRoomConnector 経由ではなく
                // 扉グラフを自前で辿って経路存在を確認する
                var start = Vector2I.Zero;
                var visited = new HashSet<Vector2I> { start };
                var queue = new Queue<Vector2I>();
                queue.Enqueue(start);
                bool reached = start == bossPosition;

                while (queue.Count > 0 && !reached)
                {
                    foreach (var door in rooms[queue.Dequeue()].Doors)
                    {
                        if (!rooms.ContainsKey(door.ConnectedRoomPosition) || !visited.Add(door.ConnectedRoomPosition))
                        {
                            continue;
                        }

                        if (door.ConnectedRoomPosition == bossPosition)
                        {
                            reached = true;
                            break;
                        }

                        queue.Enqueue(door.ConnectedRoomPosition);
                    }
                }

                Assert.IsTrue(reached, $"シード {seed}: Start → Boss の経路がない");
            }
        }

        [Test]
        public async Task GenerateLevel_MultipleSeeds_AllDoorsAreSymmetric()
        {
            // 複数シードで全扉が対称（相手側にも自分を指す扉がある）こと
            for (int seed = 0; seed < SEED_COUNT; seed++)
            {
                var model = new LevelGenerationModel(seed);
                var rooms = await model.GenerateLevelAsync();

                foreach (var (position, room) in rooms)
                {
                    foreach (var door in room.Doors)
                    {
                        Assert.IsTrue(rooms.ContainsKey(door.ConnectedRoomPosition),
                            $"シード {seed}: 扉の接続先 {door.ConnectedRoomPosition} が存在しない");

                        var paired = rooms[door.ConnectedRoomPosition].Doors
                            .Any(d => d.ConnectedRoomPosition == position);
                        Assert.IsTrue(paired, $"シード {seed}: 部屋 {position} → {door.ConnectedRoomPosition} の対の扉がない");
                    }
                }
            }
        }

        [Test]
        public async Task GenerateLevel_MultipleSeeds_AllRoomsAreGenerated()
        {
            // 複数シードで全部屋の IsGenerated が true になること
            for (int seed = 0; seed < SEED_COUNT; seed++)
            {
                var model = new LevelGenerationModel(seed);
                var rooms = await model.GenerateLevelAsync();

                Assert.IsTrue(rooms.Values.All(r => r.IsGenerated), $"シード {seed}: IsGenerated が false の部屋がある");
            }
        }

        [Test]
        public async Task GenerateLevel_MultipleSeeds_RoomTemplatesCoverAllRooms()
        {
            // 複数シードで RoomTemplates が全部屋分そろっていること
            for (int seed = 0; seed < SEED_COUNT; seed++)
            {
                var model = new LevelGenerationModel(seed);
                var rooms = await model.GenerateLevelAsync();

                Assert.AreEqual(rooms.Count, model.RoomTemplates.Count, $"シード {seed}: RoomTemplates の件数が一致しない");
                foreach (var position in rooms.Keys)
                {
                    Assert.IsTrue(model.RoomTemplates.ContainsKey(position), $"シード {seed}: 部屋 {position} のテンプレートがない");
                }
            }
        }

        [Test]
        public async Task SetSeed_RegeneratesAccordingToNewSeed()
        {
            // SetSeed 後の再生成が新しいシードに従うこと（同じシードなら同じ結果、異なるシードなら異なる結果になる）
            var model = new LevelGenerationModel(1);
            _ = await model.GenerateLevelAsync();

            model.SetSeed(999);
            var roomsA = await model.GenerateLevelAsync();
            var signatureA = BuildLevelSignature(roomsA, model);

            model.SetSeed(999);
            var roomsB = await model.GenerateLevelAsync();
            var signatureB = BuildLevelSignature(roomsB, model);

            var referenceModel = new LevelGenerationModel(999);
            var referenceRooms = await referenceModel.GenerateLevelAsync();
            var referenceSignature = BuildLevelSignature(referenceRooms, referenceModel);

            Assert.AreEqual(signatureA, signatureB, "同一シードでの再生成結果が一致しない");
            Assert.AreEqual(referenceSignature, signatureA, "SetSeed の結果が同一シードのコンストラクタ生成と一致しない");
        }
    }
}
