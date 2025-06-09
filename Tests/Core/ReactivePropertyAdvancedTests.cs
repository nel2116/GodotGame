using NUnit.Framework;
using System.Collections.Generic;
using Core.Reactive;

namespace Tests.Core
{
    public class ReactivePropertyAdvancedTests
    {
        [Test]
        public void ValueChanged_Observable_Notifies()
        {
            var prop = new ReactiveProperty<int>(0);
            int received = -1;
            using (prop.ValueChanged.Subscribe(v => received = v))
            {
                prop.Value = 5;
            }
            Assert.AreEqual(5, received);
        }

        [Test]
        public void SetValidator_PreventsInvalidValue()
        {
            var prop = new ReactiveProperty<int>(0);
            prop.SetValidator(v => v >= 0);
            prop.Value = -1;
            Assert.AreEqual(0, prop.Value);
        }

        [Test]
        public void BeginUpdate_SuppressesNotifications()
        {
            var prop = new ReactiveProperty<int>(0);
            var list = new List<int>();
            using (prop.Subscribe(v => list.Add(v)))
            {
                prop.BeginUpdate();
                prop.Value = 1;
                prop.Value = 2;
                prop.EndUpdate();
            }
            CollectionAssert.AreEqual(new[] { 2 }, list);
        }
    }
}
