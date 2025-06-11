using System;
using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Core.Events
{
    /// <summary>
    /// ゲームイベントを発行・購読するバス
    /// </summary>
    public class GameEventBus : IGameEventBus, IDisposable
    {
        private readonly ConcurrentDictionary<Type, ISubject<GameEvent>> _subjects = new();
        private readonly object _dispose_lock = new();
        private bool _disposed;

        /// <summary>
        /// イベントを発行
        /// </summary>
        public void Publish<T>(T evt) where T : GameEvent
        {
            var subject = GetOrCreateSubject(typeof(T));
            subject.OnNext(evt);
        }

        /// <summary>
        /// 指定型のイベントストリームを取得
        /// </summary>
        public IObservable<T> GetEventStream<T>() where T : GameEvent
        {
            return GetOrCreateSubject(typeof(T)).OfType<T>();
        }

        private ISubject<GameEvent> GetOrCreateSubject(Type type)
        {
            return _subjects.GetOrAdd(type, _ => Subject.Synchronize(new Subject<GameEvent>()));
        }

        /// <summary>
        /// バスが保持するリソースを解放する
        /// </summary>
        public void Dispose()
        {
            lock (_dispose_lock)
            {
                if (_disposed)
                {
                    return;
                }

                foreach (var subject in _subjects.Values)
                {
                    subject.OnCompleted();
                    if (subject is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }
                _subjects.Clear();
                _disposed = true;
            }
        }
    }
}
