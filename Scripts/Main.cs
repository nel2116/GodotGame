using Godot;

public partial class Main : Node3D
{
	// ゲーム開始時に初期化処理を行うための関数
	public override void _Ready()
	{
		// ここで初期化処理を記述します
		// 例: プレイヤーの初期位置設定やスコアのリセットなど
		GD.Print("ゲームの初期化処理を実行します。");
	}
}
