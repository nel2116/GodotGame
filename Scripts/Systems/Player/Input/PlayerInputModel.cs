using System;
using System.Collections.Generic;
using Core.Reactive;
using Godot;
using Core.Events;
using Systems.Player.Events;
using Core.Utilities;

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
        private readonly IGameEventBus _eventBus;

        public InputState CurrentState => _currentState;
        public bool IsEnabled => _isEnabled;

        public PlayerInputModel()
        {
            _eventBus = GameEventBus.Instance;
            _isEnabled = false;
            InitializeActions();
        }

        /// <summary>
        /// イベントバスを指定したコンストラクタ
        /// </summary>
        public PlayerInputModel(IGameEventBus eventBus)
        {
            _eventBus = eventBus;
            _isEnabled = false;
            InitializeActions();
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        public void Initialize()
        {
            _isEnabled = true;
            _eventBus.Publish(new InputEnabledChangedEvent(true));
        }

        /// <summary>
        /// 更新処理
        /// </summary>
        public void UpdateInput()
        {
            if (!_isEnabled) return;

            _currentState.Update();
            ProcessInput();
            _eventBus.Publish(new InputStateChangedEvent(_currentState));
        }

        private void InitializeActions()
        {
            _actions["Move"] = new InputAction("Move", InputType.Vector2);
            _actions["Move"].ExecuteAction = () => {
                // テスト環境ではログ出力を無効化（パフォーマンス向上のため）
                // GodotMock.Print($"Executing move action with input: {_currentState.MovementInput}");
                if (_currentState.MovementInput != Vector2.Zero)
                {
                    var direction = _currentState.MovementInput.Normalized();
                    _eventBus.Publish(new MovementInputEvent(direction));
                }
            };

            _actions["Jump"] = new InputAction("Jump", InputType.Button);
            _actions["Jump"].ExecuteAction = () => {
                if (_currentState.ButtonStates["Jump"])
                {
                    _eventBus.Publish(new JumpInputEvent());
                }
            };

            _actions["Attack"] = new InputAction("Attack", InputType.Button);
            _actions["Attack"].ExecuteAction = () => {
                if (_currentState.ButtonStates["Attack"])
                {
                    _eventBus.Publish(new AttackInputEvent());
                }
            };

            _actions["Dash"] = new InputAction("Dash", InputType.Button);
            _actions["Dash"].ExecuteAction = () => {
                if (_currentState.ButtonStates["Dash"])
                {
                    _eventBus.Publish(new DashInputEvent());
                }
            };
        }

        private void ProcessInput()
        {
            // テスト環境ではログ出力を無効化（パフォーマンス向上のため）
            // GodotMock.Print($"Processing move input: {_currentState.MovementInput}");

            // 移動入力の処理
            if (_currentState.MovementInput != Vector2.Zero)
            {
                if (_actions.TryGetValue("Move", out var action))
                {
                    // TryGetValueがtrueを返した場合、actionはnullではない
                    // テスト環境ではログ出力を無効化（パフォーマンス向上のため）
                    // GodotMock.Print($"Action triggered: {action.Name}");
                    action!.ExecuteAction();
                }
                else
                {
                    // Moveアクションが見つからない場合のログ出力
                    if (!GodotMock.IsTestEnvironment())
                    {
                        GD.PrintErr("Move action not found in _actions dictionary");
                    }
                }
            }

            // ボタン入力の処理
            foreach (var action in _actions.Values)
            {
                if (action != null &&
                    action.Type == InputType.Button &&
                    _currentState.ButtonStates.ContainsKey(action.Name) &&
                    _currentState.ButtonStates[action.Name])
                {
                    // テスト環境ではログ出力を無効化（パフォーマンス向上のため）
                    // GodotMock.Print($"Action {action.Name} is triggered");
                    action.ExecuteAction();
                }
            }
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}
