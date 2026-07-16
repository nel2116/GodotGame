using Core.Events;
using Core.Reactive;
using Core.ViewModels;
using Systems.Player.Events;
using Systems.Common.Events;

namespace Systems.Player.State
{
    /// <summary>
    /// プレイヤー状態ビューモデル。
    /// 状態モデルの状態を View に公開し、変更時にイベントを発行する。
    /// </summary>
    public class PlayerStateViewModel : ViewModelBase
    {
        private readonly PlayerStateModel _model;
        private readonly ReactiveProperty<string> _currentStateName;
        private readonly ReactiveProperty<bool> _canChangeState;

        public ReactiveProperty<string> CurrentState => _currentStateName;
        public ReactiveProperty<bool> CanChangeState => _canChangeState;

        public PlayerStateViewModel(PlayerStateModel model, IGameEventBus bus)
            : base(bus)
        {
            _model = model;
            _currentStateName = new ReactiveProperty<string>().AddTo(Disposables);
            _canChangeState = new ReactiveProperty<bool>().AddTo(Disposables);

            _currentStateName.Subscribe(OnStateChanged).AddTo(Disposables);
            _canChangeState.Subscribe(OnCanChangeStateChanged).AddTo(Disposables);
        }

        /// <summary>
        /// 状態システムを初期化し、初期状態を反映する。
        /// </summary>
        public void Initialize()
        {
            _model.Initialize();
            UpdateStateDisplay();
        }

        /// <summary>
        /// 状態システムを更新し、最新の状態を反映する。
        /// </summary>
        public void UpdateState()
        {
            _model.Update();
            UpdateStateDisplay();
        }

        /// <summary>
        /// 状態変更を要求する。
        /// </summary>
        /// <param name="newState">新しい状態名</param>
        public void HandleStateChange(string newState)
        {
            _model.ChangeState(newState);
        }

        /// <summary>
        /// モデルの状態を取得し、ReactiveProperty に反映する。
        /// 変更時にイベントが自動的に発行される。
        /// </summary>
        private void UpdateStateDisplay()
        {
            _currentStateName.Value = _model.CurrentState;
            _canChangeState.Value = _model.CanChangeState;
        }

        /// <summary>
        /// 状態が変更されたときにイベントを発行する。
        /// </summary>
        /// <param name="state">新しい状態名</param>
        private void OnStateChanged(string state)
        {
            EventBus.Publish(new StateChangedEvent(state));
        }

        /// <summary>
        /// 状態変更可能フラグが変更されたときにイベントを発行する。
        /// </summary>
        /// <param name="canChange">状態変更が可能かどうか</param>
        private void OnCanChangeStateChanged(bool canChange)
        {
            EventBus.Publish(new CanChangeStateChangedEvent(canChange));
        }
    }
}
