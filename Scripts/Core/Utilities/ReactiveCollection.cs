using System;
using System.Collections;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Reactive.Linq;

namespace Core.Utilities
{
    /// <summary>
    /// 変更通知付きコレクション
    /// </summary>
    public class ReactiveCollection<T> : IList<T>, IDisposable
    {
        private readonly List<T> _items = new();
        private readonly Subject<CollectionChangedEvent<T>> _changedSubject = new();
        private bool _disposed;

        /// <summary>
        /// 変更通知ストリーム
        /// </summary>
        public IObservable<CollectionChangedEvent<T>> Changed => _changedSubject.AsObservable();

        public T this[int index]
        {
            get => _items[index];
            set
            {
                var old = _items[index];
                _items[index] = value;
                _changedSubject.OnNext(new CollectionChangedEvent<T>(CollectionChangeType.Remove, old));
                _changedSubject.OnNext(new CollectionChangedEvent<T>(CollectionChangeType.Add, value));
            }
        }

        public int Count => _items.Count;
        public bool IsReadOnly => false;

        public void Add(T item)
        {
            _items.Add(item);
            _changedSubject.OnNext(new CollectionChangedEvent<T>(CollectionChangeType.Add, item));
        }

        public void Clear()
        {
            foreach (var item in _items)
            {
                _changedSubject.OnNext(new CollectionChangedEvent<T>(CollectionChangeType.Remove, item));
            }
            _items.Clear();
        }

        public bool Contains(T item) => _items.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) => _items.CopyTo(array, arrayIndex);

        public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public int IndexOf(T item) => _items.IndexOf(item);

        public void Insert(int index, T item)
        {
            _items.Insert(index, item);
            _changedSubject.OnNext(new CollectionChangedEvent<T>(CollectionChangeType.Add, item));
        }

        public bool Remove(T item)
        {
            if (_items.Remove(item))
            {
                _changedSubject.OnNext(new CollectionChangedEvent<T>(CollectionChangeType.Remove, item));
                return true;
            }
            return false;
        }

        public void RemoveAt(int index)
        {
            var item = _items[index];
            _items.RemoveAt(index);
            _changedSubject.OnNext(new CollectionChangedEvent<T>(CollectionChangeType.Remove, item));
        }

        public void Dispose()
        {
            if (_disposed) return;
            _changedSubject.Dispose();
            _disposed = true;
        }
    }
}
