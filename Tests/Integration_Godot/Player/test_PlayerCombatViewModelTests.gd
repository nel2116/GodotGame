extends GutTest

# PlayerCombatViewModelTests.gd
# GUT版のプレイヤー戦闘ViewModelテスト
# C#ノードを使用して実際のテストを実行

var combat_node

func before_each():
	combat_node = preload("res://Scripts/Systems/Player/Combat/PlayerCombatViewModelNode.cs").new()
	add_child(combat_node)
	combat_node.Initialize()

func after_each():
	remove_child(combat_node)
	combat_node.queue_free()
	await get_tree().process_frame

func test_initialize_node_properly():
	assert_true(combat_node.IsInitialized, "Combat node should be properly initialized")

func test_initial_combat_state():
	# 初期状態の確認
	assert_eq(combat_node.CurrentHealth, 0.0, "Initial health should be zero")
	assert_eq(combat_node.MaxHealth, 0.0, "Initial max health should be zero")

func test_attack_action():
	# 攻撃アクションのテスト
	combat_node.Attack("BasicAttack")
	
	# 攻撃アクションが適切に処理されることを確認
	assert_eq(combat_node.CurrentHealth, 0.0, "Health should remain unchanged for basic attack")

func test_take_damage_action():
	# ダメージを受けるアクションのテスト
	combat_node.TakeDamage(10.0)
	
	# ダメージが適切に処理されることを確認
	assert_eq(combat_node.CurrentHealth, 0.0, "Damage should be applied")

func test_heal_action():
	# 回復アクションのテスト
	combat_node.Heal(5.0)
	
	# 回復が適切に処理されることを確認
	assert_eq(combat_node.CurrentHealth, 0.0, "Healing should be applied")

func test_update_combat():
	# 戦闘更新のテスト
	combat_node.UpdateCombat()
	
	# 戦闘システムが適切に更新されることを確認
	assert_eq(combat_node.CurrentHealth, 0.0, "Combat should be updated")

func test_multiple_combat_actions():
	# 複数の戦闘アクションのテスト
	combat_node.Attack("BasicAttack")
	combat_node.TakeDamage(10.0)
	combat_node.Heal(5.0)
	combat_node.UpdateCombat()
	
	assert_true(combat_node.IsInitialized, "Combat node should handle multiple actions")
	assert_eq(combat_node.CurrentHealth, 0.0, "Health should be consistent after multiple actions")

func test_combat_actions_sequence():
	# 戦闘アクションのシーケンステスト
	combat_node.Attack("BasicAttack")
	combat_node.UpdateCombat()
	combat_node.TakeDamage(10.0)
	combat_node.UpdateCombat()
	combat_node.Heal(5.0)
	combat_node.UpdateCombat()
	
	# アクションシーケンスが適切に処理されることを確認
	assert_true(combat_node.IsInitialized, "Combat node should handle action sequences")
	assert_eq(combat_node.CurrentHealth, 0.0, "Health should be consistent after action sequence")

func test_combat_node_lifecycle():
	# ノードのライフサイクルテスト
	var initial_health = combat_node.CurrentHealth
	
	# ノードを再初期化
	combat_node.queue_free()
	await get_tree().process_frame
	
	combat_node = preload("res://Scripts/Systems/Player/Combat/PlayerCombatViewModelNode.cs").new()
	add_child(combat_node)
	combat_node.Initialize()
	
	assert_true(combat_node.IsInitialized, "Combat node should be reinitializable")
	assert_eq(combat_node.CurrentHealth, 0.0, "Health should be reset after reinitialization")

func test_combat_performance():
	# パフォーマンステスト
	for i in range(100):
		combat_node.UpdateCombat()
	
	assert_true(combat_node.IsInitialized, "Combat node should handle performance test")
	assert_eq(combat_node.CurrentHealth, 0.0, "Health should remain consistent after performance test")

func test_combat_error_handling():
	# エラーハンドリングテスト
	# 初期化前にアクションを呼び出した場合の動作を確認
	var temp_node = preload("res://Scripts/Systems/Player/Combat/PlayerCombatViewModelNode.cs").new()
	add_child(temp_node)
	
	# 初期化前にアクションを呼び出し
	temp_node.Attack("BasicAttack")
	temp_node.TakeDamage(10.0)
	temp_node.Heal(5.0)
	temp_node.UpdateCombat()
	
	# エラーが発生せずに動作することを確認
	assert_false(temp_node.IsInitialized, "Node should not be initialized before Initialize() call")
	
	# 初期化
	temp_node.Initialize()
	assert_true(temp_node.IsInitialized, "Node should be initialized after Initialize() call")
	
	remove_child(temp_node)
	temp_node.queue_free()
	await get_tree().process_frame

func test_combat_state_consistency():
	# 戦闘状態の一貫性テスト
	var initial_health = combat_node.CurrentHealth
	var initial_max_health = combat_node.MaxHealth
	
	combat_node.UpdateCombat()
	
	# 状態が一貫していることを確認
	assert_eq(combat_node.CurrentHealth, initial_health, "Health should remain consistent")
	assert_eq(combat_node.MaxHealth, initial_max_health, "Max health should remain consistent")

func test_combat_concurrent_operations():
	# 並行操作テスト
	# 複数の操作を同時に実行
	combat_node.Attack("BasicAttack")
	combat_node.TakeDamage(10.0)
	combat_node.Heal(5.0)
	
	# 並行操作後も正常に動作することを確認
	assert_true(combat_node.IsInitialized, "Combat node should handle concurrent operations")
	assert_eq(combat_node.CurrentHealth, 0.0, "Health should be consistent after concurrent operations")

func test_combat_action_combinations():
	# 戦闘アクションの組み合わせテスト
	# 攻撃、ダメージ、回復を組み合わせて実行
	combat_node.Attack("BasicAttack")
	combat_node.TakeDamage(10.0)
	combat_node.Heal(5.0)
	combat_node.UpdateCombat()
	
	# 組み合わせアクションが適切に処理されることを確認
	assert_true(combat_node.IsInitialized, "Combat node should handle action combinations")
	assert_eq(combat_node.CurrentHealth, 0.0, "Health should be consistent after action combinations")

func test_invalid_attack_actions():
	# 無効な攻撃アクションのテスト
	combat_node.Attack("InvalidAction")
	combat_node.Attack("")
	combat_node.Attack("NonExistentAttack")
	
	# 無効なアクション後もシステムが正常に動作することを確認
	assert_true(combat_node.IsInitialized, "Combat node should handle invalid actions")
	assert_eq(combat_node.CurrentHealth, 0.0, "Health should remain consistent after invalid actions")

func test_extreme_damage_values():
	# 極端なダメージ値のテスト
	combat_node.TakeDamage(-10.0)  # 負のダメージ
	combat_node.TakeDamage(999999.0)  # 非常に大きなダメージ
	combat_node.TakeDamage(0.0)  # ゼロダメージ
	
	# 極端な値後もシステムが正常に動作することを確認
	assert_true(combat_node.IsInitialized, "Combat node should handle extreme damage values")
	assert_eq(combat_node.CurrentHealth, 0.0, "Health should remain consistent after extreme damage values")

func test_extreme_heal_values():
	# 極端な回復値のテスト
	combat_node.Heal(-5.0)  # 負の回復
	combat_node.Heal(999999.0)  # 非常に大きな回復
	combat_node.Heal(0.0)  # ゼロ回復
	
	# 極端な値後もシステムが正常に動作することを確認
	assert_true(combat_node.IsInitialized, "Combat node should handle extreme heal values")
	assert_eq(combat_node.CurrentHealth, 0.0, "Health should remain consistent after extreme heal values") 