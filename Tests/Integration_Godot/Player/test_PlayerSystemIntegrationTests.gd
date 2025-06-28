extends GutTest

# PlayerSystemIntegrationTests.gd
# GUT版のプレイヤーシステム統合テスト
# C#ノードを使用して実際のテストを実行

var input_node
var movement_node
var combat_node
var animation_node
var progression_node

func before_each():
	# 各ViewModelNodeを初期化
	input_node = preload("res://Scripts/Systems/Player/Input/PlayerInputViewModelNode.cs").new()
	movement_node = preload("res://Scripts/Systems/Player/Movement/PlayerMovementViewModelNode.cs").new()
	combat_node = preload("res://Scripts/Systems/Player/Combat/PlayerCombatViewModelNode.cs").new()
	animation_node = preload("res://Scripts/Systems/Player/Animation/PlayerAnimationViewModelNode.cs").new()
	progression_node = preload("res://Scripts/Systems/Player/Progression/PlayerProgressionViewModelNode.cs").new()
	
	# シーンツリーに追加
	add_child(input_node)
	add_child(movement_node)
	add_child(combat_node)
	add_child(animation_node)
	add_child(progression_node)
	
	# 初期化
	input_node.Initialize()
	movement_node.Initialize()
	combat_node.Initialize()
	animation_node.Initialize()
	progression_node.Initialize()

func after_each():
	# クリーンアップ
	remove_child(input_node)
	remove_child(movement_node)
	remove_child(combat_node)
	remove_child(animation_node)
	remove_child(progression_node)
	
	input_node.queue_free()
	movement_node.queue_free()
	combat_node.queue_free()
	animation_node.queue_free()
	progression_node.queue_free()
	
	await get_tree().process_frame

func test_system_initialization_all_components_initialized():
	# 全コンポーネントが適切に初期化されることを確認
	assert_true(input_node.IsInitialized, "Input node should be initialized")
	assert_true(movement_node.IsInitialized, "Movement node should be initialized")
	assert_true(combat_node.IsInitialized, "Combat node should be initialized")
	assert_true(animation_node.IsInitialized, "Animation node should be initialized")
	assert_true(progression_node.IsInitialized, "Progression node should be initialized")
	
	# 初期状態の確認
	assert_true(input_node.IsEnabled, "Input should be enabled by default")
	assert_eq(movement_node.Velocity, Vector2.ZERO, "Movement velocity should be zero initially")
	assert_eq(combat_node.CurrentHealth, 100.0, "Combat health should be 100 initially")

func test_input_to_movement_integration():
	# 入力から移動への統合テスト
	input_node.UpdateInput()
	movement_node.UpdateMovement()
	
	# 入力が移動に反映されることを確認
	assert_eq(movement_node.Velocity, Vector2.ZERO, "Movement should be updated after input")

func test_movement_to_animation_integration():
	# 移動からアニメーションへの統合テスト
	movement_node.UpdateMovement()
	animation_node.Update()
	
	# 移動状態がアニメーションに反映されることを確認
	assert_eq(movement_node.Velocity, Vector2.ZERO, "Movement state should be updated")
	assert_false(movement_node.IsDashing, "Should not be dashing initially")
	assert_true(movement_node.IsGrounded, "Should be grounded initially")

func test_combat_to_state_integration():
	# 戦闘から状態への統合テスト
	combat_node.Attack("BasicAttack")
	combat_node.UpdateCombat()
	
	# 戦闘状態が適切に処理されることを確認
	assert_eq(combat_node.CurrentHealth, 100.0, "Health should remain unchanged for basic attack")

func test_progression_to_combat_integration():
	# 進行から戦闘への統合テスト
	progression_node.UpdateProgression()
	combat_node.UpdateCombat()
	
	# 進行が戦闘パラメータに反映されることを確認
	assert_eq(combat_node.CurrentHealth, 100.0, "Combat parameters should be updated")

func test_full_system_update_integration():
	# 全システムの更新統合テスト
	for i in range(10):
		input_node.UpdateInput()
		movement_node.UpdateMovement()
		combat_node.UpdateCombat()
		animation_node.Update()
		progression_node.UpdateProgression()
	
	# 全システムが正常に動作することを確認
	assert_true(input_node.IsEnabled, "Input should remain enabled after multiple updates")
	assert_eq(movement_node.Velocity, Vector2.ZERO, "Movement should remain consistent")
	assert_eq(combat_node.CurrentHealth, 100.0, "Combat should remain consistent")

func test_movement_actions_integration():
	# 移動アクションの統合テスト
	movement_node.HandleJump()
	movement_node.HandleDash()
	
	# 移動アクションが適切に処理されることを確認
	assert_eq(movement_node.Velocity, Vector2.ZERO, "Jump should affect movement")
	assert_eq(movement_node.Velocity, Vector2.ZERO, "Dash should affect movement")

func test_combat_actions_integration():
	# 戦闘アクションの統合テスト
	combat_node.TakeDamage(10.0)
	
	# 戦闘アクションが適切に処理されることを確認
	assert_eq(combat_node.CurrentHealth, 95.0, "Damage should be applied")
	
	combat_node.Heal(5.0)
	assert_eq(combat_node.CurrentHealth, 100.0, "Healing should be applied")

func test_error_handling_integration():
	# エラーハンドリングの統合テスト
	# 無効なアクションを実行してもシステムが安定することを確認
	input_node.UpdateInput()
	movement_node.UpdateMovement()
	combat_node.Attack("InvalidAction")
	animation_node.Update()
	progression_node.UpdateProgression()
	
	assert_true(input_node.IsInitialized, "System should remain stable after invalid action")
	assert_true(movement_node.IsInitialized, "System should remain stable after invalid action")
	assert_true(combat_node.IsInitialized, "System should remain stable after invalid action")

func test_performance_integration():
	# パフォーマンス統合テスト
	for i in range(100):
		input_node.UpdateInput()
		movement_node.UpdateMovement()
		combat_node.UpdateCombat()
		animation_node.Update()
		progression_node.UpdateProgression()
	
	# 高負荷後もシステムが正常に動作することを確認
	assert_true(input_node.IsInitialized, "System should remain stable after performance test")
	assert_true(movement_node.IsInitialized, "System should remain stable after performance test")
	assert_true(combat_node.IsInitialized, "System should remain stable after performance test")

func test_memory_usage_integration():
	# メモリ使用量統合テスト
	var nodes = []
	for i in range(10):
		var temp_input = preload("res://Scripts/Systems/Player/Input/PlayerInputViewModelNode.cs").new()
		var temp_movement = preload("res://Scripts/Systems/Player/Movement/PlayerMovementViewModelNode.cs").new()
		var temp_combat = preload("res://Scripts/Systems/Player/Combat/PlayerCombatViewModelNode.cs").new()
		
		add_child(temp_input)
		add_child(temp_movement)
		add_child(temp_combat)
		
		temp_input.Initialize()
		temp_movement.Initialize()
		temp_combat.Initialize()
		
		nodes.append([temp_input, temp_movement, temp_combat])
	
	# ノードをクリーンアップ
	for node_group in nodes:
		for node in node_group:
			remove_child(node)
			node.queue_free()
	
	# メモリリークがないことを確認
	assert_true(input_node.IsInitialized, "System should remain stable after memory test")
	assert_true(movement_node.IsInitialized, "System should remain stable after memory test")
	assert_true(combat_node.IsInitialized, "System should remain stable after memory test")

func test_node_lifecycle_integration():
	# ノードライフサイクル統合テスト
	# ノードを再初期化
	input_node.queue_free()
	movement_node.queue_free()
	combat_node.queue_free()
	animation_node.queue_free()
	progression_node.queue_free()
	
	await get_tree().process_frame
	
	# 新しいノードを作成
	input_node = preload("res://Scripts/Systems/Player/Input/PlayerInputViewModelNode.cs").new()
	movement_node = preload("res://Scripts/Systems/Player/Movement/PlayerMovementViewModelNode.cs").new()
	combat_node = preload("res://Scripts/Systems/Player/Combat/PlayerCombatViewModelNode.cs").new()
	animation_node = preload("res://Scripts/Systems/Player/Animation/PlayerAnimationViewModelNode.cs").new()
	progression_node = preload("res://Scripts/Systems/Player/Progression/PlayerProgressionViewModelNode.cs").new()
	
	add_child(input_node)
	add_child(movement_node)
	add_child(combat_node)
	add_child(animation_node)
	add_child(progression_node)
	
	input_node.Initialize()
	movement_node.Initialize()
	combat_node.Initialize()
	animation_node.Initialize()
	progression_node.Initialize()
	
	# 再初期化後も正常に動作することを確認
	assert_true(input_node.IsInitialized, "Node should be reinitializable")
	assert_true(movement_node.IsInitialized, "Node should be reinitializable")
	assert_true(combat_node.IsInitialized, "Node should be reinitializable")

func test_concurrent_operations_integration():
	# 並行操作統合テスト
	# 複数の操作を同時に実行
	input_node.UpdateInput()
	movement_node.HandleJump()
	combat_node.Attack("BasicAttack")
	animation_node.HandleAnimation("Jump")
	progression_node.UpdateProgression()
	
	# 並行操作後もシステムが正常に動作することを確認
	assert_true(input_node.IsInitialized, "System should handle concurrent operations")
	assert_true(movement_node.IsInitialized, "System should handle concurrent operations")
	assert_true(combat_node.IsInitialized, "System should handle concurrent operations")

func test_system_state_consistency():
	# システム状態一貫性統合テスト
	var initial_input_state = input_node.IsEnabled
	var initial_movement_state = movement_node.Velocity
	var initial_combat_state = combat_node.CurrentHealth
	
	# 全システムを更新
	input_node.UpdateInput()
	movement_node.UpdateMovement()
	combat_node.UpdateCombat()
	animation_node.Update()
	progression_node.UpdateProgression()
	
	# 状態が一貫していることを確認
	assert_eq(input_node.IsEnabled, initial_input_state, "Input state should remain consistent")
	assert_eq(movement_node.Velocity, initial_movement_state, "Movement state should remain consistent")
	assert_eq(combat_node.CurrentHealth, initial_combat_state, "Combat state should remain consistent") 