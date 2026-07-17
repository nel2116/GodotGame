using System.Collections.Generic;
using System.Linq;

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

        /// <summary>
        /// 無敵区間の開始オフセット（アクション開始フレームからの相対値、-1は無敵なし）
        /// </summary>
        public int InvincibilityStartFrame { get; }

        /// <summary>
        /// 無敵区間の終了オフセット（アクション開始フレームからの相対値、-1は無敵なし）
        /// </summary>
        public int InvincibilityEndFrame { get; }

        /// <summary>
        /// アクションによる移動距離（メートル）
        /// </summary>
        public float MovementDistance { get; }

        /// <summary>
        /// 空中制御率（0.0〜1.0、地上アクションは1.0）
        /// </summary>
        public float AirControlRate { get; }

        /// <summary>
        /// キャンセル可能な遷移先アクション名の一覧
        /// </summary>
        public IReadOnlyList<string> CancelableTo { get; }

        /// <summary>
        /// キャンセル優先度（値が大きいほど優先）
        /// </summary>
        public int Priority { get; }

        public ActionFrameData(
            string actionName,
            int totalFrames,
            int startupFrames,
            int activeFrames,
            int recoveryFrames,
            int invincibilityStartFrame = -1,
            int invincibilityEndFrame = -1,
            float movementDistance = 0f,
            float airControlRate = 1f,
            IEnumerable<string>? cancelableTo = null,
            int priority = 0)
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
            Priority = priority;
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
        /// 無敵フレームか判定する
        /// </summary>
        public bool IsInvincible(int currentFrame)
        {
            if (InvincibilityStartFrame < 0 || InvincibilityEndFrame < 0) return false;
            var offset = currentFrame - StartFrame;
            return offset >= InvincibilityStartFrame && offset <= InvincibilityEndFrame;
        }

        /// <summary>
        /// 指定アクションへキャンセル可能な遷移先として登録されているか判定する
        /// </summary>
        public bool CanCancelTo(string actionName)
        {
            return CancelableTo.Contains(actionName);
        }
    }
}
