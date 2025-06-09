using System;
using Core.Events;
using Core.Reactive;

namespace Core.ViewModels
{
    /// <summary>
    /// ViewModelの基底クラス
    /// </summary>
    public abstract class ViewModelBase : IDisposable
    {
        /// <summary>
        /// リソース管理用のCompositeDisposable
        /// </summary>
        protected readonly CompositeDisposable Disposables = new();

        /// <summary>
        /// イベントバス
        /// </summary>
        protected readonly IGameEventBus EventBus;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        protected ViewModelBase(IGameEventBus eventBus)
        {
            EventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        }

        /// <summary>
        /// リソースの解放
        /// </summary>
        public virtual void Dispose()
        {
            Disposables.Dispose();
        }

        /// <summary>
        /// イベントの購読
        /// </summary>
        protected IDisposable SubscribeToEvent<T>(Action<T> onNext) where T : GameEvent
        {
            var subscription = EventBus.GetEventStream<T>().Subscribe(onNext);
            Disposables.Add(subscription);
            return subscription;
        }

        /// <summary>
        /// リアクティブプロパティの値を取得
        /// </summary>
        protected T GetValue<T>(IReactiveProperty<T> property)
        {
            return property.Value;
        }

        /// <summary>
        /// リアクティブプロパティの値を設定
        /// </summary>
        protected void SetValue<T>(IReactiveProperty<T> property, T value)
        {
            property.Value = value;
        }
    }
}
