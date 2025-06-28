using NUnit.Framework;
using Core.Events;
using Systems.Common.State;
using Systems.Common.Events;
using System.Threading.Tasks;

namespace Tests.Core.State
{
    public class CommonStateViewModelTests
    {
        [Test]
        public async Task ChangeState_PublishesStateEvent()
        {
            // 準備
            var bus = new GameEventBus();
            var model = new CommonStateModel();
            var vm = new CommonStateViewModel(model, bus);
            vm.Initialize();
            StateChangedEvent receivedEvent = null;
            bus.GetEventStream<StateChangedEvent>().Subscribe(e => receivedEvent = e);
            
            // 実行
            vm.ChangeState("NewState");
            await Task.Delay(20); // イベント処理の遅延を考慮（バッファリング16ms + 余裕）
            
            // 検証
            Assert.That(receivedEvent, Is.Not.Null);
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

