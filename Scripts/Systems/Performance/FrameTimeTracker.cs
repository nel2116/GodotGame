namespace Systems.Performance
{
    /// <summary>
    /// フレーム時間を記録し、目標FPSに対する予算超過を判定するクラス
    /// </summary>
    public class FrameTimeTracker
    {
        /// <summary>
        /// 目標フレーム時間（秒）
        /// </summary>
        public float TargetFrameTime { get; }

        /// <summary>
        /// 直近のフレーム時間（秒）
        /// </summary>
        public float CurrentFrameTime { get; private set; }

        /// <summary>
        /// 直近のフレーム時間から算出したFPS
        /// </summary>
        public float CurrentFps => CurrentFrameTime > 0f ? 1f / CurrentFrameTime : 0f;

        public FrameTimeTracker(float targetFps = 60f)
        {
            TargetFrameTime = 1f / targetFps;
        }

        /// <summary>
        /// フレーム時間を記録する
        /// </summary>
        public void RecordFrame(float deltaSeconds)
        {
            CurrentFrameTime = deltaSeconds;
        }

        /// <summary>
        /// 目標フレーム時間内に収まっているか判定する
        /// </summary>
        public bool IsWithinBudget()
        {
            return CurrentFrameTime <= TargetFrameTime;
        }
    }
}
