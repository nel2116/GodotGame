using System;

namespace Core.Events
{
    /// <summary>
    /// ゲームイベントの基底クラス
    /// </summary>
    public abstract class GameEvent : IGameEvent
    {
        /// <summary>
        /// イベント発生時刻
        /// </summary>
        public DateTime Timestamp { get; } = DateTime.UtcNow;
    }
}
