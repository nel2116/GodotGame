using System;
using System.Collections.Concurrent;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Core.Events
{
    /// <summary>
    /// ゲームイベントを発行・購読するバス
    /// </summary>
    public class GameEventBus : IGameEventBus
    {
        private readonly ConcurrentDictionary<Type, ISubject<IGameEvent>> _subjects = new();

        /// <summary>
        /// イベントを発行
        /// </summary>
        public void Publish<T>(T evt) where T : IGameEvent
        {
            var subject = GetOrCreateSubject(typeof(T));
            subject.OnNext(evt);
        }

        /// <summary>
        /// 指定型のイベントストリームを取得
        /// </summary>
        public IObservable<T> GetEventStream<T>() where T : IGameEvent
        {
            return GetOrCreateSubject(typeof(T)).OfType<T>();
        }

        private ISubject<IGameEvent> GetOrCreateSubject(Type type)
        {
            return _subjects.GetOrAdd(type, _ => Subject.Synchronize(new Subject<IGameEvent>()));
        }
    }
}
