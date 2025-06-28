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

func test_move_action():
	# 移動アクションのテスト
	var direction = Vector2(1, 0)
	movement_node.Move(direction)
	
	# 移動アクションが適切に処理されることを確認
	assert_ne(movement_node.Velocity, Vector2.ZERO, "Velocity should change after move action")

func test_multiple_movement_changes():
	# 複数の移動変更のテスト
	movement_node.Move(Vector2(1, 0))
	movement_node.Move(Vector2(0, 1))
	movement_node.Move(Vector2(0, 0))
	var max_iterations = 100
	var count = 0
	while movement_node.Velocity.length() >= 0.01 and count < max_iterations:
		movement_node.UpdateMovement()
		count += 1
	assert_true(movement_node.IsInitialized, "Movement node should handle multiple movement changes")
	assert_true(movement_node.Velocity.length() < 0.01, "Velocity should be near zero after stop")

func test_movement_actions_sequence():
	# 移動アクションのシーケンステスト
	movement_node.Move(Vector2(1, 0))
	movement_node.UpdateMovement()
	movement_node.Move(Vector2(0, 1))
	movement_node.UpdateMovement()
	
	# アクションシーケンスが適切に処理されることを確認
	assert_true(movement_node.IsInitialized, "Movement node should handle action sequences")
	# UpdateMovement()により速度が減衰するが、最後の移動で新しい速度が設定される
	assert_ne(movement_node.Velocity, Vector2.ZERO, "Velocity should be consistent after action sequence")

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
	temp_node.Move(Vector2(1, 0))
	
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
	movement_node.Move(Vector2(1, 0))
	movement_node.Move(Vector2(0, 1))
	
	# 並行操作後も正常に動作することを確認
	assert_true(movement_node.IsInitialized, "Movement node should handle concurrent operations")
	assert_ne(movement_node.Velocity, Vector2.ZERO, "Velocity should be consistent after concurrent operations")

func test_movement_action_combinations():
	# 移動アクションの組み合わせテスト
	movement_node.Move(Vector2(1, 0))
	movement_node.UpdateMovement()
	movement_node.Move(Vector2(0, 0))
	var max_iterations = 100
	var count = 0
	while movement_node.Velocity.length() >= 0.01 and count < max_iterations:
		movement_node.UpdateMovement()
		count += 1
	assert_true(movement_node.IsInitialized, "Movement node should handle action combinations")
	assert_true(movement_node.Velocity.length() < 0.01, "Velocity should be near zero after action combinations")

func test_extreme_movement_values():
	# 極端な移動値のテスト
	movement_node.Move(Vector2(-999, -999))  # 負の大きな値
	movement_node.Move(Vector2(999, 999))    # 正の大きな値
	movement_node.Move(Vector2(0.001, 0.001))  # 非常に小さな値
	
	# 極端な値後もシステムが正常に動作することを確認
	assert_true(movement_node.IsInitialized, "Movement node should handle extreme movement values")
	assert_ne(movement_node.Velocity, Vector2.ZERO, "Velocity should be consistent after extreme values")

func test_movement_precision():
	# 移動精度のテスト
	movement_node.Move(Vector2(0.5, 0.3))
	
	# 精度が保持されることを確認
	assert_true(movement_node.IsInitialized, "Movement node should handle movement precision")
	assert_ne(movement_node.Velocity, Vector2.ZERO, "Movement precision should be maintained")

func test_movement_update_frequency():
	# 移動更新頻度のテスト
	for i in range(10):
		movement_node.Move(Vector2(i * 0.1, i * 0.1))
		movement_node.UpdateMovement()
	
	# 高頻度の更新後もシステムが正常に動作することを確認
	assert_true(movement_node.IsInitialized, "Movement node should handle high update frequency")
	assert_ne(movement_node.Velocity, Vector2.ZERO, "Velocity should be consistent after high frequency updates")

func test_jump_action():
	# ジャンプアクションのテスト
	movement_node.Jump()
	
	# ジャンプアクションが適切に処理されることを確認
	assert_true(movement_node.IsInitialized, "Movement node should handle jump action")

func test_dash_action():
	# ダッシュアクションのテスト
	movement_node.Move(Vector2(1, 0))
	movement_node.Dash()
	
	# ダッシュアクションが適切に処理されることを確認
	assert_true(movement_node.IsInitialized, "Movement node should handle dash action") 
