using NUnit.Framework;
using Systems.Player.Combat;
using Systems.Player.State;
using Core.Events;

namespace Tests.Core.Player.Combat
{
    public class InvincibilityManagerTests : TestBase
    {
        [Test]
        public void IsInvincible_FalseWhenNoActionRunning()
        {
            var bus = new GameEventBus();
            var frameManager = new FrameStateManager(bus);
            var invincibilityManager = new InvincibilityManager(frameManager);

            Assert.IsFalse(invincibilityManager.IsInvincible());
        }

        [Test]
        public void IsInvincible_TrueDuringActionInvincibilityWindow()
        {
            var bus = new GameEventBus();
            var frameManager = new FrameStateManager(bus);
            var invincibilityManager = new InvincibilityManager(frameManager);

            var dodge = StandardActionFrameData.Dodge();
            frameManager.StartAction(dodge);

            for (int i = 0; i < 3; i++) frameManager.Tick();
            Assert.IsTrue(invincibilityManager.IsInvincible());

            for (int i = 0; i < 8; i++) frameManager.Tick();
            Assert.IsFalse(invincibilityManager.IsInvincible());
        }

        [Test]
        public void SetForcedInvincible_OverridesActionState()
        {
            var bus = new GameEventBus();
            var frameManager = new FrameStateManager(bus);
            var invincibilityManager = new InvincibilityManager(frameManager);

            Assert.IsFalse(invincibilityManager.IsInvincible());

            invincibilityManager.SetForcedInvincible(true);
            Assert.IsTrue(invincibilityManager.IsInvincible());

            invincibilityManager.SetForcedInvincible(false);
            Assert.IsFalse(invincibilityManager.IsInvincible());
        }
    }
}
