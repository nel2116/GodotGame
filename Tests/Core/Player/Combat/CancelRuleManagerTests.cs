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

        [Test]
        public void CanCancel_AttackL1ToChargeAttack_MatchesSpecification()
        {
            var bus = new GameEventBus();
            var frameManager = new FrameStateManager(bus);
            var ruleManager = new CancelRuleManager(frameManager);
            ruleManager.InitializeDefaultRules();

            var action = new ActionFrameData("Attack_L1", 20, 4, 4, 12);
            frameManager.StartAction(action);
            for (int i = 0; i < 14; i++) frameManager.Tick();
            Assert.IsTrue(ruleManager.CanCancel("ChargeAttack"));
        }

        [Test]
        public void CanCancel_AttackL2ToDodge_MatchesSpecification()
        {
            var bus = new GameEventBus();
            var frameManager = new FrameStateManager(bus);
            var ruleManager = new CancelRuleManager(frameManager);
            ruleManager.InitializeDefaultRules();

            var action = new ActionFrameData("Attack_L2", 22, 3, 5, 14);
            frameManager.StartAction(action);
            for (int i = 0; i < 16; i++) frameManager.Tick();
            Assert.IsTrue(ruleManager.CanCancel("Dodge"));
        }

        [Test]
        public void CanCancel_ChargeAttackToDodge_MatchesSpecification()
        {
            var bus = new GameEventBus();
            var frameManager = new FrameStateManager(bus);
            var ruleManager = new CancelRuleManager(frameManager);
            ruleManager.InitializeDefaultRules();

            var action = new ActionFrameData("ChargeAttack", 40, 16, 6, 18);
            frameManager.StartAction(action);
            for (int i = 0; i < 30; i++) frameManager.Tick();
            Assert.IsTrue(ruleManager.CanCancel("Dodge"));
        }

        [Test]
        public void CanCancel_JumpToDodge_MatchesSpecification()
        {
            var bus = new GameEventBus();
            var frameManager = new FrameStateManager(bus);
            var ruleManager = new CancelRuleManager(frameManager);
            ruleManager.InitializeDefaultRules();

            var action = new ActionFrameData("Jump", 30, 2, 28, 0);
            frameManager.StartAction(action);
            for (int i = 0; i < 3; i++) frameManager.Tick();
            Assert.IsTrue(ruleManager.CanCancel("Dodge"));
        }
    }
}
