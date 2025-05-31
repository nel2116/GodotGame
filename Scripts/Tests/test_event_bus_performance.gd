extends GutTest

var event_bus: EventBus
var test_results: Dictionary = {}
var _suppress_warnings: bool = false

# テスト用のリスナー
func _on_test_event(_listener_id: int) -> void:
	pass

# 警告を抑制するカスタム関数
func _push_warning(message: String) -> void:
	if not _suppress_warnings:
		push_warning(message)

# テスト実行前の準備
func before_each() -> void:
	# EventTypesの初期化を確実に行う
	EventTypes.get_event_type("test_event")
	
	event_bus = EventBus.new()
	add_child(event_bus)
	test_results = {
		"listener_add_time": [],
		"event_emit_time": [],
		"async_event_emit_time": [],
		"memory_usage": []
	}
	_suppress_warnings = false

# テスト実行後のクリーンアップ
func after_each() -> void:
	if is_instance_valid(event_bus):
		# すべてのリスナーを解除
		for event_name in event_bus._listeners.keys():
			for listener_data in event_bus._listeners[event_name].duplicate():
				event_bus.remove_listener(event_name, listener_data.listener)
		event_bus._listeners.clear()
		event_bus.queue_free()
		await event_bus.tree_exited
		event_bus = null
	test_results.clear()
	_suppress_warnings = false
	
	# メモリの解放を待機
	await get_tree().process_frame
	await get_tree().process_frame

# メモリ使用量を取得
func get_memory_usage() -> int:
	return OS.get_static_memory_usage()

# パフォーマンス計測用のヘルパー関数
func measure_execution_time(callable: Callable) -> float:
	var start_time = Time.get_ticks_msec()
	callable.call()
	return Time.get_ticks_msec() - start_time

# 一意のリスナーを作成
func create_unique_listener() -> Callable:
	# 各リスナーに一意のIDを付与
	var listener_id = randi()
	return Callable(self, "_on_test_event").bind(listener_id)

# イベントタイプの検証パフォーマンステスト
func test_event_type_validation_performance() -> void:
	var test_count = 1000
	var total_time = 0.0
	
	# 有効なイベントタイプのテスト
	for i in range(test_count):
		var execution_time = measure_execution_time(
			func(): EventTypes.validate_event_type("test_event", [])
		)
		total_time += execution_time
		test_results.listener_add_time.append(execution_time)
	
	var average_time = total_time / test_count
	var max_time = test_results.listener_add_time.max()
	var min_time = test_results.listener_add_time.min()
	
	# パフォーマンス基準の設定（調整版）
	var max_allowed_time = 2.0  # 2ms
	var max_allowed_average = 0.05  # 0.05ms
	
	# アサーション
	assert_true(average_time < max_allowed_average, 
		"イベントタイプ検証の平均実行時間が許容値を超えています: %.2f ms" % average_time)
	assert_true(max_time < max_allowed_time,
		"イベントタイプ検証の最大実行時間が許容値を超えています: %.2f ms" % max_time)
	
	# 結果の出力
	print("イベントタイプ検証の平均実行時間: %.2f ms" % average_time)
	print("最大実行時間: %.2f ms" % max_time)
	print("最小実行時間: %.2f ms" % min_time)

# リスナー追加のパフォーマンステスト
func test_listener_add_performance() -> void:
	var listener_count = 1000
	var total_time = 0.0
	
	# テスト用のイベント名を使用
	var event_name = "test_event"
	
	# 既存のリスナーをクリア
	event_bus._listeners.clear()
	
	for i in range(listener_count):
		var listener = create_unique_listener()
		var priority = i % 5  # 優先順位をランダムに設定
		
		var execution_time = measure_execution_time(
			func(): event_bus.add_listener(event_name, listener, priority)
		)
		total_time += execution_time
		test_results.listener_add_time.append(execution_time)
	
	var average_time = total_time / listener_count
	var max_time = test_results.listener_add_time.max()
	var min_time = test_results.listener_add_time.min()
	
	# パフォーマンス基準の設定
	var max_allowed_time = 2.0  # 2ms
	var max_allowed_average = 0.1  # 0.1ms
	
	# アサーション
	assert_true(average_time < max_allowed_average, 
		"リスナー追加の平均実行時間が許容値を超えています: %.2f ms" % average_time)
	assert_true(max_time < max_allowed_time,
		"リスナー追加の最大実行時間が許容値を超えています: %.2f ms" % max_time)
	
	# 結果の出力
	print("リスナー追加の平均実行時間: %.2f ms" % average_time)
	print("最大実行時間: %.2f ms" % max_time)
	print("最小実行時間: %.2f ms" % min_time)

# イベント発火のパフォーマンステスト
func test_event_emit_performance() -> void:
	# テスト用のイベント名を使用
	var event_name = "test_event"
	
	# 1000個のリスナーを登録
	for i in range(1000):
		var listener = create_unique_listener()
		event_bus.add_listener(event_name, listener)
	
	var emit_count = 100
	var total_time = 0.0
	
	for i in range(emit_count):
		var execution_time = measure_execution_time(
			func(): event_bus.emit_event(event_name)
		)
		total_time += execution_time
		test_results.event_emit_time.append(execution_time)
	
	var average_time = total_time / emit_count
	var max_time = test_results.event_emit_time.max()
	var min_time = test_results.event_emit_time.min()
	
	# パフォーマンス基準の設定
	var max_allowed_time = 5.0  # 5ms
	var max_allowed_average = 2.0  # 2ms
	
	# アサーション
	assert_true(average_time < max_allowed_average,
		"イベント発火の平均実行時間が許容値を超えています: %.2f ms" % average_time)
	assert_true(max_time < max_allowed_time,
		"イベント発火の最大実行時間が許容値を超えています: %.2f ms" % max_time)
	
	# 結果の出力
	print("イベント発火の平均実行時間: %.2f ms" % average_time)
	print("最大実行時間: %.2f ms" % max_time)
	print("最小実行時間: %.2f ms" % min_time)

# 非同期イベント処理のパフォーマンステスト
func test_async_event_performance() -> void:
	# テスト用のイベント名を使用
	var event_name = "test_event_async"
	
	# 1000個のリスナーを登録
	for i in range(1000):
		var listener = create_unique_listener()
		# 重複登録を防ぐため、既存のリスナーを確認
		if not event_bus.has_listener(event_name, listener):
			event_bus.add_listener(event_name, listener)
	
	var emit_count = 100
	var total_time = 0.0
	
	for i in range(emit_count):
		var execution_time = measure_execution_time(
			func(): event_bus.emit_event_async(event_name)
		)
		total_time += execution_time
		test_results.async_event_emit_time.append(execution_time)
	
	# 非同期処理の完了を待つ
	await event_bus.async_queue_processed
	
	var average_time = total_time / emit_count
	var max_time = test_results.async_event_emit_time.max()
	var min_time = test_results.async_event_emit_time.min()
	
	# パフォーマンス基準の設定
	var max_allowed_time = 10.0  # 10ms
	var max_allowed_average = 5.0  # 5ms
	
	# アサーション
	assert_true(average_time < max_allowed_average,
		"非同期イベント発火の平均実行時間が許容値を超えています: %.2f ms" % average_time)
	assert_true(max_time < max_allowed_time,
		"非同期イベント発火の最大実行時間が許容値を超えています: %.2f ms" % max_time)
	
	# 結果の出力
	print("非同期イベント発火の平均実行時間: %.2f ms" % average_time)
	print("最大実行時間: %.2f ms" % max_time)
	print("最小実行時間: %.2f ms" % min_time)

# メモリ使用量のテスト
func test_memory_usage() -> void:
	# 警告の抑制を開始
	_suppress_warnings = true
	
	# テスト用のイベント名を使用
	var event_name = "test_event"
	
	var initial_memory = get_memory_usage()
	test_results.memory_usage.append(initial_memory)
	
	# 1000個のリスナーを登録
	for i in range(1000):
		var listener = create_unique_listener()
		# 重複登録を防ぐため、既存のリスナーを確認
		if not event_bus.has_listener(event_name, listener):
			event_bus.add_listener(event_name, listener)
	
	var memory_after_listeners = get_memory_usage()
	test_results.memory_usage.append(memory_after_listeners)
	
	# 100回イベントを発火
	for i in range(100):
		event_bus.emit_event(event_name)
	
	var memory_after_events = get_memory_usage()
	test_results.memory_usage.append(memory_after_events)
	
	# メモリ使用量の計算
	var listener_memory_increase = memory_after_listeners - initial_memory
	var event_memory_increase = memory_after_events - memory_after_listeners
	
	# メモリ使用量の基準設定
	var max_allowed_listener_memory = 1024 * 1000  # 1MB
	var max_allowed_event_memory = 1024 * 10  # 10KB
	
	# アサーション
	assert_true(listener_memory_increase < max_allowed_listener_memory,
		"リスナー登録によるメモリ増加量が許容値を超えています: %d bytes" % listener_memory_increase)
	assert_true(event_memory_increase < max_allowed_event_memory,
		"イベント発火によるメモリ増加量が許容値を超えています: %d bytes" % event_memory_increase)
	
	# 結果の出力
	print("初期メモリ使用量: %d bytes" % initial_memory)
	print("リスナー登録後のメモリ使用量: %d bytes" % memory_after_listeners)
	print("イベント発火後のメモリ使用量: %d bytes" % memory_after_events)
	print("リスナー登録による増加量: %d bytes" % listener_memory_increase)
	print("イベント発火による増加量: %d bytes" % event_memory_increase)
	
	# 警告の抑制を解除
	_suppress_warnings = false

# パフォーマンステストの実行
func run_performance_tests() -> void:
	print("=== EventBus パフォーマンステスト開始 ===")
	
	test_event_type_validation_performance()
	test_listener_add_performance()
	test_event_emit_performance()
	test_async_event_performance()
	test_memory_usage()
	
	print("=== EventBus パフォーマンステスト終了 ===") 
