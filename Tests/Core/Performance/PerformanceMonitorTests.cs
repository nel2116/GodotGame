using NUnit.Framework;
using Systems.Performance;
using Core.Events;

namespace Tests.Core.Performance
{
    public class PerformanceMonitorTests : TestBase
    {
        [Test]
        public void Update_PublishesWarning_WhenFrameTimeExceedsTarget()
        {
            var bus = new GameEventBus();
            var frameTimeTracker = new FrameTimeTracker();
            var inputLatencyMonitor = new InputLatencyMonitor();
            var monitor = new PerformanceMonitor(frameTimeTracker, inputLatencyMonitor, bus);

            PerformanceWarningEvent? received = null;
            using (bus.GetEventStream<PerformanceWarningEvent>().Subscribe(e => received = e))
            {
                frameTimeTracker.RecordFrameTime(1f / 30f); // 30FPS相当、目標60FPSを超過
                monitor.Update();
            }

            Assert.IsNotNull(received);
            Assert.That(received!.MetricName, Is.EqualTo("FrameTime"));
        }

        [Test]
        public void Update_DoesNotPublishWarning_WhenFrameTimeWithinTarget()
        {
            var bus = new GameEventBus();
            var frameTimeTracker = new FrameTimeTracker();
            var inputLatencyMonitor = new InputLatencyMonitor();
            var monitor = new PerformanceMonitor(frameTimeTracker, inputLatencyMonitor, bus);

            var receivedCount = 0;
            using (bus.GetEventStream<PerformanceWarningEvent>().Subscribe(_ => receivedCount++))
            {
                frameTimeTracker.RecordFrameTime(1f / 60f);
                monitor.Update();
            }

            Assert.That(receivedCount, Is.EqualTo(0));
        }

        [Test]
        public void Update_PublishesWarning_WhenInputLatencyExceedsThreshold()
        {
            var bus = new GameEventBus();
            var frameTimeTracker = new FrameTimeTracker();
            var inputLatencyMonitor = new InputLatencyMonitor();
            var monitor = new PerformanceMonitor(frameTimeTracker, inputLatencyMonitor, bus);

            PerformanceWarningEvent? received = null;
            using (bus.GetEventStream<PerformanceWarningEvent>().Subscribe(e => received = e))
            {
                inputLatencyMonitor.RecordInput(0.0);
                inputLatencyMonitor.RecordProcessed(0.15); // 企画仕様の0.10秒を超過
                monitor.Update();
            }

            Assert.IsNotNull(received);
            Assert.That(received!.MetricName, Is.EqualTo("InputLatency"));
        }
    }
}
