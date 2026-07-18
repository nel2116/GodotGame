using NUnit.Framework;
using Systems.Player.State;

namespace Tests.Core.Player.State
{
    /// <summary>
    /// 企画ドキュメント「プレイヤーアクション・フレーム表」との整合性を検証する
    /// </summary>
    public class StandardActionFrameDataTests : TestBase
    {
        [Test]
        public void AttackL1_MatchesSpecification()
        {
            var data = StandardActionFrameData.AttackL1();
            Assert.That(data.TotalFrames, Is.EqualTo(20));
            Assert.That(data.StartupFrames, Is.EqualTo(4));
            Assert.That(data.ActiveFrames, Is.EqualTo(4));
            Assert.That(data.RecoveryFrames, Is.EqualTo(12));
            Assert.That(data.MovementDistance, Is.EqualTo(0.3f));
            Assert.IsFalse(data.IsInvincible(0));
        }

        [Test]
        public void AttackL2_MatchesSpecification()
        {
            var data = StandardActionFrameData.AttackL2();
            Assert.That(data.TotalFrames, Is.EqualTo(22));
            Assert.That(data.StartupFrames, Is.EqualTo(3));
            Assert.That(data.ActiveFrames, Is.EqualTo(5));
            Assert.That(data.RecoveryFrames, Is.EqualTo(14));
            Assert.That(data.MovementDistance, Is.EqualTo(0.35f));
        }

        [Test]
        public void ChargeAttack_MatchesSpecification()
        {
            var data = StandardActionFrameData.ChargeAttack();
            Assert.That(data.TotalFrames, Is.EqualTo(40));
            Assert.That(data.StartupFrames, Is.EqualTo(16));
            Assert.That(data.ActiveFrames, Is.EqualTo(6));
            Assert.That(data.RecoveryFrames, Is.EqualTo(18));
            Assert.That(data.MovementDistance, Is.EqualTo(0.5f));
        }

        [Test]
        public void Dodge_HasInvincibilityWindowFrom3To10()
        {
            var data = StandardActionFrameData.Dodge();
            data.SetStartFrame(0);

            Assert.That(data.TotalFrames, Is.EqualTo(26));
            Assert.That(data.MovementDistance, Is.EqualTo(3f));
            Assert.IsFalse(data.IsInvincible(2));
            Assert.IsTrue(data.IsInvincible(3));
            Assert.IsTrue(data.IsInvincible(10));
            Assert.IsFalse(data.IsInvincible(11));
        }

        [Test]
        public void Jump_HasReducedAirControlRate()
        {
            var data = StandardActionFrameData.Jump();
            Assert.That(data.MovementDistance, Is.EqualTo(4f));
            Assert.That(data.AirControlRate, Is.EqualTo(0.6f));
        }
    }
}
