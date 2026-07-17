using System.Collections.Generic;
using Godot;

namespace Systems.Dungeon.TileMap
{
    /// <summary>
    /// タイルセット管理
    /// <see cref="TileType"/> から <see cref="TileTemplate"/>（ソースID・アトラス座標）へのマッピングを管理する
    /// 実際の .tres タイルセット資産がなくても検証できるよう、マッピングをコンストラクタから外部注入できる設計とする
    /// </summary>
    public class TileSetManager
    {
        /// <summary>
        /// タイル種別からタイルテンプレートへのマッピング
        /// </summary>
        private readonly IReadOnlyDictionary<TileType, TileTemplate> templates;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="templates">
        /// タイル種別ごとのテンプレートのマッピング。省略した場合は <see cref="CreateDefaultTemplates"/> による既定マッピングを使用する
        /// </param>
        public TileSetManager(IReadOnlyDictionary<TileType, TileTemplate>? templates = null)
        {
            this.templates = templates ?? CreateDefaultTemplates();
        }

        /// <summary>
        /// 指定したタイル種別に対応するタイルテンプレートを取得する
        /// </summary>
        /// <param name="type">取得対象のタイル種別</param>
        /// <returns>対応するタイルテンプレート</returns>
        /// <exception cref="KeyNotFoundException">対応するテンプレートが登録されていない場合</exception>
        public TileTemplate GetTemplate(TileType type)
        {
            return templates[type];
        }

        /// <summary>
        /// 既定のタイル種別マッピングを生成する
        /// 実際のタイルセット資産（アトラス座標）は未確定のため、ソースID 0 上の仮のアトラス座標を割り当てる
        /// 見た目上の壁として扱う <see cref="TileType.SecretWall"/> は <see cref="TileType.Wall"/> と同じアトラス座標とする
        /// </summary>
        /// <returns>タイル種別ごとの既定タイルテンプレートのマッピング</returns>
        private static IReadOnlyDictionary<TileType, TileTemplate> CreateDefaultTemplates()
        {
            return new Dictionary<TileType, TileTemplate>
            {
                [TileType.Floor] = new TileTemplate { Type = TileType.Floor, SourceId = 0, AtlasCoords = new Vector2I(0, 0) },
                [TileType.Wall] = new TileTemplate { Type = TileType.Wall, SourceId = 0, AtlasCoords = new Vector2I(1, 0) },
                [TileType.Door] = new TileTemplate { Type = TileType.Door, SourceId = 0, AtlasCoords = new Vector2I(2, 0) },
                [TileType.LockedDoor] = new TileTemplate { Type = TileType.LockedDoor, SourceId = 0, AtlasCoords = new Vector2I(3, 0) },
                [TileType.SecretWall] = new TileTemplate { Type = TileType.SecretWall, SourceId = 0, AtlasCoords = new Vector2I(1, 0) },
                [TileType.Obstacle] = new TileTemplate { Type = TileType.Obstacle, SourceId = 0, AtlasCoords = new Vector2I(4, 0) }
            };
        }
    }
}
