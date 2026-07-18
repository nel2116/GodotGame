using Systems.Dungeon.Data;

namespace Systems.Dungeon.Gimmicks
{
    /// <summary>
    /// 隠し通路ギミック
    /// 発見されるまでは隠し扉として振る舞い、発動すると通常の扉として開通するギミックのドメインロジックを表す
    /// </summary>
    public static class HiddenPassageGimmick
    {
        /// <summary>
        /// このギミックの効果を説明するテキスト
        /// </summary>
        public const string Description = "隠された通路。発見して発動すると通常の扉として開通する。";

        /// <summary>
        /// 指定したギミックが発動可能かどうかを判定する
        /// </summary>
        /// <param name="gimmick">判定対象のギミックデータ（null の場合は発動不可）</param>
        /// <returns>種類が隠し通路（<see cref="GimmickType.HiddenPassage"/>）かつ未発動であれば true</returns>
        public static bool CanActivate(GimmickData? gimmick)
        {
            return gimmick != null && gimmick.Type == GimmickType.HiddenPassage && !gimmick.IsActive;
        }
    }
}
