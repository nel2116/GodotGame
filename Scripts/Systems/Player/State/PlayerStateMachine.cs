using System;
using System.Reactive.Linq;
using Core.Events;
using Systems.Common.Events;
using Systems.Player.Events;

namespace Systems.Player.State
{
    /// <summary>
    /// プレイヤーの状態遷移を管理するクラス
    /// </summary>
    public class PlayerStateMachine
    {
        private readonly FrameStateManager _frame_manager;
        private readonly IGameEventBus _event_bus;
        private string _current_state = "Idle";

        public string CurrentState => _current_state;

        public PlayerStateMachine(FrameStateManager frameManager, IGameEventBus eventBus)
        {
            _frame_manager = frameManager;
            _event_bus = eventBus;
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        public void Initialize()
        {
            _event_bus.GetEventStream<ActionEndedEvent>()
                .Subscribe(OnActionEnded);
            _event_bus.Publish(new StateChangedEvent(_current_state));
        }

        /// <summary>
        /// 状態を更新する
        /// </summary>
        public void Update()
        {
            _frame_manager.Tick();
        }

        /// <summary>
        /// アクション開始
        /// </summary>
        public void StartAction(ActionFrameData data)
        {
            _current_state = data.ActionName;
            _frame_manager.StartAction(data);
            _event_bus.Publish(new StateChangedEvent(_current_state));
        }

        private void OnActionEnded(ActionEndedEvent evt)
        {
            _current_state = "Idle";
            _event_bus.Publish(new StateChangedEvent(_current_state));
        }
    }
}
