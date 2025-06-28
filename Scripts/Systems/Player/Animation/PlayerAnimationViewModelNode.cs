using Godot;
using Systems.Player.Animation;
using Core.Events;

public partial class PlayerAnimationViewModelNode : Node
{
    private PlayerAnimationViewModel? _viewModel;
    
    /// <summary>
    /// ViewModelが初期化されているかどうかを確認
    /// </summary>
    public bool IsInitialized => _viewModel != null;

    public string CurrentAnimation => _viewModel?.CurrentAnimation?.Value ?? "Idle";

    public void Initialize()
    {
        var bus = GameEventBus.Instance;
        var model = new PlayerAnimationModel(bus);
        _viewModel = new PlayerAnimationViewModel(model, bus);
        _viewModel.Initialize();
    }

    /// <summary>
    /// ViewModelが初期化されていることを確認し、未初期化の場合はエラーメッセージを出力してfalseを返す
    /// </summary>
    private bool EnsureViewModelInitialized()
    {
        if (_viewModel == null)
        {
            GD.PrintErr("PlayerAnimationViewModelNode: ViewModel is not initialized. Call Initialize() first.");
            return false;
        }
        return true;
    }

    public void Update()
    {
        if (!EnsureViewModelInitialized()) return;
        
        _viewModel.Update();
    }

    public void HandleAnimation(string animationName)
    {
        if (!EnsureViewModelInitialized()) return;
        
        _viewModel.HandleAnimation(animationName);
    }
} 