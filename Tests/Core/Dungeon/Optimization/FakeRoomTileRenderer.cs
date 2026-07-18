using System.Collections.Generic;
using Godot;
using Systems.Dungeon.Data;
using Systems.Dungeon.Models;
using Systems.Dungeon.Optimization;

namespace Tests.Core.Dungeon.Optimization
{
    /// <summary>
    /// テスト用の <see cref="IRoomTileRenderer"/> スパイ実装
    /// 実際にタイルマップへの描画は行わず、ApplyRoom/ClearRoom の呼び出し履歴（対象部屋の位置）のみを記録する
    /// </summary>
    public class FakeRoomTileRenderer : IRoomTileRenderer
    {
        /// <summary>
        /// ApplyRoom が呼び出された部屋位置の履歴（呼び出し順）
        /// </summary>
        public List<Vector2I> AppliedRoomPositions { get; } = new();

        /// <summary>
        /// ClearRoom が呼び出された部屋位置の履歴（呼び出し順）
        /// </summary>
        public List<Vector2I> ClearedRoomPositions { get; } = new();

        /// <inheritdoc />
        public void ApplyRoom(RoomData room, RoomTemplate template)
        {
            AppliedRoomPositions.Add(room.Position);
        }

        /// <inheritdoc />
        public void ClearRoom(RoomData room)
        {
            ClearedRoomPositions.Add(room.Position);
        }
    }
}
