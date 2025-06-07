using NUnit.Framework;
using Core.Events;
using System;
using System.Reactive.Linq;

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
    }
}
