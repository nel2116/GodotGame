using System;
using Core.Events;
using Core.Reactive;
using Core.Utilities;
using System.Threading.Tasks;

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
        /// ビジー状態
        /// </summary>
        protected ReactiveProperty<bool> IsBusy { get; } = new(false);

        /// <summary>
        /// ViewModelの状態
        /// </summary>
        public ReactiveProperty<ViewModelState> State { get; } = new(ViewModelState.Initial);

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
        /// IDisposable を登録
        /// </summary>
        protected void AddDisposable(IDisposable disposable)
        {
            if (disposable == null) return;
            Disposables.Add(disposable);
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
        /// ReactiveCommand を生成
        /// </summary>
        protected ReactiveCommand CreateCommand()
        {
            return new ReactiveCommand().AddTo(Disposables);
        }

        /// <summary>
        /// 型付き ReactiveCommand を生成
        /// </summary>
        protected ReactiveCommand<T> CreateCommand<T>()
        {
            return new ReactiveCommand<T>().AddTo(Disposables);
        }

        /// <summary>
        /// 非同期処理を実行
        /// </summary>
        protected async Task ExecuteAsync(Func<Task> action)
        {
            try
            {
                IsBusy.Value = true;
                await action();
            }
            finally
            {
                IsBusy.Value = false;
            }
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

        /// <summary>
        /// アクティブ化処理
        /// </summary>
        public virtual void Activate()
        {
            OnActivate();
        }

        /// <summary>
        /// 非アクティブ化処理
        /// </summary>
        public virtual void Deactivate()
        {
            OnDeactivate();
        }

        /// <summary>
        /// アクティブ化時の処理
        /// </summary>
        protected virtual void OnActivate()
        {
            State.Value = ViewModelState.Active;
        }

        /// <summary>
        /// 非アクティブ化時の処理
        /// </summary>
        protected virtual void OnDeactivate()
        {
            State.Value = ViewModelState.Inactive;
        }
    }
}
