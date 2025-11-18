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

    public float CurrentHealth
    {
        get
        {
            if (_viewModel?.CurrentHealth != null)
            {
                var currentHealth = _viewModel.CurrentHealth;
                if (currentHealth != null)
                    return currentHealth.Value;
            }
            return 0f;
        }
    }

    public float MaxHealth
    {
        get
        {
            if (_viewModel?.MaxHealth != null)
            {
                var maxHealth = _viewModel.MaxHealth;
                if (maxHealth != null)
                    return maxHealth.Value;
            }
            return 0f;
        }
    }

    public void Initialize()
    {
        var bus = GameEventBus.Instance;
        var model = new PlayerCombatModel(bus);
        _viewModel = new PlayerCombatViewModel(model, bus);
        _viewModel.Initialize();
    }

    public void UpdateCombat()
    {
        if (!EnsureViewModelInitialized(_viewModel)) return;
        _viewModel!.UpdateCombat();
    }

    public void Attack(string actionName)
    {
        if (!EnsureViewModelInitialized(_viewModel)) return;
        _viewModel!.Attack(actionName);
    }

    public void TakeDamage(float damage)
    {
        if (!EnsureViewModelInitialized(_viewModel)) return;
        _viewModel!.TakeDamage(damage);
    }

    public void Heal(float amount)
    {
        if (!EnsureViewModelInitialized(_viewModel)) return;
        _viewModel!.Heal(amount);
    }
}
