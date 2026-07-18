using Godot;

namespace Systems.Dungeon.Utilities
{
    /// <summary>
    /// ダンジョン生成に関するユーティリティ
    /// </summary>
    public static class DungeonUtils
    {
        /// <summary>
        /// 2 点間のユークリッド距離を計算する
        /// </summary>
        /// <param name="pos1">位置 1</param>
        /// <param name="pos2">位置 2</param>
        /// <returns>2 点間の距離</returns>
        public static float CalculateDistance(Vector2I pos1, Vector2I pos2)
        {
            return ((Vector2)pos1).DistanceTo((Vector2)pos2);
        }

        /// <summary>
        /// ワールド座標からその座標を含む部屋の位置（部屋グリッドに整列した左上座標）を計算する
        /// </summary>
        /// <param name="worldPosition">ワールド座標</param>
        /// <returns>部屋の位置（ROOM_SIZE の倍数に切り下げ）</returns>
        public static Vector2I CalculateRoomPosition(Vector2 worldPosition)
        {
            return new Vector2I(
                Mathf.FloorToInt(worldPosition.X / DungeonConstants.ROOM_SIZE) * DungeonConstants.ROOM_SIZE,
                Mathf.FloorToInt(worldPosition.Y / DungeonConstants.ROOM_SIZE) * DungeonConstants.ROOM_SIZE
            );
        }

        /// <summary>
        /// 部屋の位置として有効か判定する
        /// 部屋グリッド（ROOM_SIZE の倍数）に整列し、かつ配置範囲内であることを確認する
        /// </summary>
        /// <param name="position">判定対象の位置</param>
        /// <returns>有効な部屋位置の場合は true</returns>
        public static bool IsValidRoomPosition(Vector2I position)
        {
            // 部屋グリッドに整列しているか
            if (position.X % DungeonConstants.ROOM_SIZE != 0 || position.Y % DungeonConstants.ROOM_SIZE != 0)
            {
                return false;
            }

            // 配置範囲内か（原点を中心とした GENERATION_GRID_RANGE グリッド以内）
            int maxCoordinate = DungeonConstants.GENERATION_GRID_RANGE * DungeonConstants.ROOM_SIZE;
            return position.X >= -maxCoordinate && position.X <= maxCoordinate &&
                   position.Y >= -maxCoordinate && position.Y <= maxCoordinate;
        }
    }
}
