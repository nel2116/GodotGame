using System;
using System.Collections.Generic;
using Godot;
using Systems.Dungeon.Data;
using Systems.Dungeon.Utilities;

namespace Systems.Dungeon.Models
{
    /// <summary>
    /// 部屋レイアウト生成器
    /// 部屋（ROOM_SIZE × ROOM_SIZE タイル）の内部レイアウトを生成する
    /// 壁・床はタイル配列を保持しない暗黙表現（外周 = 壁（扉部を除く）、内部 = 床）とし、
    /// 部屋タイプに応じた障害物と扉の配置を <see cref="RoomTemplate"/> として返す
    /// </summary>
    public class RoomLayoutGenerator
    {
        /// <summary>
        /// レイアウト生成に使用する乱数（テスト再現性のため注入する）
        /// </summary>
        private readonly Random random;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="random">レイアウト生成に使用する乱数生成器</param>
        /// <exception cref="ArgumentNullException">random が null の場合</exception>
        public RoomLayoutGenerator(Random random)
        {
            this.random = random ?? throw new ArgumentNullException(nameof(random));
        }

        /// <summary>
        /// 部屋の内部レイアウトを生成する
        /// 部屋の扉位置をローカル座標へ変換し、部屋タイプに応じた障害物を内部領域へ配置する
        /// 生成完了後は room.IsGenerated を true にする
        /// </summary>
        /// <param name="room">レイアウトを生成する部屋データ</param>
        /// <returns>障害物・扉の配置（部屋ローカル座標）を保持する部屋テンプレート</returns>
        /// <exception cref="ArgumentNullException">room が null の場合</exception>
        public RoomTemplate GenerateLayout(RoomData room)
        {
            if (room == null)
            {
                throw new ArgumentNullException(nameof(room));
            }

            var template = new RoomTemplate { Type = room.Type };

            // 扉位置（ワールドタイル座標）を部屋ローカル座標（外周上）へ変換して保持する
            foreach (var door in room.Doors)
            {
                template.DoorPositions.Add(door.Position - room.Position);
            }

            // 部屋タイプに応じた障害物を配置する
            GenerateObstacles(room.Type, template);

            room.IsGenerated = true;
            return template;
        }

        /// <summary>
        /// 部屋タイプに応じた障害物を内部領域（外周の壁を除く 1..ROOM_SIZE-2）へ配置する
        /// 扉の前後を塞がないよう、扉の周囲 1 タイルには配置しない
        /// </summary>
        /// <param name="type">部屋の種類</param>
        /// <param name="template">配置結果を格納する部屋テンプレート</param>
        private void GenerateObstacles(RoomType type, RoomTemplate template)
        {
            int count = GetObstacleCount(type);
            if (count <= 0)
            {
                return;
            }

            // 扉の周囲 1 タイルを配置禁止にする（扉を塞がないため）
            var blocked = new HashSet<Vector2I>();
            foreach (var door in template.DoorPositions)
            {
                for (int dx = -1; dx <= 1; dx++)
                {
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        blocked.Add(door + new Vector2I(dx, dy));
                    }
                }
            }

            // 内部領域（外周の壁を除く）から配置候補を固定順で列挙する（シード再現性のため）
            var candidates = new List<Vector2I>();
            for (int x = 1; x <= DungeonConstants.ROOM_SIZE - 2; x++)
            {
                for (int y = 1; y <= DungeonConstants.ROOM_SIZE - 2; y++)
                {
                    var cell = new Vector2I(x, y);
                    if (!blocked.Contains(cell))
                    {
                        candidates.Add(cell);
                    }
                }
            }

            // 候補から重複なしでランダムに選択する
            for (int i = 0; i < count && candidates.Count > 0; i++)
            {
                int index = random.Next(candidates.Count);
                template.ObstaclePositions.Add(candidates[index]);
                candidates.RemoveAt(index);
            }
        }

        /// <summary>
        /// 部屋タイプごとの障害物数を取得する
        /// 開始部屋は安全地帯のため障害物を置かない
        /// </summary>
        /// <param name="type">部屋の種類</param>
        /// <returns>配置する障害物の数</returns>
        private static int GetObstacleCount(RoomType type)
        {
            return type switch
            {
                RoomType.Start => 0,
                RoomType.Combat => 4,
                RoomType.Treasure => 2,
                RoomType.Boss => 2,
                RoomType.Secret => 1,
                _ => 0
            };
        }
    }
}
