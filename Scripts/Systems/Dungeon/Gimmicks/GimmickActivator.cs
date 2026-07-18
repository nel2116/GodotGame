using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Godot;
using Systems.Dungeon.Data;

namespace Systems.Dungeon.Gimmicks
{
    /// <summary>
    /// ギミック発動管理
    /// 配置済みギミック（隠し通路・鍵扉）の発動可否判定と状態遷移を担う
    /// イベント発行は行わず、戻り値と部屋データの状態変化のみで結果を表現する
    /// </summary>
    public class GimmickActivator
    {
        /// <summary>
        /// 隠し通路ギミックを発動する
        /// 発動に成功すると、対象ギミックと接続先の対応するギミックの両方を有効化し、
        /// 対応する扉（自室・接続先の両側）を通常の扉（<see cref="DoorType.Normal"/>）に変更する
        /// 接続先の部屋・扉・ギミックが見つからない場合は、どちらの側も変更せずに失敗を返す
        /// </summary>
        /// <param name="rooms">部屋データの辞書（部屋位置がキー）</param>
        /// <param name="roomPosition">ギミックが属する部屋の位置</param>
        /// <param name="gimmickPosition">発動対象のギミックの位置</param>
        /// <returns>発動に成功した場合は true。ギミックが存在しない、種類が異なる、既に発動済み、接続先の対応データが見つからない場合は false</returns>
        public bool TryActivateHiddenPassage(Dictionary<Vector2I, RoomData> rooms, Vector2I roomPosition, Vector2I gimmickPosition)
        {
            if (!TryFindGimmick(rooms, roomPosition, gimmickPosition, out var room, out var gimmick) ||
                !HiddenPassageGimmick.CanActivate(gimmick))
            {
                return false;
            }

            var door = room.Doors.FirstOrDefault(d => d.Position == gimmickPosition);
            if (door == null ||
                !TryFindConnectedGimmick(rooms, roomPosition, door, GimmickType.HiddenPassage, out var connectedDoor, out var connectedGimmick))
            {
                return false;
            }

            gimmick.IsActive = true;
            door.Type = DoorType.Normal;
            connectedDoor.Type = DoorType.Normal;
            connectedGimmick.IsActive = true;

            return true;
        }

        /// <summary>
        /// 鍵扉ギミックを発動（解錠）する
        /// 発動に成功すると、対象ギミックと接続先の対応するギミックの両方を有効化し、
        /// 対応する扉（自室・接続先の両側）の施錠を解除する（扉の種類は <see cref="DoorType.Locked"/> のまま据え置く）
        /// 接続先の部屋・扉・ギミックが見つからない場合は、どちらの側も変更せずに失敗を返す
        /// </summary>
        /// <param name="rooms">部屋データの辞書（部屋位置がキー）</param>
        /// <param name="roomPosition">ギミックが属する部屋の位置</param>
        /// <param name="gimmickPosition">発動対象のギミックの位置</param>
        /// <param name="hasKey">鍵を所持しているかどうか（鍵アイテムシステムは Week2 範囲外のため呼び出し側の判定結果を受け取る）</param>
        /// <returns>発動に成功した場合は true。鍵を所持していない、ギミックが存在しない、既に発動済み、接続先の対応データが見つからない場合は false</returns>
        public bool TryActivateLockedDoor(Dictionary<Vector2I, RoomData> rooms, Vector2I roomPosition, Vector2I gimmickPosition, bool hasKey)
        {
            if (!TryFindGimmick(rooms, roomPosition, gimmickPosition, out var room, out var gimmick) ||
                !LockedDoorGimmick.CanActivate(gimmick, hasKey))
            {
                return false;
            }

            var door = room.Doors.FirstOrDefault(d => d.Position == gimmickPosition);
            if (door == null ||
                !TryFindConnectedGimmick(rooms, roomPosition, door, GimmickType.LockedDoor, out var connectedDoor, out var connectedGimmick))
            {
                return false;
            }

            gimmick.IsActive = true;
            door.IsLocked = false;
            connectedDoor.IsLocked = false;
            connectedGimmick.IsActive = true;

            return true;
        }

        /// <summary>
        /// 指定した扉の接続先の部屋から、対となる扉と対応するギミックを探す
        /// 接続先の部屋・対となる扉・対応するギミックのいずれかが見つからない場合は失敗する
        /// </summary>
        /// <param name="rooms">部屋データの辞書（部屋位置がキー）</param>
        /// <param name="roomPosition">発動元の部屋の位置</param>
        /// <param name="door">発動対象の扉（発動元の部屋に属する）</param>
        /// <param name="gimmickType">探索対象のギミックの種類</param>
        /// <param name="connectedDoor">見つかった接続先の扉（見つからない場合は null）</param>
        /// <param name="connectedGimmick">見つかった接続先のギミック（見つからない場合は null）</param>
        /// <returns>接続先の扉・ギミックの両方が見つかった場合は true</returns>
        private static bool TryFindConnectedGimmick(
            Dictionary<Vector2I, RoomData> rooms,
            Vector2I roomPosition,
            DoorData door,
            GimmickType gimmickType,
            [NotNullWhen(true)] out DoorData? connectedDoor,
            [NotNullWhen(true)] out GimmickData? connectedGimmick)
        {
            connectedGimmick = null;

            if (!rooms.TryGetValue(door.ConnectedRoomPosition, out var connectedRoom))
            {
                connectedDoor = null;
                return false;
            }

            connectedDoor = connectedRoom.GetDoorTo(roomPosition);
            if (connectedDoor == null)
            {
                return false;
            }

            var connectedDoorPosition = connectedDoor.Position;
            connectedGimmick = connectedRoom.Gimmicks.FirstOrDefault(
                g => g.Type == gimmickType && g.Position == connectedDoorPosition);
            return connectedGimmick != null;
        }

        /// <summary>
        /// 指定した部屋・位置に該当するギミックデータを探す
        /// </summary>
        /// <param name="rooms">部屋データの辞書</param>
        /// <param name="roomPosition">ギミックが属する部屋の位置</param>
        /// <param name="gimmickPosition">探索対象のギミックの位置</param>
        /// <param name="room">見つかった部屋データ（見つからない場合は null）</param>
        /// <param name="gimmick">見つかったギミックデータ（見つからない場合は null）</param>
        /// <returns>部屋・ギミックの両方が見つかった場合は true</returns>
        private static bool TryFindGimmick(
            Dictionary<Vector2I, RoomData> rooms,
            Vector2I roomPosition,
            Vector2I gimmickPosition,
            [NotNullWhen(true)] out RoomData? room,
            [NotNullWhen(true)] out GimmickData? gimmick)
        {
            if (!rooms.TryGetValue(roomPosition, out room))
            {
                gimmick = null;
                return false;
            }

            gimmick = room.Gimmicks.FirstOrDefault(g => g.Position == gimmickPosition);
            return gimmick != null;
        }
    }
}
