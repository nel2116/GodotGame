extends GutTest

# CommonMovementViewModelTests.gd
# GUT版の共通移動ViewModelテスト
# ※C#クラスを直接参照しているため、GDScriptからは実行不可。
#   テストを有効化するにはC#ノードのGDScriptラッパーまたはテスト用ダミーが必要です。

# var event_bus: NewGameProject.GameEventBus
# var movement_model: NewGameProject.CommonMovementModel
# var movement_view_model: NewGameProject.CommonMovementViewModel

# func before_each():
# 	event_bus = NewGameProject.GameEventBus.new()
# 	movement_model = NewGameProject.CommonMovementModel.new(event_bus)
# 	movement_view_model = NewGameProject.CommonMovementViewModel.new(movement_model, event_bus)

# func after_each():
# 	movement_view_model.free()
# 	movement_model.free()
# 	event_bus.free()

# func test_initialize_default_velocity_zero():
# 	movement_view_model.initialize()
# 	assert_eq(movement_view_model.velocity.value, Vector2.ZERO)

# func test_update_movement_publishes_velocity_event():
# 	var received_event = null
# 	event_bus.get_event_stream("MovementVelocityChangedEvent").subscribe(
# 		func(event): received_event = event
# 	)
# 	movement_view_model.initialize()
# 	movement_view_model.update_movement()
# 	await get_tree().process_frame
# 	assert_not_null(received_event)
# 	assert_eq(received_event.velocity, Vector2.ZERO)

# func test_set_velocity_updates_value():
# 	movement_view_model.initialize()
# 	var new_velocity = Vector2(10, 5)
# 	movement_view_model.set_velocity(new_velocity)
# 	assert_eq(movement_view_model.velocity.value, new_velocity)

# func test_set_velocity_publishes_event():
# 	var received_event = null
# 	event_bus.get_event_stream("MovementVelocityChangedEvent").subscribe(
# 		func(event): received_event = event
# 	)
# 	movement_view_model.initialize()
# 	var new_velocity = Vector2(10, 5)
# 	movement_view_model.set_velocity(new_velocity)
# 	await get_tree().process_frame
# 	assert_not_null(received_event)
# 	assert_eq(received_event.velocity, new_velocity) 