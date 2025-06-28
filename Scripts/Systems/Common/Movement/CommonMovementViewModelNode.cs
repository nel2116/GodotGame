using Godot;
using Systems.Common.Movement;
using Core.Events;

public partial class CommonMovementViewModelNode : Node
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
        var model = new CommonMovementModel(bus);
        _viewModel = new CommonMovementViewModel(model, bus);
        _viewModel.Initialize();
    }

    public void UpdateMovement()
    {
        if (_viewModel == null)
        {
            GD.PrintErr("CommonMovementViewModelNode: ViewModel is not initialized. Call Initialize() first.");
            return;
        }
        
        _viewModel.UpdateMovement();
    }

    public void SetVelocity(Vector2 velocity)
    {
        if (_viewModel == null)
        {
            GD.PrintErr("CommonMovementViewModelNode: ViewModel is not initialized. Call Initialize() first.");
            return;
        }
        
        _viewModel.SetVelocity(velocity);
    }
} 