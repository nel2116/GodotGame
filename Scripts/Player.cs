using System;
using Core.Events;
using Core.Utilities;
using Godot;
using Systems.Player.Animation;
using Systems.Player.Combat;
using Systems.Player.Config;
using Systems.Player.Debug;
using Systems.Player.Input;
using Systems.Player.Movement;
using Systems.Player.Progression;
using Systems.Player.State;

public partial class Player : CharacterBody3D
{
	private IGameEventBus _eventBus = default!;
	private PlayerInputViewModel _inputViewModel = default!;
	private PlayerMovementViewModel _movementViewModel = default!;
	private PlayerCombatViewModel _combatViewModel = default!;
	private PlayerAnimationViewModel _animationViewModel = default!;
	private PlayerStateViewModel _stateViewModel = default!;
	private PlayerProgressionViewModel _progressionViewModel = default!;
	private PlayerDebugger? _debugger;
	private bool _hasInitialized;

	/// <summary>
	/// 入力設定とプレイヤーサブシステムを初期化する。
	/// 初期化に失敗した場合は例外を再スローし、ゲームを続行できない状態にする。
	/// </summary>
	public override void _Ready()
	{
		base._Ready();

		PlayerInputConfig.Initialize();
		_eventBus = GameEventBus.Instance;

		try
		{
			InitializeViewModels();
			InitializeDebugger();
			_hasInitialized = true;
		}
		catch (Exception exception)
		{
			LogInitializationFailure(exception);
			throw;
		}
	}

	/// <summary>
	/// 各ビューモデルを生成し初期化する。
	/// 初期化順序は依存関係に基づいて決定される（入力→移動→戦闘→アニメーション→状態→進行度）。
	/// </summary>
	private void InitializeViewModels()
	{
		_inputViewModel = CreateViewModel(
			() => new PlayerInputModel(),
			static (model, bus) => new PlayerInputViewModel(model, bus),
			static viewModel => viewModel.Initialize());

		_movementViewModel = CreateViewModel(
			() => new PlayerMovementModel(_eventBus),
			static (model, bus) => new PlayerMovementViewModel(model, bus),
			static viewModel => viewModel.Initialize());

		_combatViewModel = CreateViewModel(
			() => new PlayerCombatModel(_eventBus),
			static (model, bus) => new PlayerCombatViewModel(model, bus),
			static viewModel => viewModel.Initialize());

		_animationViewModel = CreateViewModel(
			() => new PlayerAnimationModel(_eventBus),
			static (model, bus) => new PlayerAnimationViewModel(model, bus),
			static viewModel => viewModel.Initialize());

		_stateViewModel = CreateViewModel(
			() => new PlayerStateModel(_eventBus),
			static (model, bus) => new PlayerStateViewModel(model, bus),
			static viewModel => viewModel.Initialize());

		_progressionViewModel = CreateViewModel(
			() => new PlayerProgressionModel(),
			static (model, bus) => new PlayerProgressionViewModel(model, bus),
			static viewModel => viewModel.Initialize());
	}

	/// <summary>
	/// デバッグ機能を準備し、ビルド構成に応じて有効化する。
	/// </summary>
	private void InitializeDebugger()
	{
		_debugger = new PlayerDebugger(_inputViewModel, _movementViewModel);
		_debugger.SetEnabled(OS.IsDebugBuild());
	}

	/// <summary>
	/// 各サブシステムを更新し、計算結果を物理ボディに適用する。
	/// 更新順序：入力 → 移動 → ゲームプレイシステム → 物理適用 → 接地状態更新 → デバッグ情報出力
	/// </summary>
	public override void _PhysicsProcess(double delta)
	{
		if (!_hasInitialized)
		{
			return;
		}

		UpdateInputSystem();
		UpdateMovementSystem();
		UpdateGameplaySystems();
		ApplyMovementToBody();
		UpdateGroundedState();
		PrintDebugInformation();
	}

	/// <summary>
	/// 入力システムを更新し、最新の入力状態を反映する。
	/// </summary>
	private void UpdateInputSystem()
	{
		_inputViewModel.UpdateInput();
	}

	/// <summary>
	/// 移動システムを更新し、速度と状態を計算する。
	/// </summary>
	private void UpdateMovementSystem()
	{
		_movementViewModel.UpdateMovement();
	}

	/// <summary>
	/// 戦闘・アニメーション・状態・進行度の各システムを更新する。
	/// </summary>
	private void UpdateGameplaySystems()
	{
		_combatViewModel.Update();
		_animationViewModel.Update();
		_stateViewModel.UpdateState();
		_progressionViewModel.Update();
	}

	/// <summary>
	/// モデルの速度を CharacterBody3D に転写して移動させる。
	/// 平面速度（XZ）と垂直速度（Y）を組み合わせて3D速度ベクトルを構築する。
	/// </summary>
	private void ApplyMovementToBody()
	{
		var horizontalVelocity = _movementViewModel.Velocity.Value;
		var verticalVelocity = _movementViewModel.Model.VerticalVelocity;

		Velocity = new Vector3(horizontalVelocity.X, verticalVelocity, horizontalVelocity.Y);
		MoveAndSlide();
	}

	/// <summary>
	/// Godot の接地判定をビューモデルへ反映する。
	/// CharacterBody3D の IsOnFloor() を使用して接地状態を判定し、モデルとビューモデルに同期する。
	/// </summary>
	private void UpdateGroundedState()
	{
		var isGrounded = IsOnFloor();
		_movementViewModel.Model.SetGroundedState(isGrounded);
		_movementViewModel.IsGrounded.Value = isGrounded;
	}

	/// <summary>
	/// デバッグビルド時のみ、デバッグ情報を出力する。
	/// </summary>
	private void PrintDebugInformation()
	{
		_debugger?.PrintDebugInfo();
	}

	/// <summary>
	/// ノード削除時にリソースを解放する。
	/// </summary>
	public override void _ExitTree()
	{
		DisposeViewModels();
		base._ExitTree();
	}

	/// <summary>
	/// 作成済みビューモデルを破棄する。
	/// </summary>
	private void DisposeViewModels()
	{
		_inputViewModel?.Dispose();
		_movementViewModel?.Dispose();
		_combatViewModel?.Dispose();
		_animationViewModel?.Dispose();
		_stateViewModel?.Dispose();
		_progressionViewModel?.Dispose();
	}

	/// <summary>
	/// 初期化失敗の詳細をログに残す。
	/// </summary>
	private static void LogInitializationFailure(Exception exception)
	{
		if (!GodotMock.IsTestEnvironment())
		{
			GD.PrintErr($"Failed to initialize player systems: {exception.Message}");
		}
	}

	/// <summary>
	/// モデル生成から初期化までをまとめるファクトリメソッド。
	/// 各 ViewModel の生成パターンを統一し、初期化順序を保証する。
	/// </summary>
	private TViewModel CreateViewModel<TModel, TViewModel>(
		Func<TModel> modelFactory,
		Func<TModel, IGameEventBus, TViewModel> viewModelFactory,
		Action<TViewModel> initializer)
	{
		if (_eventBus == null)
		{
			throw new InvalidOperationException("Event bus must be initialized before creating view models.");
		}

		var model = modelFactory();
		var viewModel = viewModelFactory(model, _eventBus);
		initializer(viewModel);
		return viewModel;
	}
}
