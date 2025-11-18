using System.Collections.Generic;
using Godot;
using Core.Utilities;

namespace Systems.Player.Input
{
	/// <summary>
	/// 入力の最新状態を保持し、入力ソースとの橋渡しを行う。
	/// </summary>
	public class InputState
	{
		private static readonly IReadOnlyDictionary<string, string> ButtonBindings =
			new Dictionary<string, string>
			{
				{ PlayerInputActionNames.Jump, PlayerInputActionNames.InputMap.Jump },
				{ PlayerInputActionNames.Attack, PlayerInputActionNames.InputMap.Attack },
				{ PlayerInputActionNames.Dash, PlayerInputActionNames.InputMap.Dash }
			};

		public Dictionary<string, bool> ButtonStates { get; } = new();
		public Vector2 MovementInput { get; private set; }

		/// <summary>
		/// Godot の入力マップから現在の入力状態を取得する。
		/// </summary>
		public void Update()
		{
			UpdateMovementInput();
			UpdateButtonStates();
		}

		/// <summary>
		/// 指定されたアクションが押下されているか確認する。
		/// </summary>
		public bool IsButtonPressed(string actionName)
		{
			return ButtonStates.TryGetValue(actionName, out var pressed) && pressed;
		}

		/// <summary>
		/// 現在の状態をコピーして外部比較やバインディング用に利用する。
		/// </summary>
		public InputState Clone()
		{
			var clone = new InputState();
			clone.MovementInput = MovementInput;
			foreach (var kv in ButtonStates)
			{
				clone.ButtonStates[kv.Key] = kv.Value;
			}
			return clone;
		}

		/// <summary>
		/// 移動入力とボタン状態が等しいかを比較する。
		/// </summary>
		public bool IsEquivalentTo(InputState other)
		{
			if (other == null)
			{
				return false;
			}

			if (!MovementInput.IsEqualApprox(other.MovementInput))
			{
				return false;
			}

			if (ButtonStates.Count != other.ButtonStates.Count)
			{
				return false;
			}

			foreach (var kv in ButtonStates)
			{
				if (!other.ButtonStates.TryGetValue(kv.Key, out var otherValue) || otherValue != kv.Value)
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// テスト用に移動入力を直接設定する。
		/// </summary>
		public void SetMovementInput(Vector2 input)
		{
			MovementInput = input;
		}

		private void UpdateMovementInput()
		{
			MovementInput = GodotMock.GetVector(
				PlayerInputActionNames.InputMap.MoveLeft,
				PlayerInputActionNames.InputMap.MoveRight,
				PlayerInputActionNames.InputMap.MoveUp,
				PlayerInputActionNames.InputMap.MoveDown);
		}

		private void UpdateButtonStates()
		{
			foreach (var binding in ButtonBindings)
			{
				ButtonStates[binding.Key] = GodotMock.IsActionPressed(binding.Value);
			}
		}
	}
}
