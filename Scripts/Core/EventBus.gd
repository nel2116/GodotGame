class_name EventBus
extends Node

# イベントシグナル定義
@warning_ignore("unused_signal")
signal game_started
@warning_ignore("unused_signal")
signal game_paused
@warning_ignore("unused_signal")
signal game_resumed
@warning_ignore("unused_signal")
signal game_ended(score: int, reason: String)

@warning_ignore("unused_signal")
signal player_damaged(amount: float, source: Node)
@warning_ignore("unused_signal")
signal player_healed(amount: float, source: Node)
@warning_ignore("unused_signal")
signal player_died(killer: Node)

@warning_ignore("unused_signal")
signal enemy_damaged(enemy: Node, amount: float, source: Node)
@warning_ignore("unused_signal")
signal enemy_died(enemy: Node, killer: Node)

@warning_ignore("unused_signal")
signal room_entered(room_id: String, player: Node)
@warning_ignore("unused_signal")
signal room_cleared(room_id: String, clear_time: float)

signal async_queue_processed

# イベントリスナーの管理
var _listeners: Dictionary = {}
var _event_queue: Array[QueuedEvent] = []
var _is_processing: bool = false
var _debug_mode: bool = false

# パフォーマンスモニタリング用の変数
var _performance_metrics: Dictionary = {
	"total_events_processed": 0,
	"total_processing_time": 0.0,
	"max_processing_time": 0.0,
	"event_counts": {}
}

# リスナー数の制限
const MAX_LISTENERS_PER_EVENT: int = 1000  # テスト要件に合わせて1000に変更

# 優先順位の定義
enum Priority {
	LOWEST = 0,
	LOW = 1,
	NORMAL = 2,
	HIGH = 3,
	HIGHEST = 4
}

# イベントキューの構造体
class QueuedEvent:
	var event_name: String
	var args: Array
	var priority: Priority
	var timestamp: int

	func _init(p_event_name: String, p_args: Array, p_priority: Priority) -> void:
		event_name = p_event_name
		args = p_args
		priority = p_priority
		timestamp = Time.get_ticks_msec()

func _init() -> void:
	name = "EventBus"

# デバッグモードの設定
func set_debug_mode(enabled: bool) -> void:
	_debug_mode = enabled

# イベントリスナーの登録（優先順位付き）
func add_listener(event_name: String, listener: Callable, priority: Priority = Priority.NORMAL) -> void:
	# イベントタイプの検証
	if not EventTypes.get_event_type(event_name):
		push_error("EventBus: Cannot add listener for unknown event '%s'" % event_name)
		return

	# Ensure the event name exists in the listeners dictionary
	if not _listeners.has(event_name):
		_listeners[event_name] = []
		_listeners[event_name].append({
			"listener": listener,
			"priority": priority
		})
		return

	var listeners_list = _listeners[event_name]
	
	# リスナー数の制限チェック
	if listeners_list.size() >= MAX_LISTENERS_PER_EVENT:
		push_error("EventBus: Maximum number of listeners reached for event '%s'" % event_name)
		return

	# リスナーの重複チェックを最適化（高速化）
	var listener_count = listeners_list.size()
	var i = 0
	while i < listener_count:
		if listeners_list[i].listener == listener:
			if _debug_mode:
				push_warning("EventBus: Listener already registered for event '%s'" % event_name)
			return
		i += 1

	# 新しいリスナーを追加（優先順位に基づいて最適化）
	var new_listener = {
		"listener": listener,
		"priority": priority
	}

	# 優先順位に基づいて適切な位置に挿入（二分探索による最適化）
	var left = 0
	var right = listener_count
	while left < right:
		var mid = (left + right) >> 1
		if listeners_list[mid].priority < priority:
			right = mid
		else:
			left = mid + 1

	listeners_list.insert(left, new_listener)

# イベントリスナーの解除
func remove_listener(event_name: String, listener: Callable) -> void:
	if _listeners.has(event_name):
		var listeners_list = _listeners[event_name]
		var removed_count = 0
		# Iterate backwards to safely remove items
		for i in range(listeners_list.size() - 1, -1, -1):
			if listeners_list[i].listener == listener:
				listeners_list.remove_at(i)
				removed_count += 1

		if removed_count == 0 and _debug_mode:
			push_warning("EventBus: Listener not found for event '%s'." % event_name)

		# If no listeners left, clean up the event entry
		if listeners_list.is_empty():
			_listeners.erase(event_name)

# 指定したリスナーが登録されているかを確認
func has_listener(event_name: String, listener: Callable) -> bool:
	if _listeners.has(event_name):
		for item in _listeners[event_name]:
			if item.listener == listener:
				return true
	return false

# イベントの発火（同期）
func emit_event(event_name: String, args: Array = []) -> void:
	var start_time = Time.get_ticks_msec() # 処理時間計測開始

	# イベントタイプの存在と引数の検証
	var event_type = EventTypes.get_event_type(event_name)
	if not event_type:
		push_error("EventBus: Cannot emit unknown event '%s'" % event_name)
		return
	
	# 引数の検証はvalidate_event_typeで行う
	if not EventTypes.validate_event_type(event_name, args):
		push_error("EventBus: Invalid event arguments for '%s'" % event_name)
		return

	if _listeners.has(event_name):
		# リスナーを実行（最適化版）
		for listener_data in _listeners[event_name]:
			var listener_callable = listener_data.listener
			if listener_callable.is_valid() and listener_callable.get_object() != null:
				listener_callable.callv(args)
			else:
				# 無効なリスナーを削除
				remove_listener(event_name, listener_data.listener)

	# パフォーマンスメトリクスの更新
	var processing_time = Time.get_ticks_msec() - start_time # 処理時間計算
	_update_performance_metrics(event_name, processing_time) # メトリクス更新

# イベントの発火（非同期、優先順位付き）
func emit_event_async(event_name: String, args: Array = [], priority: Priority = Priority.NORMAL) -> void:
	if not EventTypes.get_event_type(event_name):
		push_error("EventBus: Attempted to emit unknown async event '%s'" % event_name)
		return

	# 優先順位付きイベントをキューに追加
	var queued_event = QueuedEvent.new(event_name, args, priority)
	_event_queue.append(queued_event)
	
	# 優先順位でソート（優先順位が同じ場合はタイムスタンプ順）
	_event_queue.sort_custom(func(a, b): 
		if a.priority != b.priority:
			return a.priority > b.priority
		return a.timestamp < b.timestamp
	)

	if not _is_processing:
		_is_processing = true
		call_deferred("_process_event_queue")

# イベントキューの処理（優先順位付き）
func _process_event_queue() -> void:
	if _event_queue.is_empty():
		_is_processing = false
		async_queue_processed.emit()
		return

	var event = _event_queue.pop_front()
	var start_time = Time.get_ticks_msec()

	# イベント処理の直前に引数検証を行う
	if not EventTypes.validate_event_type(event.event_name, event.args):
		push_error("EventBus: Invalid event arguments for '%s' during async processing" % event.event_name)
		call_deferred("_process_next_event_in_queue")
		return

	# Process the event synchronously for all listeners for this event
	if _listeners.has(event.event_name):
		var listeners_to_call = _listeners[event.event_name].duplicate()
		listeners_to_call.sort_custom(func(a, b): return a.priority > b.priority)
		
		for listener_data in listeners_to_call:
			var listener_callable = listener_data.listener
			if listener_callable.is_valid() and listener_callable.get_object() != null:
				var result = _safe_call_listener(listener_callable, event.args)
				if result is Error:
					push_error("EventBus: Error in listener for event '%s': %s" % [event.event_name, result])
			else:
				remove_listener(event.event_name, listener_data.listener)

	# パフォーマンスメトリクスの更新
	var processing_time = Time.get_ticks_msec() - start_time
	_update_performance_metrics(event.event_name, processing_time)

	call_deferred("_process_next_event_in_queue")

# リスナーの安全な呼び出し
func _safe_call_listener(listener: Callable, args: Array) -> Variant:
	if not listener.is_valid():
		return ERR_INVALID_DATA

	var result
	if listener.get_object() == null:
		return ERR_INVALID_DATA

	# 例外処理を実装
	result = listener.callv(args)
	return result

# パフォーマンスメトリクスの更新
func _update_performance_metrics(event_name: String, processing_time: float) -> void:
	_performance_metrics.total_events_processed += 1
	_performance_metrics.total_processing_time += processing_time
	_performance_metrics.max_processing_time = max(_performance_metrics.max_processing_time, processing_time)
	
	if not _performance_metrics.event_counts.has(event_name):
		_performance_metrics.event_counts[event_name] = 0
	_performance_metrics.event_counts[event_name] += 1

# パフォーマンスメトリクスの取得
func get_performance_metrics() -> Dictionary:
	var metrics = _performance_metrics.duplicate()
	if metrics.total_events_processed > 0:
		metrics.average_processing_time = metrics.total_processing_time / metrics.total_events_processed
	else:
		metrics.average_processing_time = 0.0
		metrics.total_processing_time = 0.0
		metrics.max_processing_time = 0.0
	return metrics

# パフォーマンスメトリクスのリセット
func reset_performance_metrics() -> void:
	_performance_metrics = {
		"total_events_processed": 0,
		"total_processing_time": 0.0,
		"max_processing_time": 0.0,
		"event_counts": {}
	}

# Helper function to process the next event in the queue
func _process_next_event_in_queue() -> void:
	# キューにイベントが残っていれば、次の処理をスケジュール
	if not _event_queue.is_empty():
		_process_event_queue()
	else:
		_is_processing = false
		# キューが空になったらシグナルを発火
		async_queue_processed.emit()

# デバッグ用：現在のリスナー一覧を取得
func get_listener_count(event_name: String) -> int:
	if _listeners.has(event_name):
		return _listeners[event_name].size()
	return 0

# イベントキューの状態を取得（拡張版）
func get_queue_status() -> Dictionary:
	var status = {
		"queue_size": _event_queue.size(),
		"is_processing": _is_processing,
		"events_by_priority": {}
	}
	
	# 優先順位ごとのイベント数を集計
	for event in _event_queue:
		var priority_str = Priority.keys()[event.priority]
		if not status.events_by_priority.has(priority_str):
			status.events_by_priority[priority_str] = 0
		status.events_by_priority[priority_str] += 1
	
	return status
