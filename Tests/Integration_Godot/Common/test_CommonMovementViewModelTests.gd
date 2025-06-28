extends GutTest

# CommonMovementViewModelTests.gd
# GUT版の共通移動ViewModelテスト
# C#ノードを使用して実際のテストを実行

var movement_node

func before_each():
	movement_node = preload("res://Scripts/Systems/Common/Movement/CommonMovementViewModelNode.cs").new()
	add_child(movement_node)
	movement_node.Initialize()

func after_each():
	remove_child(movement_node)
	movement_node.queue_free()
	await get_tree().process_frame

func test_initialize_node_properly():
	assert_true(movement_node.IsInitialized, "Movement node should be properly initialized")

func test_initial_movement_state():
	# 初期状態の確認
	assert_eq(movement_node.Velocity, Vector2.ZERO, "Initial velocity should be zero")

func test_update_movement_default_velocity_zero():
	movement_node.UpdateMovement()
	assert_eq(movement_node.Velocity, Vector2.ZERO, "Default velocity should be zero after update")

func test_set_velocity_action():
	# 速度設定アクションのテスト
	var new_velocity = Vector2(10, 5)
	movement_node.SetVelocity(new_velocity)
	
	# 速度設定アクションが適切に処理されることを確認
	assert_eq(movement_node.Velocity, new_velocity, "Velocity should be set to new value")

func test_multiple_velocity_changes():
	# 複数の速度変更のテスト
	movement_node.SetVelocity(Vector2(5, 3))
	movement_node.SetVelocity(Vector2(15, 8))
	movement_node.SetVelocity(Vector2(0, 0))
	
	assert_true(movement_node.IsInitialized, "Movement node should handle multiple velocity changes")
	assert_eq(movement_node.Velocity, Vector2.ZERO, "Velocity should be consistent after multiple changes")

func test_movement_actions_sequence():
	# 移動アクションのシーケンステスト
	movement_node.SetVelocity(Vector2(10, 5))
	movement_node.UpdateMovement()
	movement_node.SetVelocity(Vector2(20, 10))
	movement_node.UpdateMovement()
	
	# アクションシーケンスが適切に処理されることを確認
	assert_true(movement_node.IsInitialized, "Movement node should handle action sequences")
	assert_eq(movement_node.Velocity, Vector2(20, 10), "Velocity should be consistent after action sequence")

func test_movement_node_lifecycle():
	# ノードのライフサイクルテスト
	var initial_velocity = movement_node.Velocity
	
	# ノードを再初期化
	movement_node.queue_free()
	await get_tree().process_frame
	
	movement_node = preload("res://Scripts/Systems/Common/Movement/CommonMovementViewModelNode.cs").new()
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
	var temp_node = preload("res://Scripts/Systems/Common/Movement/CommonMovementViewModelNode.cs").new()
	add_child(temp_node)
	
	# 初期化前にアクションを呼び出し
	temp_node.UpdateMovement()
	temp_node.SetVelocity(Vector2(10, 5))
	
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
	
	movement_node.UpdateMovement()
	
	# 状態が一貫していることを確認
	assert_eq(movement_node.Velocity, initial_velocity, "Velocity should remain consistent")

func test_movement_concurrent_operations():
	# 並行操作テスト
	# 複数の操作を同時に実行
	movement_node.UpdateMovement()
	movement_node.SetVelocity(Vector2(10, 5))
	movement_node.SetVelocity(Vector2(20, 10))
	
	# 並行操作後も正常に動作することを確認
	assert_true(movement_node.IsInitialized, "Movement node should handle concurrent operations")
	assert_eq(movement_node.Velocity, Vector2(20, 10), "Velocity should be consistent after concurrent operations")

func test_movement_action_combinations():
	# 移動アクションの組み合わせテスト
	# 速度設定と更新を組み合わせて実行
	movement_node.SetVelocity(Vector2(10, 5))
	movement_node.UpdateMovement()
	movement_node.SetVelocity(Vector2(0, 0))
	movement_node.UpdateMovement()
	
	# 組み合わせアクションが適切に処理されることを確認
	assert_true(movement_node.IsInitialized, "Movement node should handle action combinations")
	assert_eq(movement_node.Velocity, Vector2.ZERO, "Velocity should be consistent after action combinations")

func test_extreme_velocity_values():
	# 極端な速度値のテスト
	movement_node.SetVelocity(Vector2(-999, -999))  # 負の大きな値
	movement_node.SetVelocity(Vector2(999, 999))    # 正の大きな値
	movement_node.SetVelocity(Vector2(0.001, 0.001))  # 非常に小さな値
	
	# 極端な値後もシステムが正常に動作することを確認
	assert_true(movement_node.IsInitialized, "Movement node should handle extreme velocity values")
	assert_eq(movement_node.Velocity, Vector2(0.001, 0.001), "Velocity should be consistent after extreme values")

func test_velocity_precision():
	# 速度精度のテスト
	movement_node.SetVelocity(Vector2(3.14159, 2.71828))
	
	# 精度が保持されることを確認
	assert_true(movement_node.IsInitialized, "Movement node should handle velocity precision")
	assert_eq(movement_node.Velocity, Vector2(3.14159, 2.71828), "Velocity precision should be maintained")

func test_movement_update_frequency():
	# 移動更新頻度のテスト
	for i in range(10):
		movement_node.SetVelocity(Vector2(i, i))
		movement_node.UpdateMovement()
	
	# 高頻度の更新後もシステムが正常に動作することを確認
	assert_true(movement_node.IsInitialized, "Movement node should handle high update frequency")
	assert_eq(movement_node.Velocity, Vector2(9, 9), "Velocity should be consistent after high frequency updates") 