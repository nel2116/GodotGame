using NUnit.Framework;
using Core.Reactive;
using System;

namespace Tests.Core
{
    public class ReactivePropertyTests
    {
        [Test]
        public void ValueChange_NotifiesSubscribers()
        {
            var property = new ReactiveProperty<int>(0);
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
            var property = new ReactiveProperty<int>(0);
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
            var property = new ReactiveProperty<int>(0);
            int notified = 0;
            var subscription = property.Subscribe(v => notified++);
            property.Dispose();
            Assert.Throws<ObjectDisposedException>(() => property.Value = 1);
            subscription.Dispose();
            Assert.AreEqual(0, notified);
        }
    }
}
