using NUnit.Framework;
using System;
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
    }
}
