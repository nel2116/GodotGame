using Core.Events;

namespace Systems.Player.Events
{
    /// <summary>
    /// 入力状態変更イベント
    /// </summary>
    public class InputStateChangedEvent : GameEvent
    {
        public Input.InputState State { get; }
        public InputStateChangedEvent(Input.InputState state)
        {
            State = state;
        }
    }

    /// <summary>
    /// 入力有効状態変更イベント
    /// </summary>
    public class InputEnabledChangedEvent : GameEvent
    {
        public bool Enabled { get; }
        public InputEnabledChangedEvent(bool enabled)
        {
            Enabled = enabled;
        }
    }
}
