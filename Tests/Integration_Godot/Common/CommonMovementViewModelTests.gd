extends GutTest

# CommonMovementViewModelTests.gd
# GUT版の共通移動ViewModelテスト

var event_bus: GameEventBus
var movement_model: CommonMovementModel
var movement_view_model: CommonMovementViewModel

func before_each():
	event_bus = GameEventBus.new()
	movement_model = CommonMovementModel.new(event_bus)
	movement_view_model = CommonMovementViewModel.new(movement_model, event_bus)

func after_each():
	movement_view_model.free()
	movement_model.free()
	event_bus.free()

func test_initialize_default_velocity_zero():
	# 準備・実行
	movement_view_model.initialize()
	
	# 検証
	assert_eq(movement_view_model.velocity.value, Vector2.ZERO)

func test_update_movement_publishes_velocity_event():
	# 準備
	var received_event = null
	event_bus.get_event_stream("MovementVelocityChangedEvent").subscribe(
		func(event): received_event = event
	)
	movement_view_model.initialize()
	
	# 実行
	movement_view_model.update_movement()
	
	# 少し待機してイベント処理を完了させる
	await get_tree().process_frame
	
	# 検証
	assert_not_null(received_event)
	assert_eq(received_event.velocity, Vector2.ZERO)

func test_set_velocity_updates_value():
	# 準備
	movement_view_model.initialize()
	var new_velocity = Vector2(10, 5)
	
	# 実行
	movement_view_model.set_velocity(new_velocity)
	
	# 検証
	assert_eq(movement_view_model.velocity.value, new_velocity)

func test_set_velocity_publishes_event():
	# 準備
	var received_event = null
	event_bus.get_event_stream("MovementVelocityChangedEvent").subscribe(
		func(event): received_event = event
	)
	movement_view_model.initialize()
	var new_velocity = Vector2(10, 5)
	
	# 実行
	movement_view_model.set_velocity(new_velocity)
	
	# 少し待機してイベント処理を完了させる
	await get_tree().process_frame
	
	# 検証
	assert_not_null(received_event)
	assert_eq(received_event.velocity, new_velocity) 