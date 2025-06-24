using NUnit.Framework;
using Systems.Player.State;
using Systems.Player.Events;
using Core.Events;

namespace Tests.Core.Player.State
{
    public class FrameStateManagerTests
    {
        [Test]
        public void Tick_IncrementsFrame()
        {
            var bus = new GameEventBus();
            var manager = new FrameStateManager(bus);
            manager.Tick();
            Assert.That(manager.CurrentFrame, Is.EqualTo(1));
        }

        [Test]
        public void StartAction_SetsCurrentAction()
        {
            var bus = new GameEventBus();
            var manager = new FrameStateManager(bus);
            var data = new ActionFrameData("Test", 10, 1, 5, 4);
            manager.StartAction(data);
            Assert.That(manager.CurrentAction, Is.Not.Null);
            Assert.That(manager.CurrentAction!.ActionName, Is.EqualTo("Test"));
        }

        [Test]
        public void IsInCancelableFrame_ReturnsTrueWithinRange()
        {
            var bus = new GameEventBus();
            var manager = new FrameStateManager(bus);
            var data = new ActionFrameData("Test", 10, 1, 5, 4);
            manager.StartAction(data);
            for (int i = 0; i < 5; i++) manager.Tick();
            Assert.IsTrue(manager.IsInCancelableFrame(2, 5));
        }
    }
}
