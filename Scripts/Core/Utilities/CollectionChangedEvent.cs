namespace Core.Utilities
{
    /// <summary>
    /// コレクション変更イベント
    /// </summary>
    public class CollectionChangedEvent<T>
    {
        public CollectionChangeType ChangeType { get; }
        public T Item { get; }

        public CollectionChangedEvent(CollectionChangeType changeType, T item)
        {
            ChangeType = changeType;
            Item = item;
        }
    }
}
