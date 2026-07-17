namespace Systems.Dungeon.Utilities
{
    /// <summary>
    /// ダンジョン生成に関する定数定義
    /// </summary>
    public static class DungeonConstants
    {
        /// <summary>
        /// 部屋の一辺のサイズ（タイル数）
        /// </summary>
        public const int ROOM_SIZE = 16;

        /// <summary>
        /// 1 フロアあたりの部屋数
        /// </summary>
        public const int ROOM_COUNT = 8;

        /// <summary>
        /// 部屋接続の最大試行回数
        /// </summary>
        public const int MAX_CONNECTION_ATTEMPTS = 100;

        /// <summary>
        /// 部屋間の最小距離
        /// </summary>
        public const float MIN_ROOM_DISTANCE = 16.0f;

        /// <summary>
        /// 部屋配置グリッドの範囲（原点を中心に -GENERATION_GRID_RANGE 〜 +GENERATION_GRID_RANGE）
        /// </summary>
        public const int GENERATION_GRID_RANGE = 3;
    }
}
