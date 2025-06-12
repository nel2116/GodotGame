using NUnit.Framework;
using Systems.Player.Input;
using Systems.Player.Movement;
using Core.Events;
using Godot;
using System.Reflection;

namespace Tests.Core.Player.Input
{
    public class InputMovementIntegrationTests
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
        public void InputModel_Move_UpdatesMovementModel()
        {
            var bus = new GameEventBus();
            var inputModel = new PlayerInputModel();
            var movementModel = new PlayerMovementModel(bus);
            movementModel.Initialize();
            inputModel.Initialize();

            var state = GetPrivateField<InputState>(inputModel, "_currentState");
            SetMovementInput(state, new Vector2(1, 0));

            var method = typeof(PlayerInputModel).GetMethod("ProcessInput", BindingFlags.NonPublic | BindingFlags.Instance);
            method!.Invoke(inputModel, null);

            Assert.AreEqual(new Vector2(5, 0), movementModel.Velocity);
        }
    }
}

