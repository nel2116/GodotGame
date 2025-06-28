using Godot;
using Systems.Player.Movement;
using Core.Events;
using Core.ViewModels;

public partial class PlayerMovementViewModelNode : BaseViewModelNode
{
    private PlayerMovementViewModel? _viewModel;
    
    /// <summary>
    /// ViewModelが初期化されているかどうかを確認
    /// </summary>
    public bool IsInitialized => _viewModel != null;

    public Vector2 Velocity => _viewModel?.Velocity?.Value ?? Vector2.Zero;
    public bool IsGrounded => _viewModel?.IsGrounded?.Value ?? false;
    public bool IsDashing => _viewModel?.IsDashing?.Value ?? false;

    public void Initialize()
    {
        var bus = GameEventBus.Instance; // シングルトン or GDScriptから渡す場合は修正
        var model = new PlayerMovementModel(bus);
        _viewModel = new PlayerMovementViewModel(model, bus);
        _viewModel.Initialize();
    }

    public void UpdateMovement()
    {
        if (!EnsureViewModelInitialized(_viewModel, "PlayerMovementViewModelNode")) return;
        
        _viewModel.UpdateMovement();
    }

    public void HandleJump()
    {
        if (!EnsureViewModelInitialized(_viewModel, "PlayerMovementViewModelNode")) return;
        
        _viewModel.HandleJump();
    }

    public void HandleDash()
    {
        if (!EnsureViewModelInitialized(_viewModel, "PlayerMovementViewModelNode")) return;
        
        _viewModel.HandleDash();
    }
} 