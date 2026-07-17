using NUnit.Framework;
using Systems.Performance;
using Core.Events;

namespace Tests.Core.Performance
{
    public class PerformanceMonitorTests
    {
        [Test]
        public void RecordFrame_WithinBudget_DoesNotPublishWarning()
        {
            var bus = new GameEventBus();
            var monitor = new PerformanceMonitor(bus);

            PerformanceWarningEvent? received = null;
            bus.GetEventStream<PerformanceWarningEvent>().Subscribe(e => received = e);

            monitor.RecordFrame(0.010f);

            Assert.IsNull(received);
        }

        [Test]
        public void RecordFrame_ExceedsBudget_PublishesFrameTimeWarning()
        {
            var bus = new GameEventBus();
            var monitor = new PerformanceMonitor(bus);

            PerformanceWarningEvent? received = null;
            bus.GetEventStream<PerformanceWarningEvent>().Subscribe(e => received = e);

            monitor.RecordFrame(0.030f);

            Assert.IsNotNull(received);
            Assert.That(received!.MetricName, Is.EqualTo("FrameTime"));
        }

        [Test]
        public void RecordInputProcessed_ExceedsBudget_PublishesInputLatencyWarning()
        {
            var current = new System.DateTime(2026, 1, 1);
            var frameTracker = new FrameTimeTracker();
            var latencyMonitor = new InputLatencyMonitor(() => current);
            var bus = new GameEventBus();
            var monitor = new PerformanceMonitor(bus, frameTracker, latencyMonitor);

            PerformanceWarningEvent? received = null;
            bus.GetEventStream<PerformanceWarningEvent>().Subscribe(e => received = e);

            monitor.RecordInputReceived();
            current = current.AddSeconds(0.15);
            monitor.RecordInputProcessed();

            Assert.IsNotNull(received);
            Assert.That(received!.MetricName, Is.EqualTo("InputLatency"));
        }

        [Test]
        public void RecordInputProcessed_WithinBudget_DoesNotPublishWarning()
        {
            var current = new System.DateTime(2026, 1, 1);
            var frameTracker = new FrameTimeTracker();
            var latencyMonitor = new InputLatencyMonitor(() => current);
            var bus = new GameEventBus();
            var monitor = new PerformanceMonitor(bus, frameTracker, latencyMonitor);

            PerformanceWarningEvent? received = null;
            bus.GetEventStream<PerformanceWarningEvent>().Subscribe(e => received = e);

            monitor.RecordInputReceived();
            current = current.AddSeconds(0.02);
            monitor.RecordInputProcessed();

            Assert.IsNull(received);
        }
    }
}
