using System;
using System.Collections.Generic;
using System.Linq;

namespace Systems.Player.State
{
    /// <summary>
    /// アクションのフレームデータを管理するクラス
    /// </summary>
    public class ActionFrameData
    {
        /// <summary>
        /// 無敵区間が設定されていないことを示す値
        /// </summary>
        public const int NoInvincibility = -1;

        public string ActionName { get; }
        public int StartFrame { get; private set; }
        public int TotalFrames { get; }
        public int StartupFrames { get; }
        public int ActiveFrames { get; }
        public int RecoveryFrames { get; }

        /// <summary>
        /// 無敵区間の開始オフセット（アクション開始からの相対フレーム）
        /// </summary>
        public int InvincibilityStartFrame { get; }

        /// <summary>
        /// 無敵区間の終了オフセット（アクション開始からの相対フレーム）
        /// </summary>
        public int InvincibilityEndFrame { get; }

        /// <summary>
        /// アクションによる移動距離
        /// </summary>
        public float MovementDistance { get; }

        /// <summary>
        /// 空中制御率（0.0〜1.0を想定）
        /// </summary>
        public float AirControlRate { get; }

        /// <summary>
        /// キャンセル可能な遷移先アクション名の一覧（企画上の仕様データ）
        /// </summary>
        public IReadOnlyList<string> CancelableTo { get; }

        public ActionFrameData(
            string actionName,
            int totalFrames,
            int startupFrames,
            int activeFrames,
            int recoveryFrames,
            int invincibilityStartFrame = NoInvincibility,
            int invincibilityEndFrame = NoInvincibility,
            float movementDistance = 0f,
            float airControlRate = 0f,
            IEnumerable<string>? cancelableTo = null)
        {
            ActionName = actionName;
            StartFrame = 0;
            TotalFrames = totalFrames;
            StartupFrames = startupFrames;
            ActiveFrames = activeFrames;
            RecoveryFrames = recoveryFrames;
            InvincibilityStartFrame = invincibilityStartFrame;
            InvincibilityEndFrame = invincibilityEndFrame;
            MovementDistance = movementDistance;
            AirControlRate = airControlRate;
            CancelableTo = (cancelableTo ?? Enumerable.Empty<string>()).ToList();
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

        /// <summary>
        /// 無敵区間内か判定する
        /// </summary>
        public bool IsInvincible(int currentFrame)
        {
            if (InvincibilityStartFrame == NoInvincibility || InvincibilityEndFrame == NoInvincibility)
            {
                return false;
            }

            var offset = currentFrame - StartFrame;
            return offset >= InvincibilityStartFrame && offset <= InvincibilityEndFrame;
        }
    }
}
