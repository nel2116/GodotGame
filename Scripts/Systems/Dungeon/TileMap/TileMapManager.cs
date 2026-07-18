using System.Collections.Generic;
using Godot;
using Systems.Dungeon.Data;
using Systems.Dungeon.Utilities;

namespace Systems.Dungeon.TileMap
{
    /// <summary>
    /// タイルマップ管理
    /// <see cref="RoomTileGenerator"/> が生成したタイル配置を実際の Godot <see cref="TileMapLayer"/> へ反映する薄いラッパー
    /// タイル種別の判定・座標計算などのロジックは一切持たず、渡された配置をそのまま SetCell するだけに留める
    /// </summary>
    public partial class TileMapManager : Node
    {
        /// <summary>
        /// 部屋 1 つ分のタイル配置を <see cref="TileMapLayer"/> へ反映する
        /// </summary>
        /// <param name="layer">反映先のタイルマップレイヤー</param>
        /// <param name="placements">反映するワールドタイル座標とタイル種別の組の一覧（<see cref="RoomTileGenerator.GenerateTiles"/> の結果を想定）</param>
        /// <param name="tileSetManager">タイル種別からソースID・アトラス座標を引くタイルセット管理</param>
        public void ApplyTiles(TileMapLayer layer, IEnumerable<(Vector2I WorldPosition, TileType Type)> placements, TileSetManager tileSetManager)
        {
            foreach (var (worldPosition, type) in placements)
            {
                UpdateTile(layer, worldPosition, type, tileSetManager);
            }
        }

        /// <summary>
        /// 単一タイルを更新する
        /// ギミック発動等で扉の状態が変わりタイル種別が動的に変化する場合に呼び出す想定
        /// </summary>
        /// <param name="layer">更新先のタイルマップレイヤー</param>
        /// <param name="worldPosition">更新するワールドタイル座標</param>
        /// <param name="type">更新後のタイル種別</param>
        /// <param name="tileSetManager">タイル種別からソースID・アトラス座標を引くタイルセット管理</param>
        public void UpdateTile(TileMapLayer layer, Vector2I worldPosition, TileType type, TileSetManager tileSetManager)
        {
            var template = tileSetManager.GetTemplate(type);
            layer.SetCell(worldPosition, template.SourceId, template.AtlasCoords);
        }

        /// <summary>
        /// 部屋 1 つ分のタイルを <see cref="TileMapLayer"/> から消去する
        /// 部屋がアクティブな部屋集合から外れた際（<see cref="Optimization.RoomLifecycleManager"/>）に呼び出す想定
        /// </summary>
        /// <param name="layer">消去対象のタイルマップレイヤー</param>
        /// <param name="room">消去する部屋データ</param>
        public void ClearRoomTiles(TileMapLayer layer, RoomData room)
        {
            for (int x = 0; x < DungeonConstants.ROOM_SIZE; x++)
            {
                for (int y = 0; y < DungeonConstants.ROOM_SIZE; y++)
                {
                    layer.SetCell(room.Position + new Vector2I(x, y), -1);
                }
            }
        }
    }
}
