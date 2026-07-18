using Godot;

namespace Systems.Dungeon.Data
{
    /// <summary>
    /// ギミックデータ
    /// 部屋内に配置されるギミック（隠し通路・鍵扉・宝箱・罠）の情報を保持する
    /// </summary>
    public class GimmickData
    {
        /// <summary>
        /// ギミックの位置（タイル座標）
        /// </summary>
        public Vector2I Position { get; set; }

        /// <summary>
        /// ギミックの種類
        /// </summary>
        public GimmickType Type { get; set; }

        /// <summary>
        /// ギミックが有効かどうか
        /// </summary>
        public bool IsActive { get; set; }
    }
}
