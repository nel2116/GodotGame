namespace Systems.Dungeon.Data
{
    /// <summary>
    /// 部屋の種類
    /// </summary>
    public enum RoomType
    {
        /// <summary>開始部屋</summary>
        Start,

        /// <summary>戦闘部屋</summary>
        Combat,

        /// <summary>宝物部屋</summary>
        Treasure,

        /// <summary>ボス部屋</summary>
        Boss,

        /// <summary>隠し部屋</summary>
        Secret
    }

    /// <summary>
    /// 扉の種類
    /// </summary>
    public enum DoorType
    {
        /// <summary>通常の扉</summary>
        Normal,

        /// <summary>鍵付きの扉</summary>
        Locked,

        /// <summary>隠し扉</summary>
        Secret
    }

    /// <summary>
    /// ギミックの種類
    /// </summary>
    public enum GimmickType
    {
        /// <summary>隠し通路</summary>
        HiddenPassage,

        /// <summary>鍵扉</summary>
        LockedDoor,

        /// <summary>宝箱</summary>
        TreasureChest,

        /// <summary>罠</summary>
        Trap
    }
}
