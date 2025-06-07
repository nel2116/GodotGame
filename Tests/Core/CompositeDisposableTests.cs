using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Reactive;

namespace Tests.Core
{
    public class CompositeDisposableTests
    {
        private class DummyDisposable : IDisposable
        {
            public bool Disposed { get; private set; }
            public void Dispose() => Disposed = true;
        }

        [Test]
        public void AddAndDispose_DisposesAllResources()
        {
            var d1 = new DummyDisposable();
            var d2 = new DummyDisposable();
            var composite = new CompositeDisposable();

            composite.Add(d1);
            composite.Add(d2);
            composite.Dispose();

            Assert.IsTrue(d1.Disposed);
            Assert.IsTrue(d2.Disposed);
        }

        [Test]
        public void AddRange_AddsAllItems()
        {
            var d1 = new DummyDisposable();
            var d2 = new DummyDisposable();
            var composite = new CompositeDisposable();

            composite.AddRange(new[] { d1, d2 });
            composite.Dispose();

            Assert.IsTrue(d1.Disposed);
            Assert.IsTrue(d2.Disposed);
        }

        [Test]
        public void Remove_ReturnsTrueAndDoesNotDispose()
        {
            var d1 = new DummyDisposable();
            var composite = new CompositeDisposable();

            composite.Add(d1);
            var removed = composite.Remove(d1);
            composite.Dispose();

            Assert.IsTrue(removed);
            Assert.IsFalse(d1.Disposed);
        }

        [Test]
        public void Clear_DisposesAllAndEmpties()
        {
            var d1 = new DummyDisposable();
            var d2 = new DummyDisposable();
            var composite = new CompositeDisposable();

            composite.Add(d1);
            composite.Add(d2);
            composite.Clear();

            Assert.IsTrue(d1.Disposed);
            Assert.IsTrue(d2.Disposed);

            composite.Dispose();
        }

        [Test]
        public void ThreadSafety_AddFromMultipleThreads()
        {
            var composite = new CompositeDisposable();
            var list = new List<DummyDisposable>();

            Parallel.For(0, 100, _ =>
            {
                var d = new DummyDisposable();
                lock (list)
                {
                    list.Add(d);
                }
                composite.Add(d);
            });

            composite.Dispose();

            Assert.IsTrue(list.All(d => d.Disposed));
        }

        [Test]
        public void Dispose_LargeNumberOfResources()
        {
            var composite = new CompositeDisposable();
            var disposables = new List<DummyDisposable>();
            for (int i = 0; i < 1000; i++)
            {
                var d = new DummyDisposable();
                disposables.Add(d);
                composite.Add(d);
            }

            composite.Dispose();

            Assert.IsTrue(disposables.All(d => d.Disposed));
        }

        [Test]
        public void CircularReference_DisposeSafely()
        {
            var composite = new CompositeDisposable();
            composite.Add(composite);
            composite.Dispose();
            Assert.Pass();
        }
    }
}
