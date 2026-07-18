using System;
using System.Collections.Generic;

namespace Systems.Dungeon.Optimization
{
    /// <summary>
    /// 汎用 LRU（Least Recently Used）キャッシュ
    /// 容量を超えて要素が追加された場合、最も長くアクセスされていない要素を追い出す
    /// </summary>
    public class LruCache<TKey, TValue> where TKey : notnull
    {
        private readonly int capacity;
        private readonly Dictionary<TKey, LinkedListNode<(TKey Key, TValue Value)>> map = new();
        private readonly LinkedList<(TKey Key, TValue Value)> usageOrder = new();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="capacity">保持できる最大要素数</param>
        /// <exception cref="ArgumentOutOfRangeException">capacity が 1 未満の場合</exception>
        public LruCache(int capacity)
        {
            if (capacity < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity), "容量は1以上である必要があります。");
            }

            this.capacity = capacity;
        }

        /// <summary>
        /// 現在保持している要素数
        /// </summary>
        public int Count => map.Count;

        /// <summary>
        /// 要素を追加または更新する
        /// 容量を超える場合は、最も長くアクセスされていない要素を追い出してから追加する
        /// </summary>
        /// <param name="key">追加するキー</param>
        /// <param name="value">追加する値</param>
        public void Add(TKey key, TValue value)
        {
            if (map.TryGetValue(key, out var existing))
            {
                usageOrder.Remove(existing);
            }
            else if (map.Count >= capacity)
            {
                var oldest = usageOrder.First;
                if (oldest != null)
                {
                    usageOrder.RemoveFirst();
                    map.Remove(oldest.Value.Key);
                }
            }

            map[key] = usageOrder.AddLast((key, value));
        }

        /// <summary>
        /// キーに対応する値の取得を試みる
        /// 取得に成功した場合、そのキーは最も新しくアクセスされた要素として扱われる
        /// </summary>
        /// <param name="key">取得対象のキー</param>
        /// <param name="value">見つかった値（見つからない場合は既定値）</param>
        /// <returns>キーが存在した場合は true</returns>
        public bool TryGet(TKey key, out TValue value)
        {
            if (!map.TryGetValue(key, out var node))
            {
                value = default!;
                return false;
            }

            usageOrder.Remove(node);
            map[key] = usageOrder.AddLast(node.Value);

            value = node.Value.Value;
            return true;
        }

        /// <summary>
        /// 保持している全ての要素を削除する
        /// </summary>
        public void Clear()
        {
            map.Clear();
            usageOrder.Clear();
        }
    }
}
