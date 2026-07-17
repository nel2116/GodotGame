using NUnit.Framework;
using Systems.Performance;

namespace Tests.Core.Performance
{
    public class FrameTimeTrackerTests
    {
        [Test]
        public void RecordFrameTime_UpdatesCurrentFrameTime()
        {
            var tracker = new FrameTimeTracker();
            tracker.RecordFrameTime(0.016f);
            Assert.That(tracker.CurrentFrameTime, Is.EqualTo(0.016f));

            tracker.RecordFrameTime(0.02f);
            Assert.That(tracker.CurrentFrameTime, Is.EqualTo(0.02f));
        }

        [Test]
        public void AverageFrameTime_ReturnsZeroWhenNoSamples()
        {
            var tracker = new FrameTimeTracker();
            Assert.That(tracker.AverageFrameTime, Is.EqualTo(0f));
        }

        [Test]
        public void AverageFrameTime_ComputesAverageOfSamples()
        {
            var tracker = new FrameTimeTracker();
            tracker.RecordFrameTime(0.01f);
            tracker.RecordFrameTime(0.02f);
            tracker.RecordFrameTime(0.03f);

            Assert.That(tracker.AverageFrameTime, Is.EqualTo(0.02f).Within(0.0001f));
        }

        [Test]
        public void AverageFrameTime_DropsOldestSampleBeyondCapacity()
        {
            var tracker = new FrameTimeTracker(maxSamples: 2);
            tracker.RecordFrameTime(0.01f);
            tracker.RecordFrameTime(0.02f);
            tracker.RecordFrameTime(0.03f);

            Assert.That(tracker.AverageFrameTime, Is.EqualTo(0.025f).Within(0.0001f));
        }
    }
}
