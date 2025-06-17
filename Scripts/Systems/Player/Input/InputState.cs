using System.Collections.Generic;
using Godot;

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
			MovementInput = Godot.Input.GetVector("move_left", "move_right", "move_up", "move_down");
			ButtonStates["Jump"] = Godot.Input.IsActionPressed("jump");
			ButtonStates["Attack"] = Godot.Input.IsActionPressed("attack");
			ButtonStates["Dash"] = Godot.Input.IsActionPressed("dash");
		}
	}
}
