using System;
using System.Collections.Generic;
using Core.Events;
using Core.Reactive;
using Core.ViewModels;
using Systems.Dungeon.Data;

namespace Systems.Dungeon.ViewModels
{
    /// <summary>
    /// 部屋ビューモデル
    /// 単一の部屋（<see cref="RoomData"/>）の状態を View 層向けに公開する軽量な ViewModel
    /// 扉・ギミック一覧は、部屋生成後もギミック発動（<see cref="Gimmicks.GimmickActivator"/>）等によって
    /// <see cref="RoomData"/> 側の要素が in-place で書き換えられるため、キャッシュを持たず
    /// 都度 <see cref="RoomData"/> を参照するプロパティとして公開する（常に最新の状態を返す）
    /// </summary>
    public class RoomViewModel : ViewModelBase
    {
        private readonly RoomData _room;

        /// <summary>
        /// 部屋の種類
        /// </summary>
        public ReactiveProperty<RoomType> Type { get; }

        /// <summary>
        /// 部屋が訪問済みかどうか
        /// </summary>
        public ReactiveProperty<bool> IsVisited { get; }

        /// <summary>
        /// 部屋に属する扉の一覧
        /// <see cref="RoomData"/> の現在の状態をそのまま返すため、都度最新の内容を反映する
        /// </summary>
        public IReadOnlyList<DoorData> Doors => _room.Doors;

        /// <summary>
        /// 部屋に配置されたギミックの一覧
        /// <see cref="RoomData"/> の現在の状態をそのまま返すため、都度最新の内容を反映する
        /// </summary>
        public IReadOnlyList<GimmickData> Gimmicks => _room.Gimmicks;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="room">表示対象の部屋データ</param>
        /// <param name="eventBus">イベントバス</param>
        /// <exception cref="ArgumentNullException">room が null の場合</exception>
        public RoomViewModel(RoomData room, IGameEventBus eventBus) : base(eventBus)
        {
            _room = room ?? throw new ArgumentNullException(nameof(room));

            Type = new ReactiveProperty<RoomType>(_room.Type).AddTo(Disposables);
            IsVisited = new ReactiveProperty<bool>(false).AddTo(Disposables);
        }

        /// <summary>
        /// 部屋を訪問済みとしてマークする
        /// </summary>
        public void MarkVisited()
        {
            IsVisited.Value = true;
        }
    }
}
