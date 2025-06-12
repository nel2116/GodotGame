using NUnit.Framework;
using Core.Events;
using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Tests.Core
{
    public class GameEventBusTests
    {
        private class DummyEvent : GameEvent { }
        private class AnotherEvent : GameEvent { }

        [Test]
        public void Publish_NotifiesSubscribers()
        {
            var bus = new GameEventBus();
            bool notified = false;
            using (bus.GetEventStream<DummyEvent>().Subscribe(_ => notified = true))
            {
                bus.Publish(new DummyEvent());
            }
            Assert.IsTrue(notified);
        }

        [Test]
        public void Subscribe_MultipleTypes_NotifyOnlyMatching()
        {
            var bus = new GameEventBus();
            bool notified_a = false;
            bool notified_b = false;

            using (bus.GetEventStream<DummyEvent>().Subscribe(_ => notified_a = true))
            using (bus.GetEventStream<AnotherEvent>().Subscribe(_ => notified_b = true))
            {
                bus.Publish(new DummyEvent());
                bus.Publish(new AnotherEvent());
            }

            Assert.IsTrue(notified_a);
            Assert.IsTrue(notified_b);
        }

        [Test]
        public void Publish_UnsubscribedType_DoesNotNotify()
        {
            var bus = new GameEventBus();
            bool notified = false;

            using (bus.GetEventStream<DummyEvent>().Subscribe(_ => notified = true))
            {
                bus.Publish(new AnotherEvent());
            }

            Assert.IsFalse(notified);
        }

        [Test, MaxTime(1000)]
        public void Publish_Performance()
        {
            var bus = new GameEventBus();
            int count = 0;
            using (bus.GetEventStream<DummyEvent>().Subscribe(_ => count++))
            {
                for (int i = 0; i < 10000; i++)
                {
                    bus.Publish(new DummyEvent());
                }
            }
            Assert.AreEqual(10000, count);
        }

        [Test, MaxTime(3000)]
        public void Publish_LargeVolume_Performance()
        {
            var bus = new GameEventBus();
            int count = 0;
            using (bus.GetEventStream<DummyEvent>().Subscribe(_ => count++))
            {
                for (int i = 0; i < 50000; i++)
                {
                    bus.Publish(new DummyEvent());
                }
            }
            Assert.AreEqual(50000, count);
        }

        [Test]
        public void Publish_Concurrent()
        {
            var bus = new GameEventBus();
            int count = 0;
            using (bus.GetEventStream<DummyEvent>().Subscribe(_ => Interlocked.Increment(ref count)))
            {
                Parallel.For(0, 1000, _ => bus.Publish(new DummyEvent()));
            }
            Assert.AreEqual(1000, count);
        }

        /// <summary>
        /// 1 秒間連続でイベント発行し続けても安定して通知されるか検証
        /// </summary>
        [Test, MaxTime(2000)]
        public void LongRunning_Stability()
        {
            var bus = new GameEventBus();
            int count = 0;
            using (bus.GetEventStream<DummyEvent>().Subscribe(_ => Interlocked.Increment(ref count)))
            {
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                while (stopwatch.Elapsed < TimeSpan.FromSeconds(1))
                {
                    bus.Publish(new DummyEvent());
                    // 短い遅延を挟み CPU 負荷を抑える
                    Thread.Sleep(1);
                }
            }
            Assert.Greater(count, 500);
        }

        /// <summary>
        /// 多数のタスクから同時に発行しても全て処理されるか確認
        /// </summary>
        [Test, MaxTime(3000)]
        public void LoadTest_ConcurrentPublish()
        {
            var bus = new GameEventBus();
            int count = 0;
            using (bus.GetEventStream<DummyEvent>().Subscribe(_ => Interlocked.Increment(ref count)))
            {
                var tasks = new Task[20];
                for (int i = 0; i < tasks.Length; i++)
                {
                    tasks[i] = Task.Run(() =>
                    {
                        for (int j = 0; j < 1000; j++)
                        {
                            bus.Publish(new DummyEvent());
                        }
                    });
                }
                Task.WaitAll(tasks);
            }
            // 記録漏れがないことを確認するが、並列実行の揺らぎを考慮し下限のみ検証
            Assert.GreaterOrEqual(count, 20000);
        }

        /// <summary>
        /// Dispose が複数回呼ばれても例外を投げないことを確認
        /// </summary>
        [Test]
        public void Dispose_Idempotent()
        {
            var bus = new GameEventBus();
            int completed_count = 0;
            bus.GetEventStream<DummyEvent>().Subscribe(_ => { }, () => Interlocked.Increment(ref completed_count));

            bus.Dispose();
            bus.Dispose();

            Assert.AreEqual(1, completed_count);
        }
    }
}
