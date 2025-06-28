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
        var model = new CommonMovementModel();
        _viewModel = new CommonMovementViewModel(model, bus);
        _viewModel.Initialize();
    }

    /// <summary>
    /// ViewModelが初期化されていることを確認し、未初期化の場合はエラーメッセージを出力してfalseを返す
    /// </summary>
    private bool EnsureViewModelInitialized()
    {
        if (_viewModel == null)
        {
            GD.PrintErr("CommonMovementViewModelNode: ViewModel is not initialized. Call Initialize() first.");
            return false;
        }
        return true;
    }

    public void UpdateMovement()
    {
        if (!EnsureViewModelInitialized()) return;
        
        _viewModel.UpdateMovement();
    }

    public void Move(Vector2 direction)
    {
        if (!EnsureViewModelInitialized()) return;
        
        _viewModel.Move(direction);
    }

    public void Jump()
    {
        if (!EnsureViewModelInitialized()) return;
        
        _viewModel.Jump();
    }

    public void Dash()
    {
        if (!EnsureViewModelInitialized()) return;
        
        _viewModel.Dash();
    }
} 