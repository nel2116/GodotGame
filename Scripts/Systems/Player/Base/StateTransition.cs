using System;

namespace Systems.Player.Base
{
    /// <summary>
    /// 状態遷移情報
    /// </summary>
    public class StateTransition
    {
        public string ToState { get; }
        public Func<bool> Condition { get; }

        public StateTransition(string toState, Func<bool> condition)
        {
            ToState = toState;
            Condition = condition;
        }
    }
}
