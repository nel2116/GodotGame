using NUnit.Framework;
using Systems.Player.State;
using Systems.Player.Events;
using Systems.Common.Events;
using Core.Events;

namespace Tests.Core.Player.State
{
    public class PlayerStateViewModelTests
    {
        [Test]
        public void UpdateState_DefaultIdle()
        {
            var bus = new GameEventBus();
            var model = new PlayerStateModel(bus);
            var viewModel = new PlayerStateViewModel(model, bus);
            viewModel.Initialize();
            viewModel.UpdateState();
            Assert.That(viewModel.CurrentState.Value, Is.EqualTo("Idle"));
        }

        [Test]
        public void ChangeState_PublishesStateEvent()
        {
            var bus = new GameEventBus();
            
            // イベント購読を先に実行
            StateChangedEvent? received = null;
            bus.GetEventStream<StateChangedEvent>().Subscribe(e => received = e);
            
            var model = new PlayerStateModel(bus);
            var viewModel = new PlayerStateViewModel(model, bus);
            viewModel.Initialize();

            viewModel.HandleStateChange("Moving");

            // 少し待機してイベント処理を完了させる
            System.Threading.Thread.Sleep(10);

            Assert.That(viewModel.CurrentState.Value, Is.EqualTo("Moving"));
            Assert.IsNotNull(received);
            Assert.AreEqual("Moving", received!.State);
        }

        [Test]
        public void ChangeState_Invalid_PublishesError()
        {
            var bus = new GameEventBus();
            
            // イベント購読を先に実行
            ErrorEvent? error = null;
            bus.GetEventStream<ErrorEvent>().Subscribe(e => error = e);
            
            var model = new PlayerStateModel(bus);
            var viewModel = new PlayerStateViewModel(model, bus);
            viewModel.Initialize();

            viewModel.HandleStateChange("Unknown");

            // 少し待機してイベント処理を完了させる
            System.Threading.Thread.Sleep(10);

            Assert.IsNotNull(error);
            Assert.AreEqual("PlayerStateModel", error!.Exception.SystemName);
            Assert.AreEqual("ChangeState", error.Exception.Operation);
        }
    }
}
