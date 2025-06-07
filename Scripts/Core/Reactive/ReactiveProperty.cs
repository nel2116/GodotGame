using System;
using System.Collections.Generic;
using System.Reactive.Subjects;

namespace Core.Reactive
{
    /// <summary>
    /// ジェネリックなリアクティブプロパティ
    /// </summary>
    public class ReactiveProperty<T> : IReactiveProperty<T>
    {
        private T _value;
        private readonly Subject<T> _inner_subject = new();
        private readonly ISubject<T> _subject;
        private readonly object _sync_lock = new();

        /// <summary>
        /// プロパティの値
        /// </summary>
        public T Value
        {
            get => _value;
            set
            {
                bool changed = false;
                lock (_sync_lock)
                {
                    if (!EqualityComparer<T>.Default.Equals(_value, value))
                    {
                        _value = value;
                        changed = true;
                    }
                }
                if (changed)
                {
                    _subject.OnNext(value);
                }
            }
        }

        /// <summary>
        /// 初期値を設定してプロパティを生成
        /// </summary>
        public ReactiveProperty(T initialValue = default)
        {
            _value = initialValue;
            _subject = Subject.Synchronize(_inner_subject);
        }

        /// <summary>
        /// 値変更を購読
        /// </summary>
        public IDisposable Subscribe(Action<T> onNext)
        {
            return _subject.Subscribe(onNext);
        }

        /// <summary>
        /// リソースを解放
        /// </summary>
        public void Dispose()
        {
            _inner_subject.Dispose();
        }
    }
}
