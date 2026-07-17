using NUnit.Framework;
using Systems.Player.State;

namespace Tests.Core.Player.State
{
    public class ActionFrameDataTests
    {
        [Test]
        public void IsInvincible_ReturnsFalseWhenNotConfigured()
        {
            var data = new ActionFrameData("Test", 26, 1, 8, 17);
            data.SetStartFrame(0);
            Assert.IsFalse(data.IsInvincible(5));
        }

        [Test]
        public void IsInvincible_ReturnsTrueWithinConfiguredRange()
        {
            var data = new ActionFrameData(
                "Dodge", 26, 1, 8, 17,
                invincibilityStartFrame: 3, invincibilityEndFrame: 10);
            data.SetStartFrame(0);

            Assert.IsFalse(data.IsInvincible(2));
            Assert.IsTrue(data.IsInvincible(3));
            Assert.IsTrue(data.IsInvincible(10));
            Assert.IsFalse(data.IsInvincible(11));
        }

        [Test]
        public void IsInvincible_UsesStartFrameOffset()
        {
            var data = new ActionFrameData(
                "Dodge", 26, 1, 8, 17,
                invincibilityStartFrame: 3, invincibilityEndFrame: 10);
            data.SetStartFrame(100);

            Assert.IsFalse(data.IsInvincible(102));
            Assert.IsTrue(data.IsInvincible(103));
            Assert.IsTrue(data.IsInvincible(110));
        }

        [Test]
        public void CanCancelTo_ReturnsTrueForListedAction()
        {
            var data = new ActionFrameData(
                "Attack_L1", 20, 4, 4, 12,
                cancelableTo: new[] { "Dodge", "ChargeAttack" });

            Assert.IsTrue(data.CanCancelTo("Dodge"));
            Assert.IsTrue(data.CanCancelTo("ChargeAttack"));
            Assert.IsFalse(data.CanCancelTo("Attack_L2"));
        }

        [Test]
        public void MovementDistanceAndAirControlRate_DefaultToZeroAndOne()
        {
            var data = new ActionFrameData("Test", 10, 1, 5, 4);
            Assert.That(data.MovementDistance, Is.EqualTo(0f));
            Assert.That(data.AirControlRate, Is.EqualTo(1f));
        }
    }
}
