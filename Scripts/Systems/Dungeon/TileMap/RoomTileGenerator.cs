using System;
using System.Collections.Generic;
using Godot;
using Systems.Dungeon.Data;
using Systems.Dungeon.Models;
using Systems.Dungeon.Utilities;

namespace Systems.Dungeon.TileMap
{
    /// <summary>
    /// 部屋タイル生成器
    /// 部屋データと部屋テンプレートから、部屋 1 つ分の「ワールドタイル座標ごとのタイル種別」の配置を生成する
    /// 座標変換とタイル種別判定のみを行う純粋ロジックであり、Godot ノードには依存しない
    /// </summary>
    public class RoomTileGenerator
    {
        /// <summary>
        /// 部屋 1 つ分のタイル配置を生成する
        /// 内部領域（部屋ローカル座標 1..ROOM_SIZE-2）は <see cref="RoomTemplate.ObstaclePositions"/> に含まれれば
        /// <see cref="TileType.Obstacle"/>、それ以外は <see cref="TileType.Floor"/> とする
        /// 外周（部屋ローカル X/Y が 0 または ROOM_SIZE-1）は基本 <see cref="TileType.Wall"/> とし、
        /// <see cref="RoomTemplate.DoorPositions"/> に一致するセルは対応する扉（<see cref="RoomData.Doors"/>、インデックス対応）の
        /// 種類に応じて <see cref="TileType.Door"/>・<see cref="TileType.LockedDoor"/>・<see cref="TileType.SecretWall"/> のいずれかとする
        /// すべてのセルは room.Position を加算したワールドタイル座標として返す
        /// </summary>
        /// <param name="room">対象の部屋データ</param>
        /// <param name="template">対象の部屋テンプレート（room.Doors と DoorPositions がインデックスで対応していること）</param>
        /// <returns>ワールドタイル座標とタイル種別の組の一覧</returns>
        /// <exception cref="ArgumentNullException">room または template が null の場合</exception>
        public List<(Vector2I WorldPosition, TileType Type)> GenerateTiles(RoomData room, RoomTemplate template)
        {
            if (room == null)
            {
                throw new ArgumentNullException(nameof(room));
            }

            if (template == null)
            {
                throw new ArgumentNullException(nameof(template));
            }

            var doorTypeByLocalPosition = BuildDoorTypeMap(room, template);
            var result = new List<(Vector2I WorldPosition, TileType Type)>();

            for (int x = 0; x < DungeonConstants.ROOM_SIZE; x++)
            {
                for (int y = 0; y < DungeonConstants.ROOM_SIZE; y++)
                {
                    var local = new Vector2I(x, y);
                    bool isBoundary = x == 0 || y == 0 || x == DungeonConstants.ROOM_SIZE - 1 || y == DungeonConstants.ROOM_SIZE - 1;

                    var type = isBoundary
                        ? GetBoundaryTileType(local, doorTypeByLocalPosition)
                        : GetInteriorTileType(local, template);

                    result.Add((room.Position + local, type));
                }
            }

            return result;
        }

        /// <summary>
        /// 扉のローカル座標から扉種別への対応表を構築する
        /// <see cref="RoomTemplate.DoorPositions"/> と <see cref="RoomData.Doors"/> はインデックスで対応するため、
        /// 両者を先頭から順にペアリングして参照する
        /// </summary>
        /// <param name="room">対象の部屋データ</param>
        /// <param name="template">対象の部屋テンプレート</param>
        /// <returns>扉のローカル座標をキーとした扉種別の辞書</returns>
        private static Dictionary<Vector2I, DoorType> BuildDoorTypeMap(RoomData room, RoomTemplate template)
        {
            var map = new Dictionary<Vector2I, DoorType>();
            int count = Math.Min(template.DoorPositions.Count, room.Doors.Count);
            for (int i = 0; i < count; i++)
            {
                map[template.DoorPositions[i]] = room.Doors[i].Type;
            }

            return map;
        }

        /// <summary>
        /// 外周セルのタイル種別を判定する
        /// 扉のローカル座標に一致すれば扉種別に応じたタイル種別を、一致しなければ壁を返す
        /// </summary>
        /// <param name="local">判定対象の部屋ローカル座標</param>
        /// <param name="doorTypeByLocalPosition">扉のローカル座標から扉種別への対応表</param>
        /// <returns>外周セルのタイル種別</returns>
        private static TileType GetBoundaryTileType(Vector2I local, Dictionary<Vector2I, DoorType> doorTypeByLocalPosition)
        {
            if (!doorTypeByLocalPosition.TryGetValue(local, out var doorType))
            {
                return TileType.Wall;
            }

            return doorType switch
            {
                DoorType.Secret => TileType.SecretWall,
                DoorType.Locked => TileType.LockedDoor,
                _ => TileType.Door
            };
        }

        /// <summary>
        /// 内部領域セルのタイル種別を判定する
        /// 障害物のローカル座標に含まれれば障害物を、含まれなければ床を返す
        /// </summary>
        /// <param name="local">判定対象の部屋ローカル座標</param>
        /// <param name="template">対象の部屋テンプレート</param>
        /// <returns>内部領域セルのタイル種別</returns>
        private static TileType GetInteriorTileType(Vector2I local, RoomTemplate template)
        {
            return template.ObstaclePositions.Contains(local) ? TileType.Obstacle : TileType.Floor;
        }
    }
}
