using NUnit.Framework;
using Systems.Player.Combat;
using Systems.Player.Events;
using Core.Events;
using System.Threading.Tasks;

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
        public async Task Attack_PublishesAttackEvent()
        {
            var bus = new GameEventBus();
            
            // イベント購読を先に実行
            AttackExecutedEvent? received = null;
            bus.GetEventStream<AttackExecutedEvent>().Subscribe(e => received = e);
            
            var model = new PlayerCombatModel(bus);
            var viewModel = new PlayerCombatViewModel(model, bus);
            viewModel.Initialize();

            viewModel.Attack("BasicAttack");

            // 少し待機してイベント処理を完了させる
            await Task.Delay(10);

            Assert.IsNotNull(received);
            Assert.AreEqual("BasicAttack", received!.ActionName);
            Assert.AreEqual(10f, received.Damage);
        }

        [Test]
        public async Task TakeDamage_ReducesHealthAndPublishes()
        {
            var bus = new GameEventBus();
            
            // イベント購読を先に実行
            DamageTakenEvent? received = null;
            bus.GetEventStream<DamageTakenEvent>().Subscribe(e => received = e);
            
            var model = new PlayerCombatModel(bus);
            var viewModel = new PlayerCombatViewModel(model, bus);
            viewModel.Initialize();

            viewModel.TakeDamage(20f);

            // 少し待機してイベント処理を完了させる
            await Task.Delay(10);

            Assert.That(viewModel.CurrentHealth.Value, Is.EqualTo(85f));
            Assert.IsNotNull(received);
            Assert.AreEqual(15f, received!.Damage);
        }

        [Test]
        public async Task Heal_RestoresHealthAndPublishes()
        {
            var bus = new GameEventBus();
            
            // イベント購読を先に実行
            HealAppliedEvent? heal = null;
            HealthChangedEvent? changed = null;
            bus.GetEventStream<HealAppliedEvent>().Subscribe(e => heal = e);
            bus.GetEventStream<HealthChangedEvent>().Subscribe(e => changed = e);
            
            var model = new PlayerCombatModel(bus);
            var viewModel = new PlayerCombatViewModel(model, bus);
            viewModel.Initialize();

            viewModel.TakeDamage(20f);
            viewModel.Heal(10f);

            // 少し待機してイベント処理を完了させる
            await Task.Delay(10);

            Assert.That(viewModel.CurrentHealth.Value, Is.EqualTo(95f));
            Assert.IsNotNull(heal);
            Assert.AreEqual(10f, heal!.Amount);
            Assert.IsNotNull(changed);
            Assert.AreEqual(95f, changed!.Health);
        }

        [Test]
        public async Task Attack_InvalidAction_PublishesError()
        {
            var bus = new GameEventBus();
            
            // イベント購読を先に実行
            ErrorEvent? error = null;
            bus.GetEventStream<ErrorEvent>().Subscribe(e => error = e);
            
            var model = new PlayerCombatModel(bus);
            var viewModel = new PlayerCombatViewModel(model, bus);
            viewModel.Initialize();

            viewModel.Attack("Unknown");

            // 少し待機してイベント処理を完了させる
            await Task.Delay(10);

            Assert.IsNotNull(error);
            Assert.AreEqual("PlayerCombatModel", error!.Exception.SystemName);
            Assert.AreEqual("Attack", error.Exception.Operation);
        }
    }
}
