using NUnit.Framework;
using Systems.Performance;

namespace Tests.Core.Performance
{
    public class InputLatencyMonitorTests
    {
        [Test]
        public void RecordProcessed_ComputesLatencyFromRecordedInput()
        {
            var monitor = new InputLatencyMonitor();
            monitor.RecordInput(1.0);
            monitor.RecordProcessed(1.05);

            Assert.That(monitor.CurrentLatency, Is.EqualTo(0.05f).Within(0.0001f));
        }

        [Test]
        public void RecordProcessed_WithoutPrecedingInput_IsIgnored()
        {
            var monitor = new InputLatencyMonitor();
            monitor.RecordProcessed(1.0);

            Assert.That(monitor.CurrentLatency, Is.EqualTo(0f));
        }

        [Test]
        public void AverageLatency_ComputesAverageOfSamples()
        {
            var monitor = new InputLatencyMonitor();
            monitor.RecordInput(0.0);
            monitor.RecordProcessed(0.02);
            monitor.RecordInput(1.0);
            monitor.RecordProcessed(1.04);

            Assert.That(monitor.AverageLatency, Is.EqualTo(0.03f).Within(0.0001f));
        }
    }
}
