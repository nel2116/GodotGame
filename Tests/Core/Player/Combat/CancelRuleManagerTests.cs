using NUnit.Framework;
using Systems.Player.Combat;
using Systems.Player.State;
using Core.Events;

namespace Tests.Core.Player.Combat
{
    public class CancelRuleManagerTests
    {
        [Test]
        public void CanCancel_ReturnsTrueWhenRuleMatches()
        {
            var bus = new GameEventBus();
            var frameManager = new FrameStateManager(bus);
            var ruleManager = new CancelRuleManager(frameManager);
            ruleManager.InitializeDefaultRules();

            var action = new ActionFrameData("Attack_L1", 30, 5, 10, 15);
            frameManager.StartAction(action);
            for (int i = 0; i < 15; i++) frameManager.Tick();
            Assert.IsTrue(ruleManager.CanCancel("Dodge"));
        }

        [Test]
        public void CanCancel_ReturnsFalseOutsideFrame()
        {
            var bus = new GameEventBus();
            var frameManager = new FrameStateManager(bus);
            var ruleManager = new CancelRuleManager(frameManager);
            ruleManager.InitializeDefaultRules();

            var action = new ActionFrameData("Attack_L1", 30, 5, 10, 15);
            frameManager.StartAction(action);
            for (int i = 0; i < 10; i++) frameManager.Tick();
            Assert.IsFalse(ruleManager.CanCancel("Dodge"));
        }
    }
}
