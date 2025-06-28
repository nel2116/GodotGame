extends GutTest

# PlayerAnimationViewModelTests.gd
# GUT版のプレイヤーアニメーションViewModelテスト
# C#ノードを使用して実際のテストを実行

var animation_node

func before_each():
	animation_node = preload("res://Scripts/Systems/Player/Animation/PlayerAnimationViewModelNode.cs").new()
	add_child(animation_node)
	animation_node.Initialize()

func after_each():
	remove_child(animation_node)
	animation_node.queue_free()
	await get_tree().process_frame

func test_initialize_node_properly():
	assert_true(animation_node.IsInitialized, "Animation node should be properly initialized")

func test_initial_animation_state():
	# 初期状態の確認
	assert_eq(animation_node.CurrentAnimation, "Idle", "Initial animation should be Idle")

func test_play_animation_action():
	# アニメーション再生アクションのテスト
	animation_node.PlayAnimation("Jump")
	
	# アニメーション再生アクションが適切に処理されることを確認
	assert_eq(animation_node.CurrentAnimation, "Jump", "Animation should change to Jump")

func test_blend_animation_action():
	# アニメーションブレンドアクションのテスト
	animation_node.BlendAnimation("Walk", 0.5)
	
	# ブレンドアクションが適切に処理されることを確認
	assert_eq(animation_node.CurrentAnimation, "Walk", "Animation should change to Walk")

func test_stop_animation_action():
	# アニメーション停止アクションのテスト
	animation_node.PlayAnimation("Run")
	animation_node.StopAnimation()
	
	# 停止アクションが適切に処理されることを確認
	assert_eq(animation_node.CurrentAnimation, "Idle", "Animation should return to Idle after stop")

func test_update_animation():
	# アニメーション更新のテスト
	animation_node.Update()
	
	# アニメーションシステムが適切に更新されることを確認
	assert_eq(animation_node.CurrentAnimation, "Idle", "Animation should remain Idle after update")

func test_multiple_animation_actions():
	# 複数のアニメーションアクションのテスト
	animation_node.PlayAnimation("Jump")
	animation_node.BlendAnimation("Walk", 0.5)
	animation_node.StopAnimation()
	animation_node.Update()
	
	assert_true(animation_node.IsInitialized, "Animation node should handle multiple actions")
	assert_eq(animation_node.CurrentAnimation, "Idle", "Animation should be consistent after multiple actions")

func test_animation_actions_sequence():
	# アニメーションアクションのシーケンステスト
	animation_node.PlayAnimation("Jump")
	animation_node.Update()
	animation_node.BlendAnimation("Walk", 0.5)
	animation_node.Update()
	animation_node.StopAnimation()
	animation_node.Update()
	
	# アクションシーケンスが適切に処理されることを確認
	assert_true(animation_node.IsInitialized, "Animation node should handle action sequences")
	assert_eq(animation_node.CurrentAnimation, "Idle", "Animation should be consistent after action sequence")

func test_animation_node_lifecycle():
	# ノードのライフサイクルテスト
	var initial_animation = animation_node.CurrentAnimation
	
	# ノードを再初期化
	animation_node.queue_free()
	await get_tree().process_frame
	
	animation_node = preload("res://Scripts/Systems/Player/Animation/PlayerAnimationViewModelNode.cs").new()
	add_child(animation_node)
	animation_node.Initialize()
	
	assert_true(animation_node.IsInitialized, "Animation node should be reinitializable")
	assert_eq(animation_node.CurrentAnimation, "Idle", "Animation should be reset after reinitialization")

func test_animation_performance():
	# パフォーマンステスト
	for i in range(100):
		animation_node.Update()
	
	assert_true(animation_node.IsInitialized, "Animation node should handle performance test")
	assert_eq(animation_node.CurrentAnimation, "Idle", "Animation should remain consistent after performance test")

func test_animation_error_handling():
	# エラーハンドリングテスト
	# 初期化前にアクションを呼び出した場合の動作を確認
	var temp_node = preload("res://Scripts/Systems/Player/Animation/PlayerAnimationViewModelNode.cs").new()
	add_child(temp_node)
	
	# 初期化前にアクションを呼び出し
	temp_node.PlayAnimation("Jump")
	temp_node.BlendAnimation("Walk", 0.5)
	temp_node.StopAnimation()
	temp_node.Update()
	
	# エラーが発生せずに動作することを確認
	assert_false(temp_node.IsInitialized, "Node should not be initialized before Initialize() call")
	
	# 初期化
	temp_node.Initialize()
	assert_true(temp_node.IsInitialized, "Node should be initialized after Initialize() call")
	
	remove_child(temp_node)
	temp_node.queue_free()
	await get_tree().process_frame

func test_animation_state_consistency():
	# アニメーション状態の一貫性テスト
	var initial_animation = animation_node.CurrentAnimation
	
	animation_node.Update()
	
	# 状態が一貫していることを確認
	assert_eq(animation_node.CurrentAnimation, initial_animation, "Animation should remain consistent")

func test_animation_concurrent_operations():
	# 並行操作テスト
	# 複数の操作を同時に実行
	animation_node.PlayAnimation("Jump")
	animation_node.BlendAnimation("Walk", 0.5)
	animation_node.StopAnimation()
	
	# 並行操作後も正常に動作することを確認
	assert_true(animation_node.IsInitialized, "Animation node should handle concurrent operations")
	assert_eq(animation_node.CurrentAnimation, "Idle", "Animation should be consistent after concurrent operations")

func test_animation_action_combinations():
	# アニメーションアクションの組み合わせテスト
	# 再生、ブレンド、停止を組み合わせて実行
	animation_node.PlayAnimation("Jump")
	animation_node.BlendAnimation("Walk", 0.5)
	animation_node.StopAnimation()
	animation_node.Update()
	
	# 組み合わせアクションが適切に処理されることを確認
	assert_true(animation_node.IsInitialized, "Animation node should handle action combinations")
	assert_eq(animation_node.CurrentAnimation, "Idle", "Animation should be consistent after action combinations")

func test_invalid_animation_names():
	# 無効なアニメーション名のテスト
	animation_node.PlayAnimation("InvalidAnimation")
	animation_node.PlayAnimation("")
	animation_node.PlayAnimation("NonExistentAnimation")
	
	# 無効なアニメーション名後もシステムが正常に動作することを確認
	assert_true(animation_node.IsInitialized, "Animation node should handle invalid animation names")
	assert_eq(animation_node.CurrentAnimation, "Idle", "Animation should remain consistent after invalid names")

func test_extreme_blend_values():
	# 極端なブレンド値のテスト
	animation_node.BlendAnimation("Walk", -1.0)  # 負のブレンド値
	animation_node.BlendAnimation("Walk", 2.0)   # 1を超えるブレンド値
	animation_node.BlendAnimation("Walk", 0.0)   # ゼロブレンド値
	
	# 極端な値後もシステムが正常に動作することを確認
	assert_true(animation_node.IsInitialized, "Animation node should handle extreme blend values")
	assert_eq(animation_node.CurrentAnimation, "Walk", "Animation should be consistent after extreme blend values")

func test_animation_transitions():
	# アニメーション遷移のテスト
	animation_node.PlayAnimation("Idle")
	assert_eq(animation_node.CurrentAnimation, "Idle", "Should transition to Idle")
	
	animation_node.PlayAnimation("Walk")
	assert_eq(animation_node.CurrentAnimation, "Walk", "Should transition to Walk")
	
	animation_node.PlayAnimation("Run")
	assert_eq(animation_node.CurrentAnimation, "Run", "Should transition to Run")
	
	animation_node.PlayAnimation("Jump")
	assert_eq(animation_node.CurrentAnimation, "Jump", "Should transition to Jump")

func test_animation_loop_behavior():
	# アニメーションループ動作のテスト
	for i in range(5):
		animation_node.PlayAnimation("Walk")
		animation_node.Update()
		animation_node.StopAnimation()
		animation_node.Update()
	
	# ループ動作が適切に処理されることを確認
	assert_true(animation_node.IsInitialized, "Animation node should handle loop behavior")
	assert_eq(animation_node.CurrentAnimation, "Idle", "Animation should be Idle after loop") 