using Core.Events;

namespace Systems.Common.Events
{
    /// <summary>
    /// 状態変更イベント
    /// </summary>
    public class StateChangedEvent : GameEvent
    {
        public string State { get; }
        public StateChangedEvent(string state)
        {
            State = state;
        }
    }
}
