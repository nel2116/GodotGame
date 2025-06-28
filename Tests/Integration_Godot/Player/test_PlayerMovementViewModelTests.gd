extends GutTest

# PlayerMovementViewModelTests.gd
# GUT版のプレイヤー移動ViewModelテスト
# ※C#クラスを直接参照しているため、GDScriptからは実行不可。
#   テストを有効化するにはC#ノードのGDScriptラッパーまたはテスト用ダミーが必要です。

var movement_node

func before_each():
	movement_node = preload("res://Scripts/Systems/Player/Movement/PlayerMovementViewModelNode.cs").new()
	add_child(movement_node)
	movement_node.Initialize()

func after_each():
	remove_child(movement_node)
	movement_node.queue_free()
	await get_tree().process_frame

func test_update_movement_default_velocity_zero():
	movement_node.UpdateMovement()
	assert_eq(movement_node.Velocity, Vector2.ZERO)

# func test_dash_publishes_dashing_event():
# 	var received_event = null
# 	event_bus.get_event_stream("MovementDashingChangedEvent").subscribe(
# 		func(event): received_event = event
# 	)
# 	movement_view_model.initialize()
# 	movement_view_model.handle_dash()
# 	await get_tree().process_frame
# 	assert_not_null(received_event)
# 	assert_true(received_event.is_dashing)
# 	assert_true(movement_view_model.is_dashing.value)

# func test_jump_update_publishes_grounded_event():
# 	var received_event = null
# 	event_bus.get_event_stream("MovementGroundedChangedEvent").subscribe(
# 		func(event): received_event = event
# 	)
# 	movement_view_model.initialize()
# 	movement_view_model.handle_jump()
# 	movement_view_model.update_movement()
# 	await get_tree().process_frame
# 	assert_not_null(received_event)
# 	assert_false(received_event.is_grounded)
# 	assert_false(movement_view_model.is_grounded.value) 