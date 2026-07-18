using System;
using System.Collections.Generic;
using Godot;
using Systems.Dungeon.Data;
using Systems.Dungeon.Models;
using Systems.Dungeon.Utilities;

namespace Systems.Dungeon.Navigation
{
    /// <summary>
    /// ナビゲーションメッシュ
    /// ダンジョン全体の「通行可能なワールドタイル座標の集合」を構築・保持する
    /// 部屋内部の床領域（障害物を除く）と、通行可能な状態の扉のみを通行可能として扱う
    /// </summary>
    public class NavigationMesh
    {
        /// <summary>
        /// 通行可能なワールドタイル座標の集合
        /// </summary>
        private readonly HashSet<Vector2I> walkableTiles = new();

        /// <summary>
        /// 部屋位置ごとに、その部屋が寄与した通行可能タイルの集合
        /// <see cref="RebuildRoom"/> で対象部屋の寄与分のみを差し替えられるようにするための内訳
        /// </summary>
        private readonly Dictionary<Vector2I, HashSet<Vector2I>> tilesByRoom = new();

        /// <summary>
        /// 通行可能集合を構築する
        /// 呼び出しのたびに集合をクリアしてから再構築するため、扉の状態変化後（ギミック発動後）に
        /// 再呼び出しすることでメッシュを最新の状態に更新できる
        /// 部屋単位の更新のみで済む場合は、全体再構築の代わりに <see cref="RebuildRoom"/> を使用できる
        /// </summary>
        /// <param name="rooms">部屋データの辞書（部屋位置がキー）</param>
        /// <param name="roomTemplates">部屋テンプレートの辞書（部屋位置がキー、障害物のローカル座標を保持）</param>
        /// <exception cref="ArgumentNullException">rooms または roomTemplates が null の場合</exception>
        public void Build(Dictionary<Vector2I, RoomData> rooms, IReadOnlyDictionary<Vector2I, RoomTemplate> roomTemplates)
        {
            if (rooms == null)
            {
                throw new ArgumentNullException(nameof(rooms));
            }

            if (roomTemplates == null)
            {
                throw new ArgumentNullException(nameof(roomTemplates));
            }

            walkableTiles.Clear();
            tilesByRoom.Clear();

            foreach (var (position, room) in rooms)
            {
                roomTemplates.TryGetValue(position, out var template);
                var roomTiles = CollectRoomTiles(room, template);
                tilesByRoom[position] = roomTiles;
                walkableTiles.UnionWith(roomTiles);
            }
        }

        /// <summary>
        /// 単一の部屋について、通行可能集合への寄与分のみを再計算する
        /// 扉の状態変化（ギミック発動）等、影響範囲が既知の場合に、全体再構築（<see cref="Build"/>）の代わりに使用することで
        /// 更新コストを変化した部屋数に比例させる（O(全部屋) ではなく O(変化した部屋数)）
        /// 各部屋は自室が保持する扉（<see cref="DoorData"/>）のみを寄与させるため、
        /// 接続先部屋のタイルには影響せず、部屋単位で独立して安全に再構築できる
        /// </summary>
        /// <param name="roomPosition">再構築対象の部屋の位置</param>
        /// <param name="room">再構築対象の部屋データ</param>
        /// <param name="template">再構築対象の部屋テンプレート（見つからない場合は null）</param>
        /// <exception cref="ArgumentNullException">room が null の場合</exception>
        public void RebuildRoom(Vector2I roomPosition, RoomData room, RoomTemplate? template)
        {
            if (room == null)
            {
                throw new ArgumentNullException(nameof(room));
            }

            if (tilesByRoom.TryGetValue(roomPosition, out var previousTiles))
            {
                walkableTiles.ExceptWith(previousTiles);
            }

            var roomTiles = CollectRoomTiles(room, template);
            tilesByRoom[roomPosition] = roomTiles;
            walkableTiles.UnionWith(roomTiles);
        }

        /// <summary>
        /// 指定したワールドタイル座標が通行可能かどうかを判定する
        /// </summary>
        /// <param name="worldPosition">判定対象のワールドタイル座標</param>
        /// <returns>通行可能集合に含まれる場合は true</returns>
        public bool IsWalkable(Vector2I worldPosition)
        {
            return walkableTiles.Contains(worldPosition);
        }

        /// <summary>
        /// 指定したワールドタイル座標に上下左右（斜め移動なし）で隣接し、かつ通行可能なタイル座標を列挙する
        /// </summary>
        /// <param name="worldPosition">基準となるワールドタイル座標</param>
        /// <returns>通行可能な隣接タイル座標の列挙</returns>
        public IEnumerable<Vector2I> GetWalkableNeighbors(Vector2I worldPosition)
        {
            foreach (var offset in Offsets)
            {
                var neighbor = worldPosition + offset;
                if (walkableTiles.Contains(neighbor))
                {
                    yield return neighbor;
                }
            }
        }

        /// <summary>
        /// 上下左右 4 方向の座標オフセット（斜め移動なし）
        /// </summary>
        private static readonly Vector2I[] Offsets =
        {
            new(1, 0), new(-1, 0), new(0, 1), new(0, -1)
        };

        /// <summary>
        /// 部屋 1 つ分の通行可能タイル（内部の床領域＋通行可能な扉）をワールド座標で収集する
        /// </summary>
        /// <param name="room">対象の部屋データ</param>
        /// <param name="template">対象の部屋テンプレート（見つからない場合は null）</param>
        /// <returns>その部屋が寄与する通行可能タイル座標の集合</returns>
        private static HashSet<Vector2I> CollectRoomTiles(RoomData room, RoomTemplate? template)
        {
            var tiles = new HashSet<Vector2I>();
            AddRoomFloor(tiles, room, template);
            AddWalkableDoors(tiles, room);
            return tiles;
        }

        /// <summary>
        /// 部屋内部の床領域（障害物を除く）をワールド座標に変換して通行可能集合へ追加する
        /// テンプレートが見つからない場合は障害物なしとして内部領域全体を追加する
        /// </summary>
        /// <param name="tiles">追加先の通行可能タイル集合</param>
        /// <param name="room">対象の部屋データ</param>
        /// <param name="template">対象の部屋テンプレート（見つからない場合は null）</param>
        private static void AddRoomFloor(HashSet<Vector2I> tiles, RoomData room, RoomTemplate? template)
        {
            var obstacles = template?.ObstaclePositions;

            for (int x = 1; x <= DungeonConstants.ROOM_SIZE - 2; x++)
            {
                for (int y = 1; y <= DungeonConstants.ROOM_SIZE - 2; y++)
                {
                    var local = new Vector2I(x, y);
                    if (obstacles != null && obstacles.Contains(local))
                    {
                        continue;
                    }

                    tiles.Add(room.Position + local);
                }
            }
        }

        /// <summary>
        /// 通行可能な状態（<see cref="DoorType.Secret"/> ではなく、かつ施錠されていない）の扉のみを
        /// 通行可能集合へ追加する
        /// </summary>
        /// <param name="tiles">追加先の通行可能タイル集合</param>
        /// <param name="room">対象の部屋データ</param>
        private static void AddWalkableDoors(HashSet<Vector2I> tiles, RoomData room)
        {
            foreach (var door in room.Doors)
            {
                if (door.Type != DoorType.Secret && !door.IsLocked)
                {
                    tiles.Add(door.Position);
                }
            }
        }
    }
}
