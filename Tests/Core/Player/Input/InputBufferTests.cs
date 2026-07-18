using NUnit.Framework;
using Systems.Player.Input;
using Godot;

namespace Tests.Core.Player.Input
{
    public class InputBufferTests
    {
        [Test]
        public void PopAction_ReturnsHighestPriority()
        {
            var buffer = new InputBuffer();

            // Attack アクションを設定
            var state1 = new InputState();
            state1.ButtonStates[PlayerInputActionNames.Attack] = true;
            buffer.CollectInputState(state1);

            // Dash アクションを設定（最高優先度）
            var state2 = new InputState();
            state2.ButtonStates[PlayerInputActionNames.Dash] = true;
            buffer.CollectInputState(state2);

            // Jump アクションを設定
            var state3 = new InputState();
            state3.ButtonStates[PlayerInputActionNames.Jump] = true;
            buffer.CollectInputState(state3);

            var action = buffer.PopAction();
            Assert.AreEqual("Dash", action);
        }

        [Test]
        public void PopAction_ClearsBuffer()
        {
            var buffer = new InputBuffer();

            // Jump アクションを設定
            var state = new InputState();
            state.ButtonStates[PlayerInputActionNames.Jump] = true;
            buffer.CollectInputState(state);

            var first = buffer.PopAction();
            var second = buffer.PopAction();
            Assert.AreEqual("Jump", first);
            Assert.IsNull(second);
        }
    }
}
