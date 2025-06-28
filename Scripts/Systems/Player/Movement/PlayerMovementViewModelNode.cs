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

    public Vector2 Velocity 
    {
        get
        {
            if (_viewModel?.Velocity != null)
            {
                var velocity = _viewModel.Velocity;
                if (velocity != null)
                    return velocity.Value;
            }
            return Vector2.Zero;
        }
    }
    
    public bool IsGrounded 
    {
        get
        {
            if (_viewModel?.IsGrounded != null)
            {
                var isGrounded = _viewModel.IsGrounded;
                if (isGrounded != null)
                    return isGrounded.Value;
            }
            return false;
        }
    }
    
    public bool IsDashing 
    {
        get
        {
            if (_viewModel?.IsDashing != null)
            {
                var isDashing = _viewModel.IsDashing;
                if (isDashing != null)
                    return isDashing.Value;
            }
            return false;
        }
    }

    public void Initialize()
    {
        var bus = GameEventBus.Instance;
        var model = new PlayerMovementModel(bus);
        _viewModel = new PlayerMovementViewModel(model, bus);
        _viewModel.Initialize();
    }

    public void UpdateMovement()
    {
        if (!EnsureViewModelInitialized(_viewModel)) return;
        _viewModel.UpdateMovement();
    }

    public void HandleJump()
    {
        if (!EnsureViewModelInitialized(_viewModel)) return;
        _viewModel.HandleJump();
    }

    public void HandleDash()
    {
        if (!EnsureViewModelInitialized(_viewModel)) return;
        _viewModel.HandleDash();
    }
} 