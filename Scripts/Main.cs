using Godot;

public partial class Main : Node3D
{
	private const string PlayerNodePath = "Player";
	private Player? _cachedPlayer;

	/// <summary>
	/// シーンからプレイヤーノードを取得し、参照を保持する。
	/// 既にキャッシュ済みの場合は再取得をスキップする。
	/// </summary>
	public override void _Ready()
	{
		TryCachePlayerFromScene();
	}

	/// <summary>
	/// シーン上の Player ノードを検索し、参照をキャッシュする。
	/// ノードが見つからない場合はエラーログを出力して終了する。
	/// </summary>
	private void TryCachePlayerFromScene()
	{
		if (_cachedPlayer != null)
		{
			return;
		}

		if (!HasNode(PlayerNodePath))
		{
			GD.PrintErr($"Player node not found at path: '{PlayerNodePath}'. Ensure the Player node exists in the scene tree.");
			return;
		}

		_cachedPlayer = GetNode<Player>(PlayerNodePath);
	}
}
