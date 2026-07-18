using Godot;

namespace Systems.Dungeon.TileMap
{
    /// <summary>
    /// タイルテンプレート
    /// 論理タイル種別（<see cref="TileType"/>）と、実際のタイルセット上のソースID・アトラス座標の対応を表す
    /// タイル種別の判定ロジックは持たず、単純な対応関係のみを保持する純粋なデータ保持クラス
    /// </summary>
    public class TileTemplate
    {
        /// <summary>
        /// タイルの論理種別
        /// </summary>
        public TileType Type { get; set; }

        /// <summary>
        /// タイルセット上のソースID
        /// </summary>
        public int SourceId { get; set; }

        /// <summary>
        /// タイルセット上のアトラス座標
        /// </summary>
        public Vector2I AtlasCoords { get; set; }
    }
}
