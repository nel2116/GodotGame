extends Node

# イベントシグナル定義
signal game_started
signal game_paused
signal game_resumed
signal game_ended

signal player_damaged(amount: float, source: Node)
signal player_healed(amount: float, source: Node)
signal player_died

signal enemy_damaged(enemy: Node, amount: float, source: Node)
signal enemy_died(enemy: Node)

signal room_entered(room_id: String)
signal room_cleared(room_id: String)

# イベントリスナーの管理
var _listeners: Dictionary = {}

func _init() -> void:
    name = "EventBus"

# イベントリスナーの登録
func add_listener(event_name: String, listener: Callable) -> void:
    if not _listeners.has(event_name):
        _listeners[event_name] = []
    _listeners[event_name].append(listener)

# イベントリスナーの解除
func remove_listener(event_name: String, listener: Callable) -> void:
    if _listeners.has(event_name):
        _listeners[event_name].erase(listener)

# イベントの発火
func emit_event(event_name: String, args: Array = []) -> void:
    if _listeners.has(event_name):
        for listener in _listeners[event_name]:
            if listener.is_valid():
                listener.callv(args)

# デバッグ用：現在のリスナー一覧を取得
func get_listener_count(event_name: String) -> int:
    if _listeners.has(event_name):
        return _listeners[event_name].size()
    return 0
