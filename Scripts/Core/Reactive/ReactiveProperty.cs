using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Core.Reactive
{
    /// <summary>
    /// ジェネリックなリアクティブプロパティ
    /// </summary>
    public class ReactiveProperty<T> : IReactiveProperty<T>
    {
        private T _value;
        private readonly Subject<T> _raw_subject = new();
        private readonly ISubject<T> _sync_subject;
        private readonly List<IDisposable> _disposables = new();
        private Func<T, bool>? _validator;
        private bool _is_updating;
        private readonly object _sync_lock = new();
        private bool _is_disposed;

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
                    if (_is_disposed) throw new ObjectDisposedException(nameof(ReactiveProperty<T>));
                    if (_validator != null && !_validator(value))
                    {
                        throw new ArgumentException("Validation failed", nameof(value));
                    }
                    if (!EqualityComparer<T>.Default.Equals(_value, value))
                    {
                        _value = value;
                        changed = true;
                    }
                }
                if (changed && !_is_updating)
                {
                    _sync_subject.OnNext(value);
                }
            }
        }

        /// <summary>
        /// 初期値を設定してプロパティを生成
        /// </summary>
        public ReactiveProperty(T initialValue = default)
        {
            _value = initialValue;
            // Subject.Synchronize で購読処理を同期化し、スレッド安全性を確保
            _sync_subject = Subject.Synchronize(_raw_subject);
        }

        /// <summary>
        /// 値変更通知
        /// </summary>
        public IObservable<T> ValueChanged => _sync_subject.AsObservable();

        /// <summary>
        /// 値変更を購読
        /// </summary>
        public IDisposable Subscribe(Action<T> onNext)
        {
            return _sync_subject.Subscribe(onNext);
        }

        /// <summary>
        /// バリデータを設定
        /// </summary>
        public void SetValidator(Func<T, bool> validator)
        {
            _validator = validator;
        }

        /// <summary>
        /// 値を検証
        /// </summary>
        public bool Validate(T value)
        {
            return _validator?.Invoke(value) ?? true;
        }

        /// <summary>
        /// 値を変換
        /// </summary>
        public ReactiveProperty<R> Select<R>(Func<T, R> selector)
        {
            var result = new ReactiveProperty<R>(selector(_value));
            var sub = _sync_subject.Select(selector).Subscribe(v => result.Value = v);
            _disposables.Add(sub);
            return result;
        }

        /// <summary>
        /// バッチ更新開始
        /// </summary>
        public void BeginUpdate()
        {
            _is_updating = true;
        }

        /// <summary>
        /// バッチ更新終了
        /// </summary>
        public void EndUpdate()
        {
            _is_updating = false;
            _sync_subject.OnNext(_value);
        }

        /// <summary>
        /// リソースを解放
        /// </summary>
        public void Dispose()
        {
            _sync_subject.OnCompleted();
            _raw_subject.Dispose();
            foreach (var d in _disposables)
            {
                d.Dispose();
            }
            _disposables.Clear();
            _is_disposed = true;
        }
    }
}
