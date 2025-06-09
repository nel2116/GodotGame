using NUnit.Framework;
using Core.Utilities;
using System;

namespace Tests.Core
{
    public class WeakEventManagerTests
    {
        [Test]
        public void RaiseEvent_InvokesHandlers()
        {
            var mgr = new WeakEventManager();
            int called = 0;
            EventHandler handler = (_, _) => called++;
            mgr.AddHandler("test", handler);
            mgr.RaiseEvent("test", this, EventArgs.Empty);
            Assert.AreEqual(1, called);
            mgr.RemoveHandler("test", handler);
            mgr.RaiseEvent("test", this, EventArgs.Empty);
            Assert.AreEqual(1, called);
        }
    }
}
