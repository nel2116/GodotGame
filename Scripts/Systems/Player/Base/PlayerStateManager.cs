using System;
using System.Collections.Generic;
using System.Linq;

namespace Systems.Player.Base
{
    /// <summary>
    /// プレイヤー状態を管理するクラス
    /// </summary>
    public class PlayerStateManager
    {
        private readonly Dictionary<string, IState> _states = new();
        private readonly Dictionary<string, List<StateTransition>> _transitions = new();

        /// <summary>
        /// 状態を登録する
        /// </summary>
        public void RegisterState(string stateName, IState state)
        {
            if (_states.ContainsKey(stateName))
            {
                throw new ArgumentException($"State {stateName} is already registered");
            }

            _states[stateName] = state;
        }

        /// <summary>
        /// 状態遷移を登録する
        /// </summary>
        public void RegisterTransition(string fromState, string toState, Func<bool> condition)
        {
            if (!_states.ContainsKey(fromState) || !_states.ContainsKey(toState))
            {
                throw new ArgumentException("Invalid state transition");
            }

            if (!_transitions.ContainsKey(fromState))
            {
                _transitions[fromState] = new List<StateTransition>();
            }
            _transitions[fromState].Add(new StateTransition(toState, condition));
        }

        /// <summary>
        /// 遷移可能かを判定する
        /// </summary>
        public bool IsValidTransition(string fromState, string toState)
        {
            if (!_transitions.ContainsKey(fromState))
            {
                return false;
            }

            return _transitions[fromState].Any(t => t.ToState == toState && t.Condition());
        }
    }
}
