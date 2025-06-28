using NUnit.Framework;
using Systems.Player.Combat;
using Systems.Player.Events;
using Core.Events;

namespace Tests.Core.Player.Combat
{
    public class PlayerCombatViewModelTests
    {
        [Test]
        public void Attack_BasicAction_AttackPowerStable()
        {
            var bus = new GameEventBus();
            var model = new PlayerCombatModel(bus);
            var viewModel = new PlayerCombatViewModel(model, bus);
            viewModel.Initialize();
            viewModel.Attack("BasicAttack");
            Assert.That(viewModel.AttackPower.Value, Is.EqualTo(10f));
        }

        [Test]
        public void Attack_PublishesAttackEvent()
        {
            var bus = new GameEventBus();
            var model = new PlayerCombatModel(bus);
            var viewModel = new PlayerCombatViewModel(model, bus);
            viewModel.Initialize();

            AttackExecutedEvent? received = null;
            bus.GetEventStream<AttackExecutedEvent>().Subscribe(e => received = e);

            viewModel.Attack("BasicAttack");

            Assert.IsNotNull(received);
            Assert.AreEqual("BasicAttack", received!.ActionName);
            Assert.AreEqual(10f, received.Damage);
        }

        [Test]
        public void TakeDamage_ReducesHealthAndPublishes()
        {
            var bus = new GameEventBus();
            var model = new PlayerCombatModel(bus);
            var viewModel = new PlayerCombatViewModel(model, bus);
            viewModel.Initialize();

            DamageTakenEvent? received = null;
            bus.GetEventStream<DamageTakenEvent>().Subscribe(e => received = e);

            viewModel.TakeDamage(20f);

            Assert.That(viewModel.CurrentHealth.Value, Is.EqualTo(85f));
            Assert.IsNotNull(received);
            Assert.AreEqual(15f, received!.Damage);
        }

        [Test]
        public void Heal_RestoresHealthAndPublishes()
        {
            var bus = new GameEventBus();
            var model = new PlayerCombatModel(bus);
            var viewModel = new PlayerCombatViewModel(model, bus);
            viewModel.Initialize();

            HealAppliedEvent? heal = null;
            HealthChangedEvent? changed = null;
            bus.GetEventStream<HealAppliedEvent>().Subscribe(e => heal = e);
            bus.GetEventStream<HealthChangedEvent>().Subscribe(e => changed = e);

            viewModel.TakeDamage(20f);
            viewModel.Heal(10f);

            Assert.That(viewModel.CurrentHealth.Value, Is.EqualTo(95f));
            Assert.IsNotNull(heal);
            Assert.AreEqual(10f, heal!.Amount);
            Assert.IsNotNull(changed);
            Assert.AreEqual(95f, changed!.Health);
        }

        [Test]
        public void Attack_InvalidAction_PublishesError()
        {
            var bus = new GameEventBus();
            var model = new PlayerCombatModel(bus);
            var viewModel = new PlayerCombatViewModel(model, bus);
            viewModel.Initialize();

            ErrorEvent? error = null;
            bus.GetEventStream<ErrorEvent>().Subscribe(e => error = e);

            viewModel.Attack("Unknown");

            Assert.IsNotNull(error);
            Assert.AreEqual("PlayerCombatModel", error!.Exception.SystemName);
            Assert.AreEqual("Attack", error.Exception.Operation);
        }
    }
}
