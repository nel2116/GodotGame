using Systems.Player.State;

namespace Systems.Player.Combat
{
    /// <summary>
    /// 無敵時間を管理するクラス
    /// </summary>
    public class InvincibilityManager
    {
        private readonly FrameStateManager _frame_manager;
        private bool _forced_invincible;

        public InvincibilityManager(FrameStateManager frameManager)
        {
            _frame_manager = frameManager;
        }

        /// <summary>
        /// 現在無敵中か判定する（実行中アクションの無敵区間、または強制無敵状態）
        /// </summary>
        public bool IsInvincible()
        {
            if (_forced_invincible) return true;

            var action = _frame_manager.CurrentAction;
            return action != null && action.IsInvincible(_frame_manager.CurrentFrame);
        }

        /// <summary>
        /// アクションとは独立した強制無敵状態を設定する（無敵アイテム等で使用）
        /// </summary>
        public void SetForcedInvincible(bool value)
        {
            _forced_invincible = value;
        }
    }
}
