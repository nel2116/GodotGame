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
        private readonly ReactiveProperty<float> _attackPower;
        private readonly ReactiveProperty<float> _defensePower;
        private readonly ReactiveProperty<float> _currentHealth;
        private readonly ReactiveProperty<float> _maxHealth;

        public ReactiveProperty<float> AttackPower => _attackPower;
        public ReactiveProperty<float> DefensePower => _defensePower;
        public ReactiveProperty<float> CurrentHealth => _currentHealth;
        public ReactiveProperty<float> MaxHealth => _maxHealth;

        public PlayerCombatViewModel(PlayerCombatModel model, IGameEventBus bus)
            : base(bus)
        {
            _model = model;
            _attackPower = new ReactiveProperty<float>().AddTo(Disposables);
            _defensePower = new ReactiveProperty<float>().AddTo(Disposables);
            _currentHealth = new ReactiveProperty<float>().AddTo(Disposables);
            _maxHealth = new ReactiveProperty<float>().AddTo(Disposables);

            _currentHealth.Subscribe(OnHealthChanged).AddTo(Disposables);
            _attackPower.Subscribe(OnAttackPowerChanged).AddTo(Disposables);
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

        /// <summary>
        /// 毎フレーム更新処理
        /// </summary>
        public void Update()
        {
            UpdateCombat();
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
            _attackPower.Value = _model.AttackPower;
            _defensePower.Value = _model.DefensePower;
            _currentHealth.Value = _model.CurrentHealth;
            _maxHealth.Value = _model.MaxHealth;
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
