using NUnit.Framework;
using Systems.Player.Animation;
using Systems.Player.Events;
using Core.Events;

namespace Tests.Core.Player.Animation
{
    public class PlayerAnimationViewModelTests
    {
        [Test]
        public void UpdateAnimation_DefaultIdle()
        {
            var bus = new GameEventBus();
            var model = new PlayerAnimationModel(bus);
            var viewModel = new PlayerAnimationViewModel(model, bus);
            viewModel.Initialize();
            viewModel.UpdateAnimation();
            Assert.That(viewModel.CurrentAnimation.Value, Is.EqualTo("Idle"));
        }

        [Test]
        public void PlayAnimation_ValidName_PublishesEvents()
        {
            var bus = new GameEventBus();
            var model = new PlayerAnimationModel(bus);
            var viewModel = new PlayerAnimationViewModel(model, bus);
            viewModel.Initialize();

            AnimationPlayedEvent? played = null;
            AnimationChangedEvent? changed = null;
            bus.GetEventStream<AnimationPlayedEvent>().Subscribe(e => played = e);
            bus.GetEventStream<AnimationChangedEvent>().Subscribe(e => changed = e);

            viewModel.HandleAnimation("Jump");

            Assert.That(viewModel.CurrentAnimation.Value, Is.EqualTo("Jump"));
            Assert.IsNotNull(played);
            Assert.AreEqual("Jump", played!.AnimationName);
            Assert.IsNotNull(changed);
            Assert.AreEqual("Jump", changed!.Animation);
        }

        [Test]
        public void Initialize_PublishesInitialEvents()
        {
            var bus = new GameEventBus();
            var model = new PlayerAnimationModel(bus);
            AnimationPlayingChangedEvent? playing = null;
            AnimationSpeedChangedEvent? speed = null;
            bus.GetEventStream<AnimationPlayingChangedEvent>().Subscribe(e => playing = e);
            bus.GetEventStream<AnimationSpeedChangedEvent>().Subscribe(e => speed = e);

            var viewModel = new PlayerAnimationViewModel(model, bus);
            viewModel.Initialize();

            Assert.IsNotNull(playing);
            Assert.IsTrue(playing!.IsPlaying);
            Assert.IsNotNull(speed);
            Assert.AreEqual(1.0f, speed!.Speed);
        }
    }
}
