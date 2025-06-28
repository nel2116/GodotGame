using System;
using System.Collections.Generic;
using Systems.Player.Base;
using Systems.Player.Events;
using Systems.Common.Events;
using Core.Events;

namespace Systems.Player.State
{
    /// <summary>
    /// プレイヤー状態モデル
    /// </summary>
    public class PlayerStateModel : PlayerSystemBase
    {
        private readonly Dictionary<string, Base.IState> _states = new();
        private string _current_state = "Idle";
        private bool _can_change_state = true;

        public string CurrentState => _current_state;
        public bool CanChangeState => _can_change_state;

        public PlayerStateModel(IGameEventBus bus) : base(bus) { }

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

        public override void Update()
        {
            try
            {
                if (_can_change_state)
                {
                    UpdateState();
                }
            }
            catch (Exception ex)
            {
                HandleError("Update", ex);
            }
        }

        public void ChangeState(string newState)
        {
            try
            {
                if (!_states.ContainsKey(newState))
                {
                    throw new ArgumentException($"Invalid state: {newState}");
                }

                if (!_can_change_state)
                {
                    throw new InvalidOperationException("Cannot change state");
                }

                var current = _states[_current_state];
                current.Exit();
                _current_state = newState;
                _states[_current_state].Enter();
                EventBus.Publish(new StateChangedEvent(_current_state));
            }
            catch (Exception ex)
            {
                HandleError("ChangeState", ex);
            }
        }

        private void UpdateState()
        {
            var current = _states[_current_state];
            current.Update();
        }

        private void InitializeStates()
        {
            _states["Idle"] = new IdleState();
            _states["Moving"] = new MovingState();
            _states["Attacking"] = new AttackingState();
            _states["Jumping"] = new JumpingState();
            _states["Falling"] = new FallingState();
        }

        private void RegisterStateTransitions()
        {
            StateManager.RegisterTransition("Player", "Idle", () => _current_state == "Idle");
            StateManager.RegisterTransition("Player", "Moving", () => _current_state == "Moving");
            StateManager.RegisterTransition("Player", "Attacking", () => _current_state == "Attacking");
        }
    }
}
