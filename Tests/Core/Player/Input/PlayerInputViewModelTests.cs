using NUnit.Framework;
using Systems.Player.Input;
using Systems.Player.Events;
using Core.Events;
using Godot;
using System.Reflection;
using System.Threading.Tasks;

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
        public async Task UpdateInput_PublishesStateEvent()
        {
            var bus = new GameEventBus();
            
            // イベント購読を先に実行
            InputStateChangedEvent? received = null;
            bus.GetEventStream<InputStateChangedEvent>().Subscribe(e => received = e);
            
            var model = new PlayerInputModel(bus);
            var viewModel = new PlayerInputViewModel(model, bus);
            viewModel.Initialize();

            // 正しい入力更新フローを実行（UpdateInputを使用）
            viewModel.UpdateInput();

            // 少し待機してイベント処理を完了させる
            await Task.Delay(10);

            Assert.IsNotNull(received);
            Assert.IsNotNull(received!.State);
        }

        [Test]
        public async Task Initialize_PublishesEnabledEvent()
        {
            var bus = new GameEventBus();
            
            // イベント購読を先に実行
            InputEnabledChangedEvent? enabled = null;
            bus.GetEventStream<InputEnabledChangedEvent>().Subscribe(e => enabled = e);

            var model = new PlayerInputModel(bus);
            var viewModel = new PlayerInputViewModel(model, bus);
            viewModel.Initialize();

            // 少し待機してイベント処理を完了させる
            await Task.Delay(10);

            Assert.IsNotNull(enabled);
            Assert.IsTrue(enabled!.Enabled);
        }
    }
}
