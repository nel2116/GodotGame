using Godot;

namespace Systems.Dungeon.Data
{
    /// <summary>
    /// 扉データ
    /// 部屋間の接続点となる扉の位置・種類・状態を保持する
    /// </summary>
    public class DoorData
    {
        /// <summary>
        /// 扉の位置（タイル座標）
        /// </summary>
        public Vector2I Position { get; set; }

        /// <summary>
        /// 扉の種類
        /// </summary>
        public DoorType Type { get; set; }

        /// <summary>
        /// 施錠されているかどうか
        /// </summary>
        public bool IsLocked { get; set; }

        /// <summary>
        /// 接続先の部屋の位置
        /// </summary>
        public Vector2I ConnectedRoomPosition { get; set; }
    }
}
