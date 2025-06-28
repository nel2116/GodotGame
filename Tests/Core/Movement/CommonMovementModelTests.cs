using NUnit.Framework;
using Systems.Common.Movement;
using Godot;

namespace Tests.Core.Movement
{
    public class CommonMovementModelTests
    {
        [Test]
        public void Move_UpdatesVelocity()
        {
            var model = new CommonMovementModel();
            model.Initialize();
            model.Move(new Vector2(1, 0));
            Assert.AreEqual(new Vector2(1, 0) * 5.0f, model.Velocity);
        }

        [Test]
        public void Jump_SetsVerticalVelocity()
        {
            var model = new CommonMovementModel();
            model.Initialize();
            model.Jump();
            Assert.AreEqual(10.0f, model.Velocity.Y);
        }

        [Test]
        public void Dash_MultipliesVelocity()
        {
            var model = new CommonMovementModel();
            model.Initialize();
            model.Move(new Vector2(1, 0));
            var before = model.Velocity;
            model.Dash();
            Assert.AreEqual(before * 2.0f, model.Velocity);
            Assert.IsFalse(model.CanDash);
        }

        [Test]
        public void Update_FromJump_ReturnsGrounded()
        {
            var model = new CommonMovementModel();
            model.Initialize();
            model.Jump();
            model.Update();
            Assert.IsFalse(model.IsGrounded);
            for (int i = 0; i < 100; i++)
            {
                model.Update();
            }
            Assert.IsTrue(model.IsGrounded);
            Assert.IsTrue(model.CanJump);
        }
        [Test]
        public void Move_NotGrounded_DoesNotChangeVelocity()
        {
            var model = new CommonMovementModel();
            model.Initialize();
            model.Jump();
            model.Update();
            var before = model.Velocity;
            model.Move(new Vector2(1, 0));
            Assert.AreEqual(before, model.Velocity);
        }

        [Test]
        public void Dash_ResetsAfterUpdate()
        {
            var model = new CommonMovementModel();
            model.Initialize();
            model.Move(new Vector2(1, 0));
            model.Dash();
            Assert.IsFalse(model.CanDash);
            model.Update();
            Assert.IsTrue(model.CanDash);
            Assert.AreEqual(new Vector2(9, 0), model.Velocity);
        }

        [Test]
        public void Jump_NotGrounded_NoEffect()
        {
            var model = new CommonMovementModel();
            model.Initialize();
            model.Jump();
            model.Update();
            var before = model.Velocity;
            model.Jump();
            Assert.AreEqual(before, model.Velocity);
        }

        [Test]
        public void Dash_WhenCannotDash_NoEffect()
        {
            var model = new CommonMovementModel();
            model.Initialize();
            model.Move(new Vector2(1, 0));
            model.Dash();
            var after_dash = model.Velocity;
            model.Dash();
            Assert.AreEqual(after_dash, model.Velocity);
        }
    }
}
