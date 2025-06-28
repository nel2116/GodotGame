using Godot;
using Systems.Player.Movement;
using Core.Events;

public partial class PlayerMovementViewModelNode : Node
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

    /// <summary>
    /// ViewModelが初期化されていることを確認し、未初期化の場合はエラーメッセージを出力してfalseを返す
    /// </summary>
    private bool EnsureViewModelInitialized()
    {
        if (_viewModel == null)
        {
            GD.PrintErr("PlayerMovementViewModelNode: ViewModel is not initialized. Call Initialize() first.");
            return false;
        }
        return true;
    }

    public void UpdateMovement()
    {
        if (!EnsureViewModelInitialized()) return;
        
        _viewModel.UpdateMovement();
    }

    public void HandleJump()
    {
        if (!EnsureViewModelInitialized()) return;
        
        _viewModel.HandleJump();
    }

    public void HandleDash()
    {
        if (!EnsureViewModelInitialized()) return;
        
        _viewModel.HandleDash();
    }
} 