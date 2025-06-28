using Godot;
using Systems.Player.Input;
using Core.Events;
using Core.ViewModels;

public partial class PlayerInputViewModelNode : BaseViewModelNode
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
        if (!EnsureViewModelInitialized(_viewModel, "PlayerInputViewModelNode")) return;
        
        _viewModel.UpdateInput();
    }
} 