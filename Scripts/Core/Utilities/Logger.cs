using System;
using Core.Events;

namespace Core.Utilities
{
    /// <summary>
    /// イベントバスへログを送信するロガー
    /// </summary>
    public class Logger
    {
        private readonly IGameEventBus _eventBus;
        private readonly LogLevel _minimumLevel;

        public Logger(IGameEventBus eventBus, LogLevel minimumLevel = LogLevel.Info)
        {
            _eventBus = eventBus;
            _minimumLevel = minimumLevel;
        }

        public void Log(LogLevel level, string message, Exception? ex = null)
        {
            if (level < _minimumLevel) return;

            var logEvent = new LogEvent
            {
                Level = level,
                Message = message,
                Exception = ex,
                Timestamp = DateTime.UtcNow
            };

            _eventBus.Publish(logEvent);
        }
    }
}
