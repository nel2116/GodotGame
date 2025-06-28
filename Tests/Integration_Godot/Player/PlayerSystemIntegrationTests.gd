extends GutTest

# PlayerSystemIntegrationTests.gd
# GUT版のプレイヤーシステム統合テスト

var event_bus: GameEventBus
var input_view_model: PlayerInputViewModel
var movement_view_model: PlayerMovementViewModel
var combat_view_model: PlayerCombatViewModel
var animation_view_model: PlayerAnimationViewModel
var state_view_model: PlayerStateViewModel
var progression_view_model: PlayerProgressionViewModel

func before_each():
	event_bus = GameEventBus.new()
	initialize_view_models()

func after_each():
	input_view_model.free()
	movement_view_model.free()
	combat_view_model.free()
	animation_view_model.free()
	state_view_model.free()
	progression_view_model.free()
	event_bus.free()

func initialize_view_models():
	var input_model = PlayerInputModel.new(event_bus)
	input_view_model = PlayerInputViewModel.new(input_model, event_bus)
	input_view_model.initialize()

	var movement_model = PlayerMovementModel.new(event_bus)
	movement_view_model = PlayerMovementViewModel.new(movement_model, event_bus)
	movement_view_model.initialize()

	var combat_model = PlayerCombatModel.new(event_bus)
	combat_view_model = PlayerCombatViewModel.new(combat_model, event_bus)
	combat_view_model.initialize()

	var animation_model = PlayerAnimationModel.new(event_bus)
	animation_view_model = PlayerAnimationViewModel.new(animation_model, event_bus)
	animation_view_model.initialize()

	var state_model = PlayerStateModel.new(event_bus)
	state_view_model = PlayerStateViewModel.new(state_model, event_bus)
	state_view_model.initialize()

	var progression_model = PlayerProgressionModel.new()
	progression_view_model = PlayerProgressionViewModel.new(progression_model, event_bus)
	progression_view_model.initialize()

func test_system_initialization_all_components_initialized():
	# 検証
	assert_not_null(input_view_model)
	assert_not_null(movement_view_model)
	assert_not_null(combat_view_model)
	assert_not_null(animation_view_model)
	assert_not_null(state_view_model)
	assert_not_null(progression_view_model)

func test_input_to_movement_integration():
	# 実行
	input_view_model.update_input()
	movement_view_model.update_movement()
	
	# 検証（エラーが発生しないことを確認）
	# GUTでは例外が発生した場合自動的にテストが失敗する

func test_movement_to_animation_integration():
	# 実行
	movement_view_model.update_movement()
	animation_view_model.update()
	
	# 検証（エラーが発生しないことを確認）

func test_combat_to_state_integration():
	# 実行
	combat_view_model.attack("BasicAttack")
	state_view_model.update_state()
	
	# 検証（エラーが発生しないことを確認）

func test_progression_to_combat_integration():
	# 実行
	progression_view_model.add_experience(100)
	progression_view_model.update()
	combat_view_model.update_combat()
	
	# 検証（エラーが発生しないことを確認）

func test_full_system_update_integration():
	# 実行
	for i in range(10):
		input_view_model.update_input()
		movement_view_model.update_movement()
		combat_view_model.update_combat()
		animation_view_model.update()
		state_view_model.update_state()
		progression_view_model.update()
	
	# 検証（エラーが発生しないことを確認）

func test_event_communication_integration():
	# 準備
	var event_received = false
	event_bus.get_event_stream("MovementVelocityChangedEvent").subscribe(
		func(event): event_received = true
	)
	
	# 実行
	movement_view_model.update_movement()
	
	# 少し待機してイベント処理を完了させる
	await get_tree().process_frame
	
	# 検証
	assert_true(event_received, "Movement event should be published")

func test_error_handling_integration():
	# 準備
	var error_received = false
	event_bus.get_event_stream("ErrorEvent").subscribe(
		func(event): error_received = true
	)
	
	# 実行
	combat_view_model.attack("InvalidAction")
	
	# 少し待機してイベント処理を完了させる
	await get_tree().process_frame
	
	# 検証
	assert_true(error_received, "Error event should be published")

func test_performance_integration():
	# 実行
	for i in range(1000):
		input_view_model.update_input()
		movement_view_model.update_movement()
		combat_view_model.update_combat()
		animation_view_model.update()
		state_view_model.update_state()
		progression_view_model.update()
	
	# 検証（エラーが発生しないことを確認）

func test_memory_usage_integration():
	# 実行
	for i in range(100):
		input_view_model.update_input()
		movement_view_model.update_movement()
		combat_view_model.update_combat()
		animation_view_model.update()
		state_view_model.update_state()
		progression_view_model.update()
	
	# 検証（エラーが発生しないことを確認） 