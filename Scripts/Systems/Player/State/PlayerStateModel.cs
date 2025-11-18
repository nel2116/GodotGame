using System;
using System.Collections.Generic;
using Systems.Player.Base;
using Systems.Player.Events;
using Systems.Common.Events;
using Core.Events;

namespace Systems.Player.State
{
    /// <summary>
    /// プレイヤー状態モデル。
    /// 状態マシンを管理し、状態遷移を制御する。
    /// </summary>
    public class PlayerStateModel : PlayerSystemBase
    {
        private const string DefaultStateName = "Idle";

        private readonly Dictionary<string, Base.IState> _states = new();
        private string _currentStateName = DefaultStateName;
        private bool _canChangeState = true;

        public string CurrentState => _currentStateName;
        public bool CanChangeState => _canChangeState;

        public PlayerStateModel(IGameEventBus bus) : base(bus) { }

        /// <summary>
        /// 状態システムを初期化し、すべての状態を登録する。
        /// </summary>
        public override void Initialize()
        {
            try
            {
                InitializeStates();
                RegisterStateTransitions();
                StateManager.RegisterState("Player", new IdleState());
            }
            catch (Exception ex)
            {
                HandleError("Initialize", ex);
            }
        }

        /// <summary>
        /// 状態システムを更新する。
        /// 状態変更が可能な場合のみ現在の状態を更新する。
        /// </summary>
        public override void Update()
        {
            try
            {
                if (!_canChangeState)
                {
                    return;
                }

                UpdateState();
            }
            catch (Exception ex)
            {
                HandleError("Update", ex);
            }
        }

        /// <summary>
        /// 状態を変更する。
        /// 無効な状態名や状態変更が不可能な場合は例外をスローする。
        /// </summary>
        /// <param name="newState">新しい状態名</param>
        /// <exception cref="ArgumentException">無効な状態名が指定された場合</exception>
        /// <exception cref="InvalidOperationException">状態変更が不可能な場合</exception>
        public void ChangeState(string newState)
        {
            try
            {
                ValidateStateChange(newState);

                var currentState = _states[_currentStateName];
                currentState.Exit();
                _currentStateName = newState;
                _states[_currentStateName].Enter();
                EventBus.Publish(new StateChangedEvent(_currentStateName));
            }
            catch (Exception ex)
            {
                HandleError("ChangeState", ex);
            }
        }

        /// <summary>
        /// 状態変更の妥当性を検証する。
        /// </summary>
        /// <param name="newState">新しい状態名</param>
        /// <exception cref="ArgumentException">無効な状態名が指定された場合</exception>
        /// <exception cref="InvalidOperationException">状態変更が不可能な場合</exception>
        private void ValidateStateChange(string newState)
        {
            if (!_states.ContainsKey(newState))
            {
                throw new ArgumentException($"Invalid state: {newState}", nameof(newState));
            }

            if (!_canChangeState)
            {
                throw new InvalidOperationException("Cannot change state while state change is disabled.");
            }
        }

        /// <summary>
        /// 現在の状態を更新する。
        /// </summary>
        private void UpdateState()
        {
            var currentState = _states[_currentStateName];
            currentState.Update();
        }

        /// <summary>
        /// すべての状態を初期化し、辞書に登録する。
        /// </summary>
        private void InitializeStates()
        {
            _states["Idle"] = new IdleState();
            _states["Moving"] = new MovingState();
            _states["Attacking"] = new AttackingState();
            _states["Jumping"] = new JumpingState();
            _states["Falling"] = new FallingState();
        }

        /// <summary>
        /// 状態遷移を登録する。
        /// </summary>
        private void RegisterStateTransitions()
        {
            StateManager.RegisterTransition("Player", "Idle", () => _currentStateName == "Idle");
            StateManager.RegisterTransition("Player", "Moving", () => _currentStateName == "Moving");
            StateManager.RegisterTransition("Player", "Attacking", () => _currentStateName == "Attacking");
        }
    }
}
