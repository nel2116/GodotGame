using System;

namespace Systems.Player.State
{
    /// <summary>
    /// アクションのフレームデータを管理するクラス
    /// </summary>
    public class ActionFrameData
    {
        public string ActionName { get; }
        public int StartFrame { get; private set; }
        public int TotalFrames { get; }
        public int StartupFrames { get; }
        public int ActiveFrames { get; }
        public int RecoveryFrames { get; }

        public ActionFrameData(string actionName, int startFrame, int totalFrames, int startupFrames, int activeFrames, int recoveryFrames)
        {
            ActionName = actionName;
            StartFrame = startFrame;
            TotalFrames = totalFrames;
            StartupFrames = startupFrames;
            ActiveFrames = activeFrames;
            RecoveryFrames = recoveryFrames;
        }

        /// <summary>
        /// 開始フレームを設定する
        /// </summary>
        public void SetStartFrame(int frame)
        {
            StartFrame = frame;
        }

        /// <summary>
        /// スタートアップフレームか判定する
        /// </summary>
        public bool IsStartup(int currentFrame)
        {
            var offset = currentFrame - StartFrame;
            return offset >= 0 && offset < StartupFrames;
        }

        /// <summary>
        /// アクティブフレームか判定する
        /// </summary>
        public bool IsActive(int currentFrame)
        {
            var offset = currentFrame - StartFrame;
            return offset >= StartupFrames && offset < StartupFrames + ActiveFrames;
        }

        /// <summary>
        /// リカバリーフレームか判定する
        /// </summary>
        public bool IsRecovery(int currentFrame)
        {
            var offset = currentFrame - StartFrame;
            return offset >= StartupFrames + ActiveFrames && offset < TotalFrames;
        }
    }
}
