using NUnit.Framework;
using Systems.Player.Input;

namespace Tests.Core.Player.Input
{
    public class InputRingBufferTests
    {
        [Test]
        public void Add_OverCapacity_OverwritesOldest()
        {
            var buffer = new InputRingBuffer<int>(3);
            buffer.Add(1);
            buffer.Add(2);
            buffer.Add(3);
            buffer.Add(4);
            var items = buffer.GetItems();
            CollectionAssert.AreEqual(new[] {2,3,4}, items);
        }
    }
}
