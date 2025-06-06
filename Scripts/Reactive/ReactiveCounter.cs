using Godot;
using UniRx;

public partial class ReactiveCounter : Node
{
    // Observable property for demonstration
    public ReactiveProperty<int> Count { get; } = new ReactiveProperty<int>(0);

    public override void _Ready()
    {
        // Subscribe to value changes
        Count.Subscribe(value => GD.Print($"Count updated: {value}"));
    }

    public void Increment()
    {
        Count.Value++;
    }
}
