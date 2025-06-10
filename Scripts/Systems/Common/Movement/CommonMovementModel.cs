using System;
using Core.Reactive;
using Godot;

namespace Systems.Common.Movement
{
    /// <summary>
    /// 共通移動システムのモデル
    /// </summary>
    public class CommonMovementModel : IMovementSystem
    {
        private readonly CompositeDisposable _disposables = new();
        private Vector2 _velocity;
        private bool _is_grounded;
        private bool _can_jump;
        private bool _can_dash;

        /// <summary>
        /// 接地判定に使用する許容誤差
        /// </summary>
        private const float GROUNDED_THRESHOLD = 0.01f;

        /// <summary>
        /// 現在の速度
        /// </summary>
        public Vector2 Velocity => _velocity;

        /// <summary>
        /// 接地しているか
        /// </summary>
        public bool IsGrounded => _is_grounded;

        /// <summary>
        /// ジャンプ可能か
        /// </summary>
        public bool CanJump => _can_jump;

        /// <summary>
        /// ダッシュ可能か
        /// </summary>
        public bool CanDash => _can_dash;

        /// <inheritdoc />
        public void Initialize()
        {
            _velocity = Vector2.Zero;
            _is_grounded = true;
            _can_jump = true;
            _can_dash = true;
        }

        /// <inheritdoc />
        public void Update()
        {
            UpdateGroundedState();
            UpdateMovementState();
        }

        /// <inheritdoc />
        public void Move(Vector2 direction)
        {
            if (_is_grounded)
            {
                _velocity = direction * GetMovementSpeed();
            }
        }

        /// <inheritdoc />
        public void Jump()
        {
            if (_is_grounded && _can_jump)
            {
                _velocity = new Vector2(_velocity.X, GetJumpForce());
                _can_jump = false;
            }
        }

        /// <inheritdoc />
        public void Dash()
        {
            if (_can_dash)
            {
                _velocity *= GetDashMultiplier();
                _can_dash = false;
            }
        }

        private void UpdateGroundedState()
        {
            _is_grounded = Math.Abs(_velocity.Y) <= GROUNDED_THRESHOLD;
            if (_is_grounded)
            {
                _can_jump = true;
                _can_dash = true;
            }
        }

        private void UpdateMovementState()
        {
            // 速度減衰の簡易処理
            _velocity *= 0.9f;
            if (_velocity.LengthSquared() < 0.001f)
            {
                _velocity = Vector2.Zero;
            }
        }

        private float GetMovementSpeed() => 5.0f;

        private float GetJumpForce() => 10.0f;

        private float GetDashMultiplier() => 2.0f;

        /// <inheritdoc />
        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}
