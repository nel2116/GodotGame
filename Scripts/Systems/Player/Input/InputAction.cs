using System;

namespace Systems.Player.Input
{
	/// <summary>
	/// 入力アクションを表現するシンプルなデータクラス。
	/// </summary>
	public class InputAction
	{
		public string Name { get; }
		public InputType Type { get; }
		public Action? ExecuteAction { get; set; }

		public InputAction(string name, InputType type)
		{
			Name = name;
			Type = type;
		}

		/// <summary>
		/// アクションに紐づいた処理を実行する。
		/// </summary>
		public void Execute()
		{
			ExecuteAction?.Invoke();
		}
	}
}
