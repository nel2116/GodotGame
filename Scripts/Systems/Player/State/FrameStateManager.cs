using Core.Events;
using Systems.Player.Events;

namespace Systems.Player.State
{
    /// <summary>
    /// フレーム単位の状態管理を行うクラス
    /// </summary>
    public class FrameStateManager
    {
        private int _current_frame;
        private ActionFrameData? _current_action;
        private readonly IGameEventBus _event_bus;

        public int CurrentFrame => _current_frame;
        public ActionFrameData? CurrentAction => _current_action;

        public FrameStateManager(IGameEventBus eventBus)
        {
            _event_bus = eventBus;
        }

        /// <summary>
        /// フレームを進める
        /// </summary>
        public void Tick()
        {
            _current_frame++;
            _event_bus.Publish(new FrameAdvancedEvent(_current_frame));
            CheckActionEnd();
        }

        /// <summary>
        /// アクション開始
        /// </summary>
        public void StartAction(ActionFrameData data)
        {
            data.SetStartFrame(_current_frame);
            _current_action = data;
            _event_bus.Publish(new ActionStartedEvent(data.ActionName));
        }

        private void CheckActionEnd()
        {
            if (_current_action == null) return;
            if (_current_frame - _current_action.StartFrame >= _current_action.TotalFrames)
            {
                var name = _current_action.ActionName;
                _current_action = null;
                _event_bus.Publish(new ActionEndedEvent(name));
            }
        }

        /// <summary>
        /// キャンセル可能か判定する
        /// </summary>
        public bool IsInCancelableFrame(int start, int end)
        {
            if (_current_action == null) return false;
            var offset = _current_frame - _current_action.StartFrame;
            return offset >= start && offset <= end;
        }
    }
}
