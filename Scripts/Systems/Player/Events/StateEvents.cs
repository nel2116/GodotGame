using Core.Events;

namespace Systems.Player.Events
{
    /// <summary>
    /// 状態変更可能フラグ変更イベント
    /// </summary>
    public class CanChangeStateChangedEvent : GameEvent
    {
        public bool CanChange { get; }
        public CanChangeStateChangedEvent(bool canChange)
        {
            CanChange = canChange;
        }
    }
}
