using NUnit.Framework;
using Core.Utilities;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tests.Core
{
    public class ReactiveCollectionTests
    {
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

    }
}
