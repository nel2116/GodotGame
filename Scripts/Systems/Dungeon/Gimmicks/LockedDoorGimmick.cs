using Systems.Dungeon.Data;

namespace Systems.Dungeon.Gimmicks
{
    /// <summary>
    /// 鍵扉ギミック
    /// 鍵を所持している場合のみ解錠できる扉ギミックのドメインロジックを表す
    /// </summary>
    public static class LockedDoorGimmick
    {
        /// <summary>
        /// このギミックの効果を説明するテキスト
        /// </summary>
        public const string Description = "鍵のかかった扉。鍵を所持した状態で発動すると解錠される。";

        /// <summary>
        /// 指定したギミックが発動（解錠）可能かどうかを判定する
        /// </summary>
        /// <param name="gimmick">判定対象のギミックデータ（null の場合は発動不可）</param>
        /// <param name="hasKey">鍵を所持しているかどうか</param>
        /// <returns>鍵を所持しており、種類が鍵扉（<see cref="GimmickType.LockedDoor"/>）かつ未発動であれば true</returns>
        public static bool CanActivate(GimmickData? gimmick, bool hasKey)
        {
            return hasKey && gimmick != null && gimmick.Type == GimmickType.LockedDoor && !gimmick.IsActive;
        }
    }
}
