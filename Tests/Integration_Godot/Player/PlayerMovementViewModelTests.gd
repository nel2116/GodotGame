extends GutTest

# PlayerMovementViewModelTests.gd
# GUT版のプレイヤー移動ViewModelテスト

var event_bus: GameEventBus
var movement_model: PlayerMovementModel
var movement_view_model: PlayerMovementViewModel

func before_each():
	event_bus = GameEventBus.new()
	movement_model = PlayerMovementModel.new(event_bus)
	movement_view_model = PlayerMovementViewModel.new(movement_model, event_bus)

func after_each():
	movement_view_model.free()
	movement_model.free()
	event_bus.free()

func test_update_movement_default_velocity_zero():
	# 準備
	movement_view_model.initialize()
	
	# 実行
	movement_view_model.update_movement()
	
	# 検証
	assert_eq(movement_view_model.velocity.value, Vector2.ZERO)

func test_dash_publishes_dashing_event():
	# 準備
	var received_event = null
	event_bus.get_event_stream("MovementDashingChangedEvent").subscribe(
		func(event): received_event = event
	)
	movement_view_model.initialize()
	
	# 実行
	movement_view_model.handle_dash()
	
	# 少し待機してイベント処理を完了させる
	await get_tree().process_frame
	
	# 検証
	assert_not_null(received_event)
	assert_true(received_event.is_dashing)
	assert_true(movement_view_model.is_dashing.value)

func test_jump_update_publishes_grounded_event():
	# 準備
	var received_event = null
	event_bus.get_event_stream("MovementGroundedChangedEvent").subscribe(
		func(event): received_event = event
	)
	movement_view_model.initialize()
	
	# 実行
	movement_view_model.handle_jump()
	movement_view_model.update_movement()
	
	# 少し待機してイベント処理を完了させる
	await get_tree().process_frame
	
	# 検証
	assert_not_null(received_event)
	assert_false(received_event.is_grounded)
	assert_false(movement_view_model.is_grounded.value) 