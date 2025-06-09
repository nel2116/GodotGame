using System;
using System.Collections.Generic;

namespace Core.Utilities
{
    /// <summary>
    /// 弱参照でイベントハンドラを管理するクラス
    /// </summary>
    public class WeakEventManager
    {
        private readonly Dictionary<string, List<WeakReference>> _handlers = new();

        public void AddHandler(string eventName, EventHandler handler)
        {
            if (!_handlers.TryGetValue(eventName, out var list))
            {
                list = new List<WeakReference>();
                _handlers[eventName] = list;
            }
            list.Add(new WeakReference(handler));
        }

        public void RemoveHandler(string eventName, EventHandler handler)
        {
            if (_handlers.TryGetValue(eventName, out var list))
            {
                list.RemoveAll(wr => !wr.IsAlive || wr.Target == handler);
            }
        }

        /// <summary>
        /// 登録されたハンドラを呼び出す
        /// </summary>
        public void RaiseEvent(string eventName, object sender, EventArgs args)
        {
            if (_handlers.TryGetValue(eventName, out var list))
            {
                var dead = new List<WeakReference>();
                foreach (var wr in list)
                {
                    if (wr.IsAlive && wr.Target is EventHandler handler)
                    {
                        handler(sender, args);
                    }
                    else
                    {
                        dead.Add(wr);
                    }
                }
                foreach (var d in dead)
                {
                    list.Remove(d);
                }
            }
        }
    }
}
