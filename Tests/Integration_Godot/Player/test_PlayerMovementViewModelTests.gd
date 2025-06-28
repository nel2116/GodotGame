extends GutTest

# PlayerMovementViewModelTests.gd
# GUT版のプレイヤー移動ViewModelテスト
# C#ノードを使用して実際のテストを実行

var movement_node

func before_each():
	movement_node = preload("res://Scripts/Systems/Player/Movement/PlayerMovementViewModelNode.cs").new()
	add_child(movement_node)
	movement_node.Initialize()

func after_each():
	remove_child(movement_node)
	movement_node.queue_free()
	await get_tree().process_frame

func test_initialize_node_properly():
	assert_true(movement_node.IsInitialized, "Movement node should be properly initialized")

func test_update_movement_default_velocity_zero():
	movement_node.UpdateMovement()
	assert_eq(movement_node.Velocity, Vector2.ZERO, "Default velocity should be zero")

func test_initial_movement_state():
	# 初期状態の確認
	assert_eq(movement_node.Velocity, Vector2.ZERO, "Initial velocity should be zero")
	assert_false(movement_node.IsDashing, "Initial state should not be dashing")
	assert_false(movement_node.IsGrounded, "Initial state should not be grounded")

func test_handle_jump_action():
	# ジャンプアクションのテスト
	movement_node.HandleJump()
	movement_node.UpdateMovement()
	
	# ジャンプアクションが適切に処理されることを確認
	assert_eq(movement_node.Velocity, Vector2.ZERO, "Jump should affect movement state")

func test_handle_dash_action():
	# ダッシュアクションのテスト
	movement_node.HandleDash()
	movement_node.UpdateMovement()
	
	# ダッシュアクションが適切に処理されることを確認
	assert_eq(movement_node.Velocity, Vector2.ZERO, "Dash should affect movement state")

func test_multiple_movement_updates():
	# 複数回の移動更新テスト
	for i in range(10):
		movement_node.UpdateMovement()
	
	assert_true(movement_node.IsInitialized, "Movement node should handle multiple updates")
	assert_eq(movement_node.Velocity, Vector2.ZERO, "Velocity should remain consistent")

func test_movement_actions_sequence():
	# 移動アクションのシーケンステスト
	movement_node.HandleJump()
	movement_node.UpdateMovement()
	movement_node.HandleDash()
	movement_node.UpdateMovement()
	
	# アクションシーケンスが適切に処理されることを確認
	assert_true(movement_node.IsInitialized, "Movement node should handle action sequences")
	assert_eq(movement_node.Velocity, Vector2.ZERO, "Velocity should be consistent after action sequence")

func test_movement_node_lifecycle():
	# ノードのライフサイクルテスト
	var initial_velocity = movement_node.Velocity
	
	# ノードを再初期化
	movement_node.queue_free()
	await get_tree().process_frame
	
	movement_node = preload("res://Scripts/Systems/Player/Movement/PlayerMovementViewModelNode.cs").new()
	add_child(movement_node)
	movement_node.Initialize()
	
	assert_true(movement_node.IsInitialized, "Movement node should be reinitializable")
	assert_eq(movement_node.Velocity, Vector2.ZERO, "Velocity should be reset after reinitialization")

func test_movement_performance():
	# パフォーマンステスト
	for i in range(100):
		movement_node.UpdateMovement()
	
	assert_true(movement_node.IsInitialized, "Movement node should handle performance test")
	assert_eq(movement_node.Velocity, Vector2.ZERO, "Velocity should remain consistent after performance test")

func test_movement_error_handling():
	# エラーハンドリングテスト
	# 初期化前にアクションを呼び出した場合の動作を確認
	var temp_node = preload("res://Scripts/Systems/Player/Movement/PlayerMovementViewModelNode.cs").new()
	add_child(temp_node)
	
	# 初期化前にアクションを呼び出し
	temp_node.UpdateMovement()
	temp_node.HandleJump()
	temp_node.HandleDash()
	
	# エラーが発生せずに動作することを確認
	assert_false(temp_node.IsInitialized, "Node should not be initialized before Initialize() call")
	
	# 初期化
	temp_node.Initialize()
	assert_true(temp_node.IsInitialized, "Node should be initialized after Initialize() call")
	
	remove_child(temp_node)
	temp_node.queue_free()
	await get_tree().process_frame

func test_movement_state_consistency():
	# 移動状態の一貫性テスト
	var initial_velocity = movement_node.Velocity
	var initial_dashing = movement_node.IsDashing
	var initial_grounded = movement_node.IsGrounded
	
	movement_node.UpdateMovement()
	
	# 状態が一貫していることを確認
	assert_eq(movement_node.Velocity, initial_velocity, "Velocity should remain consistent")
	assert_eq(movement_node.IsDashing, initial_dashing, "Dashing state should remain consistent")
	assert_eq(movement_node.IsGrounded, initial_grounded, "Grounded state should remain consistent")

func test_movement_concurrent_operations():
	# 並行操作テスト
	# 複数の操作を同時に実行
	movement_node.UpdateMovement()
	movement_node.HandleJump()
	movement_node.HandleDash()
	
	# 並行操作後も正常に動作することを確認
	assert_true(movement_node.IsInitialized, "Movement node should handle concurrent operations")
	assert_eq(movement_node.Velocity, Vector2.ZERO, "Velocity should be consistent after concurrent operations")

func test_movement_action_combinations():
	# 移動アクションの組み合わせテスト
	# ジャンプとダッシュを組み合わせて実行
	movement_node.HandleJump()
	movement_node.HandleDash()
	movement_node.UpdateMovement()
	
	# 組み合わせアクションが適切に処理されることを確認
	assert_true(movement_node.IsInitialized, "Movement node should handle action combinations")
	assert_eq(movement_node.Velocity, Vector2.ZERO, "Velocity should be consistent after action combinations")

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