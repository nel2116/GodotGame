using NUnit.Framework;
using Systems.Player.Animation;
using Systems.Player.Events;
using Core.Events;
using System.Threading.Tasks;

namespace Tests.Core.Player.Animation
{
    public class PlayerAnimationModelTests
    {
        [Test]
        public async Task BlendAnimation_PublishesEvents()
        {
            var bus = new GameEventBus();
            
            // イベント購読を先に実行
            AnimationBlendStartedEvent? started = null;
            AnimationBlendCompletedEvent? completed = null;
            bus.GetEventStream<AnimationBlendStartedEvent>().Subscribe(e => started = e);
            bus.GetEventStream<AnimationBlendCompletedEvent>().Subscribe(e => completed = e);
            
            var model = new PlayerAnimationModel(bus);
            model.Initialize();

            await model.BlendAnimationAsync("Idle", "Jump", 0.1f);

            // 少し待機してイベント処理を完了させる
            await Task.Delay(10);

            Assert.IsNotNull(started);
            Assert.AreEqual("Idle", started!.FromAnimation);
            Assert.AreEqual("Jump", started.ToAnimation);
            Assert.IsNotNull(completed);
            Assert.AreEqual("Jump", completed!.AnimationName);
        }

        [Test]
        public async Task PlayAnimation_InvalidName_PublishesError()
        {
            var bus = new GameEventBus();
            
            // イベント購読を先に実行
            ErrorEvent? error = null;
            bus.GetEventStream<ErrorEvent>().Subscribe(e => error = e);
            
            var model = new PlayerAnimationModel(bus);
            model.Initialize();

            model.PlayAnimation("Invalid");

            // 少し待機してイベント処理を完了させる
            await Task.Delay(10);

            Assert.IsNotNull(error);
            Assert.AreEqual("PlayerAnimationModel", error!.Exception.SystemName);
            Assert.AreEqual("PlayAnimation", error.Exception.Operation);
        }
    }
}
