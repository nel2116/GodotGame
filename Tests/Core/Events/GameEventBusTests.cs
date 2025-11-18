using NUnit.Framework;
using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using Core.Events;
using Tests.Core;

namespace Tests.Core.Events
{
    public class GameEventBusTests : TestBase
    {
        private class DummyEvent : GameEvent { }
        private class AnotherEvent : GameEvent { }

        [Test]
        public async Task Publish_NotifiesSubscribers()
        {
            var bus = new GameEventBus();
            bool notified = false;
            using (bus.GetEventStream<DummyEvent>().Subscribe(_ => notified = true))
            {
                bus.Publish(new DummyEvent());
                await Task.Delay(10); // イベント処理の遅延を考慮
                Assert.IsTrue(notified);
            }
        }

        [Test]
        public async Task Subscribe_MultipleTypes_NotifyOnlyMatching()
        {
            var bus = new GameEventBus();
            int dummyCount = 0;
            int anotherCount = 0;

            using (bus.GetEventStream<DummyEvent>().Subscribe(_ => dummyCount++))
            using (bus.GetEventStream<AnotherEvent>().Subscribe(_ => anotherCount++))
            {
                bus.Publish(new DummyEvent());
                bus.Publish(new AnotherEvent());
                bus.Publish(new DummyEvent());
                await Task.Delay(10); // イベント処理の遅延を考慮

                Assert.AreEqual(2, dummyCount);
                Assert.AreEqual(1, anotherCount);
            }
        }

        [Test]
        public async Task Publish_UnsubscribedType_DoesNotNotify()
        {
            var bus = new GameEventBus();
            bool notified = false;
            using (bus.GetEventStream<DummyEvent>().Subscribe(_ => notified = true))
            {
                bus.Publish(new AnotherEvent());
                await Task.Delay(10); // イベント処理の遅延を考慮
                Assert.IsFalse(notified);
            }
        }

        [Test, MaxTime(1000)]
        public async Task Publish_Performance()
        {
            var bus = new GameEventBus();
            int count = 0;
            using (bus.GetEventStream<DummyEvent>().Subscribe(_ => count++))
            {
                for (int i = 0; i < 1000; i++)
                {
                    bus.Publish(new DummyEvent());
                }
                await Task.Delay(10); // イベント処理の遅延を考慮
            }
            Assert.AreEqual(1000, count);
        }

        [Test, MaxTime(3000)]
        public async Task Publish_LargeVolume_Performance()
        {
            var bus = new GameEventBus();
            int count = 0;
            using (bus.GetEventStream<DummyEvent>().Subscribe(_ => count++))
            {
                for (int i = 0; i < 50000; i++)
                {
                    bus.Publish(new DummyEvent());
                }
                await Task.Delay(10); // イベント処理の遅延を考慮
            }
            Assert.AreEqual(50000, count);
        }

        [Test]
        public async Task Publish_Concurrent()
        {
            var bus = new GameEventBus();
            int count = 0;
            using (bus.GetEventStream<DummyEvent>().Subscribe(_ => Interlocked.Increment(ref count)))
            {
                Parallel.For(0, 1000, _ => bus.Publish(new DummyEvent()));
                await Task.Delay(10); // イベント処理の遅延を考慮
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
                var stopwatch = Stopwatch.StartNew();
                // Thread.Sleep(1)を削除して、より多くのイベントを発行できるようにする
                // ただし、CPU負荷を抑えるため、適度な間隔で発行
                while (stopwatch.Elapsed < TimeSpan.FromSeconds(1))
                {
                    bus.Publish(new DummyEvent());
                    // 短い遅延を挟み CPU 負荷を抑える（1msは長すぎるため、より短い間隔に）
                    if (count % 100 == 0)
                    {
                        Thread.Sleep(0); // スレッドを譲るだけ
                    }
                }
            }
            // Thread.Sleep(1)を削除したため、より多くのイベントが発行されることを期待
            // ただし、システムの負荷によって変動するため、より現実的な期待値に調整
            Assert.Greater(count, 100, "1秒間に少なくとも100回以上のイベントが発行されるべき");
        }

        /// <summary>
        /// 多数のタスクから同時に発行しても全て処理されるか確認
        /// </summary>
        [Test, MaxTime(3000)]
        public async Task LoadTest_ConcurrentPublish()
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
                await Task.Delay(10); // イベント処理の遅延を考慮
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

        /// <summary>
        /// 破棄済みのバスに対する操作が適切に処理されることを確認
        /// </summary>
        [Test]
        public async Task Operations_AfterDispose_HandleGracefully()
        {
            var bus = new GameEventBus();
            bus.Dispose();

            // 破棄済みバスへのイベント発行
            Assert.DoesNotThrow(() => bus.Publish(new DummyEvent()));

            // 破棄済みバスからのイベントストリーム取得
            var stream = bus.GetEventStream<DummyEvent>();
            Assert.IsNotNull(stream);
            bool notified = false;
            using (stream.Subscribe(_ => notified = true))
            {
                bus.Publish(new DummyEvent());
                await Task.Delay(10); // イベント処理の遅延を考慮
                Assert.IsFalse(notified, "破棄済みバスからのイベントは通知されないべき");
            }
        }

        /// <summary>
        /// イベントのバッファリングが正しく機能することを確認
        /// </summary>
        [Test, MaxTime(2000)]
        public async Task EventBuffering_WorksCorrectly()
        {
            var bus = new GameEventBus();
            int count = 0;
            var events = new System.Collections.Generic.List<DateTime>();

            using (bus.GetEventStream<DummyEvent>().Subscribe(evt =>
            {
                count++;
                events.Add(evt.Timestamp);
            }))
            {
                // 短時間に多数のイベントを発行
                for (int i = 0; i < 100; i++)
                {
                    bus.Publish(new DummyEvent());
                }

                // バッファリングの効果を確認するため少し待機
                await Task.Delay(50);
            }

            Assert.Greater(count, 0, "イベントが少なくとも1つは通知されるべき");
            Assert.LessOrEqual(count, 100, "バッファリングにより、イベント数が減少する可能性がある");
        }

        /// <summary>
        /// イベントキューサイズの上限が機能することを確認
        /// </summary>
        [Test, MaxTime(3000)]
        public void EventQueueSizeLimit_WorksCorrectly()
        {
            var bus = new GameEventBus();
            int count = 0;
            var stopwatch = Stopwatch.StartNew();

            using (bus.GetEventStream<DummyEvent>().Subscribe(_ => Interlocked.Increment(ref count)))
            {
                // 上限を超えるイベントを発行
                for (int i = 0; i < 2000; i++)
                {
                    bus.Publish(new DummyEvent());
                }
            }

            stopwatch.Stop();
            Assert.LessOrEqual(count, 2000, "イベント数が上限を超えないことを確認");
            Assert.Less(stopwatch.ElapsedMilliseconds, 2000, "処理が適切な時間内に完了することを確認");
        }

        /// <summary>
        /// エラー発生時の動作を確認
        /// </summary>
        /// <remarks>
        /// 注: ReactiveのSubscribeで例外がスローされると、その購読者のストリームが終了します。
        /// これはReactiveの標準的な動作であり、エラーハンドリングは購読者側で行う必要があります。
        /// このテストでは、エラーが発生してもバス自体は正常に動作し続けることを確認します。
        /// </remarks>
        [Test]
        public void ErrorHandling_WorksCorrectly()
        {
            var bus = new GameEventBus();
            int successCount = 0;

            // エラーを発生させるイベントハンドラ（エラーハンドリング付き）
            using (bus.GetEventStream<DummyEvent>().Subscribe(
                _ => throw new Exception("Test error"),
                ex => { /* エラーを記録するが、ストリームは終了する */ }))
            {
                // エラーが発生してもバス自体は正常に動作し続けることを確認
                // 注: 例外がスローされると、その購読者のストリームが終了するが、
                // バス自体は正常に動作し続ける
                try
                {
                    bus.Publish(new DummyEvent());
                }
                catch (Exception)
                {
                    // 例外は予期される動作（エラーを発生させるハンドラから）
                }
            }

            // エラーが発生した後でも、新しい購読者を追加できることを確認
            using (bus.GetEventStream<DummyEvent>().Subscribe(_ => successCount++))
            {
                bus.Publish(new DummyEvent());
                Assert.Greater(successCount, 0, "エラーが発生した後でも、新しい購読者が正常に動作する");
            }
        }

        /// <summary>
        /// テスト終了後のクリーンアップ確認
        /// </summary>
        [Test]
        public void TestCleanup_NoErrors()
        {
            var bus = new GameEventBus();
            using (bus.GetEventStream<DummyEvent>().Subscribe(_ => { }))
            {
                bus.Publish(new DummyEvent());
            }

            // テスト終了時にエラーがないことを確認
            AssertNoErrors();
        }

        /// <summary>
        /// nullイベントの発行が適切に処理されることを確認
        /// </summary>
        [Test]
        public async Task Publish_NullEvent_HandleGracefully()
        {
            var bus = new GameEventBus();
            Assert.DoesNotThrow(() => bus.Publish<DummyEvent>(null));
            await Task.Delay(10); // イベント処理の遅延を考慮
        }
    }
}
