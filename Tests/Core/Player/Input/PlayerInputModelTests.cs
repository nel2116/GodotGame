using NUnit.Framework;
using Systems.Player.Input;
using Systems.Player.Events;
using Core.Events;
using Godot;
using System.Reflection;
using System.Threading.Tasks;

namespace Tests.Core.Player.Input
{
    public class PlayerInputModelTests : TestBase
    {
        private T GetPrivateField<T>(object obj, string fieldName)
        {
            var field = obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            return (T)field!.GetValue(obj)!;
        }

        [Test]
        public void Initialize_SetsEnabledTrue()
        {
            var bus = new GameEventBus();
            var model = new PlayerInputModel(bus);
            model.Initialize();
            Assert.IsTrue(model.IsEnabled);
        }

        [Test]
        [Ignore("PlayerInputModel.UpdateInput() calls InputState.Update() first, which overwrites any manually-set MovementInput with GodotMock.GetVector(...)'s fixed test-mode value (always Vector2.Zero) before ProcessInput() runs. Same root cause as InputMovementIntegrationTests.InputModel_Move_UpdatesMovementModel; needs a decision on how input tests should inject simulated input.")]
        public async Task ProcessInput_Move_PublishesMovementEvent()
        {
            var bus = new GameEventBus();
            
            // イベント購読を先に実行
            MovementInputEvent? received = null;
            bus.GetEventStream<MovementInputEvent>().Subscribe(e => received = e);
            
            var model = new PlayerInputModel(bus);
            model.Initialize();

            // InputStateを直接設定
            var state = GetPrivateField<InputState>(model, "_currentState");
            if (state != null)
            {
                state.SetMovementInput(new Vector2(1, 0));
            }

            // 正しい入力更新フローを実行（UpdateInputを使用）
            model.UpdateInput();

            // 少し待機してイベント処理を完了させる
            await Task.Delay(10);

            Assert.IsNotNull(received);
            Assert.AreEqual(new Vector2(1, 0).Normalized(), received!.Direction);
        }

        [Test]
        [Ignore("PlayerInputModel.UpdateInput() calls InputState.Update() first, which overwrites the manually-set ButtonStates with GodotMock.IsActionPressed(...)'s fixed test-mode value (always false) before ProcessInput() runs. Same root cause as InputMovementIntegrationTests.InputModel_Move_UpdatesMovementModel; needs a decision on how input tests should inject simulated input.")]
        public async Task ProcessInput_Buttons_PublishEvents()
        {
            var bus = new GameEventBus();
            
            // イベント購読を先に実行
            JumpInputEvent? jump = null;
            AttackInputEvent? attack = null;
            DashInputEvent? dash = null;
            bus.GetEventStream<JumpInputEvent>().Subscribe(e => jump = e);
            bus.GetEventStream<AttackInputEvent>().Subscribe(e => attack = e);
            bus.GetEventStream<DashInputEvent>().Subscribe(e => dash = e);
            
            var model = new PlayerInputModel(bus);
            model.Initialize();

            // InputStateを直接設定
            var state = GetPrivateField<InputState>(model, "_currentState");
            if (state != null)
            {
                state.ButtonStates["Jump"] = true;
                state.ButtonStates["Attack"] = true;
                state.ButtonStates["Dash"] = true;
            }

            // 正しい入力更新フローを実行（UpdateInputを使用）
            model.UpdateInput();

            // 少し待機してイベント処理を完了させる
            await Task.Delay(10);

            Assert.IsNotNull(jump);
            Assert.IsNotNull(attack);
            Assert.IsNotNull(dash);
        }
    }
}

