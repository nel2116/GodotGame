using System;

namespace Core.Events
{
    /// <summary>
    /// ゲームイベントの共通インターフェース
    /// </summary>
    public interface IGameEvent
    {
        /// <summary>
        /// 発生時刻
        /// </summary>
        DateTime Timestamp { get; }
    }
}
