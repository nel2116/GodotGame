using System;
using Core.Events;

namespace Core.Utilities
{
    /// <summary>
    /// ログイベント
    /// </summary>
    public class LogEvent : GameEvent
    {
        public LogLevel Level { get; set; }
        public string Message { get; set; } = string.Empty;
        public Exception? Exception { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
