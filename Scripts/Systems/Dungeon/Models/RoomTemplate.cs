using System.Collections.Generic;
using Godot;
using Systems.Dungeon.Data;

namespace Systems.Dungeon.Models
{
    /// <summary>
    /// 部屋テンプレート
    /// 部屋レイアウト生成の結果（障害物・扉の配置）を部屋ローカル座標で保持する
    /// 壁・床はタイル配列として保持せず「外周 = 壁（扉部を除く）、内部 = 床」の暗黙表現とする
    /// </summary>
    public class RoomTemplate
    {
        /// <summary>
        /// 部屋の種類
        /// </summary>
        public RoomType Type { get; set; }

        /// <summary>
        /// 障害物の位置一覧（部屋ローカル座標、内部領域 1..ROOM_SIZE-2）
        /// </summary>
        public List<Vector2I> ObstaclePositions { get; set; } = new();

        /// <summary>
        /// 扉の位置一覧（部屋ローカル座標、外周 0..ROOM_SIZE-1 上）
        /// </summary>
        public List<Vector2I> DoorPositions { get; set; } = new();
    }
}
