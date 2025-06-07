using NUnit.Framework;
using Core.Events;
using System;
using System.Reactive.Linq;

namespace Tests.Core
{
    public class GameEventBusTests
    {
        private class DummyEvent : GameEvent { }

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
    }
}
