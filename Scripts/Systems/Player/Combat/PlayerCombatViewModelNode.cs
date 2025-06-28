using Godot;
using Systems.Player.Combat;
using Core.Events;

public partial class PlayerCombatViewModelNode : Node
{
    private PlayerCombatViewModel _viewModel;

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
        _viewModel?.UpdateCombat();
    }

    public void Attack(string actionName)
    {
        _viewModel?.Attack(actionName);
    }

    public void TakeDamage(float damage)
    {
        _viewModel?.TakeDamage(damage);
    }

    public void Heal(float amount)
    {
        _viewModel?.Heal(amount);
    }
} 