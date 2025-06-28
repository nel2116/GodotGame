using Godot;
using Core.Utilities;

public partial class Main : Node3D
{
	// ゲーム開始時に初期化処理を行う
	public override void _Ready()
	{
		GodotMock.Print("ゲームの初期化処理を実行します。");
	}

}
