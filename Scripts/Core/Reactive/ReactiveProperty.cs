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
        private readonly Subject<T> _subject = new();

        /// <summary>
        /// プロパティの値
        /// </summary>
        public T Value
        {
            get => _value;
            set
            {
                if (!EqualityComparer<T>.Default.Equals(_value, value))
                {
                    _value = value;
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
            _subject.Dispose();
        }
    }
}
