using Godot;

public partial class Main : Node3D
{
    // コアシステムのインスタンス
    private readonly EventBus event_bus = new();
    private readonly StateManager state_manager = new();
    private readonly PlayerStateMachine player_state_machine = new();
    private readonly InputBuffer input_buffer = new();

    // ゲーム開始時に初期化処理を行う
    public override void _Ready()
    {
        AddChild(event_bus);
        AddChild(state_manager);
        AddChild(player_state_machine);
        AddChild(input_buffer);

        player_state_machine.EventBus = event_bus;

        GD.Print("ゲームの初期化処理を実行します。");
    }

    // 入力イベントを受け取りバッファへ送る
    public override void _Input(InputEvent @event)
    {
        input_buffer.Enqueue(@event);
    }
}

