using Core.Events;

namespace Systems.Player.Events
{
    /// <summary>
    /// フレーム進行イベント
    /// </summary>
    public class FrameAdvancedEvent : GameEvent
    {
        public int Frame { get; }
        public FrameAdvancedEvent(int frame)
        {
            Frame = frame;
        }
    }

    /// <summary>
    /// アクション開始イベント
    /// </summary>
    public class ActionStartedEvent : GameEvent
    {
        public string ActionName { get; }
        public ActionStartedEvent(string actionName)
        {
            ActionName = actionName;
        }
    }

    /// <summary>
    /// アクション終了イベント
    /// </summary>
    public class ActionEndedEvent : GameEvent
    {
        public string ActionName { get; }
        public ActionEndedEvent(string actionName)
        {
            ActionName = actionName;
        }
    }

    /// <summary>
    /// アクションキャンセルイベント
    /// </summary>
    public class ActionCanceledEvent : GameEvent
    {
        public string ActionName { get; }
        public ActionCanceledEvent(string actionName)
        {
            ActionName = actionName;
        }
    }
}
