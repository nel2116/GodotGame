using System.Collections.Generic;
using Godot;
using Systems.Dungeon.Data;

namespace Systems.Dungeon.Interfaces
{
    /// <summary>
    /// 部屋接続のインターフェース
    /// 生成された部屋同士を扉で接続し、経路の妥当性を検証する
    /// </summary>
    public interface IRoomConnector
    {
        /// <summary>
        /// 部屋同士を接続する
        /// </summary>
        /// <param name="rooms">接続対象の部屋データの辞書</param>
        void ConnectRooms(Dictionary<Vector2I, RoomData> rooms);

        /// <summary>
        /// 全部屋が到達可能に接続されているか検証する
        /// </summary>
        /// <param name="rooms">検証対象の部屋データの辞書</param>
        /// <returns>接続が妥当な場合は true</returns>
        bool ValidateConnections(Dictionary<Vector2I, RoomData> rooms);

        /// <summary>
        /// 開始位置から終了位置までの経路を探索する
        /// </summary>
        /// <param name="start">開始部屋の位置</param>
        /// <param name="end">終了部屋の位置</param>
        /// <returns>経路上の部屋位置のリスト（経路が存在しない場合は空リスト）</returns>
        List<Vector2I> FindPath(Vector2I start, Vector2I end);
    }
}
