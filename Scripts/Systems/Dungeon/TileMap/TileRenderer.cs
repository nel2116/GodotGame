using System.Collections.Generic;
using Godot;

namespace Systems.Dungeon.TileMap
{
    /// <summary>
    /// タイル描画管理
    /// 部屋単位の描画ノードを登録し、視界外の部屋の描画をスキップ（非表示化）する薄いGodotノードラッパー
    /// フォグ・オブ・ウォー等の詳細な視界計算は行わず、部屋単位の表示/非表示切り替えのみを提供する
    /// </summary>
    public partial class TileRenderer : Node
    {
        /// <summary>
        /// 部屋の位置から、その部屋の描画を担うノードへの対応表
        /// </summary>
        private readonly Dictionary<Vector2I, CanvasItem> roomLayers = new();

        /// <summary>
        /// 部屋の描画ノードを登録する
        /// 同じ部屋位置で再登録した場合は登録内容を上書きする
        /// </summary>
        /// <param name="roomPosition">登録対象の部屋の位置</param>
        /// <param name="layer">その部屋の描画を担うノード（<see cref="TileMapLayer"/> 等）</param>
        public void RegisterRoomLayer(Vector2I roomPosition, CanvasItem layer)
        {
            roomLayers[roomPosition] = layer;
        }

        /// <summary>
        /// 指定した部屋位置の描画の表示/非表示を切り替える
        /// 対象の部屋が未登録の場合は何もしない
        /// </summary>
        /// <param name="roomPosition">対象の部屋の位置</param>
        /// <param name="visible">表示する場合は true、非表示にする場合は false</param>
        public void SetRoomVisible(Vector2I roomPosition, bool visible)
        {
            if (roomLayers.TryGetValue(roomPosition, out var layer))
            {
                layer.Visible = visible;
            }
        }
    }
}
