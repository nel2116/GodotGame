using Godot;
using Core.Utilities;

public partial class Main : Node3D
{
	private Player? _player;

	// ゲーム開始時に初期化処理を行う
	public override void _Ready()
	{
		// テスト環境ではログ出力を無効化（パフォーマンス向上のため）
		// GodotMock.Print("ゲームの初期化処理を実行します。");
		
		// プレイヤーの初期化
		_player = new Player();
		AddChild(_player);
	}

}
