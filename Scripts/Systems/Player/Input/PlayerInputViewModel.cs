using Core.Events;
using Core.Reactive;
using Core.ViewModels;
using Systems.Player.Events;

namespace Systems.Player.Input
{
	/// <summary>
	/// プレイヤー入力ビューモデル。
	/// 入力状態の変更を検出し、変更時のみイベントを発行する。
	/// </summary>
	public class PlayerInputViewModel : ViewModelBase
	{
		private readonly PlayerInputModel _model;
		private InputState? _previousStateSnapshot;
		public ReactiveProperty<InputState> CurrentState { get; }
		public ReactiveProperty<bool> IsEnabled { get; }

		public PlayerInputViewModel(PlayerInputModel model, IGameEventBus bus)
			: base(bus)
		{
			_model = model;
			CurrentState = new ReactiveProperty<InputState>().AddTo(Disposables);
			IsEnabled = new ReactiveProperty<bool>().AddTo(Disposables);

			CurrentState.Subscribe(OnInputStateChanged).AddTo(Disposables);
			IsEnabled.Subscribe(OnEnabledChanged).AddTo(Disposables);
		}

		/// <summary>
		/// 入力システムを初期化し、初期状態を反映する。
		/// </summary>
		public void Initialize()
		{
			_model.Initialize();
			UpdateInputState();
		}

		/// <summary>
		/// 入力状態を更新し、変更があればイベントを発行する。
		/// </summary>
		public void UpdateInput()
		{
			_model.UpdateInput();
			UpdateInputState();
		}

		/// <summary>
		/// モデルの入力状態を取得し、前回の状態と比較して変更があれば通知する。
		/// 状態が同じ場合は IsEnabled のみ更新して早期リターンする。
		/// </summary>
		private void UpdateInputState()
		{
			var currentSnapshot = _model.CurrentState.Clone();
			var hasStateChanged = _previousStateSnapshot == null || !_previousStateSnapshot.IsEquivalentTo(currentSnapshot);

			if (!hasStateChanged)
			{
				IsEnabled.Value = _model.IsEnabled;
				return;
			}

			_previousStateSnapshot = currentSnapshot;
			CurrentState.Value = currentSnapshot;
			IsEnabled.Value = _model.IsEnabled;
		}

		/// <summary>
		/// 入力状態が変更されたときにイベントを発行する。
		/// </summary>
		/// <param name="state">新しい入力状態</param>
		private void OnInputStateChanged(InputState state)
		{
			EventBus.Publish(new InputStateChangedEvent(state));
		}

		/// <summary>
		/// 入力の有効/無効状態が変更されたときにイベントを発行する。
		/// </summary>
		/// <param name="enabled">入力が有効かどうか</param>
		private void OnEnabledChanged(bool enabled)
		{
			EventBus.Publish(new InputEnabledChangedEvent(enabled));
		}
	}
}
