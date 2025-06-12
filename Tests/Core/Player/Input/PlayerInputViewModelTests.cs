using NUnit.Framework;
using Systems.Player.Input;
using Systems.Player.Events;
using Core.Events;
using Godot;

namespace Tests.Core.Player.Input
{
    public class PlayerInputViewModelTests
    {
        [Test]
        public void Initialize_DefaultState_IsEnabled()
        {
            var bus = new GameEventBus();
            var model = new PlayerInputModel(bus);
            var viewModel = new PlayerInputViewModel(model, bus);
            viewModel.Initialize();
            Assert.That(viewModel.IsEnabled.Value, Is.True);
        }

        [Test]
        public void UpdateInput_PublishesStateEvent()
        {
            var bus = new GameEventBus();
            var model = new PlayerInputModel(bus);
            var viewModel = new PlayerInputViewModel(model, bus);
            viewModel.Initialize();

            InputStateChangedEvent? received = null;
            bus.GetEventStream<InputStateChangedEvent>().Subscribe(e => received = e);

            viewModel.UpdateInput();

            Assert.IsNotNull(received);
            Assert.IsNotNull(received!.State);
        }

        [Test]
        public void Initialize_PublishesEnabledEvent()
        {
            var bus = new GameEventBus();
            var model = new PlayerInputModel(bus);
            InputEnabledChangedEvent? enabled = null;
            bus.GetEventStream<InputEnabledChangedEvent>().Subscribe(e => enabled = e);

            var viewModel = new PlayerInputViewModel(model, bus);
            viewModel.Initialize();

            Assert.IsNotNull(enabled);
            Assert.IsTrue(enabled!.Enabled);
        }
    }
}
