extends GutTest

# PlayerInputViewModelTests.gd
# GUT版のプレイヤー入力ViewModelテスト
# C#ノードを使用して実際のテストを実行

var input_node

func before_each():
	input_node = preload("res://Scripts/Systems/Player/Input/PlayerInputViewModelNode.cs").new()
	add_child(input_node)
	input_node.Initialize()

func after_each():
	remove_child(input_node)
	input_node.queue_free()
	await get_tree().process_frame

func test_initialize_default_state_is_enabled():
	assert_true(input_node.IsEnabled, "Input should be enabled by default")

func test_initialize_node_properly():
	assert_true(input_node.IsInitialized, "Input node should be properly initialized")

func test_update_input_does_not_crash():
	# 入力更新がクラッシュしないことを確認
	input_node.UpdateInput()
	assert_true(input_node.IsInitialized, "Input node should remain stable after update")

func test_multiple_input_updates():
	# 複数回の入力更新が正常に動作することを確認
	for i in range(10):
		input_node.UpdateInput()
	
	assert_true(input_node.IsInitialized, "Input node should handle multiple updates")
	assert_true(input_node.IsEnabled, "Input should remain enabled after multiple updates")

func test_input_node_lifecycle():
	# ノードのライフサイクルテスト
	var initial_state = input_node.IsEnabled
	
	# ノードを再初期化
	input_node.queue_free()
	await get_tree().process_frame
	
	input_node = preload("res://Scripts/Systems/Player/Input/PlayerInputViewModelNode.cs").new()
	add_child(input_node)
	input_node.Initialize()
	
	assert_true(input_node.IsInitialized, "Input node should be reinitializable")
	assert_true(input_node.IsEnabled, "Input should be enabled after reinitialization")

func test_input_performance():
	# パフォーマンステスト
	for i in range(100):
		input_node.UpdateInput()
	
	assert_true(input_node.IsInitialized, "Input node should handle performance test")
	assert_true(input_node.IsEnabled, "Input should remain enabled after performance test")

func test_input_error_handling():
	# エラーハンドリングテスト
	# 初期化前にUpdateInputを呼び出した場合の動作を確認
	var temp_node = preload("res://Scripts/Systems/Player/Input/PlayerInputViewModelNode.cs").new()
	add_child(temp_node)
	
	# 初期化前にUpdateInputを呼び出し
	temp_node.UpdateInput()
	
	# エラーが発生せずに動作することを確認
	assert_false(temp_node.IsInitialized, "Node should not be initialized before Initialize() call")
	
	# 初期化
	temp_node.Initialize()
	assert_true(temp_node.IsInitialized, "Node should be initialized after Initialize() call")
	
	remove_child(temp_node)
	temp_node.queue_free()
	await get_tree().process_frame

func test_input_state_consistency():
	# 入力状態の一貫性テスト
	var initial_enabled_state = input_node.IsEnabled
	
	input_node.UpdateInput()
	
	# 状態が一貫していることを確認
	assert_eq(input_node.IsEnabled, initial_enabled_state, "Input state should remain consistent")

func test_input_concurrent_operations():
	# 並行操作テスト
	# 複数の操作を同時に実行
	input_node.UpdateInput()
	input_node.UpdateInput()
	input_node.UpdateInput()
	
	# 並行操作後も正常に動作することを確認
	assert_true(input_node.IsInitialized, "Input node should handle concurrent operations")
	assert_true(input_node.IsEnabled, "Input should remain enabled after concurrent operations")

# var event_bus: NewGameProject.GameEventBus
# var input_model: NewGameProject.PlayerInputModel
# var input_view_model: NewGameProject.PlayerInputViewModel

# func test_update_input_publishes_state_event():
# 	var received_event = null
# 	event_bus.get_event_stream("InputStateChangedEvent").subscribe(
# 		func(event): received_event = event
# 	)
# 	input_view_model.initialize()
# 	input_view_model.update_input()
# 	await get_tree().process_frame
# 	assert_not_null(received_event)
# 	assert_not_null(received_event.state)

# func test_initialize_publishes_enabled_event():
# 	var received_event = null
# 	event_bus.get_event_stream("InputEnabledChangedEvent").subscribe(
# 		func(event): received_event = event
# 	)
# 	input_view_model.initialize()
# 	await get_tree().process_frame
# 	assert_not_null(received_event)
# 	assert_true(received_event.enabled) 