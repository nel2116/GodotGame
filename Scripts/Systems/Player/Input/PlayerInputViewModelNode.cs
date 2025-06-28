using Godot;
using Systems.Player.Input;
using Core.Events;

public partial class PlayerInputViewModelNode : Node
{
    private PlayerInputViewModel? _viewModel;
    
    /// <summary>
    /// ViewModelが初期化されているかどうかを確認
    /// </summary>
    public bool IsInitialized => _viewModel != null;

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
        if (_viewModel == null)
        {
            GD.PrintErr("PlayerInputViewModelNode: ViewModel is not initialized. Call Initialize() first.");
            return;
        }
        
        _viewModel.UpdateInput();
    }
} 