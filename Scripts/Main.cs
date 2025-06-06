using Godot;

public partial class Main : Node3D
{
    // ゲーム開始時に初期化処理を行うための関数
    public override void _Ready()
    {
        GD.Print("ゲームの初期化処理を実行します。");

        // プロトタイプシーンを読み込み、ツリーに追加
        var scene = GD.Load<PackedScene>("res://Scenes/ReactivePrototype.tscn");
        if (scene != null)
        {
            var instance = scene.Instantiate();
            AddChild(instance);
        }
    }
}
