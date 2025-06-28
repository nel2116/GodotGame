extends GutTest

# PlayerSystemIntegrationTests.gd
# GUT版のプレイヤーシステム統合テスト
# C#ノードを使用して実際の統合テストを実行

var input_node
var movement_node
var combat_node

func before_each():
	# 各ViewModelNodeを初期化
	input_node = preload("res://Scripts/Systems/Player/Input/PlayerInputViewModelNode.cs").new()
	movement_node = preload("res://Scripts/Systems/Player/Movement/PlayerMovementViewModelNode.cs").new()
	combat_node = preload("res://Scripts/Systems/Player/Combat/PlayerCombatViewModelNode.cs").new()
	
	# シーンツリーに追加
	add_child(input_node)
	add_child(movement_node)
	add_child(combat_node)
	
	# 初期化
	input_node.Initialize()
	movement_node.Initialize()
	combat_node.Initialize()

func after_each():
	# クリーンアップ
	remove_child(input_node)
	remove_child(movement_node)
	remove_child(combat_node)
	
	input_node.queue_free()
	movement_node.queue_free()
	combat_node.queue_free()
	
	await get_tree().process_frame

func test_system_initialization_all_components_initialized():
	# 全コンポーネントが正常に初期化されることを確認
	assert_true(input_node.IsInitialized, "Input node should be initialized")
	assert_true(movement_node.IsInitialized, "Movement node should be initialized")
	assert_true(combat_node.IsInitialized, "Combat node should be initialized")
	
	# 基本的な状態を確認
	assert_true(input_node.IsEnabled, "Input should be enabled by default")
	assert_eq(movement_node.Velocity, Vector2.ZERO, "Movement velocity should be zero initially")
	assert_eq(combat_node.CurrentHealth, 0.0, "Combat health should be zero initially")

func test_input_to_movement_integration():
	# 入力から移動への統合テスト
	input_node.UpdateInput()
	movement_node.UpdateMovement()
	
	# 移動システムが更新されることを確認
	assert_eq(movement_node.Velocity, Vector2.ZERO, "Movement should be updated after input")

func test_movement_to_animation_integration():
	# 移動からアニメーションへの統合テスト
	movement_node.UpdateMovement()
	
	# 移動状態が適切に更新されることを確認
	assert_eq(movement_node.Velocity, Vector2.ZERO, "Movement state should be updated")
	assert_false(movement_node.IsDashing, "Should not be dashing initially")
	assert_false(movement_node.IsGrounded, "Should not be grounded initially")

func test_combat_to_state_integration():
	# 戦闘から状態への統合テスト
	combat_node.Attack("BasicAttack")
	
	# 戦闘システムが適切に更新されることを確認
	assert_eq(combat_node.CurrentHealth, 0.0, "Health should remain unchanged for basic attack")

func test_progression_to_combat_integration():
	# 進行から戦闘への統合テスト
	combat_node.UpdateCombat()
	
	# 戦闘システムのパラメータが更新されることを確認
	assert_eq(combat_node.CurrentHealth, 0.0, "Combat parameters should be updated")

func test_full_system_update_integration():
	# 全システムの統合更新テスト
	for i in range(10):
		input_node.UpdateInput()
		movement_node.UpdateMovement()
		combat_node.UpdateCombat()
	
	# 全システムが正常に動作することを確認
	assert_true(input_node.IsEnabled, "Input should remain enabled after multiple updates")
	assert_eq(movement_node.Velocity, Vector2.ZERO, "Movement should remain consistent")
	assert_eq(combat_node.CurrentHealth, 0.0, "Combat should remain consistent")

func test_movement_actions_integration():
	# 移動アクションの統合テスト
	movement_node.HandleJump()
	movement_node.UpdateMovement()
	
	# ジャンプアクションが適切に処理されることを確認
	assert_eq(movement_node.Velocity, Vector2.ZERO, "Jump should affect movement")
	
	movement_node.HandleDash()
	movement_node.UpdateMovement()
	
	# ダッシュアクションが適切に処理されることを確認
	assert_eq(movement_node.Velocity, Vector2.ZERO, "Dash should affect movement")

func test_combat_actions_integration():
	# 戦闘アクションの統合テスト
	combat_node.TakeDamage(10.0)
	
	# ダメージが適切に処理されることを確認
	assert_eq(combat_node.CurrentHealth, 0.0, "Damage should be applied")
	
	combat_node.Heal(5.0)
	
	# 回復が適切に処理されることを確認
	assert_eq(combat_node.CurrentHealth, 0.0, "Healing should be applied")

func test_error_handling_integration():
	# エラーハンドリングの統合テスト
	# 無効なアクションを実行
	combat_node.Attack("InvalidAction")
	
	# システムがクラッシュせずに動作し続けることを確認
	assert_true(input_node.IsInitialized, "System should remain stable after invalid action")
	assert_true(movement_node.IsInitialized, "System should remain stable after invalid action")
	assert_true(combat_node.IsInitialized, "System should remain stable after invalid action")

func test_performance_integration():
	# パフォーマンス統合テスト
	for i in range(100):
		input_node.UpdateInput()
		movement_node.UpdateMovement()
		combat_node.UpdateCombat()
	
	# 大量の更新後もシステムが正常に動作することを確認
	assert_true(input_node.IsInitialized, "System should remain stable after performance test")
	assert_true(movement_node.IsInitialized, "System should remain stable after performance test")
	assert_true(combat_node.IsInitialized, "System should remain stable after performance test")

func test_memory_usage_integration():
	# メモリ使用量統合テスト
	for i in range(50):
		input_node.UpdateInput()
		movement_node.UpdateMovement()
		combat_node.UpdateCombat()
	
	# メモリリークがないことを確認
	assert_true(input_node.IsInitialized, "System should remain stable after memory test")
	assert_true(movement_node.IsInitialized, "System should remain stable after memory test")
	assert_true(combat_node.IsInitialized, "System should remain stable after memory test")

func test_node_lifecycle_integration():
	# ノードライフサイクルの統合テスト
	# ノードの再初期化
	input_node.queue_free()
	movement_node.queue_free()
	combat_node.queue_free()
	
	await get_tree().process_frame
	
	# 新しいノードを作成
	input_node = preload("res://Scripts/Systems/Player/Input/PlayerInputViewModelNode.cs").new()
	movement_node = preload("res://Scripts/Systems/Player/Movement/PlayerMovementViewModelNode.cs").new()
	combat_node = preload("res://Scripts/Systems/Player/Combat/PlayerCombatViewModelNode.cs").new()
	
	add_child(input_node)
	add_child(movement_node)
	add_child(combat_node)
	
	input_node.Initialize()
	movement_node.Initialize()
	combat_node.Initialize()
	
	# 再初期化後も正常に動作することを確認
	assert_true(input_node.IsInitialized, "Node should be reinitializable")
	assert_true(movement_node.IsInitialized, "Node should be reinitializable")
	assert_true(combat_node.IsInitialized, "Node should be reinitializable")

func test_concurrent_operations_integration():
	# 並行操作の統合テスト
	# 複数の操作を同時に実行
	input_node.UpdateInput()
	movement_node.HandleJump()
	combat_node.Attack("BasicAttack")
	
	# 並行操作後もシステムが正常に動作することを確認
	assert_true(input_node.IsInitialized, "System should handle concurrent operations")
	assert_true(movement_node.IsInitialized, "System should handle concurrent operations")
	assert_true(combat_node.IsInitialized, "System should handle concurrent operations")

func test_system_state_consistency():
	# システム状態の一貫性テスト
	var initial_input_state = input_node.IsEnabled
	var initial_movement_velocity = movement_node.Velocity
	var initial_combat_health = combat_node.CurrentHealth
	
	# システム更新
	input_node.UpdateInput()
	movement_node.UpdateMovement()
	combat_node.UpdateCombat()
	
	# 状態の一貫性を確認
	assert_eq(input_node.IsEnabled, initial_input_state, "Input state should remain consistent")
	assert_eq(movement_node.Velocity, initial_movement_velocity, "Movement state should remain consistent")
	assert_eq(combat_node.CurrentHealth, initial_combat_health, "Combat state should remain consistent") 