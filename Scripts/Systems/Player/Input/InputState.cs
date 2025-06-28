using System.Collections.Generic;
using Godot;
using Core.Utilities;

namespace Systems.Player.Input
{
	/// <summary>
	/// 入力状態を保持するクラス
	/// </summary>
	public class InputState
	{
		public Dictionary<string, bool> ButtonStates { get; } = new();
		public Vector2 MovementInput { get; private set; }

		/// <summary>
		/// 入力状態を更新する
		/// </summary>
		public void Update()
		{
			MovementInput = GodotMock.GetVector("move_left", "move_right", "move_up", "move_down");
			ButtonStates["Jump"] = GodotMock.IsActionPressed("jump");
			ButtonStates["Attack"] = GodotMock.IsActionPressed("attack");
			ButtonStates["Dash"] = GodotMock.IsActionPressed("dash");
		}

		/// <summary>
		/// テスト用：移動入力を設定する
		/// </summary>
		public void SetMovementInput(Vector2 input)
		{
			MovementInput = input;
		}
	}
}
