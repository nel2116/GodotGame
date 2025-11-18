using Core.Events;
using Core.Reactive;
using Core.ViewModels;
using Systems.Player.Events;

namespace Systems.Player.Input
{
	/// <summary>
	/// プレイヤー入力ビューモデル
	/// </summary>
	public class PlayerInputViewModel : ViewModelBase
	{
		private readonly PlayerInputModel _model;
		private InputState? _lastSnapshot;
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
		/// 初期化処理
		/// </summary>
		public void Initialize()
		{
			_model.Initialize();
			UpdateInputState();
		}

		/// <summary>
		/// 入力更新
		/// </summary>
		public void UpdateInput()
		{
			_model.UpdateInput();
			UpdateInputState();
		}

		private void UpdateInputState()
		{
			var snapshot = _model.CurrentState.Clone();
			if (_lastSnapshot != null && _lastSnapshot.IsEquivalentTo(snapshot))
			{
				IsEnabled.Value = _model.IsEnabled;
				return;
			}

			_lastSnapshot = snapshot;
			CurrentState.Value = snapshot;
			IsEnabled.Value = _model.IsEnabled;
		}

		private void OnInputStateChanged(InputState state)
		{
			EventBus.Publish(new InputStateChangedEvent(state));
		}

		private void OnEnabledChanged(bool enabled)
		{
			EventBus.Publish(new InputEnabledChangedEvent(enabled));
		}
	}
}
