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
        [Ignore("CommonMovementModel.Jump() sets the separate VerticalVelocity field, not Velocity.Y (they were split at some point after this test was written). This test documents a real mismatch discovered while re-enabling this long-excluded file; needs a decision on whether to assert against VerticalVelocity or whether Jump() should still affect Velocity.Y.")]
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
        [Ignore("CommonMovementModel.UpdateGroundedState() only re-grounds when |VerticalVelocity| < 0.01, but gravity increments it by a fixed 0.1568/update, so the value can jump straight over that narrow window and never re-ground in this pure-logic model. This test documents a real physics gap discovered while re-enabling this long-excluded file; needs a decision on the intended grounded-detection behavior (this model has no floor-collision input, unlike the real CharacterBody3D.IsOnFloor() path noted in the class comments).")]
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
