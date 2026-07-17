using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using Systems.Dungeon.Data;
using Systems.Dungeon.Interfaces;
using Systems.Dungeon.Utilities;

namespace Systems.Dungeon.Models
{
    /// <summary>
    /// レベル生成モデル
    /// 部屋位置の配置・部屋タイプの割り当て・部屋接続・レイアウト生成を統括し、
    /// ダンジョン 1 フロア分の <see cref="RoomData"/> 群を生成する
    /// </summary>
    public class LevelGenerationModel : ILevelGenerator
    {
        /// <summary>
        /// 部屋位置の配置・部屋タイプ割り当てに使用する乱数（SetSeed で再生成される）
        /// </summary>
        private Random random = null!;

        /// <summary>
        /// 部屋レイアウトの生成器（SetSeed のたびに新しい乱数で再生成される）
        /// </summary>
        private RoomLayoutGenerator roomLayoutGenerator = null!;

        /// <summary>
        /// 部屋接続モデル（SetSeed のたびに新しい乱数で再生成される）
        /// </summary>
        private RoomConnectionModel roomConnectionModel = null!;

        /// <summary>
        /// 直近の GenerateLevelAsync で生成された部屋テンプレート（部屋位置がキー）
        /// RoomData を変更せずに障害物・扉のローカル座標情報を公開するためのプロパティ
        /// </summary>
        public IReadOnlyDictionary<Vector2I, RoomTemplate> RoomTemplates { get; private set; } = new Dictionary<Vector2I, RoomTemplate>();

        /// <summary>
        /// コンストラクタ（デフォルトシード）
        /// 実行時刻に基づく非決定的なシードで初期化する
        /// </summary>
        public LevelGenerationModel() : this(System.Environment.TickCount)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="seed">乱数シード値</param>
        public LevelGenerationModel(int seed)
        {
            SetSeed(seed);
        }

        /// <summary>
        /// 乱数シードを設定する
        /// 内部の乱数・部屋レイアウト生成器・部屋接続モデルをすべて再生成し、生成状態をリセットする
        /// </summary>
        /// <param name="seed">乱数シード値</param>
        public void SetSeed(int seed)
        {
            random = new Random(seed);
            roomLayoutGenerator = new RoomLayoutGenerator(random);
            roomConnectionModel = new RoomConnectionModel(random);
            RoomTemplates = new Dictionary<Vector2I, RoomTemplate>();
        }

        /// <summary>
        /// レベルを非同期で生成する
        /// 部屋位置の生成 → 部屋タイプ割り当て → 部屋接続 → レイアウト生成の順に処理し、
        /// ValidateLevel に合格するまで最大 MAX_CONNECTION_ATTEMPTS 回まで再生成する
        /// 処理は軽量な同期処理のため Task.Run は使用しない
        /// </summary>
        /// <returns>部屋位置をキーとした部屋データの辞書</returns>
        /// <exception cref="InvalidOperationException">最大試行回数を超えても有効なレベルを生成できなかった場合</exception>
        public Task<Dictionary<Vector2I, RoomData>> GenerateLevelAsync()
        {
            for (int attempt = 0; attempt < DungeonConstants.MAX_CONNECTION_ATTEMPTS; attempt++)
            {
                var rooms = GenerateRoomPositions();
                AssignRoomTypes(rooms);

                // 扉決定（ConnectRooms）→ レイアウト生成の順で処理する
                // RoomLayoutGenerator.GenerateLayout は room.Doors が確定済みであることを前提に
                // 扉のワールド座標を部屋ローカル座標へ変換するため、この順序が必須となる
                roomConnectionModel.ConnectRooms(rooms);
                var templates = GenerateLayouts(rooms);

                if (ValidateLevel(rooms))
                {
                    RoomTemplates = templates;
                    return Task.FromResult(rooms);
                }
            }

            throw new InvalidOperationException(
                $"最大試行回数（{DungeonConstants.MAX_CONNECTION_ATTEMPTS}）を超えても有効なレベルを生成できませんでした。");
        }

        /// <summary>
        /// 生成されたレベルが要件を満たしているか検証する
        /// 部屋数・開始部屋の位置・ボス部屋の一意性・全部屋の連結性・開始部屋からボス部屋への経路を確認する
        /// </summary>
        /// <param name="rooms">検証対象の部屋データの辞書</param>
        /// <returns>検証に合格した場合は true</returns>
        public bool ValidateLevel(Dictionary<Vector2I, RoomData> rooms)
        {
            if (rooms == null || rooms.Count != DungeonConstants.ROOM_COUNT)
            {
                return false;
            }

            if (!rooms.TryGetValue(Vector2I.Zero, out var startRoom) || startRoom.Type != RoomType.Start)
            {
                return false;
            }

            if (rooms.Values.Count(room => room.Type == RoomType.Boss) != 1)
            {
                return false;
            }

            if (!roomConnectionModel.ValidateConnections(rooms))
            {
                return false;
            }

            var bossPosition = rooms.First(kvp => kvp.Value.Type == RoomType.Boss).Key;
            return roomConnectionModel.FindPath(Vector2I.Zero, bossPosition).Count > 0;
        }

        /// <summary>
        /// 部屋位置を生成する
        /// 開始部屋を原点に固定し、残り ROOM_COUNT - 1 部屋を隣接空きセルから成長させる方式で選び、
        /// 全部屋がグリッド上（上下左右隣接）で連結になるように配置する
        /// </summary>
        /// <returns>部屋位置をキーとした部屋データの辞書（タイプ未割り当て）</returns>
        private Dictionary<Vector2I, RoomData> GenerateRoomPositions()
        {
            var placed = new HashSet<Vector2I> { Vector2I.Zero };
            var frontier = new HashSet<Vector2I>();
            AddNeighborsToFrontier(Vector2I.Zero, placed, frontier);

            while (placed.Count < DungeonConstants.ROOM_COUNT)
            {
                // HashSet の列挙順に依存しないよう固定順に並べてから乱数で選ぶ（シード再現性のため）
                var candidates = frontier.OrderBy(p => p.X).ThenBy(p => p.Y).ToList();
                var chosen = candidates[random.Next(candidates.Count)];

                frontier.Remove(chosen);
                placed.Add(chosen);
                AddNeighborsToFrontier(chosen, placed, frontier);
            }

            var rooms = new Dictionary<Vector2I, RoomData>();
            foreach (var position in placed)
            {
                rooms[position] = new RoomData
                {
                    Position = position,
                    Size = new Vector2I(DungeonConstants.ROOM_SIZE, DungeonConstants.ROOM_SIZE)
                };
            }

            return rooms;
        }

        /// <summary>
        /// 指定位置のグリッド隣接セルのうち、未配置かつ配置範囲内のものを配置候補集合へ追加する
        /// </summary>
        /// <param name="position">基準となる部屋位置</param>
        /// <param name="placed">配置済みの部屋位置の集合</param>
        /// <param name="frontier">配置候補の部屋位置の集合</param>
        private static void AddNeighborsToFrontier(Vector2I position, HashSet<Vector2I> placed, HashSet<Vector2I> frontier)
        {
            foreach (var neighbor in GetGridNeighbors(position))
            {
                if (!placed.Contains(neighbor) && DungeonUtils.IsValidRoomPosition(neighbor))
                {
                    frontier.Add(neighbor);
                }
            }
        }

        /// <summary>
        /// 指定位置の上下左右にグリッド隣接する部屋位置を列挙する
        /// </summary>
        /// <param name="position">基準となる部屋位置</param>
        /// <returns>上下左右に隣接する 4 つの部屋位置</returns>
        private static IEnumerable<Vector2I> GetGridNeighbors(Vector2I position)
        {
            yield return position + new Vector2I(DungeonConstants.ROOM_SIZE, 0);
            yield return position + new Vector2I(-DungeonConstants.ROOM_SIZE, 0);
            yield return position + new Vector2I(0, DungeonConstants.ROOM_SIZE);
            yield return position + new Vector2I(0, -DungeonConstants.ROOM_SIZE);
        }

        /// <summary>
        /// 部屋タイプを割り当てる
        /// 原点を Start、Start からグリッド距離が最も遠い部屋を Boss とし、
        /// 残り 6 部屋には Combat×4・Treasure×1・Secret×1 をランダムに割り当てる
        /// </summary>
        /// <param name="rooms">部屋データの辞書（位置は生成済み、タイプは未割り当て）</param>
        private void AssignRoomTypes(Dictionary<Vector2I, RoomData> rooms)
        {
            rooms[Vector2I.Zero].Type = RoomType.Start;

            var bossPosition = FindFarthestRoomPosition(rooms, Vector2I.Zero);
            rooms[bossPosition].Type = RoomType.Boss;

            // 固定順に並べてから乱数でタイプ配列をシャッフルする（シード再現性のため）
            var remainingPositions = rooms.Keys
                .Where(position => position != Vector2I.Zero && position != bossPosition)
                .OrderBy(position => position.X).ThenBy(position => position.Y)
                .ToList();

            var typePool = new List<RoomType>
            {
                RoomType.Combat, RoomType.Combat, RoomType.Combat, RoomType.Combat,
                RoomType.Treasure,
                RoomType.Secret
            };
            ShuffleTypePool(typePool);

            for (int i = 0; i < remainingPositions.Count; i++)
            {
                rooms[remainingPositions[i]].Type = typePool[i];
            }
        }

        /// <summary>
        /// Fisher-Yates 法でタイプ一覧をシャッフルする
        /// </summary>
        /// <param name="typePool">シャッフル対象の部屋タイプ一覧</param>
        private void ShuffleTypePool(List<RoomType> typePool)
        {
            for (int i = typePool.Count - 1; i > 0; i--)
            {
                int j = random.Next(i + 1);
                (typePool[i], typePool[j]) = (typePool[j], typePool[i]);
            }
        }

        /// <summary>
        /// 開始位置からグリッド隣接のみを辿った幅優先探索で、最もホップ数が遠い部屋位置を求める
        /// </summary>
        /// <param name="rooms">部屋データの辞書</param>
        /// <param name="start">探索の開始位置</param>
        /// <returns>開始位置からグリッド距離が最も遠い部屋位置（複数ある場合は座標順で先頭）</returns>
        private static Vector2I FindFarthestRoomPosition(Dictionary<Vector2I, RoomData> rooms, Vector2I start)
        {
            var distances = new Dictionary<Vector2I, int> { [start] = 0 };
            var queue = new Queue<Vector2I>();
            queue.Enqueue(start);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                foreach (var neighbor in GetGridNeighbors(current))
                {
                    if (rooms.ContainsKey(neighbor) && !distances.ContainsKey(neighbor))
                    {
                        distances[neighbor] = distances[current] + 1;
                        queue.Enqueue(neighbor);
                    }
                }
            }

            return distances
                .OrderByDescending(kvp => kvp.Value)
                .ThenBy(kvp => kvp.Key.X)
                .ThenBy(kvp => kvp.Key.Y)
                .First().Key;
        }

        /// <summary>
        /// 各部屋のレイアウトを生成する
        /// 部屋の扉（ConnectRooms 済み）を部屋ローカル座標へ変換し、部屋タイプに応じた障害物を配置する
        /// </summary>
        /// <param name="rooms">扉決定済みの部屋データの辞書</param>
        /// <returns>部屋位置をキーとした部屋テンプレートの辞書</returns>
        private Dictionary<Vector2I, RoomTemplate> GenerateLayouts(Dictionary<Vector2I, RoomData> rooms)
        {
            var templates = new Dictionary<Vector2I, RoomTemplate>();

            // 固定順（座標順）で処理し、シード再現性を確保する
            foreach (var position in rooms.Keys.OrderBy(p => p.X).ThenBy(p => p.Y))
            {
                templates[position] = roomLayoutGenerator.GenerateLayout(rooms[position]);
            }

            return templates;
        }
    }
}
