using NUnit.Framework;
using Systems.Player.Combat;
using Systems.Player.State;
using Systems.Player.Events;
using Core.Events;

namespace Tests.Core.Player.Combat
{
    public class PlayerCombatModelTests : TestBase
    {
        [Test]
        public void TakeDamage_WithoutInvincibilityManager_AppliesDamage()
        {
            var bus = new GameEventBus();
            var model = new PlayerCombatModel(bus);
            model.Initialize();

            model.TakeDamage(20f);

            Assert.That(model.CurrentHealth, Is.EqualTo(85f));
        }

        [Test]
        public void TakeDamage_WhileInvincible_IgnoresDamage()
        {
            var bus = new GameEventBus();
            var frameManager = new FrameStateManager(bus);
            var invincibilityManager = new InvincibilityManager(frameManager);
            var model = new PlayerCombatModel(bus, invincibilityManager);
            model.Initialize();

            invincibilityManager.SetForcedInvincible(true);
            model.TakeDamage(20f);

            Assert.That(model.CurrentHealth, Is.EqualTo(model.MaxHealth));
        }

        [Test]
        public void TakeDamage_WhileInvincible_PublishesDamageAvoidedEvent()
        {
            var bus = new GameEventBus();
            var frameManager = new FrameStateManager(bus);
            var invincibilityManager = new InvincibilityManager(frameManager);
            var model = new PlayerCombatModel(bus, invincibilityManager);
            model.Initialize();

            DamageAvoidedEvent? received = null;
            bus.GetEventStream<DamageAvoidedEvent>().Subscribe(e => received = e);

            invincibilityManager.SetForcedInvincible(true);
            model.TakeDamage(20f);

            Assert.IsNotNull(received);
            Assert.That(received!.Damage, Is.EqualTo(20f));
        }

        [Test]
        public void TakeDamage_AfterInvincibilityEnds_AppliesDamageAgain()
        {
            var bus = new GameEventBus();
            var frameManager = new FrameStateManager(bus);
            var invincibilityManager = new InvincibilityManager(frameManager);
            var model = new PlayerCombatModel(bus, invincibilityManager);
            model.Initialize();

            invincibilityManager.SetForcedInvincible(true);
            model.TakeDamage(20f);
            Assert.That(model.CurrentHealth, Is.EqualTo(model.MaxHealth));

            invincibilityManager.SetForcedInvincible(false);
            model.TakeDamage(20f);
            Assert.That(model.CurrentHealth, Is.EqualTo(85f));
        }
    }
}
