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
        private float _verticalVelocity = 0f; // 3DのY軸（上下）用
        private bool _is_grounded;
        private bool _can_jump;
        private bool _can_dash;

        /// <summary>
        /// 接地判定に使用する許容誤差
        /// </summary>
        private const float GROUNDED_THRESHOLD = 0.01f;

        /// <summary>
        /// 速度減衰率
        /// </summary>
        private const float DAMPING_FACTOR = 0.9f;

        /// <summary>
        /// 速度ゼロ判定に使用するしきい値
        /// </summary>
        private const float VELOCITY_THRESHOLD = 0.001f;

        /// <summary>
        /// 重力加速度
        /// </summary>
        private const float GRAVITY = 9.8f;

        /// <summary>
        /// 現在の速度
        /// </summary>
        public Vector2 Velocity => _velocity;

        /// <summary>
        /// 垂直速度
        /// </summary>
        public float VerticalVelocity => _verticalVelocity;

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
            _verticalVelocity = 0f;
            _is_grounded = true;
            _can_jump = true;
            _can_dash = true;
        }

        /// <inheritdoc />
        public void Update()
        {
            UpdateGroundedState();
            UpdateMovementState();
            ApplyGravity();
        }

        /// <inheritdoc />
        public void Move(Vector2 direction)
        {
            if (_is_grounded)
            {
                if (direction == Vector2.Zero)
                {
                    // しきい値以下なら完全にゼロにする
                    if (Mathf.Abs(_velocity.X) < 0.01f) _velocity.X = 0;
                    if (Mathf.Abs(_velocity.Y) < 0.01f) _velocity.Y = 0;
                }
                else
                {
                    _velocity = direction * GetMovementSpeed();
                }
            }
        }

        /// <inheritdoc />
        public void Jump()
        {
            if (_can_jump && _is_grounded)
            {
                _verticalVelocity = -GetJumpForce();
                _is_grounded = false;
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
            // 接地判定はCharacterBody3DのIsOnFloor()を使用
            _is_grounded = Mathf.Abs(_verticalVelocity) < GROUNDED_THRESHOLD;
            GD.Print($"Grounded state: {_is_grounded}, Y velocity: {_verticalVelocity}");
            if (_is_grounded)
            {
                _can_jump = true;
                _can_dash = true;
                _verticalVelocity = 0f;
            }
        }

        private void UpdateMovementState()
        {
            if (_is_grounded)
            {
                _velocity.X *= DAMPING_FACTOR;
                _velocity.Y *= DAMPING_FACTOR;
                if (Mathf.Abs(_velocity.X) < 0.01f) _velocity.X = 0;
                if (Mathf.Abs(_velocity.Y) < 0.01f) _velocity.Y = 0;
            }
        }

        private void ApplyGravity()
        {
            if (!_is_grounded)
            {
                _verticalVelocity += GRAVITY * 0.016f; // 約60FPSを想定
                GD.Print($"Applying gravity, new Y velocity: {_verticalVelocity}");
            }
        }

        private float GetMovementSpeed()
        {
            return 5.0f; // 基本移動速度
        }

        private float GetJumpForce()
        {
            return 10.0f; // ジャンプ力
        }

        private float GetDashMultiplier()
        {
            return 2.0f; // ダッシュ時の速度倍率
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}
