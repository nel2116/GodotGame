using Core.Events;
using Core.Reactive;
using Core.ViewModels;
using Systems.Player.Events;

namespace Systems.Player.Combat
{
    /// <summary>
    /// プレイヤー戦闘ビューモデル
    /// </summary>
    public class PlayerCombatViewModel : ViewModelBase
    {
        private readonly PlayerCombatModel _model;
        private readonly ReactiveProperty<float> _attack_power;
        private readonly ReactiveProperty<float> _defense_power;
        private readonly ReactiveProperty<float> _current_health;
        private readonly ReactiveProperty<float> _max_health;

        public ReactiveProperty<float> AttackPower => _attack_power;
        public ReactiveProperty<float> DefensePower => _defense_power;
        public ReactiveProperty<float> CurrentHealth => _current_health;
        public ReactiveProperty<float> MaxHealth => _max_health;

        public PlayerCombatViewModel(PlayerCombatModel model, IGameEventBus bus)
            : base(bus)
        {
            _model = model;
            _attack_power = new ReactiveProperty<float>().AddTo(Disposables);
            _defense_power = new ReactiveProperty<float>().AddTo(Disposables);
            _current_health = new ReactiveProperty<float>().AddTo(Disposables);
            _max_health = new ReactiveProperty<float>().AddTo(Disposables);

            _current_health.Subscribe(OnHealthChanged).AddTo(Disposables);
            _attack_power.Subscribe(OnAttackPowerChanged).AddTo(Disposables);
        }

        public void Initialize()
        {
            _model.Initialize();
            UpdateCombatState();
        }

        public void UpdateCombat()
        {
            _model.Update();
            UpdateCombatState();
        }

        public void Attack(string actionName)
        {
            _model.Attack(actionName);
            UpdateCombatState();
        }

        public void TakeDamage(float damage)
        {
            _model.TakeDamage(damage);
            UpdateCombatState();
        }

        public void Heal(float amount)
        {
            _model.Heal(amount);
            UpdateCombatState();
        }

        private void UpdateCombatState()
        {
            _attack_power.Value = _model.AttackPower;
            _defense_power.Value = _model.DefensePower;
            _current_health.Value = _model.CurrentHealth;
            _max_health.Value = _model.MaxHealth;
        }

        private void OnHealthChanged(float health)
        {
            EventBus.Publish(new HealthChangedEvent(health));
        }

        private void OnAttackPowerChanged(float power)
        {
            EventBus.Publish(new AttackPowerChangedEvent(power));
        }
    }
}
