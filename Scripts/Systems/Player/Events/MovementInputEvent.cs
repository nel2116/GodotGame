using Godot;
using Core.Events;

namespace Systems.Player.Events
{
    /// <summary>
    /// 移動入力イベント
    /// </summary>
    public class MovementInputEvent : GameEvent
    {
        public Vector2 Direction { get; }

        public MovementInputEvent(Vector2 direction) => Direction = direction;
    }
} 