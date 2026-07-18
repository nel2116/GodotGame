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
            StateChangedEvent? receivedEvent = null;
            bus.GetEventStream<StateChangedEvent>().Subscribe(e => receivedEvent = e);

            // 実行 - 有効な状態遷移を実行（Idle -> Walk）
            vm.ChangeState("Walk");
            await Task.Delay(10); // イベント処理の遅延を考慮

            // 検証
            Assert.That(receivedEvent, Is.Not.Null);
            Assert.That(receivedEvent!.State, Is.EqualTo("Walk"));
        }

        [Test]
        public async Task ChangeState_Invalid_NoEvent()
        {
            var bus = new GameEventBus();
            var model = new CommonStateModel();
            var vm = new CommonStateViewModel(model, bus);
            bool called = false;
            // 購読をInitialize()の前に設定（Initialize()で発行されるイベントは無視）
            bus.GetEventStream<StateChangedEvent>().Subscribe(_ => called = true);
            vm.Initialize();
            await Task.Delay(10); // Initialize()のイベント処理を待つ
            called = false; // Initialize()のイベントをリセット
            vm.ChangeState("Invalid");
            await Task.Delay(10); // イベント処理の遅延を考慮
            Assert.IsFalse(called, "無効な状態変更ではイベントが発行されないべき");
            Assert.AreEqual("Idle", vm.CurrentState.Value);
        }
    }
}

