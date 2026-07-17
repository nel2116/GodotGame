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
        /// 通行可能集合を構築する
        /// 呼び出しのたびに集合をクリアしてから再構築するため、扉の状態変化後（ギミック発動後）に
        /// 再呼び出しすることでメッシュを最新の状態に更新できる
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

            foreach (var (position, room) in rooms)
            {
                roomTemplates.TryGetValue(position, out var template);
                AddRoomFloor(room, template);
                AddWalkableDoors(room);
            }
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
        /// 部屋内部の床領域（障害物を除く）をワールド座標に変換して通行可能集合へ追加する
        /// テンプレートが見つからない場合は障害物なしとして内部領域全体を追加する
        /// </summary>
        /// <param name="room">対象の部屋データ</param>
        /// <param name="template">対象の部屋テンプレート（見つからない場合は null）</param>
        private void AddRoomFloor(RoomData room, RoomTemplate? template)
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

                    walkableTiles.Add(room.Position + local);
                }
            }
        }

        /// <summary>
        /// 通行可能な状態（<see cref="DoorType.Secret"/> ではなく、かつ施錠されていない）の扉のみを
        /// 通行可能集合へ追加する
        /// </summary>
        /// <param name="room">対象の部屋データ</param>
        private void AddWalkableDoors(RoomData room)
        {
            foreach (var door in room.Doors)
            {
                if (door.Type != DoorType.Secret && !door.IsLocked)
                {
                    walkableTiles.Add(door.Position);
                }
            }
        }
    }
}
