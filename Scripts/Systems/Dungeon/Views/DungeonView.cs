using System;
using System.Collections.Generic;
using System.Linq;
using Core.Events;
using Godot;
using Systems.Dungeon.Data;
using Systems.Dungeon.Events;
using Systems.Dungeon.Gimmicks;
using Systems.Dungeon.Models;
using Systems.Dungeon.Navigation;
using Systems.Dungeon.TileMap;
using Systems.Dungeon.Utilities;
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

        /// <summary>
        /// 経路表示用の線（シーン上の子ノード "PathLine" を使用）
        /// </summary>
        private Line2D _pathLine = default!;

        /// <summary>
        /// 現在の部屋情報を表示するラベル
        /// </summary>
        private Label _roomInfoLabel = default!;

        /// <summary>
        /// 直近の操作結果（入室・ギミック発動成否）を表示するラベル
        /// </summary>
        private Label _resultLabel = default!;

        /// <summary>
        /// 隣接部屋への遷移ボタンを並べるコンテナ
        /// </summary>
        private HBoxContainer _roomButtonsContainer = default!;

        /// <summary>
        /// 未発動ギミックの発動ボタンを並べるコンテナ
        /// </summary>
        private VBoxContainer _gimmickButtonsContainer = default!;

        private DungeonViewModel _viewModel = default!;
        private LevelGenerationModel _levelGenerationModel = default!;
        private readonly RoomTileGenerator _roomTileGenerator = new();
        private readonly TileSetManager _tileSetManager = new();
        private readonly TileMapManager _tileMapManager = new();
        private readonly List<IDisposable> _eventSubscriptions = new();

        /// <summary>
        /// ノード初期化
        /// Model/ViewModel層を構築し、デバッグ用イベントログ購読を登録したうえで
        /// 固定シードによるレベル生成を開始する
        /// </summary>
        public override void _Ready()
        {
            _tileMapLayer = GetNode<TileMapLayer>("TileMapLayer");
            _pathLine = GetNode<Line2D>("PathLine");
            _roomInfoLabel = GetNode<Label>("DebugUI/RootPanel/RoomInfoLabel");
            _resultLabel = GetNode<Label>("DebugUI/RootPanel/ResultLabel");
            _roomButtonsContainer = GetNode<HBoxContainer>("DebugUI/RootPanel/RoomButtonsContainer");
            _gimmickButtonsContainer = GetNode<VBoxContainer>("DebugUI/RootPanel/GimmickButtonsContainer");

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
            SubscribeUiUpdates(eventBus);

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
            RefreshRoomUi();
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
            _eventSubscriptions.Add(eventBus.GetEventStream<LevelGeneratedEvent>()
                .Subscribe(e => GD.Print($"[Dungeon] LevelGenerated: RoomCount={e.RoomCount}")));
            _eventSubscriptions.Add(eventBus.GetEventStream<RoomEnteredEvent>()
                .Subscribe(e => GD.Print($"[Dungeon] RoomEntered: Position={e.RoomPosition} Type={e.RoomType}")));
            _eventSubscriptions.Add(eventBus.GetEventStream<HiddenPassageRevealedEvent>()
                .Subscribe(e => GD.Print($"[Dungeon] HiddenPassageRevealed: Room={e.RoomPosition} Gimmick={e.GimmickPosition}")));
            _eventSubscriptions.Add(eventBus.GetEventStream<LockedDoorUnlockedEvent>()
                .Subscribe(e => GD.Print($"[Dungeon] LockedDoorUnlocked: Room={e.RoomPosition} Gimmick={e.GimmickPosition}")));
            _eventSubscriptions.Add(eventBus.GetEventStream<GimmickActivationFailedEvent>()
                .Subscribe(e => GD.Print($"[Dungeon] GimmickActivationFailed: Room={e.RoomPosition} Gimmick={e.GimmickPosition} Type={e.GimmickType}")));
        }

        /// <summary>
        /// UI表示更新用のイベント購読を登録する
        /// 部屋入室・ギミック発動結果に応じて、現在部屋情報ラベル・結果ラベル・遷移/発動ボタン・タイル表示を更新する
        /// </summary>
        /// <param name="eventBus">購読対象のイベントバス</param>
        private void SubscribeUiUpdates(IGameEventBus eventBus)
        {
            _eventSubscriptions.Add(eventBus.GetEventStream<RoomEnteredEvent>()
                .Subscribe(e =>
                {
                    _resultLabel.Text = $"入室: {e.RoomPosition} ({e.RoomType})";
                    RefreshRoomUi();
                }));
            _eventSubscriptions.Add(eventBus.GetEventStream<HiddenPassageRevealedEvent>()
                .Subscribe(e =>
                {
                    _resultLabel.Text = $"隠し通路を発見しました: {e.GimmickPosition}";
                    RefreshDoorTiles(e.RoomPosition, e.GimmickPosition);
                    RefreshRoomUi();
                }));
            _eventSubscriptions.Add(eventBus.GetEventStream<LockedDoorUnlockedEvent>()
                .Subscribe(e =>
                {
                    _resultLabel.Text = $"鍵扉を解錠しました: {e.GimmickPosition}";
                    RefreshDoorTiles(e.RoomPosition, e.GimmickPosition);
                    RefreshRoomUi();
                }));
            _eventSubscriptions.Add(eventBus.GetEventStream<GimmickActivationFailedEvent>()
                .Subscribe(e =>
                {
                    _resultLabel.Text = $"ギミック発動に失敗しました: {e.GimmickType} @ {e.GimmickPosition}";
                }));
        }

        /// <summary>
        /// 現在の部屋情報表示・遷移ボタン・ギミック発動ボタンを最新状態へ再構築する
        /// </summary>
        private void RefreshRoomUi()
        {
            var currentPosition = _viewModel.CurrentRoomPosition.Value;
            if (!_viewModel.Rooms.Value.TryGetValue(currentPosition, out var room))
            {
                return;
            }

            _roomInfoLabel.Text = $"現在の部屋: {currentPosition} ({room.Type})";

            RebuildRoomButtons(currentPosition, room);
            RebuildGimmickButtons(currentPosition, room);
        }

        /// <summary>
        /// 現在の部屋が持つ扉ごとに、接続先の部屋へ遷移するボタンを再構築する
        /// 鍵扉・隠し扉の未開通状態にかかわらず、デバッグ用途として全ての扉を遷移対象として表示する
        /// </summary>
        /// <param name="currentPosition">現在の部屋の位置</param>
        /// <param name="room">現在の部屋データ</param>
        private void RebuildRoomButtons(Vector2I currentPosition, RoomData room)
        {
            foreach (var child in _roomButtonsContainer.GetChildren())
            {
                child.QueueFree();
            }

            foreach (var door in room.Doors)
            {
                var targetPosition = door.ConnectedRoomPosition;
                var button = new Button { Text = $"→ {targetPosition} [{door.Type}]" };
                button.Pressed += () => OnRoomTransitionPressed(currentPosition, targetPosition);
                _roomButtonsContainer.AddChild(button);
            }
        }

        /// <summary>
        /// 現在の部屋が持つ未発動ギミック（隠し通路・鍵扉）ごとに、発動を試みるボタンを再構築する
        /// </summary>
        /// <param name="currentPosition">現在の部屋の位置</param>
        /// <param name="room">現在の部屋データ</param>
        private void RebuildGimmickButtons(Vector2I currentPosition, RoomData room)
        {
            foreach (var child in _gimmickButtonsContainer.GetChildren())
            {
                child.QueueFree();
            }

            foreach (var gimmick in room.Gimmicks)
            {
                if (gimmick.IsActive) continue;
                if (gimmick.Type != GimmickType.HiddenPassage && gimmick.Type != GimmickType.LockedDoor) continue;

                var button = new Button { Text = $"発動: {gimmick.Type} @ {gimmick.Position}" };
                button.Pressed += () => OnGimmickActivatePressed(currentPosition, gimmick);
                _gimmickButtonsContainer.AddChild(button);
            }
        }

        /// <summary>
        /// 部屋遷移ボタン押下時の処理
        /// 現在部屋の中心から遷移先部屋の中心までの経路を表示したうえで、遷移先の部屋へ入室する
        /// </summary>
        /// <param name="fromPosition">現在の部屋の位置</param>
        /// <param name="targetPosition">遷移先の部屋の位置</param>
        private void OnRoomTransitionPressed(Vector2I fromPosition, Vector2I targetPosition)
        {
            DrawPathPreview(fromPosition, targetPosition);
            _viewModel.EnterRoom(targetPosition);
        }

        /// <summary>
        /// ギミック発動ボタン押下時の処理
        /// 鍵の所持は簡易的に常に true として扱う（本格的なアイテムシステムは対象外）
        /// </summary>
        /// <param name="roomPosition">ギミックが属する部屋の位置</param>
        /// <param name="gimmick">発動対象のギミックデータ</param>
        private void OnGimmickActivatePressed(Vector2I roomPosition, GimmickData gimmick)
        {
            if (gimmick.Type == GimmickType.HiddenPassage)
            {
                _viewModel.TryActivateHiddenPassage(roomPosition, gimmick.Position);
            }
            else if (gimmick.Type == GimmickType.LockedDoor)
            {
                _viewModel.TryActivateLockedDoor(roomPosition, gimmick.Position, hasKey: true);
            }
        }

        /// <summary>
        /// 部屋の中心タイル同士の経路を探索し、<see cref="_pathLine"/> へ反映する
        /// </summary>
        /// <param name="fromRoomPosition">開始部屋の位置</param>
        /// <param name="toRoomPosition">目標部屋の位置</param>
        private void DrawPathPreview(Vector2I fromRoomPosition, Vector2I toRoomPosition)
        {
            var roomCenterOffset = new Vector2I(DungeonConstants.ROOM_SIZE / 2, DungeonConstants.ROOM_SIZE / 2);
            var startTile = fromRoomPosition + roomCenterOffset;
            var goalTile = toRoomPosition + roomCenterOffset;

            var path = _viewModel.FindPath(startTile, goalTile);
            _pathLine.Points = path.Select(tile => _tileMapLayer.MapToLocal(tile)).ToArray();
        }

        /// <summary>
        /// ギミック発動により状態が変化した扉のタイルを、自室・接続先の部屋の両方について再描画する
        /// </summary>
        /// <param name="roomPosition">ギミックが属していた部屋の位置</param>
        /// <param name="doorPosition">状態が変化した扉の位置（ギミック位置と同一）</param>
        private void RefreshDoorTiles(Vector2I roomPosition, Vector2I doorPosition)
        {
            if (!_viewModel.Rooms.Value.TryGetValue(roomPosition, out var room))
            {
                return;
            }

            var door = room.Doors.FirstOrDefault(d => d.Position == doorPosition);
            if (door == null)
            {
                return;
            }

            UpdateDoorTile(door);

            if (_viewModel.Rooms.Value.TryGetValue(door.ConnectedRoomPosition, out var connectedRoom))
            {
                var connectedDoor = connectedRoom.GetDoorTo(roomPosition);
                if (connectedDoor != null)
                {
                    UpdateDoorTile(connectedDoor);
                }
            }
        }

        /// <summary>
        /// 単一の扉タイルを現在の扉データの状態に合わせて更新する
        /// </summary>
        /// <param name="door">更新対象の扉データ</param>
        private void UpdateDoorTile(DoorData door)
        {
            var type = RoomTileGenerator.GetDoorTileType(door);
            _tileMapManager.UpdateTile(_tileMapLayer, door.Position, type, _tileSetManager);
        }

        /// <summary>
        /// ノード破棄時にイベント購読とViewModelを解放する
        /// </summary>
        public override void _ExitTree()
        {
            foreach (var subscription in _eventSubscriptions)
            {
                subscription.Dispose();
            }
            _eventSubscriptions.Clear();

            _tileMapManager.Free();
            _viewModel.Dispose();
        }
    }
}
