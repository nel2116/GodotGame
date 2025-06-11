using System;
using System.Collections.Generic;
using Systems.Player.Base;
using Systems.Player.Events;
using Core.Events;
using Systems.Player.State;

namespace Systems.Player.Combat
{
    /// <summary>
    /// プレイヤー戦闘モデル
    /// </summary>
    public class PlayerCombatModel : PlayerSystemBase
    {
        private readonly Dictionary<string, CombatData> _actions = new();
        private float _attack_power;
        private float _defense_power;
        private float _max_health;
        private float _current_health;

        public float AttackPower => _attack_power;
        public float DefensePower => _defense_power;
        public float MaxHealth => _max_health;
        public float CurrentHealth => _current_health;

        public PlayerCombatModel(IGameEventBus bus) : base(bus) { }

        public override void Initialize()
        {
            try
            {
                LoadCombatActions();
                _attack_power = 10f;
                _defense_power = 5f;
                _max_health = 100f;
                _current_health = _max_health;
                StateManager.RegisterState("Combat", new AttackingState());
                StateManager.RegisterTransition("Combat", "Damaged", () => _current_health < _max_health * 0.5f);
            }
            catch (Exception ex)
            {
                HandleError("Initialize", ex);
            }
        }

        public override void Update()
        {
            // 特別な更新処理はなし
        }

        public void Attack(string actionName)
        {
            try
            {
                if (!_actions.ContainsKey(actionName))
                {
                    throw new ArgumentException($"Invalid action: {actionName}");
                }

                var action = _actions[actionName];
                var damage = _attack_power * action.DamageMultiplier;
                action.OnExecute(damage);
                EventBus.Publish(new AttackExecutedEvent(actionName, damage));
            }
            catch (Exception ex)
            {
                HandleError("Attack", ex);
            }
        }

        public void TakeDamage(float damage)
        {
            try
            {
                var actual = MathF.Max(0, damage - _defense_power);
                _current_health = MathF.Max(0, _current_health - actual);
                EventBus.Publish(new DamageTakenEvent(actual));
            }
            catch (Exception ex)
            {
                HandleError("TakeDamage", ex);
            }
        }

        public void Heal(float amount)
        {
            try
            {
                _current_health = MathF.Min(_max_health, _current_health + amount);
                EventBus.Publish(new HealAppliedEvent(amount));
            }
            catch (Exception ex)
            {
                HandleError("Heal", ex);
            }
        }

        private void LoadCombatActions()
        {
            _actions["BasicAttack"] = new CombatData("BasicAttack", 1f, _ => { });
            _actions["StrongAttack"] = new CombatData("StrongAttack", 2f, _ => { });
            _actions["SpecialAttack"] = new CombatData("SpecialAttack", 3f, _ => { });
        }
    }
}
