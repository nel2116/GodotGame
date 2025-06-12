using Core.Events;
using Core.Reactive;
using Godot;
using Systems.Common.Movement;
using Systems.Player.Events;

namespace Systems.Player.Movement
{
    /// <summary>
    /// プレイヤー移動モデル
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
        /// ダッシュ中か
        /// </summary>
        public bool IsDashing => _isDashing;

        /// <summary>
        /// 初期化処理
        /// </summary>
        public new void Initialize()
        {
            base.Initialize();
            _isDashing = false;

            // イベントの購読
            _eventBus.GetEventStream<MovementInputEvent>()
                .Subscribe(OnMovementInput)
                .AddTo(_disposables);
        }

        private void OnMovementInput(MovementInputEvent evt)
        {
            GD.Print($"Received movement input: {evt.Direction}");
            Move(evt.Direction);
        }

        /// <summary>
        /// 更新処理
        /// </summary>
        public new void Update()
        {
            base.Update();
        }

        /// <summary>
        /// ジャンプ処理
        /// </summary>
        public new void Jump()
        {
            base.Jump();
        }

        /// <summary>
        /// ダッシュ処理
        /// </summary>
        public new void Dash()
        {
            if (!_isDashing)
            {
                base.Dash();
                _isDashing = true;
            }
        }

        /// <summary>
        /// ダッシュ終了
        /// </summary>
        public void StopDash()
        {
            _isDashing = false;
        }

        public new void Dispose()
        {
            base.Dispose();
            _disposables.Dispose();
        }
    }
}
