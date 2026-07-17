using NUnit.Framework;
using Systems.Player.State;

namespace Tests.Core.Player.State
{
    public class ActionFrameDataTests
    {
        [Test]
        public void IsInvincible_ReturnsFalseWhenNotConfigured()
        {
            var data = new ActionFrameData("Test", 10, 1, 5, 4);
            data.SetStartFrame(0);

            for (int frame = 0; frame < 10; frame++)
            {
                Assert.IsFalse(data.IsInvincible(frame));
            }
        }

        [Test]
        public void IsInvincible_ReturnsTrueWithinConfiguredRange()
        {
            var data = new ActionFrameData("Dodge", 20, 2, 10, 8, invincibilityStartFrame: 3, invincibilityEndFrame: 8);
            data.SetStartFrame(5);

            Assert.IsFalse(data.IsInvincible(5 + 2));
            Assert.IsTrue(data.IsInvincible(5 + 3));
            Assert.IsTrue(data.IsInvincible(5 + 8));
            Assert.IsFalse(data.IsInvincible(5 + 9));
        }

        [Test]
        public void MovementDistanceAndAirControlRate_AreStoredCorrectly()
        {
            var data = new ActionFrameData(
                "Dash",
                12,
                2,
                6,
                4,
                movementDistance: 3.5f,
                airControlRate: 0.25f);

            Assert.That(data.MovementDistance, Is.EqualTo(3.5f));
            Assert.That(data.AirControlRate, Is.EqualTo(0.25f));
        }

        [Test]
        public void CancelableTo_DefaultsToEmptyList()
        {
            var data = new ActionFrameData("Attack_L1", 30, 5, 10, 15);
            Assert.That(data.CancelableTo, Is.Empty);
        }

        [Test]
        public void CancelableTo_StoresProvidedActionNames()
        {
            var data = new ActionFrameData(
                "Attack_L1",
                30,
                5,
                10,
                15,
                cancelableTo: new[] { "Dodge", "Attack_L2" });

            Assert.That(data.CancelableTo, Is.EquivalentTo(new[] { "Dodge", "Attack_L2" }));
        }
    }
}
