using Godot;

public partial class Main : Node3D
{
	// ゲーム開始時に初期化処理を行う
	public override void _Ready()
	{
		GD.Print("ゲームの初期化処理を実行します。");
	}

}
