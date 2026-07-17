using System;
using System.Collections.Generic;
using Core.Events;
using Godot;
using Systems.Dungeon.Events;
using Systems.Dungeon.Gimmicks;
using Systems.Dungeon.Models;
using Systems.Dungeon.Navigation;
using Systems.Dungeon.TileMap;
using Systems.Dungeon.ViewModels;

namespace Systems.Dungeon.Views
{
    /// <summary>
    /// ダンジョンビュー
    /// <see cref="DungeonViewModel"/>（レベル生成・ギミック配置・ナビゲーション統合）を初期化し、
    /// 生成結果をタイルマップへ反映する。ロジックはModel/ViewModel層に委ね、
    /// 本クラスはGodotノードとの薄い橋渡しに徹する
    /// </summary>
    public partial class DungeonView : Node2D
    {
        /// <summary>
        /// デバッグ用の固定レベル生成シード値
        /// </summary>
        private const int DebugSeed = 12345;

        /// <summary>
        /// タイルマップの反映先レイヤー（シーン上の子ノード "TileMapLayer" を使用）
        /// </summary>
        private TileMapLayer _tileMapLayer = default!;

        private DungeonViewModel _viewModel = default!;
        private LevelGenerationModel _levelGenerationModel = default!;
        private readonly RoomTileGenerator _roomTileGenerator = new();
        private readonly TileSetManager _tileSetManager = new();
        private readonly TileMapManager _tileMapManager = new();
        private readonly List<IDisposable> _debugLogSubscriptions = new();

        /// <summary>
        /// ノード初期化
        /// Model/ViewModel層を構築し、デバッグ用イベントログ購読を登録したうえで
        /// 固定シードによるレベル生成を開始する
        /// </summary>
        public override void _Ready()
        {
            _tileMapLayer = GetNode<TileMapLayer>("TileMapLayer");

            var eventBus = GameEventBus.Instance;
            _levelGenerationModel = new LevelGenerationModel(DebugSeed);
            var gimmickPlacementModel = new GimmickPlacementModel(new Random(DebugSeed));
            var gimmickActivator = new GimmickActivator();
            var navigationManager = new NavigationManager();

            _viewModel = new DungeonViewModel(
                _levelGenerationModel,
                gimmickPlacementModel,
                gimmickActivator,
                navigationManager,
                eventBus);

            SubscribeDebugLogs(eventBus);

            InitializeLevelAsync();
        }

        /// <summary>
        /// レベル生成を行い、完了後に全部屋をタイルマップへ反映する
        /// Godotの<see cref="_Ready"/>は同期メソッドのため、非同期処理はfire-and-forgetの
        /// async voidヘルパーとして切り出す
        /// </summary>
        private async void InitializeLevelAsync()
        {
            await _viewModel.GenerateLevelAsync(DebugSeed);
            RenderRooms();
        }

        /// <summary>
        /// 生成済みの全部屋をタイルマップへ反映する
        /// </summary>
        private void RenderRooms()
        {
            foreach (var (position, room) in _viewModel.Rooms.Value)
            {
                if (!_levelGenerationModel.RoomTemplates.TryGetValue(position, out var template))
                {
                    continue;
                }

                var tiles = _roomTileGenerator.GenerateTiles(room, template);
                _tileMapManager.ApplyTiles(_tileMapLayer, tiles, _tileSetManager);
            }
        }

        /// <summary>
        /// ダンジョン関連イベントの発行状況をデバッグログ（<see cref="GD.Print(Variant[])"/>）へ出力する購読を登録する
        /// </summary>
        /// <param name="eventBus">購読対象のイベントバス</param>
        private void SubscribeDebugLogs(IGameEventBus eventBus)
        {
            _debugLogSubscriptions.Add(eventBus.GetEventStream<LevelGeneratedEvent>()
                .Subscribe(e => GD.Print($"[Dungeon] LevelGenerated: RoomCount={e.RoomCount}")));
            _debugLogSubscriptions.Add(eventBus.GetEventStream<RoomEnteredEvent>()
                .Subscribe(e => GD.Print($"[Dungeon] RoomEntered: Position={e.RoomPosition} Type={e.RoomType}")));
            _debugLogSubscriptions.Add(eventBus.GetEventStream<HiddenPassageRevealedEvent>()
                .Subscribe(e => GD.Print($"[Dungeon] HiddenPassageRevealed: Room={e.RoomPosition} Gimmick={e.GimmickPosition}")));
            _debugLogSubscriptions.Add(eventBus.GetEventStream<LockedDoorUnlockedEvent>()
                .Subscribe(e => GD.Print($"[Dungeon] LockedDoorUnlocked: Room={e.RoomPosition} Gimmick={e.GimmickPosition}")));
            _debugLogSubscriptions.Add(eventBus.GetEventStream<GimmickActivationFailedEvent>()
                .Subscribe(e => GD.Print($"[Dungeon] GimmickActivationFailed: Room={e.RoomPosition} Gimmick={e.GimmickPosition} Type={e.GimmickType}")));
        }

        /// <summary>
        /// ノード破棄時にイベント購読とViewModelを解放する
        /// </summary>
        public override void _ExitTree()
        {
            foreach (var subscription in _debugLogSubscriptions)
            {
                subscription.Dispose();
            }
            _debugLogSubscriptions.Clear();

            _tileMapManager.Free();
            _viewModel.Dispose();
        }
    }
}
