using Godot;
using Systems.Player.Combat;
using Core.Events;
using Core.ViewModels;

public partial class PlayerCombatViewModelNode : BaseViewModelNode
{
    private PlayerCombatViewModel? _viewModel;
    
    /// <summary>
    /// ViewModelが初期化されているかどうかを確認
    /// </summary>
    public bool IsInitialized => _viewModel != null;

    public float CurrentHealth => _viewModel?.CurrentHealth?.Value ?? 0f;
    public float MaxHealth => _viewModel?.MaxHealth?.Value ?? 0f;

    public void Initialize()
    {
        var bus = GameEventBus.Instance;
        var model = new PlayerCombatModel(bus);
        _viewModel = new PlayerCombatViewModel(model, bus);
        _viewModel.Initialize();
    }

    public void UpdateCombat()
    {
        if (!EnsureViewModelInitialized(_viewModel, "PlayerCombatViewModelNode")) return;
        
        _viewModel.UpdateCombat();
    }

    public void Attack(string actionName)
    {
        if (!EnsureViewModelInitialized(_viewModel, "PlayerCombatViewModelNode")) return;
        
        _viewModel.Attack(actionName);
    }

    public void TakeDamage(float damage)
    {
        if (!EnsureViewModelInitialized(_viewModel, "PlayerCombatViewModelNode")) return;
        
        _viewModel.TakeDamage(damage);
    }

    public void Heal(float amount)
    {
        if (!EnsureViewModelInitialized(_viewModel, "PlayerCombatViewModelNode")) return;
        
        _viewModel.Heal(amount);
    }
} 