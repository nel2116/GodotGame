using Godot;
using Systems.Player.Input;
using Core.Events;

public partial class PlayerInputViewModelNode : Node
{
    private PlayerInputViewModel _viewModel;

    public bool IsEnabled => _viewModel?.IsEnabled?.Value ?? false;

    public void Initialize()
    {
        var bus = GameEventBus.Instance;
        var model = new PlayerInputModel(bus);
        _viewModel = new PlayerInputViewModel(model, bus);
        _viewModel.Initialize();
    }

    public void UpdateInput()
    {
        _viewModel?.UpdateInput();
    }
} 