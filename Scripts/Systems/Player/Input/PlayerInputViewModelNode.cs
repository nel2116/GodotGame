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

    /// <summary>
    /// ViewModelが初期化されていることを確認し、未初期化の場合はエラーメッセージを出力してfalseを返す
    /// </summary>
    private bool EnsureViewModelInitialized()
    {
        if (_viewModel == null)
        {
            GD.PrintErr("PlayerInputViewModelNode: ViewModel is not initialized. Call Initialize() first.");
            return false;
        }
        return true;
    }

    public void UpdateInput()
    {
        if (!EnsureViewModelInitialized()) return;
        
        _viewModel.UpdateInput();
    }
} 