using System.Collections.Generic;
using System.Linq;
using Systems.Player.State;

namespace Systems.Player.Combat
{
    /// <summary>
    /// キャンセルルールを管理するクラス
    /// </summary>
    public class CancelRuleManager
    {
        private readonly List<CancelRule> _rules = new();
        private readonly FrameStateManager _frame_manager;

        public CancelRuleManager(FrameStateManager frameManager)
        {
            _frame_manager = frameManager;
        }

        /// <summary>
        /// ルールを初期化する
        /// </summary>
        public void InitializeDefaultRules()
        {
            _rules.Add(new CancelRule(
                "Attack_L1",
                14,
                20,
                new[] { "Dodge", "Attack_L2" },
                1));
            _rules.Add(new CancelRule(
                "Dodge",
                18,
                26,
                new[] { "Attack_L1", "ChargeAttack" },
                1));
        }

        /// <summary>
        /// キャンセル可能か判定する
        /// </summary>
        public bool CanCancel(string toAction)
        {
            if (_frame_manager.CurrentAction == null) return false;
            var fromAction = _frame_manager.CurrentAction.ActionName;
            var offset = _frame_manager.CurrentFrame - _frame_manager.CurrentAction.StartFrame;
            return _rules
                .Where(r => r.FromAction == fromAction)
                .Where(r => r.CanCancel(toAction, offset))
                .OrderByDescending(r => r.Priority)
                .Any();
        }
    }
}
