using System;
using System.Collections.Generic;
using Godot;
using Systems.Dungeon.Utilities;

namespace Systems.Dungeon.Models
{
    /// <summary>
    /// 接続経路探索
    /// 部屋間を接続する扉の位置計算と、部屋グリッド上の経路探索を行う
    /// </summary>
    public class ConnectionPathFinder
    {
        /// <summary>
        /// 部屋 1 の外周上における、部屋 2 へ向かう扉の位置（ワールドタイル座標）を計算する
        /// 部屋 2 に面する辺を差分の主軸（絶対値が大きい軸、同値なら X 軸）で決定し、
        /// 辺方向の座標は部屋 2 の中心へ向けて角を避ける範囲（1..ROOM_SIZE-2）にクランプする
        /// グリッド隣接する 2 部屋では互いの扉が共有境界を挟んで向かい合う位置になる
        /// </summary>
        /// <param name="room1">扉を配置する部屋の位置（部屋の左上、ROOM_SIZE の倍数）</param>
        /// <param name="room2">接続先の部屋の位置（部屋の左上、ROOM_SIZE の倍数）</param>
        /// <returns>部屋 1 の外周上の扉位置（ワールドタイル座標）</returns>
        /// <exception cref="ArgumentException">room1 と room2 が同一位置の場合</exception>
        public Vector2I FindDoorPosition(Vector2I room1, Vector2I room2)
        {
            if (room1 == room2)
            {
                throw new ArgumentException("同一位置の部屋同士に扉は配置できません。", nameof(room2));
            }

            var delta = room2 - room1;
            int roomMax = DungeonConstants.ROOM_SIZE - 1;
            int half = DungeonConstants.ROOM_SIZE / 2;

            if (Math.Abs(delta.X) >= Math.Abs(delta.Y))
            {
                // 左右の辺に扉を配置する（X 軸が主軸）
                int edgeX = delta.X > 0 ? room1.X + roomMax : room1.X;
                int targetY = Math.Clamp(room2.Y + half, room1.Y + 1, room1.Y + roomMax - 1);
                return new Vector2I(edgeX, targetY);
            }
            else
            {
                // 上下の辺に扉を配置する（Y 軸が主軸）
                int edgeY = delta.Y > 0 ? room1.Y + roomMax : room1.Y;
                int targetX = Math.Clamp(room2.X + half, room1.X + 1, room1.X + roomMax - 1);
                return new Vector2I(targetX, edgeY);
            }
        }

        /// <summary>
        /// 部屋グリッド上で開始位置から終了位置までの経路を探索する
        /// X 軸方向 → Y 軸方向の順に ROOM_SIZE 刻みで進む L 字経路を返す
        /// </summary>
        /// <param name="start">開始部屋の位置（ROOM_SIZE の倍数）</param>
        /// <param name="end">終了部屋の位置（ROOM_SIZE の倍数）</param>
        /// <returns>開始位置から終了位置までの部屋位置のリスト（両端を含む）</returns>
        /// <exception cref="ArgumentException">start と end の差分が ROOM_SIZE の倍数でない場合</exception>
        public List<Vector2I> FindOptimalPath(Vector2I start, Vector2I end)
        {
            var delta = end - start;
            if (delta.X % DungeonConstants.ROOM_SIZE != 0 || delta.Y % DungeonConstants.ROOM_SIZE != 0)
            {
                throw new ArgumentException("開始位置と終了位置は部屋グリッド（ROOM_SIZE の倍数）に整列している必要があります。", nameof(end));
            }

            var path = new List<Vector2I> { start };
            var current = start;

            // X 軸方向に 1 部屋ずつ進む
            while (current.X != end.X)
            {
                current = new Vector2I(current.X + Math.Sign(end.X - current.X) * DungeonConstants.ROOM_SIZE, current.Y);
                path.Add(current);
            }

            // Y 軸方向に 1 部屋ずつ進む
            while (current.Y != end.Y)
            {
                current = new Vector2I(current.X, current.Y + Math.Sign(end.Y - current.Y) * DungeonConstants.ROOM_SIZE);
                path.Add(current);
            }

            return path;
        }
    }
}
