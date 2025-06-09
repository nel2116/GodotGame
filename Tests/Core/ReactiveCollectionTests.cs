using NUnit.Framework;
using Core.Utilities;
using System.Linq;

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
    }
}
