using System;
using System.Collections.Generic;

namespace Core.Utilities
{
    /// <summary>
    /// シンプルなイベント集約クラス
    /// </summary>
    public class EventAggregator
    {
        private readonly Dictionary<Type, object> _handlers = new();
        private readonly object _lock = new();

        public void Publish<T>(T message) where T : class
        {
            lock (_lock)
            {
                if (_handlers.TryGetValue(typeof(T), out var list))
                {
                    foreach (var handler in (List<Action<T>>)list)
                    {
                        handler(message);
                    }
                }
            }
        }

        public void Subscribe<T>(Action<T> handler) where T : class
        {
            lock (_lock)
            {
                var type = typeof(T);
                if (!_handlers.TryGetValue(type, out var list))
                {
                    list = new List<Action<T>>();
                    _handlers[type] = list;
                }
                ((List<Action<T>>)list).Add(handler);
            }
        }
    }
}
