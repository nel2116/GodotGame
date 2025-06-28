using Core.Events;

namespace Systems.Common.Events
{
    /// <summary>
    /// HP変更イベント
    /// </summary>
    public class HealthChangedEvent : GameEvent
    {
        public int Health { get; }
        public HealthChangedEvent(int health)
        {
            Health = health;
        }
    }
}
