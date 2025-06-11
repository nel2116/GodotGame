using NUnit.Framework;
using Systems.Player.Movement;
using Systems.Common.Events;
using Core.Events;
using Godot;

namespace Tests.Core.Player.Movement
{
    public class PlayerMovementViewModelTests
    {
        [Test]
        public void UpdateMovement_DefaultVelocity_Zero()
        {
            var bus = new GameEventBus();
            var model = new PlayerMovementModel();
            var viewModel = new PlayerMovementViewModel(model, bus);
            viewModel.Initialize();
            viewModel.UpdateMovement();
            Assert.That(viewModel.Velocity.Value, Is.EqualTo(Vector2.Zero));
        }

        [Test]
        public void Dash_PublishesDashingEvent()
        {
            var bus = new GameEventBus();
            var model = new PlayerMovementModel();
            var viewModel = new PlayerMovementViewModel(model, bus);
            viewModel.Initialize();

            MovementDashingChangedEvent? received = null;
            bus.GetEventStream<MovementDashingChangedEvent>().Subscribe(e => received = e);

            viewModel.HandleDash();

            Assert.IsNotNull(received);
            Assert.IsTrue(received!.IsDashing);
            Assert.IsTrue(viewModel.IsDashing.Value);
        }

        [Test]
        public void Jump_Update_PublishesGroundedEvent()
        {
            var bus = new GameEventBus();
            var model = new PlayerMovementModel();
            var viewModel = new PlayerMovementViewModel(model, bus);
            viewModel.Initialize();

            MovementGroundedChangedEvent? received = null;
            bus.GetEventStream<MovementGroundedChangedEvent>().Subscribe(e => received = e);

            viewModel.HandleJump();
            viewModel.UpdateMovement();

            Assert.IsNotNull(received);
            Assert.IsFalse(received!.IsGrounded);
            Assert.IsFalse(viewModel.IsGrounded.Value);
        }
    }
}
