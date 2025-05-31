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
signal game_ended

@warning_ignore("unused_signal")
signal player_damaged(amount: float, source: Node)
@warning_ignore("unused_signal")
signal player_healed(amount: float, source: Node)
@warning_ignore("unused_signal")
signal player_died

@warning_ignore("unused_signal")
signal enemy_damaged(enemy: Node, amount: float, source: Node)
@warning_ignore("unused_signal")
signal enemy_died(enemy: Node)

@warning_ignore("unused_signal")
signal room_entered(room_id: String)
@warning_ignore("unused_signal")
signal room_cleared(room_id: String)

signal async_queue_processed # Add a signal to indicate completion of the async queue processing

# イベントリスナーの管理
var _listeners: Dictionary = {}
var _event_queue: Array = []
var _is_processing: bool = false
var _debug_mode: bool = false  # デバッグモードフラグ

# 優先順位の定義
enum Priority {
	LOWEST = 0,
	LOW = 1,
	NORMAL = 2,
	HIGH = 3,
	HIGHEST = 4
}

func _init() -> void:
	name = "EventBus"

# デバッグモードの設定
func set_debug_mode(enabled: bool) -> void:
	_debug_mode = enabled

# イベントリスナーの登録（優先順位付き）
func add_listener(event_name: String, listener: Callable, priority: Priority = Priority.NORMAL) -> void:
	# Ensure the event name exists in the listeners dictionary
	if not _listeners.has(event_name):
		_listeners[event_name] = []

	# Check if the listener is already registered for this event
	for item in _listeners[event_name]:
		if item.listener == listener:
			if _debug_mode:
				# テストクラスの警告抑制メカニズムを使用
				if get_parent() and get_parent().has_method("_push_warning"):
					get_parent()._push_warning("EventBus: Listener already registered for event '%s'." % event_name)
				else:
					push_warning("EventBus: Listener already registered for event '%s'." % event_name)
			return

	# Add the listener with priority
	_listeners[event_name].append({
		"listener": listener,
		"priority": priority
	})

	# Sort listeners by priority in descending order (HIGHEST first)
	_listeners[event_name].sort_custom(func(a, b): return a.priority > b.priority)

# イベントリスナーの解除
func remove_listener(event_name: String, listener: Callable) -> void:
	if _listeners.has(event_name):
		var listeners_list = _listeners[event_name]
		var removed_count = 0
		# Iterate backwards to safely remove items
		for i in range(listeners_list.size() - 1, -1, -1):
			# Compare callable objects directly
			if listeners_list[i].listener == listener:
				listeners_list.remove_at(i)
				removed_count += 1

		if removed_count == 0:
			push_warning("EventBus: Listener not found for event '%s'." % event_name)

		# If no listeners left, clean up the event entry (optional)
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
	if _listeners.has(event_name):
		# Iterate over a copy to avoid issues if listeners are removed during emission
		var listeners_to_call = _listeners[event_name].duplicate()
		for listener_data in listeners_to_call:
			var listener_callable = listener_data.listener # Get the callable object
			# Ensure the callable is valid and can be called
			if listener_callable.is_valid() and listener_callable.get_object() != null:
				listener_callable.callv(args)
			else:
				 # Clean up invalid listeners
				 # Need to find the actual listener object in the original list to remove
				 # Use the remove_listener method which handles finding the callable
				remove_listener(event_name, listener_data.listener)

# イベントの発火（非同期）
func emit_event_async(event_name: String, args: Array = []) -> void:
	# Add the event to the queue
	_event_queue.append({
		"event_name": event_name,
		"args": args
	})

	if not _is_processing:
		_is_processing = true
		# Use call_deferred to ensure the processing starts in the next frame
		call_deferred("_process_event_queue")

# イベントキューの処理
func _process_event_queue() -> void:
	if _event_queue.is_empty():
		_is_processing = false
		async_queue_processed.emit()
		return

	var event = _event_queue.pop_front()

	# Process the event synchronously for all listeners for this event
	if _listeners.has(event.event_name):
		var listeners_to_call = _listeners[event.event_name].duplicate()
		for listener_data in listeners_to_call:
			var listener_callable = listener_data.listener
			if listener_callable.is_valid() and listener_callable.get_object() != null:
				listener_callable.callv(event.args)
			else:
				remove_listener(event.event_name, listener_data.listener)

	# Schedule the next event processing for the next frame
	call_deferred("_process_next_event_in_queue")

# Helper function to process the next event in the queue
func _process_next_event_in_queue() -> void:
	if not _event_queue.is_empty():
		_process_event_queue()
	else:
		_is_processing = false
		async_queue_processed.emit()

# デバッグ用：現在のリスナー一覧を取得
func get_listener_count(event_name: String) -> int:
	if _listeners.has(event_name):
		return _listeners[event_name].size()
	return 0

# デバッグ用：イベントキューの状態を取得
func get_queue_status() -> Dictionary:
	return {
		"queue_size": _event_queue.size(),
		"is_processing": _is_processing
	}
