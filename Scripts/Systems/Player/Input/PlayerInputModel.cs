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
	/// Godot の入力システムから入力を読み取り、対応するイベントを発行する。
	/// </summary>
	public class PlayerInputModel : IDisposable
	{
		private readonly Dictionary<string, InputAction> _actions = new();
		private readonly InputState _currentState = new();
		private bool _isInputEnabled;
		private readonly IGameEventBus _eventBus;

		public InputState CurrentState => _currentState;
		public bool IsEnabled => _isInputEnabled;

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
		/// 初期化時に一度だけ呼び出す。
		/// </summary>
		public void Initialize()
		{
			_isInputEnabled = true;
			_eventBus.Publish(new InputEnabledChangedEvent(true));
		}

		/// <summary>
		/// 入力を読み取り、対応するイベントを発行する。
		/// 入力が無効な場合は何もしない。
		/// </summary>
		public void UpdateInput()
		{
			if (!_isInputEnabled)
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

		/// <summary>
		/// ボタンアクションを登録する。
		/// ボタンが押されている場合のみイベントを発行する。
		/// </summary>
		/// <param name="name">アクション名</param>
		/// <param name="publishEvent">イベント発行処理</param>
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

		/// <summary>
		/// 入力処理を実行する。
		/// サブクラスでオーバーライド可能。
		/// </summary>
		protected virtual void ProcessInput()
		{
			ExecuteDefaultActions();
		}

		/// <summary>
		/// デフォルトのアクションを実行する。
		/// 移動入力とすべてのボタン入力を処理する。
		/// </summary>
		protected void ExecuteDefaultActions()
		{
			ExecuteAction(PlayerInputActionNames.Move);

			foreach (var buttonActionName in PlayerInputActionNames.ButtonNames)
			{
				ExecuteAction(buttonActionName);
			}
		}

		/// <summary>
		/// 指定されたアクションを実行する。
		/// アクションが登録されていない場合はエラーログを出力する。
		/// </summary>
		/// <param name="actionName">実行するアクション名</param>
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

		/// <summary>
		/// 移動アクションを処理する。
		/// 入力がゼロの場合は何もしない。
		/// </summary>
		protected virtual void HandleMoveAction()
		{
			var movementInput = _currentState.MovementInput;
			if (movementInput == Vector2.Zero)
			{
				return;
			}

			PublishMovementEvent(movementInput);
		}

		/// <summary>
		/// 移動イベントを発行する。
		/// 入力ベクトルは正規化されてから発行される。
		/// </summary>
		/// <param name="input">移動入力ベクトル</param>
		protected void PublishMovementEvent(Vector2 input)
		{
			var normalizedInput = input.Normalized();
			_eventBus.Publish(new MovementInputEvent(normalizedInput));
		}

		public void Dispose()
		{
		}
	}
}
