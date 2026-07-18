using Godot;
using Core.Events;

namespace Systems.Performance
{
    /// <summary>
    /// GodotのフレームAPIからパフォーマンス監視ロジックへ実測値を橋渡しするノード
    /// </summary>
    public partial class PerformanceMonitorNode : Node
    {
        private FrameTimeTracker? _frameTimeTracker;
        private InputLatencyMonitor? _inputLatencyMonitor;
        private PerformanceMonitor? _performanceMonitor;

        /// <summary>
        /// 監視ロジックを初期化する
        /// </summary>
        public void Initialize(IGameEventBus eventBus)
        {
            _frameTimeTracker = new FrameTimeTracker();
            _inputLatencyMonitor = new InputLatencyMonitor();
            _performanceMonitor = new PerformanceMonitor(_frameTimeTracker, _inputLatencyMonitor, eventBus);
        }

        public override void _Process(double delta)
        {
            if (_frameTimeTracker == null || _performanceMonitor == null) return;

            _frameTimeTracker.RecordFrameTime((float)delta);
            _performanceMonitor.Update();
        }

        /// <summary>
        /// 入力受信時刻を記録する
        /// </summary>
        public void RecordInputReceived()
        {
            _inputLatencyMonitor?.RecordInput(Time.GetTicksMsec() / 1000.0);
        }

        /// <summary>
        /// 入力処理完了時刻を記録する
        /// </summary>
        public void RecordInputProcessed()
        {
            _inputLatencyMonitor?.RecordProcessed(Time.GetTicksMsec() / 1000.0);
        }
    }
}
