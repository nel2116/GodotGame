using Core.Events;
using Godot;

namespace Systems.Common.Events
{
    /// <summary>
    /// 移動速度変更イベント
    /// </summary>
    public class MovementVelocityChangedEvent : GameEvent
    {
        public Vector2 Velocity { get; }
        public MovementVelocityChangedEvent(Vector2 velocity)
        {
            Velocity = velocity;
        }
    }

    /// <summary>
    /// 接地状態変更イベント
    /// </summary>
    public class MovementGroundedChangedEvent : GameEvent
    {
        public bool IsGrounded { get; }
        public MovementGroundedChangedEvent(bool grounded)
        {
            IsGrounded = grounded;
        }
    }
}
