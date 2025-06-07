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
        private bool _is_disposed;
        private readonly object _sync_lock = new();

        /// <summary>
        /// 登録されている破棄対象の数
        /// </summary>
        public int DisposableCount
        {
            get
            {
                lock (_sync_lock)
                {
                    return _disposables.Count;
                }
            }
        }

        /// <summary>
        /// 破棄対象を追加
        /// 自身を追加した場合は無限再帰を防ぐため警告を出して無視する
        /// </summary>
        public void Add(IDisposable disposable)
        {
            if (disposable == null) throw new ArgumentNullException(nameof(disposable));
            if (ReferenceEquals(disposable, this))
            {
                // 自身を追加すると無限再帰で破棄されるため無視する
                System.Diagnostics.Debug.WriteLine("Warning: Attempted to add self to CompositeDisposable. This operation is ignored.");
                return;
            }

            lock (_sync_lock)
            {
                if (_is_disposed)
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

            lock (_sync_lock)
            {
                return _disposables.Remove(disposable);
            }
        }

        /// <summary>
        /// 全ての破棄対象を解放
        /// </summary>
        public void Clear()
        {
            lock (_sync_lock)
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
            lock (_sync_lock)
            {
                if (_is_disposed) return;

                foreach (var disposable in _disposables)
                {
                    disposable.Dispose();
                }
                _disposables.Clear();
                _is_disposed = true;
            }
        }
    }
}
