using NUnit.Framework;
using Core.Events;
using Systems.Common.State;
using Systems.Common.Events;

namespace Tests.Core.State
{
    public class CommonStateViewModelTests
    {
        [Test]
        public void ChangeState_PublishesStateEvent()
        {
            var bus = new GameEventBus();
            var model = new CommonStateModel();
            var vm = new CommonStateViewModel(model, bus);
            vm.Initialize();
            StateChangedEvent? received = null;
            bus.GetEventStream<StateChangedEvent>().Subscribe(e => received = e);
            vm.ChangeState("Walk");
            Assert.IsNotNull(received);
            Assert.AreEqual("Walk", received!.State);
        }

        [Test]
        public void ChangeState_Invalid_NoEvent()
        {
            var bus = new GameEventBus();
            var model = new CommonStateModel();
            var vm = new CommonStateViewModel(model, bus);
            vm.Initialize();
            bool called = false;
            bus.GetEventStream<StateChangedEvent>().Subscribe(_ => called = true);
            vm.ChangeState("Invalid");
            Assert.IsFalse(called);
            Assert.AreEqual("Idle", vm.CurrentState.Value);
        }
    }
}

