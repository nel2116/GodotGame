using System;
using System.Collections.Generic;
using Godot;
using Core.Events;
using Systems.Player.Events;
using Core.Utilities;

namespace Systems.Player.Input
{
	/// <summary>
	/// 入力状態とイベント発行を担うプレイヤー入力モデル。
	/// </summary>
	public class PlayerInputModel : IDisposable
	{
		private readonly Dictionary<string, InputAction> _actions = new();
		private readonly InputState _currentState = new();
		private bool _isEnabled;
		private readonly IGameEventBus _eventBus;

		public InputState CurrentState => _currentState;
		public bool IsEnabled => _isEnabled;

		public PlayerInputModel()
		{
			_eventBus = GameEventBus.Instance;
			InitializeActions();
		}

		public PlayerInputModel(IGameEventBus eventBus)
		{
			_eventBus = eventBus;
			InitializeActions();
		}

		/// <summary>
		/// 入力を受け付け可能な状態にする。
		/// </summary>
		public void Initialize()
		{
			_isEnabled = true;
			_eventBus.Publish(new InputEnabledChangedEvent(true));
		}

		/// <summary>
		/// 入力を読み取り、対応するイベントを発行する。
		/// </summary>
		public void UpdateInput()
		{
			if (!_isEnabled)
			{
				return;
			}

			_currentState.Update();
			ProcessInput();
			_eventBus.Publish(new InputStateChangedEvent(_currentState));
		}

		private void InitializeActions()
		{
			RegisterAction(PlayerInputActionNames.Move, InputType.Vector2, HandleMoveAction);
			RegisterButtonAction(PlayerInputActionNames.Jump, () => _eventBus.Publish(new JumpInputEvent()));
			RegisterButtonAction(PlayerInputActionNames.Attack, () => _eventBus.Publish(new AttackInputEvent()));
			RegisterButtonAction(PlayerInputActionNames.Dash, () => _eventBus.Publish(new DashInputEvent()));
		}

		private void RegisterAction(string name, InputType type, Action executeAction)
		{
			_actions[name] = new InputAction(name, type)
			{
				ExecuteAction = executeAction
			};
		}

		private void RegisterButtonAction(string name, Action publishEvent)
		{
			RegisterAction(name, InputType.Button, () =>
			{
				if (_currentState.IsButtonPressed(name))
				{
					publishEvent();
				}
			});
		}

		protected virtual void ProcessInput()
		{
			ExecuteDefaultActions();
		}

		protected void ExecuteDefaultActions()
		{
			ExecuteAction(PlayerInputActionNames.Move);

			foreach (var actionName in PlayerInputActionNames.ButtonNames)
			{
				ExecuteAction(actionName);
			}
		}

		protected void ExecuteAction(string actionName)
		{
			if (_actions.TryGetValue(actionName, out var action))
			{
				action.Execute();
				return;
			}

			if (!GodotMock.IsTestEnvironment())
			{
				GD.PrintErr($"Action '{actionName}' was not registered.");
			}
		}

		protected virtual void HandleMoveAction()
		{
			var input = _currentState.MovementInput;
			if (input == Vector2.Zero)
			{
				return;
			}

			PublishMovementEvent(input);
		}

		protected void PublishMovementEvent(Vector2 input)
		{
			_eventBus.Publish(new MovementInputEvent(input.Normalized()));
		}

		public void Dispose()
		{
		}
	}
}
