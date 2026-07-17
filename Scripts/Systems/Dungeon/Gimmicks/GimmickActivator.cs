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
        /// </summary>
        /// <param name="rooms">部屋データの辞書（部屋位置がキー）</param>
        /// <param name="roomPosition">ギミックが属する部屋の位置</param>
        /// <param name="gimmickPosition">発動対象のギミックの位置</param>
        /// <returns>発動に成功した場合は true。ギミックが存在しない、種類が異なる、既に発動済みの場合は false</returns>
        public bool TryActivateHiddenPassage(Dictionary<Vector2I, RoomData> rooms, Vector2I roomPosition, Vector2I gimmickPosition)
        {
            if (!TryFindGimmick(rooms, roomPosition, gimmickPosition, out var room, out var gimmick) ||
                !HiddenPassageGimmick.CanActivate(gimmick))
            {
                return false;
            }

            var door = room.Doors.FirstOrDefault(d => d.Position == gimmickPosition);
            if (door == null)
            {
                return false;
            }

            gimmick!.IsActive = true;
            door.Type = DoorType.Normal;

            if (rooms.TryGetValue(door.ConnectedRoomPosition, out var connectedRoom))
            {
                var connectedDoor = connectedRoom.Doors.FirstOrDefault(d => d.ConnectedRoomPosition == roomPosition);
                if (connectedDoor != null)
                {
                    connectedDoor.Type = DoorType.Normal;

                    var connectedGimmick = connectedRoom.Gimmicks.FirstOrDefault(
                        g => g.Type == GimmickType.HiddenPassage && g.Position == connectedDoor.Position);
                    if (connectedGimmick != null)
                    {
                        connectedGimmick.IsActive = true;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 鍵扉ギミックを発動（解錠）する
        /// 発動に成功すると、対象ギミックと接続先の対応するギミックの両方を有効化し、
        /// 対応する扉（自室・接続先の両側）の施錠を解除する（扉の種類は <see cref="DoorType.Locked"/> のまま据え置く）
        /// </summary>
        /// <param name="rooms">部屋データの辞書（部屋位置がキー）</param>
        /// <param name="roomPosition">ギミックが属する部屋の位置</param>
        /// <param name="gimmickPosition">発動対象のギミックの位置</param>
        /// <param name="hasKey">鍵を所持しているかどうか（鍵アイテムシステムは Week2 範囲外のため呼び出し側の判定結果を受け取る）</param>
        /// <returns>発動に成功した場合は true。鍵を所持していない、ギミックが存在しない、既に発動済みの場合は false</returns>
        public bool TryActivateLockedDoor(Dictionary<Vector2I, RoomData> rooms, Vector2I roomPosition, Vector2I gimmickPosition, bool hasKey)
        {
            if (!TryFindGimmick(rooms, roomPosition, gimmickPosition, out var room, out var gimmick) ||
                !LockedDoorGimmick.CanActivate(gimmick, hasKey))
            {
                return false;
            }

            var door = room.Doors.FirstOrDefault(d => d.Position == gimmickPosition);
            if (door == null)
            {
                return false;
            }

            gimmick!.IsActive = true;
            door.IsLocked = false;

            if (rooms.TryGetValue(door.ConnectedRoomPosition, out var connectedRoom))
            {
                var connectedDoor = connectedRoom.Doors.FirstOrDefault(d => d.ConnectedRoomPosition == roomPosition);
                if (connectedDoor != null)
                {
                    connectedDoor.IsLocked = false;

                    var connectedGimmick = connectedRoom.Gimmicks.FirstOrDefault(
                        g => g.Type == GimmickType.LockedDoor && g.Position == connectedDoor.Position);
                    if (connectedGimmick != null)
                    {
                        connectedGimmick.IsActive = true;
                    }
                }
            }

            return true;
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
