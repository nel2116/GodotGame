using Core.Events;
using Core.Reactive;
using Core.ViewModels;
using Systems.Player.Events;
using Systems.Common.Events;

namespace Systems.Player.State
{
    /// <summary>
    /// プレイヤー状態ビューモデル
    /// </summary>
    public class PlayerStateViewModel : ViewModelBase
    {
        private readonly PlayerStateModel _model;
        private readonly ReactiveProperty<string> _current_state;
        private readonly ReactiveProperty<bool> _can_change_state;

        public ReactiveProperty<string> CurrentState => _current_state;
        public ReactiveProperty<bool> CanChangeState => _can_change_state;

        public PlayerStateViewModel(PlayerStateModel model, IGameEventBus bus)
            : base(bus)
        {
            _model = model;
            _current_state = new ReactiveProperty<string>().AddTo(Disposables);
            _can_change_state = new ReactiveProperty<bool>().AddTo(Disposables);

            _current_state.Subscribe(OnStateChanged).AddTo(Disposables);
            _can_change_state.Subscribe(OnCanChangeStateChanged).AddTo(Disposables);
        }

        public void Initialize()
        {
            _model.Initialize();
            UpdateStateDisplay();
        }

        public void UpdateState()
        {
            _model.Update();
            UpdateStateDisplay();
        }

        public void HandleStateChange(string newState)
        {
            _model.ChangeState(newState);
        }

        private void UpdateStateDisplay()
        {
            _current_state.Value = _model.CurrentState;
            _can_change_state.Value = _model.CanChangeState;
        }

        private void OnStateChanged(string state)
        {
            EventBus.Publish(new StateChangedEvent(state));
        }

        private void OnCanChangeStateChanged(bool canChange)
        {
            EventBus.Publish(new CanChangeStateChangedEvent(canChange));
        }
    }
}
