extends GutTest

var event_bus: EventBus

# Test listeners defined as methods
var execution_order: Array = []
var execution_count: int = 0

func _on_test_event_high() -> void: execution_order.append("high")
func _on_test_event_normal() -> void: execution_order.append("normal")
func _on_test_event_low() -> void: execution_order.append("low")

func _on_test_event_async() -> void: execution_count += 1

func before_each() -> void:
	event_bus = EventBus.new()
	add_child(event_bus)
	# Reset test variables
	execution_order.clear()
	execution_count = 0

func after_each() -> void:
	# Clean up event bus
	if is_instance_valid(event_bus):
		# Remove all listeners before freeing
		for event_name in event_bus._listeners.keys():
			for listener_data in event_bus._listeners[event_name].duplicate():
				event_bus.remove_listener(event_name, listener_data.listener)
		event_bus.queue_free()
		event_bus = null
	
	# Reset test variables
	execution_order.clear()
	execution_count = 0
	
	# Force garbage collection
	await get_tree().process_frame

# イベントタイプの検証テスト
func test_event_type_validation() -> void:
	# デバッグ出力：登録されているイベントタイプを確認
	# EventTypes.debug_event_types()
	
	# 有効なイベントタイプのテスト
	assert_true(EventTypes.validate_event_type("test_event", []), "test_event should be valid")
	
	# 無効なイベントタイプのテスト
	assert_false(EventTypes.validate_event_type("invalid_event", []), "invalid_event should be invalid")
	
	# パラメータ付きイベントのテスト
	var test_node = Node.new()
	add_child(test_node)
	assert_true(EventTypes.validate_event_type("player_damaged", [10.0, test_node]), "player_damaged with valid parameters should be valid")
	assert_false(EventTypes.validate_event_type("player_damaged", ["invalid", test_node]), "player_damaged with invalid parameter type should be invalid")
	assert_false(EventTypes.validate_event_type("player_damaged", [10.0]), "player_damaged with missing parameters should be invalid")
	test_node.queue_free()

# 優先順位付きリスナーのテスト
func test_priority_based_listener_execution() -> void:
	# 異なる優先順位でリスナーを登録
	event_bus.add_listener("test_event", Callable(self, "_on_test_event_normal"), EventBus.Priority.NORMAL)
	event_bus.add_listener("test_event", Callable(self, "_on_test_event_high"), EventBus.Priority.HIGH)
	event_bus.add_listener("test_event", Callable(self, "_on_test_event_low"), EventBus.Priority.LOW)
	
	# イベントを発火
	event_bus.emit_event("test_event")
	
	# 優先順位の高い順に実行されることを確認
	var expected_order = ["high", "normal", "low"]
	assert_eq_deep(execution_order, expected_order)
	
	# 実行順序をリセット
	execution_order.clear()
	
	# 再度イベントを発火して、同じ順序で実行されることを確認
	event_bus.emit_event("test_event")
	assert_eq_deep(execution_order, expected_order)

# 非同期イベント処理のテスト
func test_async_event_processing() -> void:
	# リスナーを登録
	event_bus.add_listener("test_event", Callable(self, "_on_test_event_async"))
	
	# 非同期でイベントを発火
	event_bus.emit_event_async("test_event")
	event_bus.emit_event_async("test_event")
	
	# 最初は実行されていないことを確認
	assert_eq(execution_count, 0, "Events should not be processed immediately")
	
	# 非同期キュー処理完了シグナルを待つ
	await event_bus.async_queue_processed
	
	# イベントが処理されたことを確認
	assert_eq(execution_count, 2, "Events should be processed")

# イベントキューの状態確認テスト
func test_event_queue_status() -> void:
	# リスナーを登録（非同期処理が実行されるように）
	event_bus.add_listener("test_event", Callable(self, "_on_test_event_async"))
	
	# 非同期でイベントを発火
	event_bus.emit_event_async("test_event")
	
	# キューの状態を確認 (発火直後)
	var status = event_bus.get_queue_status()
	assert_eq(status.queue_size, 1, "Queue should contain one event immediately after emission")
	assert_true(status.is_processing, "Queue should be processing immediately after emission")
	
	# 非同期キュー処理完了シグナルを待つ
	await event_bus.async_queue_processed
	
	# キューが空になったことを確認 (処理完了後)
	status = event_bus.get_queue_status()
	assert_eq(status.queue_size, 0, "Queue should be empty after processing")
	assert_false(status.is_processing, "Queue should not be processing after completion")

# リスナーの解除テスト（優先順位付き）
func test_remove_listener_with_priority() -> void:
	# リスナーを定義済みメソッドとして登録
	var listener_callable = Callable(self, "_on_test_event_async")
	event_bus.add_listener("test_event", listener_callable, EventBus.Priority.HIGH)
	
	# イベントを発火して実行を確認
	event_bus.emit_event("test_event")
	assert_eq(execution_count, 1, "Listener should be executed")
	
	# リスナーを解除
	event_bus.remove_listener("test_event", listener_callable)
	
	# 実行カウントをリセット
	execution_count = 0
	
	# 再度イベントを発火
	event_bus.emit_event("test_event")
	assert_eq(execution_count, 0, "Listener should not be executed after removal")

# 無効なイベント名のテスト
func test_invalid_event_name() -> void:
	var listener_callable = Callable(self, "_on_test_event_async")
	
	# 無効なイベント名でリスナーを登録
	event_bus.add_listener("invalid_event", listener_callable)
	
	# リスナーが登録されていないことを確認
	assert_false(event_bus.has_listener("invalid_event", listener_callable))
	
	# 無効なイベント名でイベントを発火
	event_bus.emit_event("invalid_event")
	
	# 実行カウントが増えていないことを確認
	assert_eq(execution_count, 0, "Invalid event should not be processed")

# 無効なパラメータのテスト
func test_invalid_event_parameters() -> void:
	var listener_callable = Callable(self, "_on_test_event_async")
	
	# リスナーを登録
	event_bus.add_listener("player_damaged", listener_callable)
	
	# 無効なパラメータでイベントを発火
	# テスト用に作成したNodeインスタンスを保持し、テストの最後に解放します
	var invalid_node_param = Node.new()
	event_bus.emit_event("player_damaged", ["invalid", invalid_node_param])
	
	# 実行カウントが増えていないことを確認
	assert_eq(execution_count, 0, "Event with invalid parameters should not be processed")
	
	# 作成したNodeインスタンスを解放
	invalid_node_param.queue_free()
