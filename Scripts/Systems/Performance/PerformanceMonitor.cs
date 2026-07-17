using Core.Events;

namespace Systems.Performance
{
    /// <summary>
    /// フレーム時間と入力遅延のKPIを監視し、超過時に警告イベントを発行するクラス
    /// </summary>
    public class PerformanceMonitor
    {
        private const float MAX_INPUT_LATENCY = 0.10f;

        private readonly IGameEventBus _event_bus;

        public FrameTimeTracker FrameTimeTracker { get; }
        public InputLatencyMonitor InputLatencyMonitor { get; }

        public PerformanceMonitor(
            IGameEventBus eventBus,
            FrameTimeTracker? frameTimeTracker = null,
            InputLatencyMonitor? inputLatencyMonitor = null)
        {
            _event_bus = eventBus;
            FrameTimeTracker = frameTimeTracker ?? new FrameTimeTracker();
            InputLatencyMonitor = inputLatencyMonitor ?? new InputLatencyMonitor();
        }

        /// <summary>
        /// フレーム時間を記録し、60FPS維持のKPIを確認する
        /// </summary>
        public void RecordFrame(float deltaSeconds)
        {
            FrameTimeTracker.RecordFrame(deltaSeconds);
            if (!FrameTimeTracker.IsWithinBudget())
            {
                _event_bus.Publish(new PerformanceWarningEvent(
                    "FrameTime", FrameTimeTracker.CurrentFrameTime, FrameTimeTracker.TargetFrameTime));
            }
        }

        /// <summary>
        /// 入力受付を記録する
        /// </summary>
        public void RecordInputReceived()
        {
            InputLatencyMonitor.RecordInputReceived();
        }

        /// <summary>
        /// 入力処理完了を記録し、入力遅延0.10s以下のKPIを確認する
        /// </summary>
        public void RecordInputProcessed()
        {
            InputLatencyMonitor.RecordInputProcessed();
            if (!InputLatencyMonitor.IsWithinBudget(MAX_INPUT_LATENCY))
            {
                _event_bus.Publish(new PerformanceWarningEvent(
                    "InputLatency", InputLatencyMonitor.CurrentLatency, MAX_INPUT_LATENCY));
            }
        }
    }
}
