using NUnit.Framework;
using Systems.Performance;

namespace Tests.Core.Performance
{
    public class FrameTimeTrackerTests
    {
        [Test]
        public void IsWithinBudget_TrueWhenFasterThanTarget()
        {
            var tracker = new FrameTimeTracker(60f);
            tracker.RecordFrame(0.010f);

            Assert.IsTrue(tracker.IsWithinBudget());
        }

        [Test]
        public void IsWithinBudget_FalseWhenSlowerThanTarget()
        {
            var tracker = new FrameTimeTracker(60f);
            tracker.RecordFrame(0.020f);

            Assert.IsFalse(tracker.IsWithinBudget());
        }

        [Test]
        public void CurrentFps_ComputedFromFrameTime()
        {
            var tracker = new FrameTimeTracker(60f);
            tracker.RecordFrame(0.020f);

            Assert.That(tracker.CurrentFps, Is.EqualTo(50f).Within(0.01f));
        }

        [Test]
        public void TargetFrameTime_MatchesTargetFps()
        {
            var tracker = new FrameTimeTracker(60f);
            Assert.That(tracker.TargetFrameTime, Is.EqualTo(1f / 60f).Within(0.0001f));
        }
    }
}
