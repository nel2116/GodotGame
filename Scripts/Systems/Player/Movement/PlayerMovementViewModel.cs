using Core.Events;
using Core.Reactive;
using Core.ViewModels;
using Godot;
using Systems.Common.Events;

namespace Systems.Player.Movement
{
    /// <summary>
    /// プレイヤー移動ビューモデル
    /// </summary>
    public class PlayerMovementViewModel : ViewModelBase
    {
        private readonly PlayerMovementModel _model;
        public ReactiveProperty<Vector2> Velocity { get; }
        public ReactiveProperty<bool> IsGrounded { get; }
        public ReactiveProperty<bool> IsDashing { get; }

        public PlayerMovementViewModel(PlayerMovementModel model, IGameEventBus bus)
            : base(bus)
        {
            _model = model;
            Velocity = new ReactiveProperty<Vector2>().AddTo(Disposables);
            IsGrounded = new ReactiveProperty<bool>().AddTo(Disposables);
            IsDashing = new ReactiveProperty<bool>().AddTo(Disposables);

            Velocity.Subscribe(OnVelocityChanged).AddTo(Disposables);
            IsGrounded.Subscribe(OnGroundedChanged).AddTo(Disposables);
            IsDashing.Subscribe(OnDashingChanged).AddTo(Disposables);
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        public void Initialize()
        {
            _model.Initialize();
            UpdateMovementState();
        }

        /// <summary>
        /// 更新処理
        /// </summary>
        public void UpdateMovement()
        {
            _model.Update();
            UpdateMovementState();
        }

        /// <summary>
        /// ジャンプ処理
        /// </summary>
        public void HandleJump()
        {
            _model.Jump();
        }

        /// <summary>
        /// ダッシュ処理
        /// </summary>
        public void HandleDash()
        {
            _model.Dash();
        }

        private void UpdateMovementState()
        {
            Velocity.Value = _model.Velocity;
            IsGrounded.Value = _model.IsGrounded;
            IsDashing.Value = _model.IsDashing;
        }

        private void OnVelocityChanged(Vector2 velocity)
        {
            EventBus.Publish(new MovementVelocityChangedEvent(velocity));
        }

        private void OnGroundedChanged(bool grounded)
        {
            EventBus.Publish(new MovementGroundedChangedEvent(grounded));
        }

        private void OnDashingChanged(bool dashing)
        {
            EventBus.Publish(new MovementDashingChangedEvent(dashing));
        }
    }
}
