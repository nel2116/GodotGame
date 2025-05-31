extends GutTest

var event_bus: EventBus

# Test listeners defined as methods
var execution_order: Array = []
var execution_count: int = 0

# テスト用の一時的なリスナーNodeクラス
class _TestListenerNode extends Node:
	var called_count = 0
	var listener_id = -1

	func _init(id: int):
		listener_id = id

	func _on_test_event_async(_args = null):
		called_count += 1

func _on_test_event_high() -> void: execution_order.append("high")
func _on_test_event_normal() -> void: execution_order.append("normal")
func _on_test_event_low() -> void: execution_order.append("low")

func _on_test_event_async(_listener_id = null) -> void: execution_count += 1

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

# 優先順位付きイベントキューのテスト
func test_priority_based_event_queue() -> void:
	# 異なる優先順位でイベントを発火
	event_bus.emit_event_async("test_event", [], EventBus.Priority.LOW)
	event_bus.emit_event_async("test_event", [], EventBus.Priority.HIGH)
	event_bus.emit_event_async("test_event", [], EventBus.Priority.NORMAL)
	
	# キューの状態を確認
	var status = event_bus.get_queue_status()
	assert_eq(status.queue_size, 3, "Queue should contain three events")
	assert_true(status.events_by_priority.has("HIGH"), "Should have HIGH priority events")
	assert_true(status.events_by_priority.has("NORMAL"), "Should have NORMAL priority events")
	assert_true(status.events_by_priority.has("LOW"), "Should have LOW priority events")
	
	# 非同期キュー処理完了シグナルを待つ
	await event_bus.async_queue_processed
	
	# キューが空になったことを確認
	status = event_bus.get_queue_status()
	assert_eq(status.queue_size, 0, "Queue should be empty after processing")

# リスナー数制限のテスト
func test_listener_limit() -> void:
	var event_name = "test_event"
	var listener_nodes = [] # ダミーリスナーNodeを保持する配列

	# 最大数のリスナーを登録
	for i in range(EventBus.MAX_LISTENERS_PER_EVENT):
		# ユニークなNodeインスタンスを作成し、そのメソッドをリスナーとして登録
		var listener_node = _TestListenerNode.new(i)
		add_child(listener_node) # シーントゥリーに追加して有効にする
		listener_nodes.append(listener_node)
		var listener = Callable(listener_node, "_on_test_event_async")
		event_bus.add_listener(event_name, listener)

	# リスナー数が制限に達していることを確認
	assert_eq(event_bus.get_listener_count(event_name), EventBus.MAX_LISTENERS_PER_EVENT)

	# 追加のリスナー登録を試みる
	var extra_listener_node = _TestListenerNode.new(EventBus.MAX_LISTENERS_PER_EVENT + 1)
	add_child(extra_listener_node)
	var extra_listener = Callable(extra_listener_node, "_on_test_event_async")
	event_bus.add_listener(event_name, extra_listener)

	# リスナー数が増えていないことを確認
	assert_eq(event_bus.get_listener_count(event_name), EventBus.MAX_LISTENERS_PER_EVENT)

	# テスト後クリーンアップ
	for node in listener_nodes:
		if is_instance_valid(node):
			node.queue_free()
	if is_instance_valid(extra_listener_node):
		extra_listener_node.queue_free()

# エラーハンドリングのテスト
func test_error_handling() -> void:
	var event_name = "test_event"
	
	# 無効なリスナーを登録
	var invalid_listener = Callable(null, "non_existent_method")
	event_bus.add_listener(event_name, invalid_listener)
	
	# イベントを発火
	event_bus.emit_event(event_name)
	
	# 無効なリスナーが自動的に削除されていることを確認
	assert_false(event_bus.has_listener(event_name, invalid_listener))

# パフォーマンスメトリクスのテスト
func test_performance_metrics() -> void:
	var event_name = "test_event"
	
	# リスナーを登録
	event_bus.add_listener(event_name, Callable(self, "_on_test_event_async"))
	
	# イベントを発火
	event_bus.emit_event(event_name)
	
	# パフォーマンスメトリクスを取得
	var metrics = event_bus.get_performance_metrics()
	
	# メトリクスの存在を確認
	assert_true(metrics.has("total_events_processed"), "Should have total_events_processed")
	assert_true(metrics.has("total_processing_time"), "Should have total_processing_time")
	assert_true(metrics.has("max_processing_time"), "Should have max_processing_time")
	assert_true(metrics.has("event_counts"), "Should have event_counts")
	
	# イベントが処理されたことを確認
	assert_eq(metrics.total_events_processed, 1, "Should have processed one event")
	assert_true(metrics.event_counts.has(event_name), "Should have event count for test_event")
	assert_eq(metrics.event_counts[event_name], 1, "Should have processed test_event once")
	
	# メトリクスをリセット
	event_bus.reset_performance_metrics()
	
	# リセット後のメトリクスを確認
	metrics = event_bus.get_performance_metrics()
	assert_eq(metrics.total_events_processed, 0, "Metrics should be reset")
	assert_eq(metrics.total_processing_time, 0.0, "Processing time should be reset")
	assert_eq(metrics.max_processing_time, 0.0, "Max processing time should be reset")
	assert_true(metrics.event_counts.is_empty(), "Event counts should be empty")
