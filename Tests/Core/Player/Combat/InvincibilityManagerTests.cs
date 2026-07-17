using NUnit.Framework;
using Systems.Player.Combat;
using Systems.Player.State;
using Core.Events;

namespace Tests.Core.Player.Combat
{
    public class InvincibilityManagerTests
    {
        [Test]
        public void IsCurrentlyInvincible_ReturnsFalseWhenNoActionRunning()
        {
            var bus = new GameEventBus();
            var frameManager = new FrameStateManager(bus);
            var manager = new InvincibilityManager(frameManager);

            Assert.IsFalse(manager.IsCurrentlyInvincible());
        }

        [Test]
        public void IsCurrentlyInvincible_ReturnsTrueWithinInvincibilityWindow()
        {
            var bus = new GameEventBus();
            var frameManager = new FrameStateManager(bus);
            var manager = new InvincibilityManager(frameManager);

            var action = new ActionFrameData("Dodge", 20, 2, 10, 8, invincibilityStartFrame: 2, invincibilityEndFrame: 6);
            frameManager.StartAction(action);
            for (int i = 0; i < 3; i++) frameManager.Tick();

            Assert.IsTrue(manager.IsCurrentlyInvincible());
        }

        [Test]
        public void IsCurrentlyInvincible_ReturnsFalseOutsideInvincibilityWindow()
        {
            var bus = new GameEventBus();
            var frameManager = new FrameStateManager(bus);
            var manager = new InvincibilityManager(frameManager);

            var action = new ActionFrameData("Dodge", 20, 2, 10, 8, invincibilityStartFrame: 2, invincibilityEndFrame: 6);
            frameManager.StartAction(action);
            for (int i = 0; i < 10; i++) frameManager.Tick();

            Assert.IsFalse(manager.IsCurrentlyInvincible());
        }

        [Test]
        public void TakeDamage_IsNegatedWhileInvincible()
        {
            var bus = new GameEventBus();
            var frameManager = new FrameStateManager(bus);
            var invincibilityManager = new InvincibilityManager(frameManager);
            var combatModel = new PlayerCombatModel(bus, invincibilityManager);
            combatModel.Initialize();

            var action = new ActionFrameData("Dodge", 20, 2, 10, 8, invincibilityStartFrame: 2, invincibilityEndFrame: 6);
            frameManager.StartAction(action);
            for (int i = 0; i < 3; i++) frameManager.Tick();

            var healthBeforeDamage = combatModel.CurrentHealth;
            combatModel.TakeDamage(50f);

            Assert.That(combatModel.CurrentHealth, Is.EqualTo(healthBeforeDamage));
        }

        [Test]
        public void TakeDamage_AppliesDamageWhenNotInvincible()
        {
            var bus = new GameEventBus();
            var frameManager = new FrameStateManager(bus);
            var invincibilityManager = new InvincibilityManager(frameManager);
            var combatModel = new PlayerCombatModel(bus, invincibilityManager);
            combatModel.Initialize();

            var healthBeforeDamage = combatModel.CurrentHealth;
            combatModel.TakeDamage(50f);

            Assert.That(combatModel.CurrentHealth, Is.LessThan(healthBeforeDamage));
        }
    }
}
