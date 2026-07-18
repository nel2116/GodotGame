using System.Collections.Generic;
using System.Linq;

namespace Systems.Performance
{
    /// <summary>
    /// 入力から処理までの遅延を計測するクラス
    /// </summary>
    public class InputLatencyMonitor
    {
        private readonly Queue<float> _samples = new();
        private readonly int _maxSamples;
        private double? _pendingInputTimestamp;
        private bool _isFresh;

        /// <summary>
        /// 直近に計測された入力遅延（秒）
        /// </summary>
        public float CurrentLatency { get; private set; }

        /// <summary>
        /// 記録済みサンプルの平均入力遅延（秒）
        /// </summary>
        public float AverageLatency => _samples.Count > 0 ? _samples.Average() : 0f;

        public InputLatencyMonitor(int maxSamples = 60)
        {
            _maxSamples = maxSamples;
        }

        /// <summary>
        /// 入力受信時刻を記録する
        /// </summary>
        public void RecordInput(double timestampSeconds)
        {
            _pendingInputTimestamp = timestampSeconds;
        }

        /// <summary>
        /// 入力処理完了時刻を記録し、遅延を確定する
        /// </summary>
        public void RecordProcessed(double timestampSeconds)
        {
            if (_pendingInputTimestamp == null) return;

            var latency = (float)(timestampSeconds - _pendingInputTimestamp.Value);
            _pendingInputTimestamp = null;

            CurrentLatency = latency;
            _isFresh = true;
            _samples.Enqueue(latency);
            if (_samples.Count > _maxSamples)
            {
                _samples.Dequeue();
            }
        }

        /// <summary>
        /// 直近の計測結果を取り出す。前回の TakeLatency() 呼び出し以降に新しい計測がない場合は null を返す。
        /// KPI チェック側が「まだ確認していない新鮮なサンプル」だけを評価するために使用する
        /// </summary>
        public float? TakeLatency()
        {
            if (!_isFresh) return null;
            _isFresh = false;
            return CurrentLatency;
        }
    }
}
