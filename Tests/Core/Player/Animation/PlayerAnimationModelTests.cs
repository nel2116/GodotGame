using NUnit.Framework;
using Systems.Player.Animation;
using Systems.Player.Events;
using Core.Events;

namespace Tests.Core.Player.Animation
{
    public class PlayerAnimationModelTests
    {
        [Test]
        public void BlendAnimation_PublishesEvents()
        {
            var bus = new GameEventBus();
            var model = new PlayerAnimationModel(bus);
            model.Initialize();

            AnimationBlendStartedEvent? start = null;
            AnimationBlendCompletedEvent? complete = null;
            bus.GetEventStream<AnimationBlendStartedEvent>().Subscribe(e => start = e);
            bus.GetEventStream<AnimationBlendCompletedEvent>().Subscribe(e => complete = e);

            model.BlendAnimation("Idle", "Walk", 0.5f);

            Assert.IsNotNull(start);
            Assert.AreEqual("Idle", start!.FromAnimation);
            Assert.AreEqual("Walk", start.ToAnimation);
            Assert.AreEqual(0.5f, start.BlendTime);
            Assert.IsNotNull(complete);
            Assert.AreEqual("Walk", complete!.AnimationName);
        }

        [Test]
        public void PlayAnimation_InvalidName_PublishesError()
        {
            var bus = new GameEventBus();
            var model = new PlayerAnimationModel(bus);
            model.Initialize();

            ErrorEvent? error = null;
            bus.GetEventStream<ErrorEvent>().Subscribe(e => error = e);

            model.PlayAnimation("Unknown");

            Assert.IsNotNull(error);
            Assert.AreEqual("PlayerAnimationModel", error!.Exception.SystemName);
            Assert.AreEqual("PlayAnimation", error.Exception.Operation);
        }
    }
}
