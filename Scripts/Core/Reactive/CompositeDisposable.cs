using System;
using System.Collections.Generic;

namespace Core.Reactive
{
    /// <summary>
    /// IDisposableをまとめて管理するクラス
    /// </summary>
    public class CompositeDisposable : IDisposable
    {
        private readonly List<IDisposable> _disposables = new();
        private bool _isDisposed;
        private readonly object _syncLock = new();

        /// <summary>
        /// 破棄対象を追加
        /// </summary>
        public void Add(IDisposable disposable)
        {
            if (disposable == null) throw new ArgumentNullException(nameof(disposable));

            lock (_syncLock)
            {
                if (_isDisposed)
                {
                    disposable.Dispose();
                    return;
                }
                _disposables.Add(disposable);
            }
        }

        /// <summary>
        /// 複数の破棄対象を追加
        /// </summary>
        public void AddRange(IEnumerable<IDisposable> disposables)
        {
            if (disposables == null) throw new ArgumentNullException(nameof(disposables));

            foreach (var disposable in disposables)
            {
                Add(disposable);
            }
        }

        /// <summary>
        /// 指定した破棄対象を削除
        /// </summary>
        public bool Remove(IDisposable disposable)
        {
            if (disposable == null) throw new ArgumentNullException(nameof(disposable));

            lock (_syncLock)
            {
                return _disposables.Remove(disposable);
            }
        }

        /// <summary>
        /// 全ての破棄対象を解放
        /// </summary>
        public void Clear()
        {
            lock (_syncLock)
            {
                foreach (var disposable in _disposables)
                {
                    disposable.Dispose();
                }
                _disposables.Clear();
            }
        }

        /// <summary>
        /// 自身と登録されたリソースを破棄
        /// </summary>
        public void Dispose()
        {
            lock (_syncLock)
            {
                if (_isDisposed) return;

                foreach (var disposable in _disposables)
                {
                    disposable.Dispose();
                }
                _disposables.Clear();
                _isDisposed = true;
            }
        }
    }
}
