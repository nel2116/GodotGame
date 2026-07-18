using Systems.Dungeon.Data;
using Systems.Dungeon.Models;

namespace Systems.Dungeon.Optimization
{
    /// <summary>
    /// 部屋タイル描画の境界インターフェース
    /// Godotノード（TileMapLayer）への依存を切り離し、<see cref="RoomLifecycleManager"/> を単体テスト可能にする
    /// （既存の <see cref="Interfaces.ILevelGenerator"/>/<see cref="Interfaces.IRoomConnector"/> と同じ、テスト容易性のための境界パターン）
    /// </summary>
    public interface IRoomTileRenderer
    {
        /// <summary>
        /// 部屋のタイルを描画（実体化）する
        /// </summary>
        /// <param name="room">対象の部屋データ</param>
        /// <param name="template">対象の部屋テンプレート</param>
        void ApplyRoom(RoomData room, RoomTemplate template);

        /// <summary>
        /// 部屋のタイルを消去する
        /// </summary>
        /// <param name="room">対象の部屋データ</param>
        void ClearRoom(RoomData room);
    }
}
