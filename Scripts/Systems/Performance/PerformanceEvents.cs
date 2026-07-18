using Core.Events;

namespace Systems.Performance
{
    /// <summary>
    /// パフォーマンスKPI閾値超過を通知する警告イベント
    /// </summary>
    public class PerformanceWarningEvent : GameEvent
    {
        /// <summary>
        /// 閾値を超過した指標名（例: "FrameTime", "InputLatency"）
        /// </summary>
        public string MetricName { get; }

        /// <summary>
        /// 実測値
        /// </summary>
        public float ActualValue { get; }

        /// <summary>
        /// 許容閾値
        /// </summary>
        public float ThresholdValue { get; }

        public PerformanceWarningEvent(string metricName, float actualValue, float thresholdValue)
        {
            MetricName = metricName;
            ActualValue = actualValue;
            ThresholdValue = thresholdValue;
        }
    }
}
