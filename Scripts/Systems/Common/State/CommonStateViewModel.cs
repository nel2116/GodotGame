using System.Collections.Generic;
using Core.Events;
using Core.Reactive;
using Core.ViewModels;
using Systems.Common.Events;

namespace Systems.Common.State
{
    /// <summary>
    /// 共通状態システムのビューモデル
    /// </summary>
    public class CommonStateViewModel : ViewModelBase
    {
        private readonly CommonStateModel _model;
        public ReactiveProperty<string> CurrentState { get; }
        public ReactiveProperty<Dictionary<string, string>> StateTransitions { get; }

        public CommonStateViewModel(CommonStateModel model, IGameEventBus bus)
            : base(bus)
        {
            _model = model;
            CurrentState = new ReactiveProperty<string>().AddTo(Disposables);
            StateTransitions = new ReactiveProperty<Dictionary<string, string>>().AddTo(Disposables);

            CurrentState.Subscribe(OnStateChanged).AddTo(Disposables);
        }

        /// <summary>
        /// 初期化処理
        /// </summary>
        public void Initialize()
        {
            _model.Initialize();
            UpdateStateInfo();
        }

        /// <summary>
        /// 更新処理
        /// </summary>
        public void UpdateState()
        {
            _model.Update();
            UpdateStateInfo();
        }

        /// <summary>
        /// 状態変更
        /// </summary>
        public void ChangeState(string newState)
        {
            _model.ChangeState(newState);
            CurrentState.Value = _model.CurrentState;
        }

        /// <summary>
        /// 遷移可能か
        /// </summary>
        public bool CanTransitionTo(string newState)
        {
            return _model.CanTransitionTo(newState);
        }

        private void UpdateStateInfo()
        {
            CurrentState.Value = _model.CurrentState;
            StateTransitions.Value = _model.StateTransitions;
        }

        private void OnStateChanged(string state)
        {
            EventBus.Publish(new StateChangedEvent(state));
        }
    }
}
