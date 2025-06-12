using Godot;
using Core.Events;
using Systems.Player.Input;
using Systems.Player.Movement;
using Systems.Player.Combat;
using Systems.Player.Animation;
using Systems.Player.State;
using Systems.Player.Progression;
using Systems.Player.Config;
using Systems.Player.Debug;

public partial class Player : CharacterBody3D
{
	private GameEventBus _bus = default!;
	private PlayerInputViewModel _input_vm = default!;
	private PlayerMovementViewModel _movement_vm = default!;
	private PlayerCombatViewModel _combat_vm = default!;
	private PlayerAnimationViewModel _animation_vm = default!;
	private PlayerStateViewModel _state_vm = default!;
	private PlayerProgressionViewModel _progression_vm = default!;
	private PlayerDebugger _debugger = default!;

	// 初期化処理で各サブシステムを生成する
	public override void _Ready()
	{
		base._Ready();
		
		// InputMapの設定
		PlayerInputConfig.Initialize();

		_bus = new GameEventBus();

		try
		{
			InitializeViewModels();
			InitializeDebugger();
		}
		catch (System.Exception ex)
		{
			GD.PrintErr($"Failed to initialize player systems: {ex.Message}");
			// エラー発生時の適切な処理を追加
		}
	}

	private void InitializeViewModels()
	{
		var input_model = new PlayerInputModel(_bus);
		_input_vm = new PlayerInputViewModel(input_model, _bus);
		_input_vm.Initialize();

		var movement_model = new PlayerMovementModel(_bus);
		_movement_vm = new PlayerMovementViewModel(movement_model, _bus);
		_movement_vm.Initialize();

		var combat_model = new PlayerCombatModel(_bus);
		_combat_vm = new PlayerCombatViewModel(combat_model, _bus);
		_combat_vm.Initialize();

		var animation_model = new PlayerAnimationModel(_bus);
		_animation_vm = new PlayerAnimationViewModel(animation_model, _bus);
		_animation_vm.Initialize();

		var state_model = new PlayerStateModel(_bus);
		_state_vm = new PlayerStateViewModel(state_model, _bus);
		_state_vm.Initialize();

		var progression_model = new PlayerProgressionModel();
		_progression_vm = new PlayerProgressionViewModel(progression_model, _bus);
		_progression_vm.Initialize();
	}

	private void InitializeDebugger()
	{
		_debugger = new PlayerDebugger(_input_vm, _movement_vm);
		// デバッグモードの設定（開発時のみ有効）
		_debugger.SetEnabled(OS.IsDebugBuild());
	}

	// 毎フレーム各サブシステムを更新する
	public override void _PhysicsProcess(double delta)
	{
		// 入力の更新
		_input_vm.UpdateInput();
		
		// 移動の更新（deltaを使用して時間に基づいた移動を実装）
		_movement_vm.UpdateMovement();
		
		// その他のサブシステムの更新
		_combat_vm.Update();
		_animation_vm.Update();
		_state_vm.UpdateState();
		_progression_vm.Update();
		
		// 移動速度を適用（Vector2からVector3への変換＋垂直速度）
		var velocity2D = _movement_vm.Velocity.Value;
		var verticalVelocity = _movement_vm.Model.VerticalVelocity;
		Velocity = new Vector3(velocity2D.X, verticalVelocity, velocity2D.Y);
		
		// 移動と衝突判定の処理
		MoveAndSlide();
		
		// 接地状態の更新
		_movement_vm.IsGrounded.Value = IsOnFloor();

		// デバッグ情報の出力
		_debugger.PrintDebugInfo();
	}

	// ノード削除時にリソースを解放する
	public override void _ExitTree()
	{
		_input_vm.Dispose();
		_movement_vm.Dispose();
		_combat_vm.Dispose();
		_animation_vm.Dispose();
		_state_vm.Dispose();
		_progression_vm.Dispose();
		_bus.Dispose();
		base._ExitTree();
	}
}
