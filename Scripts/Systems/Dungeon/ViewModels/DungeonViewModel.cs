using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Events;
using Core.Reactive;
using Core.ViewModels;
using Godot;
using Systems.Dungeon.Data;
using Systems.Dungeon.Events;
using Systems.Dungeon.Gimmicks;
using Systems.Dungeon.Models;
using Systems.Dungeon.Navigation;
using Systems.Dungeon.Optimization;

namespace Systems.Dungeon.ViewModels
{
    /// <summary>
    /// ダンジョンビューモデル
    /// レベル生成（<see cref="LevelGenerationModel"/>）・ギミック配置（<see cref="GimmickPlacementModel"/>）・
    /// ギミック発動（<see cref="GimmickActivator"/>）・ナビゲーション（<see cref="NavigationManager"/>）の
    /// 各システムを統合し、ダンジョン全体の状態を View 層に公開してイベントを発行する
    /// </summary>
    public class DungeonViewModel : ViewModelBase
    {
        private readonly LevelGenerationModel _levelGenerationModel;
        private readonly GimmickPlacementModel _gimmickPlacementModel;
        private readonly GimmickActivator _gimmickActivator;
        private readonly NavigationManager _navigationManager;
        private readonly DungeonOptimizationCoordinator _optimizationCoordinator;
        private readonly IRoomTileRenderer _roomTileRenderer;

        /// <summary>
        /// 生成済みの部屋データ（部屋位置がキー）
        /// View 層でのミニマップ表示等を想定した公開プロパティ
        /// </summary>
        public ReactiveProperty<Dictionary<Vector2I, RoomData>> Rooms { get; }

        /// <summary>
        /// 現在プレイヤーが位置する部屋の位置
        /// </summary>
        public ReactiveProperty<Vector2I> CurrentRoomPosition { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="levelGenerationModel">レベル生成モデル</param>
        /// <param name="gimmickPlacementModel">ギミック配置モデル</param>
        /// <param name="gimmickActivator">ギミック発動管理</param>
        /// <param name="navigationManager">ナビゲーション管理</param>
        /// <param name="optimizationCoordinator">部屋可視性・部分ナビゲーション更新を束ねる最適化ファサード</param>
        /// <param name="roomTileRenderer">部屋タイル描画器</param>
        /// <param name="eventBus">イベントバス</param>
        /// <exception cref="ArgumentNullException">levelGenerationModel・gimmickPlacementModel・gimmickActivator・navigationManager・optimizationCoordinator・roomTileRenderer のいずれかが null の場合</exception>
        public DungeonViewModel(
            LevelGenerationModel levelGenerationModel,
            GimmickPlacementModel gimmickPlacementModel,
            GimmickActivator gimmickActivator,
            NavigationManager navigationManager,
            DungeonOptimizationCoordinator optimizationCoordinator,
            IRoomTileRenderer roomTileRenderer,
            IGameEventBus eventBus) : base(eventBus)
        {
            _levelGenerationModel = levelGenerationModel ?? throw new ArgumentNullException(nameof(levelGenerationModel));
            _gimmickPlacementModel = gimmickPlacementModel ?? throw new ArgumentNullException(nameof(gimmickPlacementModel));
            _gimmickActivator = gimmickActivator ?? throw new ArgumentNullException(nameof(gimmickActivator));
            _navigationManager = navigationManager ?? throw new ArgumentNullException(nameof(navigationManager));
            _optimizationCoordinator = optimizationCoordinator ?? throw new ArgumentNullException(nameof(optimizationCoordinator));
            _roomTileRenderer = roomTileRenderer ?? throw new ArgumentNullException(nameof(roomTileRenderer));

            Rooms = new ReactiveProperty<Dictionary<Vector2I, RoomData>>(new Dictionary<Vector2I, RoomData>()).AddTo(Disposables);
            CurrentRoomPosition = new ReactiveProperty<Vector2I>(Vector2I.Zero).AddTo(Disposables);
        }

        /// <summary>
        /// ダンジョンのレベルを生成する
        /// レベル生成 → ギミック配置 → ナビゲーションメッシュ構築の順に処理し、
        /// 完了後に <see cref="Rooms"/>・<see cref="CurrentRoomPosition"/>（開始部屋 <see cref="Vector2I.Zero"/>）を更新して
        /// <see cref="LevelGeneratedEvent"/> を発行する
        /// </summary>
        /// <param name="seed">レベル生成に使用する乱数シード値</param>
        public async Task GenerateLevelAsync(int seed)
        {
            await ExecuteAsync(async () =>
            {
                _levelGenerationModel.SetSeed(seed);
                var rooms = await _levelGenerationModel.GenerateLevelAsync();
                _gimmickPlacementModel.PlaceGimmicks(rooms);
                _navigationManager.BuildMesh(rooms, _levelGenerationModel.RoomTemplates);

                Rooms.Value = rooms;
                CurrentRoomPosition.Value = Vector2I.Zero;

                EventBus.Publish(new LevelGeneratedEvent(rooms.Count));

                PublishVisibilitySync(Vector2I.Zero);
            });
        }

        /// <summary>
        /// 開始地点から目標地点までの経路を探索する
        /// </summary>
        /// <param name="start">開始地点のワールドタイル座標</param>
        /// <param name="goal">目標地点のワールドタイル座標</param>
        /// <returns>開始地点から目標地点までの経路（両端を含む）。経路が存在しない場合は空リスト</returns>
        public List<Vector2I> FindPath(Vector2I start, Vector2I goal)
        {
            return _navigationManager.FindPath(start, goal);
        }

        /// <summary>
        /// 指定した部屋に入室する
        /// 対象の部屋が存在する場合は <see cref="CurrentRoomPosition"/> を更新し、<see cref="RoomEnteredEvent"/> を発行する
        /// </summary>
        /// <param name="roomPosition">入室する部屋の位置</param>
        /// <returns>入室に成功した場合は true。対象の部屋が存在しない場合は false</returns>
        public bool EnterRoom(Vector2I roomPosition)
        {
            if (!Rooms.Value.TryGetValue(roomPosition, out var room))
            {
                return false;
            }

            CurrentRoomPosition.Value = roomPosition;
            EventBus.Publish(new RoomEnteredEvent(roomPosition, room.Type));
            PublishVisibilitySync(roomPosition);
            return true;
        }

        /// <summary>
        /// 隠し通路ギミックの発動を試みる
        /// 発動に成功した場合はナビゲーションメッシュを再構築し、<see cref="Rooms"/> の変更を通知したうえで
        /// <see cref="HiddenPassageRevealedEvent"/> を、失敗した場合は <see cref="GimmickActivationFailedEvent"/>（<see cref="GimmickType.HiddenPassage"/>）を発行する
        /// </summary>
        /// <param name="roomPosition">ギミックが属する部屋の位置</param>
        /// <param name="gimmickPosition">発動対象のギミックの位置</param>
        /// <returns>発動に成功した場合は true</returns>
        public bool TryActivateHiddenPassage(Vector2I roomPosition, Vector2I gimmickPosition)
        {
            if (_gimmickActivator.TryActivateHiddenPassage(Rooms.Value, roomPosition, gimmickPosition))
            {
                RebuildNavigationForDoor(roomPosition, gimmickPosition);
                NotifyRoomsChanged();
                EventBus.Publish(new HiddenPassageRevealedEvent(roomPosition, gimmickPosition));
                return true;
            }

            EventBus.Publish(new GimmickActivationFailedEvent(roomPosition, gimmickPosition, GimmickType.HiddenPassage));
            return false;
        }

        /// <summary>
        /// 鍵扉ギミックの発動（解錠）を試みる
        /// 発動に成功した場合はナビゲーションメッシュを再構築し、<see cref="Rooms"/> の変更を通知したうえで
        /// <see cref="LockedDoorUnlockedEvent"/> を、失敗した場合は <see cref="GimmickActivationFailedEvent"/>（<see cref="GimmickType.LockedDoor"/>）を発行する
        /// </summary>
        /// <param name="roomPosition">ギミックが属する部屋の位置</param>
        /// <param name="gimmickPosition">発動対象のギミックの位置</param>
        /// <param name="hasKey">鍵を所持しているかどうか</param>
        /// <returns>発動に成功した場合は true</returns>
        public bool TryActivateLockedDoor(Vector2I roomPosition, Vector2I gimmickPosition, bool hasKey)
        {
            if (_gimmickActivator.TryActivateLockedDoor(Rooms.Value, roomPosition, gimmickPosition, hasKey))
            {
                RebuildNavigationForDoor(roomPosition, gimmickPosition);
                NotifyRoomsChanged();
                EventBus.Publish(new LockedDoorUnlockedEvent(roomPosition, gimmickPosition));
                return true;
            }

            EventBus.Publish(new GimmickActivationFailedEvent(roomPosition, gimmickPosition, GimmickType.LockedDoor));
            return false;
        }

        /// <summary>
        /// 現在部屋を基準にアクティブな部屋集合を再計算し、部屋タイルの読み込み/解放を同期したうえで、
        /// 差分があれば <see cref="RoomsVisibilityChangedEvent"/> を1回発行する
        /// </summary>
        /// <param name="roomPosition">アクティブ集合の算出基準とする部屋の位置</param>
        private void PublishVisibilitySync(Vector2I roomPosition)
        {
            var result = _optimizationCoordinator.OnRoomEntered(roomPosition, Rooms.Value, _levelGenerationModel.RoomTemplates, _roomTileRenderer);
            if (result.Loaded.Count > 0 || result.Unloaded.Count > 0)
            {
                EventBus.Publish(new RoomsVisibilityChangedEvent(result.Loaded, result.Unloaded));
            }
        }

        /// <summary>
        /// ギミック発動により状態が変化した扉の接続先部屋を求め、発動元・接続先の 2 部屋のみナビゲーションメッシュを部分再構築する
        /// </summary>
        /// <param name="roomPosition">ギミックが属していた部屋の位置</param>
        /// <param name="doorPosition">状態が変化した扉の位置（ギミック位置と同一）</param>
        private void RebuildNavigationForDoor(Vector2I roomPosition, Vector2I doorPosition)
        {
            var door = Rooms.Value[roomPosition].Doors.FirstOrDefault(d => d.Position == doorPosition);
            if (door == null)
            {
                return;
            }

            _optimizationCoordinator.OnDoorStateChanged(roomPosition, door.ConnectedRoomPosition, Rooms.Value, _levelGenerationModel.RoomTemplates);
        }

        /// <summary>
        /// <see cref="Rooms"/> の変更を購読者に通知する
        /// ギミック発動は <see cref="RoomData"/>/<see cref="DoorData"/> を in-place で書き換えるため、
        /// 同一の辞書インスタンスを再代入するだけでは参照が変わらず <see cref="ReactiveProperty{T}"/> の変更検知（既定の等価比較）が
        /// 働かない。新しい辞書インスタンスへ詰め替えて再代入することで、確実に変更通知を発火させる
        /// </summary>
        private void NotifyRoomsChanged()
        {
            Rooms.Value = new Dictionary<Vector2I, RoomData>(Rooms.Value);
        }
    }
}
