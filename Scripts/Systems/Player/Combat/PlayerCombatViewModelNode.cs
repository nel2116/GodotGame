using Godot;
using Systems.Player.Combat;
using Core.Events;

public partial class PlayerCombatViewModelNode : Node
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
        if (_viewModel == null)
        {
            GD.PrintErr("PlayerCombatViewModelNode: ViewModel is not initialized. Call Initialize() first.");
            return;
        }
        
        _viewModel.UpdateCombat();
    }

    public void Attack(string actionName)
    {
        if (_viewModel == null)
        {
            GD.PrintErr("PlayerCombatViewModelNode: ViewModel is not initialized. Call Initialize() first.");
            return;
        }
        
        _viewModel.Attack(actionName);
    }

    public void TakeDamage(float damage)
    {
        if (_viewModel == null)
        {
            GD.PrintErr("PlayerCombatViewModelNode: ViewModel is not initialized. Call Initialize() first.");
            return;
        }
        
        _viewModel.TakeDamage(damage);
    }

    public void Heal(float amount)
    {
        if (_viewModel == null)
        {
            GD.PrintErr("PlayerCombatViewModelNode: ViewModel is not initialized. Call Initialize() first.");
            return;
        }
        
        _viewModel.Heal(amount);
    }
} 