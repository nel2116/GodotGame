using NUnit.Framework;
using Systems.Player.Input;

namespace Tests.Core.Player.Input
{
    public class InputBufferTests
    {
        [Test]
        public void PopAction_ReturnsHighestPriority()
        {
            var buffer = new InputBuffer();
            buffer.BufferAction("Attack");
            buffer.BufferAction("Dash");
            buffer.BufferAction("Jump");
            var action = buffer.PopAction();
            Assert.AreEqual("Dash", action);
        }

        [Test]
        public void PopAction_ClearsBuffer()
        {
            var buffer = new InputBuffer();
            buffer.BufferAction("Jump");
            var first = buffer.PopAction();
            var second = buffer.PopAction();
            Assert.AreEqual("Jump", first);
            Assert.IsNull(second);
        }
    }
}
