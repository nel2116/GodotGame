using System;
using System.Collections.Generic;
using Core.Reactive;
using Godot;

namespace Systems.Player.Input
{
    /// <summary>
    /// プレイヤー入力モデル
    /// </summary>
    public class PlayerInputModel : IDisposable
    {
        private readonly CompositeDisposable _disposables = new();
        private readonly Dictionary<string, InputAction> _actions = new();
        private readonly InputState _currentState = new();
        private bool _isEnabled;

        public InputState CurrentState => _currentState;
        public bool IsEnabled => _isEnabled;

        /// <summary>
        /// 初期化処理
        /// </summary>
        public void Initialize()
        {
            LoadInputActions();
            _isEnabled = true;
        }

        /// <summary>
        /// 更新処理
        /// </summary>
        public void Update()
        {
            if (!_isEnabled) return;
            _currentState.Update();
            ProcessInput();
        }

        private void LoadInputActions()
        {
            _actions["Move"] = new InputAction("Move", InputType.Vector2);
            _actions["Jump"] = new InputAction("Jump", InputType.Button);
            _actions["Attack"] = new InputAction("Attack", InputType.Button);
            _actions["Dash"] = new InputAction("Dash", InputType.Button);
        }

        private void ProcessInput()
        {
            foreach (var action in _actions.Values)
            {
                if (IsActionTriggered(action))
                {
                    action.Execute();
                }
            }
        }

        private bool IsActionTriggered(InputAction action)
        {
            return action.Type switch
            {
                InputType.Button => _currentState.ButtonStates.ContainsKey(action.Name) && _currentState.ButtonStates[action.Name],
                InputType.Vector2 => _currentState.MovementInput != Vector2.Zero,
                _ => false
            };
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}
