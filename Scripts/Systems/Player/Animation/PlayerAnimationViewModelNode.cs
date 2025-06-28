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

    public void Update()
    {
        if (_viewModel == null)
        {
            GD.PrintErr("PlayerAnimationViewModelNode: ViewModel is not initialized. Call Initialize() first.");
            return;
        }
        
        _viewModel.Update();
    }

    public void PlayAnimation(string animationName)
    {
        if (_viewModel == null)
        {
            GD.PrintErr("PlayerAnimationViewModelNode: ViewModel is not initialized. Call Initialize() first.");
            return;
        }
        
        _viewModel.PlayAnimation(animationName);
    }

    public void BlendAnimation(string animationName, float blendWeight)
    {
        if (_viewModel == null)
        {
            GD.PrintErr("PlayerAnimationViewModelNode: ViewModel is not initialized. Call Initialize() first.");
            return;
        }
        
        _viewModel.BlendAnimation(animationName, blendWeight);
    }

    public void StopAnimation()
    {
        if (_viewModel == null)
        {
            GD.PrintErr("PlayerAnimationViewModelNode: ViewModel is not initialized. Call Initialize() first.");
            return;
        }
        
        _viewModel.StopAnimation();
    }
} 