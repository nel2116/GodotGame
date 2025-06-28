using Godot;
using Systems.Player.Movement;
using Core.Events;

public partial class PlayerMovementViewModelNode : Node
{
    private PlayerMovementViewModel _viewModel;

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
        _viewModel?.UpdateMovement();
    }

    public void HandleJump()
    {
        _viewModel?.HandleJump();
    }

    public void HandleDash()
    {
        _viewModel?.HandleDash();
    }
} 