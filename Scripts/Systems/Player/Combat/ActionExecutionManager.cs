using Core.Events;
using Systems.Player.Events;
using Systems.Player.State;

namespace Systems.Player.Combat
{
    /// <summary>
    /// アクション実行を管理するクラス
    /// </summary>
    public class ActionExecutionManager
    {
        private readonly FrameStateManager _frame_manager;
        private readonly CancelRuleManager _cancel_manager;
        private readonly IGameEventBus _event_bus;

        public ActionExecutionManager(FrameStateManager frameManager, CancelRuleManager cancelManager, IGameEventBus bus)
        {
            _frame_manager = frameManager;
            _cancel_manager = cancelManager;
            _event_bus = bus;
        }

        /// <summary>
        /// アクションを実行する
        /// </summary>
        public void ExecuteAction(ActionFrameData data)
        {
            // FrameStateManager がイベントを送信するためここでは実行のみ行う
            _frame_manager.StartAction(data);
        }

        /// <summary>
        /// 現在のアクションをキャンセルして新しいアクションを実行
        /// </summary>
        public bool TryCancel(ActionFrameData newAction)
        {
            if (_cancel_manager.CanCancel(newAction.ActionName))
            {
                _event_bus.Publish(new ActionCanceledEvent(newAction.ActionName));
                _frame_manager.StartAction(newAction);
                return true;
            }
            return false;
        }
    }
}
