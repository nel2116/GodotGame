using Core.Events;
using Core.Reactive;
using Core.ViewModels;
using Godot;
using Systems.Common.Events;

namespace Systems.Player.Movement
{
    /// <summary>
    /// プレイヤー移動ビューモデル。
    /// 移動モデルの状態を View に公開し、変更時にイベントを発行する。
    /// </summary>
    public class PlayerMovementViewModel : ViewModelBase
    {
        private readonly PlayerMovementModel _model;
        public ReactiveProperty<Vector2> Velocity { get; }
        public ReactiveProperty<bool> IsGrounded { get; }
        public ReactiveProperty<bool> IsDashing { get; }
        public PlayerMovementModel Model => _model;

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
        /// 移動システムを初期化し、初期状態を反映する。
        /// </summary>
        public void Initialize()
        {
            _model.Initialize();
            UpdateMovementState();
        }

        /// <summary>
        /// 移動システムを更新し、最新の状態を反映する。
        /// </summary>
        public void UpdateMovement()
        {
            _model.Update();
            UpdateMovementState();
        }

        /// <summary>
        /// ジャンプ処理を実行する。
        /// </summary>
        public void HandleJump()
        {
            _model.Jump();
        }

        /// <summary>
        /// ダッシュ処理を実行する。
        /// </summary>
        public void HandleDash()
        {
            _model.Dash();
        }

        /// <summary>
        /// モデルの移動状態を取得し、ReactiveProperty に反映する。
        /// 変更時にイベントが自動的に発行される。
        /// </summary>
        private void UpdateMovementState()
        {
            Velocity.Value = _model.Velocity;
            IsGrounded.Value = _model.IsGrounded;
            IsDashing.Value = _model.IsDashing;
        }

        /// <summary>
        /// 速度が変更されたときにイベントを発行する。
        /// </summary>
        /// <param name="velocity">新しい速度ベクトル</param>
        private void OnVelocityChanged(Vector2 velocity)
        {
            EventBus.Publish(new MovementVelocityChangedEvent(velocity));
        }

        /// <summary>
        /// 接地状態が変更されたときにイベントを発行する。
        /// </summary>
        /// <param name="grounded">接地しているかどうか</param>
        private void OnGroundedChanged(bool grounded)
        {
            EventBus.Publish(new MovementGroundedChangedEvent(grounded));
        }

        /// <summary>
        /// ダッシュ状態が変更されたときにイベントを発行する。
        /// </summary>
        /// <param name="dashing">ダッシュ中かどうか</param>
        private void OnDashingChanged(bool dashing)
        {
            EventBus.Publish(new MovementDashingChangedEvent(dashing));
        }
    }
}
