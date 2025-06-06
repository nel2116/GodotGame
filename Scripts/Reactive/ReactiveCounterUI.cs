using Godot;
using UniRx;

public partial class ReactiveCounterUI : Node
{
    [Export] private NodePath CounterPath;
    [Export] private NodePath LabelPath;
    [Export] private NodePath ButtonPath;

    private ReactiveCounter _counter;
    private Label _label;
    private Button _button;

    public override void _Ready()
    {
        _counter = GetNode<ReactiveCounter>(CounterPath);
        _label = GetNode<Label>(LabelPath);
        _button = GetNode<Button>(ButtonPath);

        _counter.Count.Subscribe(v => _label.Text = $"Count: {v}");
        _button.Pressed += () => _counter.Increment();
    }

    public override void _Process(double delta)
    {
        Telemetry.RecordFps();
    }
}
