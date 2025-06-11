using Godot;
using Core.Events;
using Systems.Player.Input;
using Systems.Player.Movement;
using Systems.Player.Combat;
using Systems.Player.Animation;
using Systems.Player.State;
using Systems.Player.Progression;

public partial class Player : CharacterBody3D
{
    private GameEventBus _bus = default!;
    private PlayerInputViewModel _input_vm = default!;
    private PlayerMovementViewModel _movement_vm = default!;
    private PlayerCombatViewModel _combat_vm = default!;
    private PlayerAnimationViewModel _animation_vm = default!;
    private PlayerStateViewModel _state_vm = default!;
    private PlayerProgressionViewModel _progression_vm = default!;

    // 初期化処理で各サブシステムを生成する
    public override void _Ready()
    {
        _bus = new GameEventBus();

        var input_model = new PlayerInputModel();
        _input_vm = new PlayerInputViewModel(input_model, _bus);
        _input_vm.Initialize();

        var movement_model = new PlayerMovementModel();
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

    // 毎フレーム各サブシステムを更新する
    public override void _PhysicsProcess(double delta)
    {
        _input_vm.UpdateInput();
        _movement_vm.UpdateMovement();
        _combat_vm.UpdateCombat();
        _state_vm.UpdateState();
        _animation_vm.UpdateAnimation();
        _progression_vm.UpdateProgression();
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
    }
}
