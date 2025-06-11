using System;
using System.Collections.Generic;
using Core.Reactive;

namespace Systems.Common.State
{
    /// <summary>
    /// 共通状態システムのモデル
    /// </summary>
    public class CommonStateModel : IStateSystem
    {
        private readonly CompositeDisposable _disposables = new();
        private string _current_state = "Idle";
        private Dictionary<string, string> _state_transitions = new();

        /// <summary>
        /// 現在の状態
        /// </summary>
        public string CurrentState => _current_state;

        /// <summary>
        /// 状態遷移表
        /// </summary>
        public Dictionary<string, string> StateTransitions => _state_transitions;

        /// <inheritdoc />
        public void Initialize()
        {
            _current_state = "Idle";
            _state_transitions = new Dictionary<string, string>
            {
                { "Idle", "Walk" },
                { "Walk", "Run" },
                { "Run", "Jump" },
                { "Jump", "Fall" },
                { "Fall", "Idle" }
            };
        }

        /// <inheritdoc />
        public void Update()
        {
            UpdateStateTransitions();
        }

        /// <inheritdoc />
        public void ChangeState(string newState)
        {
            if (CanTransitionTo(newState))
            {
                _current_state = newState;
            }
        }

        /// <inheritdoc />
        public bool CanTransitionTo(string newState)
        {
            return _state_transitions.ContainsKey(_current_state) &&
                   _state_transitions[_current_state] == newState;
        }

        private void UpdateStateTransitions()
        {
            // 状態遷移更新処理
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}
