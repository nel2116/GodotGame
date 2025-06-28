using Core.Events;
using Core.Reactive;
using Core.ViewModels;
using Godot;
using Systems.Common.Events;

namespace Systems.Common.Movement
{
    /// <summary>
    /// 共通移動システムのビューモデル
    /// </summary>
    public class CommonMovementViewModel : ViewModelBase
    {
        private readonly CommonMovementModel _model;
        public ReactiveProperty<Vector2> Velocity { get; }
        public ReactiveProperty<bool> IsGrounded { get; }
        public ReactiveProperty<bool> CanJump { get; }
        public ReactiveProperty<bool> CanDash { get; }

        public CommonMovementViewModel(CommonMovementModel model, IGameEventBus bus)
            : base(bus)
        {
            _model = model;
            Velocity = new ReactiveProperty<Vector2>().AddTo(Disposables);
            IsGrounded = new ReactiveProperty<bool>().AddTo(Disposables);
            CanJump = new ReactiveProperty<bool>().AddTo(Disposables);
            CanDash = new ReactiveProperty<bool>().AddTo(Disposables);

            Velocity.Subscribe(OnVelocityChanged).AddTo(Disposables);
            IsGrounded.Subscribe(OnGroundedChanged).AddTo(Disposables);
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
        /// 移動処理
        /// </summary>
        public void Move(Vector2 direction)
        {
            _model.Move(direction);
            Velocity.Value = _model.Velocity;
        }

        /// <summary>
        /// ジャンプ処理
        /// </summary>
        public void Jump()
        {
            _model.Jump();
            Velocity.Value = _model.Velocity;
            CanJump.Value = _model.CanJump;
        }

        /// <summary>
        /// ダッシュ処理
        /// </summary>
        public void Dash()
        {
            _model.Dash();
            Velocity.Value = _model.Velocity;
            CanDash.Value = _model.CanDash;
        }

        private void UpdateMovementState()
        {
            Velocity.Value = _model.Velocity;
            IsGrounded.Value = _model.IsGrounded;
            CanJump.Value = _model.CanJump;
            CanDash.Value = _model.CanDash;
        }

        private void OnVelocityChanged(Vector2 velocity)
        {
            EventBus.Publish(new MovementVelocityChangedEvent(velocity));
        }

        private void OnGroundedChanged(bool grounded)
        {
            EventBus.Publish(new MovementGroundedChangedEvent(grounded));
        }
    }
}
