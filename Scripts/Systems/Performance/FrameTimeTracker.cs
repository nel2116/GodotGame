using System.Collections.Generic;
using System.Linq;

namespace Systems.Performance
{
    /// <summary>
    /// フレーム時間を記録し、現在値・平均値を提供するクラス
    /// </summary>
    public class FrameTimeTracker
    {
        private readonly Queue<float> _samples = new();
        private readonly int _maxSamples;

        /// <summary>
        /// 直近に記録されたフレーム時間（秒）
        /// </summary>
        public float CurrentFrameTime { get; private set; }

        /// <summary>
        /// 記録済みサンプルの平均フレーム時間（秒）
        /// </summary>
        public float AverageFrameTime => _samples.Count > 0 ? _samples.Average() : 0f;

        public FrameTimeTracker(int maxSamples = 60)
        {
            _maxSamples = maxSamples;
        }

        /// <summary>
        /// フレーム時間を記録する
        /// </summary>
        public void RecordFrameTime(float deltaSeconds)
        {
            CurrentFrameTime = deltaSeconds;
            _samples.Enqueue(deltaSeconds);
            if (_samples.Count > _maxSamples)
            {
                _samples.Dequeue();
            }
        }
    }
}
