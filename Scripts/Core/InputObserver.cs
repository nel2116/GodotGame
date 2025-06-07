using Godot;

#nullable enable

[GlobalClass]
public partial class InputObserver : Node
{
    // 監視対象のバッファ
    [Export]
    public InputBuffer? Buffer { get; set; }

    public override void _UnhandledInput(InputEvent @event)
    {
        Buffer?.Enqueue(@event);
    }
}

