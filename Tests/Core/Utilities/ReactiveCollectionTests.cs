using NUnit.Framework;
using Core.Utilities;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tests.Core
{
    public class ReactiveCollectionTests
    {
        // GC 変動による誤差を考慮し、許容するメモリ増加量の目安を 50KB とする
        private const long MEMORY_THRESHOLD_BYTES = 1024 * 50;
        // 初期メモリ比で 10% 以内であれば安定とみなす
        private const double MEMORY_RELATIVE_THRESHOLD = 0.1;
        [Test]
        public void Add_RaisesChangeEvent()
        {
            var col = new ReactiveCollection<int>();
            int notified = 0;
            using (col.Changed.Subscribe(e => notified = e.Item))
            {
                col.Add(1);
            }
            Assert.AreEqual(1, notified);
        }

        [Test]
        public void Remove_RaisesChangeEvent()
        {
            var col = new ReactiveCollection<string>();
            col.Add("a");
            string notified = string.Empty;
            using (col.Changed.Subscribe(e => notified = e.Item))
            {
                col.Remove("a");
            }
            Assert.AreEqual("a", notified);
        }

        [Test]
        public void Indexer_Set_ReplacesItemWithNotifications()
        {
            var col = new ReactiveCollection<int>();
            col.Add(1);
            col.Add(2);
            var events = new List<CollectionChangedEvent<int>>();
            using (col.Changed.Subscribe(e => events.Add(e)))
            {
                col[0] = 5;
            }
            Assert.AreEqual(2, events.Count);
            Assert.AreEqual(CollectionChangeType.Remove, events[0].ChangeType);
            Assert.AreEqual(1, events[0].Item);
            Assert.AreEqual(CollectionChangeType.Add, events[1].ChangeType);
            Assert.AreEqual(5, events[1].Item);
        }

        [Test]
        public void LargeAddRemove_Performance()
        {
            var col = new ReactiveCollection<int>();
            for (int i = 0; i < 10000; i++)
            {
                col.Add(i);
            }
            for (int i = 9999; i >= 0; i--)
            {
                col.RemoveAt(i);
            }
            Assert.AreEqual(0, col.Count);
        }

        /// <summary>
        /// 無効なインデックス指定時に例外が発生するか検証
        /// </summary>
        [Test]
        public void RemoveAt_InvalidIndex_Throws()
        {
            var col = new ReactiveCollection<int>();
            Assert.Throws<System.ArgumentOutOfRangeException>(() => col.RemoveAt(0));
        }

        /// <summary>
        /// 大量追加後にクリアしてもメモリが増え続けないことを確認
        /// </summary>
        [Test]
        public void AddAndClear_MemoryUsageStable()
        {
            var col = new ReactiveCollection<byte[]>();
            long before = System.GC.GetTotalMemory(true);
            for (int i = 0; i < 1000; i++)
            {
                col.Add(new byte[1024]);
            }
            col.Clear();
            System.GC.Collect();
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            long after = System.GC.GetTotalMemory(true);
            // 環境差異で多少増減するため、絶対値に加え相対値でも検証する
            Assert.Less(after - before, MEMORY_THRESHOLD_BYTES);
            Assert.Less(after, before * (1 + MEMORY_RELATIVE_THRESHOLD),
                "Memory usage after clearing should not exceed 10% of the initial memory usage.");

            // 変動を観測するため複数回計測し平均を算出
            const int iterations = 5;
            var memory_deltas = new List<long>();
            var relative_increases = new List<double>();
            for (int iteration = 0; iteration < iterations; iteration++)
            {
                var loop_col = new ReactiveCollection<byte[]>();
                long loop_before = System.GC.GetTotalMemory(true);
                for (int i = 0; i < 1000; i++)
                {
                    loop_col.Add(new byte[1024]);
                }
                loop_col.Clear();
                System.GC.Collect();
                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();
                long loop_after = System.GC.GetTotalMemory(true);
                long delta = loop_after - loop_before;
                double relative = (double)delta / loop_before;
                memory_deltas.Add(delta);
                relative_increases.Add(relative);
                TestContext.WriteLine($"Iteration {iteration + 1}: Memory delta = {delta} bytes, Relative increase = {relative:P}");
            }
            long average_delta = (long)memory_deltas.Average();
            double average_relative = relative_increases.Average();
            Assert.Less(average_delta, MEMORY_THRESHOLD_BYTES,
                $"Average memory delta ({average_delta} bytes) exceeds threshold.");
            Assert.Less(average_relative, MEMORY_RELATIVE_THRESHOLD,
                $"Average relative memory increase ({average_relative:P}) exceeds threshold.");
        }

    }
}
