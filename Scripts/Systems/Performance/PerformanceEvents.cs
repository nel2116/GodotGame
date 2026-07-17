using Core.Events;

namespace Systems.Performance
{
    /// <summary>
    /// パフォーマンスKPI超過を通知するイベント
    /// </summary>
    public class PerformanceWarningEvent : GameEvent
    {
        public string MetricName { get; }
        public float Value { get; }
        public float Threshold { get; }

        public PerformanceWarningEvent(string metricName, float value, float threshold)
        {
            MetricName = metricName;
            Value = value;
            Threshold = threshold;
        }
    }
}
