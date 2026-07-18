using System;
using Godot;
using Systems.Dungeon.Data;
using Systems.Dungeon.Models;
using Systems.Dungeon.TileMap;

namespace Systems.Dungeon.Optimization
{
    /// <summary>
    /// 部屋タイル描画器（<see cref="IRoomTileRenderer"/> 実装）
    /// 単一の共有 <see cref="TileMapLayer"/> に対して、<see cref="RoomTileGenerator"/>・<see cref="TileMapManager"/>・
    /// <see cref="TileSetManager"/> を用いて部屋単位のタイル反映・消去を行う
    /// </summary>
    public class RoomTileRenderer : IRoomTileRenderer
    {
        private readonly TileMapLayer layer;
        private readonly RoomTileGenerator roomTileGenerator;
        private readonly TileMapManager tileMapManager;
        private readonly TileSetManager tileSetManager;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="layer">反映先のタイルマップレイヤー</param>
        /// <param name="roomTileGenerator">部屋タイル生成器</param>
        /// <param name="tileMapManager">タイルマップ管理</param>
        /// <param name="tileSetManager">タイルセット管理</param>
        /// <exception cref="ArgumentNullException">いずれかの引数が null の場合</exception>
        public RoomTileRenderer(
            TileMapLayer layer,
            RoomTileGenerator roomTileGenerator,
            TileMapManager tileMapManager,
            TileSetManager tileSetManager)
        {
            this.layer = layer ?? throw new ArgumentNullException(nameof(layer));
            this.roomTileGenerator = roomTileGenerator ?? throw new ArgumentNullException(nameof(roomTileGenerator));
            this.tileMapManager = tileMapManager ?? throw new ArgumentNullException(nameof(tileMapManager));
            this.tileSetManager = tileSetManager ?? throw new ArgumentNullException(nameof(tileSetManager));
        }

        /// <inheritdoc />
        public void ApplyRoom(RoomData room, RoomTemplate template)
        {
            var tiles = roomTileGenerator.GenerateTiles(room, template);
            tileMapManager.ApplyTiles(layer, tiles, tileSetManager);
        }

        /// <inheritdoc />
        public void ClearRoom(RoomData room)
        {
            tileMapManager.ClearRoomTiles(layer, room);
        }
    }
}
