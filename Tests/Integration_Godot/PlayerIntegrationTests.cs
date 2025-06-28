using NUnit.Framework;
using System;
using System.Reflection;
using Godot;

namespace Tests.Core.Player
{
    public class PlayerIntegrationTests
    {
        private T GetPrivateField<T>(object obj, string fieldName)
        {
            var field = obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
            return (T)field!.GetValue(obj)!;
        }

        [Test]
        public void Ready_InitializesSubsystems()
        {
            var player = new global::Player();
            player._Ready();

            Assert.NotNull(GetPrivateField<object>(player, "_input_vm"));
            Assert.NotNull(GetPrivateField<object>(player, "_movement_vm"));
            Assert.NotNull(GetPrivateField<object>(player, "_combat_vm"));
            Assert.NotNull(GetPrivateField<object>(player, "_animation_vm"));
            Assert.NotNull(GetPrivateField<object>(player, "_state_vm"));
            Assert.NotNull(GetPrivateField<object>(player, "_progression_vm"));
        }

        [Test]
        public void PhysicsProcess_AfterReady_NoException()
        {
            var player = new global::Player();
            player._Ready();

            Assert.DoesNotThrow(() => player._PhysicsProcess(0.016));
        }

        [Test]
        public void ExitTree_DisposesSubsystems()
        {
            var player = new global::Player();
            player._Ready();
            player._ExitTree();

            Assert.Throws<ObjectDisposedException>(() => player._PhysicsProcess(0.016));
        }
    }
}
