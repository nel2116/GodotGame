using Systems.Player.State;

namespace Systems.Player.Combat
{
    /// <summary>
    /// 無敵時間を管理するクラス
    /// </summary>
    public class InvincibilityManager
    {
        private readonly FrameStateManager _frame_manager;

        public InvincibilityManager(FrameStateManager frameManager)
        {
            _frame_manager = frameManager;
        }

        /// <summary>
        /// 現在のフレームが無敵区間内か判定する
        /// </summary>
        public bool IsCurrentlyInvincible()
        {
            var action = _frame_manager.CurrentAction;
            if (action == null) return false;
            return action.IsInvincible(_frame_manager.CurrentFrame);
        }
    }
}
