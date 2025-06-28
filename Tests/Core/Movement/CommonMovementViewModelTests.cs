using NUnit.Framework;
using Core.Events;
using Godot;
using Systems.Common.Movement;
using Systems.Common.Events;

namespace Tests.Core.Movement
{
    public class CommonMovementViewModelTests
    {
        [Test]
        public void Move_PublishesVelocityEvent()
        {
            var bus = new GameEventBus();
            var model = new CommonMovementModel();
            var vm = new CommonMovementViewModel(model, bus);
            vm.Initialize();
            MovementVelocityChangedEvent? received = null;
            bus.GetEventStream<MovementVelocityChangedEvent>().Subscribe(e => received = e);
            vm.Move(new Vector2(1, 0));
            Assert.IsNotNull(received);
            Assert.AreEqual(new Vector2(5, 0), received!.Velocity);
        }
    }
}

