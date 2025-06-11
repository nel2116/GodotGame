using Core.Events;

namespace Systems.Player.Events
{
    /// <summary>
    /// 攻撃実行イベント
    /// </summary>
    public class AttackExecutedEvent : GameEvent
    {
        public string ActionName { get; }
        public float Damage { get; }
        public AttackExecutedEvent(string actionName, float damage)
        {
            ActionName = actionName;
            Damage = damage;
        }
    }

    /// <summary>
    /// ダメージ受け取りイベント
    /// </summary>
    public class DamageTakenEvent : GameEvent
    {
        public float Damage { get; }
        public DamageTakenEvent(float damage)
        {
            Damage = damage;
        }
    }

    /// <summary>
    /// 回復適用イベント
    /// </summary>
    public class HealAppliedEvent : GameEvent
    {
        public float Amount { get; }
        public HealAppliedEvent(float amount)
        {
            Amount = amount;
        }
    }

    /// <summary>
    /// 体力変更イベント
    /// </summary>
    public class HealthChangedEvent : GameEvent
    {
        public float Health { get; }
        public HealthChangedEvent(float health)
        {
            Health = health;
        }
    }

    /// <summary>
    /// 攻撃力変更イベント
    /// </summary>
    public class AttackPowerChangedEvent : GameEvent
    {
        public float AttackPower { get; }
        public AttackPowerChangedEvent(float power)
        {
            AttackPower = power;
        }
    }
}
