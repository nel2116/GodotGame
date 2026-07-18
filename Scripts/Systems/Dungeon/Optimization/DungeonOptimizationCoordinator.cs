using System;
using System.Collections.Generic;
using Godot;
using Systems.Dungeon.Data;
using Systems.Dungeon.Models;
using Systems.Dungeon.Navigation;

namespace Systems.Dungeon.Optimization
{
    /// <summary>
    /// ダンジョン最適化ファサード
    /// 部屋可視性判定（<see cref="RoomVisibilityManager"/>）・部屋読み込み/解放（<see cref="RoomLifecycleManager"/>）・
    /// ナビゲーションメッシュの部分再構築（<see cref="NavigationManager"/>）を束ね、<see cref="ViewModels.DungeonViewModel"/> から利用する
    /// </summary>
    public class DungeonOptimizationCoordinator
    {
        /// <summary>
        /// 現在部屋から見てアクティブとみなす部屋のホップ数（既定値: 隣接部屋まで読み込む）
        /// </summary>
        private const int ActiveRoomRadius = 1;

        private readonly RoomVisibilityManager visibilityManager;
        private readonly RoomLifecycleManager lifecycleManager;
        private readonly NavigationManager navigationManager;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <exception cref="ArgumentNullException">いずれかの引数が null の場合</exception>
        public DungeonOptimizationCoordinator(
            RoomVisibilityManager visibilityManager,
            RoomLifecycleManager lifecycleManager,
            NavigationManager navigationManager)
        {
            this.visibilityManager = visibilityManager ?? throw new ArgumentNullException(nameof(visibilityManager));
            this.lifecycleManager = lifecycleManager ?? throw new ArgumentNullException(nameof(lifecycleManager));
            this.navigationManager = navigationManager ?? throw new ArgumentNullException(nameof(navigationManager));
        }

        /// <summary>
        /// 部屋入室（またはレベル生成直後の初期化）に応じて、アクティブな部屋集合を再計算しタイルマップを同期する
        /// </summary>
        /// <param name="currentRoom">現在の部屋の位置</param>
        /// <param name="rooms">部屋データの辞書（部屋位置がキー）</param>
        /// <param name="templates">部屋テンプレートの辞書（部屋位置がキー）</param>
        /// <param name="renderer">部屋タイル描画器</param>
        /// <returns>今回読み込んだ部屋・解放した部屋の一覧</returns>
        public RoomSyncResult OnRoomEntered(
            Vector2I currentRoom,
            Dictionary<Vector2I, RoomData> rooms,
            IReadOnlyDictionary<Vector2I, RoomTemplate> templates,
            IRoomTileRenderer renderer)
        {
            var activeRooms = visibilityManager.GetActiveRooms(currentRoom, rooms, ActiveRoomRadius);
            return lifecycleManager.SyncActiveRooms(activeRooms, rooms, templates, renderer);
        }

        /// <summary>
        /// 扉の状態変化（ギミック発動）に応じて、影響を受けた部屋（発動元・接続先）のみナビゲーションメッシュを再構築する
        /// </summary>
        /// <param name="roomPosition">扉の状態が変化した部屋の位置</param>
        /// <param name="connectedRoomPosition">接続先の部屋の位置</param>
        /// <param name="rooms">部屋データの辞書（部屋位置がキー）</param>
        /// <param name="templates">部屋テンプレートの辞書（部屋位置がキー）</param>
        public void OnDoorStateChanged(
            Vector2I roomPosition,
            Vector2I connectedRoomPosition,
            Dictionary<Vector2I, RoomData> rooms,
            IReadOnlyDictionary<Vector2I, RoomTemplate> templates)
        {
            navigationManager.RebuildRooms(new[] { roomPosition, connectedRoomPosition }, rooms, templates);
        }
    }
}
