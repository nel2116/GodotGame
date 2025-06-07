using NUnit.Framework;
using Core.Reactive;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tests.Core
{
    public class ReactivePropertyTests
    {
        [Test]
        public void ValueChange_NotifiesSubscribers()
        {
            // 初期値を -1 として更新が確実に通知されるようにする
            var property = new ReactiveProperty<int>(-1);
            int notifiedValue = -1;
            using (property.Subscribe(v => notifiedValue = v))
            {
                property.Value = 42;
            }
            Assert.AreEqual(42, notifiedValue);
        }

        [Test]
        public void Constructor_SetsInitialValue()
        {
            var property = new ReactiveProperty<string>("init");
            Assert.AreEqual("init", property.Value);
        }

        [Test]
        public void MultipleChanges_NotifyInOrder()
        {
            // 初期値を -1 として更新が確実に通知されるようにする
            var property = new ReactiveProperty<int>(-1);
            var list = new System.Collections.Generic.List<int>();
            using (property.Subscribe(v => list.Add(v)))
            {
                property.Value = 1;
                property.Value = 2;
            }
            CollectionAssert.AreEqual(new[] { 1, 2 }, list);
        }

        [Test]
        public void Dispose_StopNotifications()
        {
            // 初期値を -1 として更新が確実に通知されるようにする
            var property = new ReactiveProperty<int>(-1);
            int notified = 0;
            var subscription = property.Subscribe(v => notified++);
            property.Dispose();
            Assert.Throws<ObjectDisposedException>(() => property.Value = 1);
            subscription.Dispose();
            Assert.AreEqual(0, notified);
        }

        [Test]
        public void SetSameValue_DoesNotNotify()
        {
            var property = new ReactiveProperty<int>(1);
            int count = 0;
            using (property.Subscribe(_ => count++))
            {
                property.Value = 1;
                property.Value = 2;
            }
            Assert.AreEqual(1, count);
        }

        [Test]
        public void ManySubscribers_AllReceiveUpdates()
        {
            // 初期値を -1 として並列更新時の通知を検証する
            var property = new ReactiveProperty<int>(-1);
            const int subscriber_count = 50;
            var disposables = new List<IDisposable>();
            int total = 0;

            for (int i = 0; i < subscriber_count; i++)
            {
                disposables.Add(property.Subscribe(v => Interlocked.Add(ref total, v)));
            }

            property.Value = 1;

            foreach (var d in disposables)
            {
                d.Dispose();
            }

            Assert.AreEqual(subscriber_count, total);
        }

        [Test]
        public void ThreadSafety_SetFromMultipleThreads()
        {
            var property = new ReactiveProperty<int>(-1);
            int notified = 0;
            using (property.Subscribe(_ => Interlocked.Increment(ref notified)))
            {
                Parallel.For(0, 100, i => property.Value = i);
            }
            Assert.AreEqual(100, notified);
        }
    }
}
