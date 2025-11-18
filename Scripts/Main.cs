using Godot;

public partial class Main : Node3D
{
	private const string PlayerNodePath = "Player";
	private Player? _playerInstance;

	/// <summary>
	/// シーンからプレイヤーノードを取得し、参照を保持する。
	/// </summary>
	public override void _Ready()
	{
		CachePlayerFromScene();
	}

	/// <summary>
	/// シーン上の Player ノードを検索し、重複生成を避ける。
	/// </summary>
	private void CachePlayerFromScene()
	{
		if (_playerInstance != null)
		{
			return;
		}

		if (!HasNode(PlayerNodePath))
		{
			GD.PrintErr("Player node was not found in the scene tree.");
			return;
		}

		_playerInstance = GetNode<Player>(PlayerNodePath);
	}
}
