namespace Systems.Dungeon.TileMap
{
    /// <summary>
    /// タイルの論理種別
    /// 実際のタイルセット上のソースID・アトラス座標とは独立して、タイルの役割を表す
    /// </summary>
    public enum TileType
    {
        /// <summary>床</summary>
        Floor,

        /// <summary>壁</summary>
        Wall,

        /// <summary>通常の扉</summary>
        Door,

        /// <summary>鍵扉</summary>
        LockedDoor,

        /// <summary>未発見の隠し通路（見た目は壁として扱う）</summary>
        SecretWall,

        /// <summary>障害物</summary>
        Obstacle
    }
}
