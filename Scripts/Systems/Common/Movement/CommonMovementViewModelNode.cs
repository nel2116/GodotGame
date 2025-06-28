using Godot;
using Systems.Common.Movement;
using Core.Events;
using Core.ViewModels;

public partial class CommonMovementViewModelNode : BaseViewModelNode
{
    private CommonMovementViewModel? _viewModel;
    
    /// <summary>
    /// ViewModelが初期化されているかどうかを確認
    /// </summary>
    public bool IsInitialized => _viewModel != null;

    public Vector2 Velocity => _viewModel?.Velocity?.Value ?? Vector2.Zero;

    public void Initialize()
    {
        var bus = GameEventBus.Instance;
        var model = new CommonMovementModel();
        _viewModel = new CommonMovementViewModel(model, bus);
        _viewModel.Initialize();
    }

    public void UpdateMovement()
    {
        if (!EnsureViewModelInitialized(_viewModel, "CommonMovementViewModelNode")) return;
        
        _viewModel.UpdateMovement();
    }

    public void Move(Vector2 direction)
    {
        if (!EnsureViewModelInitialized(_viewModel, "CommonMovementViewModelNode")) return;
        
        _viewModel.Move(direction);
    }

    public void Jump()
    {
        if (!EnsureViewModelInitialized(_viewModel, "CommonMovementViewModelNode")) return;
        
        _viewModel.Jump();
    }

    public void Dash()
    {
        if (!EnsureViewModelInitialized(_viewModel, "CommonMovementViewModelNode")) return;
        
        _viewModel.Dash();
    }
} 