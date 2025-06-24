using System.Collections.Generic;

namespace Systems.Player.Input
{
    /// <summary>
    /// リングバッファによる入力管理クラス
    /// </summary>
    public class InputRingBuffer<T>
    {
        private readonly T[] _buffer;
        private int _head;
        private int _count;

        public int Capacity => _buffer.Length;
        public int Count => _count;

        public InputRingBuffer(int capacity = 12)
        {
            _buffer = new T[capacity];
        }

        /// <summary>
        /// バッファに追加する
        /// </summary>
        public void Add(T item)
        {
            _buffer[_head] = item;
            _head = (_head + 1) % Capacity;
            if (_count < Capacity)
            {
                _count++;
            }
        }

        /// <summary>
        /// すべての要素を取得する
        /// </summary>
        public IEnumerable<T> GetItems()
        {
            for (int i = 0; i < _count; i++)
            {
                int index = (_head - _count + i + Capacity) % Capacity;
                yield return _buffer[index];
            }
        }

        /// <summary>
        /// バッファをクリアする
        /// </summary>
        public void Clear()
        {
            _head = 0;
            _count = 0;
        }
    }
}
