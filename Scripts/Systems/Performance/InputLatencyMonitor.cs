using System;

namespace Systems.Performance
{
    /// <summary>
    /// 入力受付から処理完了までの遅延を計測するクラス
    /// </summary>
    public class InputLatencyMonitor
    {
        private readonly Func<DateTime> _now;
        private DateTime? _inputTimestamp;

        /// <summary>
        /// 直近に計測された入力遅延（秒）
        /// </summary>
        public float CurrentLatency { get; private set; }

        public InputLatencyMonitor(Func<DateTime>? nowProvider = null)
        {
            _now = nowProvider ?? (() => DateTime.UtcNow);
        }

        /// <summary>
        /// 入力受付時刻を記録する
        /// </summary>
        public void RecordInputReceived()
        {
            _inputTimestamp = _now();
        }

        /// <summary>
        /// 入力の処理完了を記録し、遅延を確定する
        /// </summary>
        public void RecordInputProcessed()
        {
            if (_inputTimestamp == null) return;
            CurrentLatency = (float)(_now() - _inputTimestamp.Value).TotalSeconds;
            _inputTimestamp = null;
        }

        /// <summary>
        /// 許容遅延以内か判定する（企画仕様: ≤ 0.10s）
        /// </summary>
        public bool IsWithinBudget(float maxLatencySeconds = 0.10f)
        {
            return CurrentLatency <= maxLatencySeconds;
        }
    }
}
