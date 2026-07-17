using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Systems.Dungeon.Data
{
    /// <summary>
    /// 部屋データ
    /// ダンジョンを構成する 1 部屋分の位置・サイズ・種類・扉・ギミックを保持する
    /// </summary>
    public class RoomData
    {
        /// <summary>
        /// 部屋の位置（ワールド座標、部屋の左上基準）
        /// </summary>
        public Vector2I Position { get; set; }

        /// <summary>
        /// 部屋のサイズ（タイル数）
        /// </summary>
        public Vector2I Size { get; set; }

        /// <summary>
        /// 部屋の種類
        /// </summary>
        public RoomType Type { get; set; }

        /// <summary>
        /// 部屋に属する扉の一覧
        /// </summary>
        public List<DoorData> Doors { get; set; } = new();

        /// <summary>
        /// 部屋に配置されたギミックの一覧
        /// </summary>
        public List<GimmickData> Gimmicks { get; set; } = new();

        /// <summary>
        /// レイアウト生成が完了しているかどうか
        /// </summary>
        public bool IsGenerated { get; set; }

        /// <summary>
        /// 扉を追加する
        /// </summary>
        /// <param name="door">追加する扉データ</param>
        public void AddDoor(DoorData door)
        {
            Doors.Add(door);
        }

        /// <summary>
        /// ギミックを追加する
        /// </summary>
        /// <param name="gimmick">追加するギミックデータ</param>
        public void AddGimmick(GimmickData gimmick)
        {
            Gimmicks.Add(gimmick);
        }

        /// <summary>
        /// 指定した接続先部屋位置に対応する扉を取得する
        /// </summary>
        /// <param name="connectedRoomPosition">接続先の部屋の位置</param>
        /// <returns>対応する扉データ。見つからない場合は null</returns>
        public DoorData? GetDoorTo(Vector2I connectedRoomPosition)
        {
            return Doors.FirstOrDefault(d => d.ConnectedRoomPosition == connectedRoomPosition);
        }
    }
}
