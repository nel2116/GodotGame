using Godot;
using Systems.Player.Animation;
using Core.Events;
using Core.ViewModels;

public partial class PlayerAnimationViewModelNode : BaseViewModelNode
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

    public void Update()
    {
        if (!EnsureViewModelInitialized(_viewModel, "PlayerAnimationViewModelNode")) return;
        
        _viewModel.Update();
    }

    public void HandleAnimation(string animationName)
    {
        if (!EnsureViewModelInitialized(_viewModel, "PlayerAnimationViewModelNode")) return;
        
        _viewModel.HandleAnimation(animationName);
    }
} 