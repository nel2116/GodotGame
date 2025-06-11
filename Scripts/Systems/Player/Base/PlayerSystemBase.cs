using System;
using Core.Events;
using Core.Reactive;
using Systems.Player.Events;

namespace Systems.Player.Base
{
    /// <summary>
    /// プレイヤーシステムの基底クラス
    /// </summary>
    public abstract class PlayerSystemBase : IPlayerSystem
    {
        protected readonly CompositeDisposable Disposables;
        protected readonly IGameEventBus EventBus;
        protected readonly PlayerStateManager StateManager;

        protected PlayerSystemBase(IGameEventBus eventBus)
        {
            Disposables = new CompositeDisposable();
            EventBus = eventBus;
            StateManager = new PlayerStateManager();
        }

        public abstract void Initialize();

        public abstract void Update();

        /// <summary>
        /// リソースを解放する
        /// </summary>
        public virtual void Dispose()
        {
            Disposables.Dispose();
        }

        /// <summary>
        /// エラー処理を行う
        /// </summary>
        protected void HandleError(string operation, Exception ex)
        {
            var error = new PlayerSystemException(GetType().Name, operation, ex.Message);
            EventBus.Publish(new ErrorEvent(error));
        }
    }
}
