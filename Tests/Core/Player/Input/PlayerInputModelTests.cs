using NUnit.Framework;
using Systems.Player.Input;
using Systems.Player.Events;
using Core.Events;
using Godot;
using System.Reflection;

namespace Tests.Core.Player.Input
{
    public class PlayerInputModelTests
    {
        private T GetPrivateField<T>(object obj, string fieldName)
        {
            var field = obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            return (T)field!.GetValue(obj)!;
        }

        private void SetMovementInput(InputState state, Vector2 value)
        {
            var prop = typeof(InputState).GetProperty("MovementInput", BindingFlags.NonPublic | BindingFlags.Instance);
            prop!.SetValue(state, value);
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
        public void ProcessInput_Move_PublishesMovementEvent()
        {
            var bus = new GameEventBus();
            var model = new PlayerInputModel(bus);
            model.Initialize();

            var state = GetPrivateField<InputState>(model, "_currentState");
            SetMovementInput(state, new Vector2(1, 0));

            MovementInputEvent? received = null;
            bus.GetEventStream<MovementInputEvent>().Subscribe(e => received = e);

            var method = typeof(PlayerInputModel).GetMethod("ProcessInput", BindingFlags.NonPublic | BindingFlags.Instance);
            method!.Invoke(model, null);

            Assert.IsNotNull(received);
            Assert.AreEqual(new Vector2(1, 0).Normalized(), received!.Direction);
        }

        [Test]
        public void ProcessInput_Buttons_PublishEvents()
        {
            var bus = new GameEventBus();
            var model = new PlayerInputModel(bus);
            model.Initialize();

            var state = GetPrivateField<InputState>(model, "_currentState");
            state.ButtonStates["Jump"] = true;
            state.ButtonStates["Attack"] = true;
            state.ButtonStates["Dash"] = true;

            JumpInputEvent? jump = null;
            AttackInputEvent? attack = null;
            DashInputEvent? dash = null;
            bus.GetEventStream<JumpInputEvent>().Subscribe(e => jump = e);
            bus.GetEventStream<AttackInputEvent>().Subscribe(e => attack = e);
            bus.GetEventStream<DashInputEvent>().Subscribe(e => dash = e);

            var method = typeof(PlayerInputModel).GetMethod("ProcessInput", BindingFlags.NonPublic | BindingFlags.Instance);
            method!.Invoke(model, null);

            Assert.IsNotNull(jump);
            Assert.IsNotNull(attack);
            Assert.IsNotNull(dash);
        }
    }
}

