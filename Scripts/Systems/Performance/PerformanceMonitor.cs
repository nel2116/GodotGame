using Core.Events;

namespace Systems.Performance
{
    /// <summary>
    /// フレーム時間・入力遅延のKPI監視を行うクラス
    /// </summary>
    public class PerformanceMonitor
    {
        /// <summary>
        /// 企画仕様: 60FPS維持のための目標フレーム時間（秒）
        /// </summary>
        public const float TargetFrameTime = 1f / 60f;

        /// <summary>
        /// 企画仕様: 入力遅延の上限（秒）
        /// </summary>
        public const float MaxInputLatency = 0.10f;

        private readonly FrameTimeTracker _frameTimeTracker;
        private readonly InputLatencyMonitor _inputLatencyMonitor;
        private readonly IGameEventBus _eventBus;

        public FrameTimeTracker FrameTimeTracker => _frameTimeTracker;
        public InputLatencyMonitor InputLatencyMonitor => _inputLatencyMonitor;

        public PerformanceMonitor(FrameTimeTracker frameTimeTracker, InputLatencyMonitor inputLatencyMonitor, IGameEventBus eventBus)
        {
            _frameTimeTracker = frameTimeTracker;
            _inputLatencyMonitor = inputLatencyMonitor;
            _eventBus = eventBus;
        }

        /// <summary>
        /// KPIチェックを行い、閾値超過時に警告イベントを発行する
        /// </summary>
        public void Update()
        {
            CheckFrameTimeKpi();
            CheckInputLatencyKpi();
        }

        private void CheckFrameTimeKpi()
        {
            var frameTime = _frameTimeTracker.CurrentFrameTime;
            if (frameTime > TargetFrameTime)
            {
                _eventBus.Publish(new PerformanceWarningEvent("FrameTime", frameTime, TargetFrameTime));
            }
        }

        private void CheckInputLatencyKpi()
        {
            var latency = _inputLatencyMonitor.CurrentLatency;
            if (latency > MaxInputLatency)
            {
                _eventBus.Publish(new PerformanceWarningEvent("InputLatency", latency, MaxInputLatency));
            }
        }
    }
}
