using System;
using System.Collections.Generic;
using System.Linq;
using Core.Events;
using Core.Reactive;
using Core.ViewModels;
using Systems.Dungeon.Data;

namespace Systems.Dungeon.ViewModels
{
    /// <summary>
    /// 部屋ビューモデル
    /// 単一の部屋（<see cref="RoomData"/>）の状態を View 層向けに公開する軽量な ViewModel
    /// 部屋の種類は生成後に変化しないため単純なプロパティとして公開する。
    /// 扉・ギミック一覧は、部屋生成後もギミック発動（<see cref="Gimmicks.GimmickActivator"/>）等によって
    /// <see cref="RoomData"/> 側の要素が in-place で書き換えられるため、<see cref="ReactiveProperty{T}"/> でスナップショットを保持し、
    /// 変更後は <see cref="Refresh"/> を呼び出すことで最新状態への更新と変更通知を行う
    /// </summary>
    public class RoomViewModel : ViewModelBase
    {
        private readonly RoomData _room;

        /// <summary>
        /// 部屋の種類（生成後は変化しない）
        /// </summary>
        public RoomType Type { get; }

        /// <summary>
        /// 部屋が訪問済みかどうか
        /// </summary>
        public ReactiveProperty<bool> IsVisited { get; }

        /// <summary>
        /// 部屋に属する扉の一覧（<see cref="Refresh"/> 呼び出し時点のスナップショット）
        /// </summary>
        public ReactiveProperty<IReadOnlyList<DoorData>> Doors { get; }

        /// <summary>
        /// 部屋に配置されたギミックの一覧（<see cref="Refresh"/> 呼び出し時点のスナップショット）
        /// </summary>
        public ReactiveProperty<IReadOnlyList<GimmickData>> Gimmicks { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="room">表示対象の部屋データ</param>
        /// <param name="eventBus">イベントバス</param>
        /// <exception cref="ArgumentNullException">room が null の場合</exception>
        public RoomViewModel(RoomData room, IGameEventBus eventBus) : base(eventBus)
        {
            _room = room ?? throw new ArgumentNullException(nameof(room));

            Type = _room.Type;
            IsVisited = new ReactiveProperty<bool>(false).AddTo(Disposables);
            Doors = new ReactiveProperty<IReadOnlyList<DoorData>>(_room.Doors.ToList()).AddTo(Disposables);
            Gimmicks = new ReactiveProperty<IReadOnlyList<GimmickData>>(_room.Gimmicks.ToList()).AddTo(Disposables);
        }

        /// <summary>
        /// 部屋を訪問済みとしてマークする
        /// </summary>
        public void MarkVisited()
        {
            IsVisited.Value = true;
        }

        /// <summary>
        /// 扉・ギミックの一覧を <see cref="RoomData"/> の現在の状態から再取得し、変更を通知する
        /// <see cref="Gimmicks.GimmickActivator"/> 等による <see cref="RoomData"/> の in-place な状態変更後に呼び出す想定
        /// </summary>
        public void Refresh()
        {
            Doors.Value = _room.Doors.ToList();
            Gimmicks.Value = _room.Gimmicks.ToList();
        }
    }
}
