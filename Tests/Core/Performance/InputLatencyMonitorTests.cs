using System;
using NUnit.Framework;
using Systems.Performance;

namespace Tests.Core.Performance
{
    public class InputLatencyMonitorTests
    {
        [Test]
        public void RecordInputProcessed_ComputesElapsedLatency()
        {
            var current = new DateTime(2026, 1, 1, 0, 0, 0);
            var monitor = new InputLatencyMonitor(() => current);

            monitor.RecordInputReceived();
            current = current.AddSeconds(0.05);
            monitor.RecordInputProcessed();

            Assert.That(monitor.CurrentLatency, Is.EqualTo(0.05f).Within(0.0001f));
        }

        [Test]
        public void IsWithinBudget_TrueWhenUnderThreshold()
        {
            var current = new DateTime(2026, 1, 1, 0, 0, 0);
            var monitor = new InputLatencyMonitor(() => current);

            monitor.RecordInputReceived();
            current = current.AddSeconds(0.05);
            monitor.RecordInputProcessed();

            Assert.IsTrue(monitor.IsWithinBudget(0.10f));
        }

        [Test]
        public void IsWithinBudget_FalseWhenOverThreshold()
        {
            var current = new DateTime(2026, 1, 1, 0, 0, 0);
            var monitor = new InputLatencyMonitor(() => current);

            monitor.RecordInputReceived();
            current = current.AddSeconds(0.15);
            monitor.RecordInputProcessed();

            Assert.IsFalse(monitor.IsWithinBudget(0.10f));
        }

        [Test]
        public void RecordInputProcessed_WithoutReceived_DoesNothing()
        {
            var monitor = new InputLatencyMonitor();
            monitor.RecordInputProcessed();

            Assert.That(monitor.CurrentLatency, Is.EqualTo(0f));
        }
    }
}
