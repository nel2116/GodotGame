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
    }
}
