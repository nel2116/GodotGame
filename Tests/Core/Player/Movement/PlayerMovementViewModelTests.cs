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
            var model = new PlayerMovementModel(bus);
            var viewModel = new PlayerMovementViewModel(model, bus);
            viewModel.Initialize();
            viewModel.UpdateMovement();
            Assert.That(viewModel.Velocity.Value, Is.EqualTo(Vector2.Zero));
        }

        [Test]
        public void Dash_PublishesDashingEvent()
        {
            var bus = new GameEventBus();
            
            // イベント購読を先に実行
            MovementDashingChangedEvent? received = null;
            bus.GetEventStream<MovementDashingChangedEvent>().Subscribe(e => received = e);
            
            var model = new PlayerMovementModel(bus);
            var viewModel = new PlayerMovementViewModel(model, bus);
            viewModel.Initialize();

            viewModel.HandleDash();

            // 少し待機してイベント処理を完了させる
            System.Threading.Thread.Sleep(10);

            Assert.IsNotNull(received);
            Assert.IsTrue(received!.IsDashing);
            Assert.IsTrue(viewModel.IsDashing.Value);
        }

        [Test]
        public void Jump_Update_PublishesGroundedEvent()
        {
            var bus = new GameEventBus();
            
            // イベント購読を先に実行
            MovementGroundedChangedEvent? received = null;
            bus.GetEventStream<MovementGroundedChangedEvent>().Subscribe(e => received = e);
            
            var model = new PlayerMovementModel(bus);
            var viewModel = new PlayerMovementViewModel(model, bus);
            viewModel.Initialize();

            viewModel.HandleJump();
            viewModel.UpdateMovement();

            // 少し待機してイベント処理を完了させる
            System.Threading.Thread.Sleep(10);

            Assert.IsNotNull(received);
            Assert.IsFalse(received!.IsGrounded);
            Assert.IsFalse(viewModel.IsGrounded.Value);
        }
    }
}
