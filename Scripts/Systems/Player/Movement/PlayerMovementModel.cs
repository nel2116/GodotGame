using Core.Events;
using Core.Reactive;
using Godot;
using Systems.Common.Movement;
using Systems.Player.Events;
using Core.Utilities;

namespace Systems.Player.Movement
{
    /// <summary>
    /// プレイヤー移動モデル。
    /// 共通移動モデルを継承し、プレイヤー固有の移動ロジックを実装する。
    /// </summary>
    public class PlayerMovementModel : CommonMovementModel
    {
        private readonly CompositeDisposable _disposables = new();
        private bool _isDashing;
        private readonly IGameEventBus _eventBus;

        public PlayerMovementModel(IGameEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        /// <summary>
        /// 現在ダッシュ中かどうかを取得する。
        /// </summary>
        public bool IsDashing => _isDashing;

        /// <summary>
        /// 移動システムを初期化し、イベント購読を開始する。
        /// </summary>
        public new void Initialize()
        {
            base.Initialize();
            _isDashing = false;

            _eventBus.GetEventStream<MovementInputEvent>()
                .Subscribe(OnMovementInput)
                .AddTo(_disposables);
        }

        /// <summary>
        /// 移動入力イベントを受信したときに呼び出される。
        /// イベントの方向に移動を開始する。
        /// </summary>
        /// <param name="evt">移動入力イベント</param>
        private void OnMovementInput(MovementInputEvent evt)
        {
            base.Move(evt.Direction);
        }

        /// <summary>
        /// 移動システムを更新する。
        /// </summary>
        public new void Update()
        {
            base.Update();
        }

        /// <summary>
        /// ジャンプ処理を実行する。
        /// </summary>
        public new void Jump()
        {
            base.Jump();
        }

        /// <summary>
        /// ダッシュ処理を実行する。
        /// 既にダッシュ中の場合は何もしない。
        /// </summary>
        public new void Dash()
        {
            if (_isDashing)
            {
                return;
            }

            base.Dash();
            _isDashing = true;
        }

        /// <summary>
        /// ダッシュ状態を終了する。
        /// </summary>
        public void StopDash()
        {
            _isDashing = false;
        }

        /// <summary>
        /// リソースを解放する。
        /// イベント購読を解除し、基底クラスのリソースも解放する。
        /// </summary>
        public new void Dispose()
        {
            base.Dispose();
            _disposables.Dispose();
        }
    }
}
