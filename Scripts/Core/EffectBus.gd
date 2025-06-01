class_name EffectBus
extends Node

const TestEffect = preload("res://Scripts/Examples/TestEffect.gd")

# エフェクトシグナル定義
signal effect_triggered(effect_name: String, params: Dictionary)
@warning_ignore("unused_signal")
signal vfx_played(vfx_name: String, position: Vector3)
@warning_ignore("unused_signal")
signal se_played(se_name: String)
@warning_ignore("unused_signal")
signal camera_shake(intensity: float, duration: float)
signal effect_paused(effect_name: String)
signal effect_resumed(effect_name: String)
signal error_occurred(message: String)

# エフェクトリソースの管理
var _effect_resources: Dictionary = {}
var _effect_priorities: Dictionary = {}
var _effect_pool: EffectPool
var _effect_blender: EffectBlender

# EventBusへの参照
var _event_bus: EventBus

# デバッグモード
var _debug_mode: bool = false

# エフェクトの優先度管理
class PriorityQueue:
	var _queue: Array = []
	
	func push(effect_name: String, priority: int) -> void:
		_queue.append({"name": effect_name, "priority": priority})
		_queue.sort_custom(func(a, b): return a.priority > b.priority)
		print("Debug: Added effect '%s' to priority queue with priority %d" % [effect_name, priority])
	
	func pop() -> Dictionary:
		return _queue.pop_front() if not _queue.is_empty() else {}
	
	func clear() -> void:
		_queue.clear()
		print("Debug: Priority queue cleared")

var _priority_queue: PriorityQueue = PriorityQueue.new()

# エラーメッセージテンプレート
const ERROR_TEMPLATES = {
	"invalid_effect": "未登録のエフェクト '%s' が呼び出されました",
	"duplicate_effect": "エフェクト '%s' は既に登録されています",
	"invalid_resource": "無効なエフェクトリソース '%s'",
	"invalid_priority": "無効な優先度 '%d' が指定されました",
	"invalid_pool_size": "無効なプールサイズ '%d' が指定されました",
	"invalid_blend_mode": "無効なブレンドモード '%d' が指定されました",
	"invalid_duration": "無効な再生時間 '%f' が指定されました (%sから)",
	"invalid_damage_source": "無効なダメージソース",
	"invalid_enemy": "無効な敵",
	"invalid_enemy_source": "無効な敵またはダメージソース",
	"effect_not_found": "エフェクト '%s' は中断機能をサポートしていません",
	"effect_resume_not_found": "エフェクト '%s' は再開機能をサポートしていません"
}

func _init() -> void:
	name = "EffectBus"
	_setup_default_priorities()
	_effect_pool = EffectPool.new()
	_effect_blender = EffectBlender.new()
	add_child(_effect_pool)
	add_child(_effect_blender)
	
	# シグナルの接続
	_effect_blender.blend_completed.connect(_on_blend_completed)
	print("Debug: EffectBus initialized")

func _setup_default_priorities() -> void:
	# デフォルトの優先順位を設定
	_effect_priorities = {
		"player_death": 100,
		"enemy_death": 90,
		"player_hit": 80,
		"enemy_hit": 70,
		"camera_shake": 60
	}

# EventBusのリスナー設定
func setup_event_listeners(event_bus: EventBus) -> void:
	if not event_bus:
		push_error("EffectBus: Attempted to setup event listeners with null EventBus")
		return
		
	_event_bus = event_bus

	# イベントとエフェクトの紐付け
	if _event_bus.has_signal("player_damaged"):
		_event_bus.player_damaged.connect(_on_player_damaged)
	if _event_bus.has_signal("enemy_damaged"):
		_event_bus.enemy_damaged.connect(_on_enemy_damaged)
	if _event_bus.has_signal("player_died"):
		_event_bus.player_died.connect(_on_player_died)
	if _event_bus.has_signal("enemy_died"):
		_event_bus.enemy_died.connect(_on_enemy_died)
	
	print("Debug: Event listeners setup completed")

# エフェクトリソースの登録
func register_effect(effect_name: String, resource: Resource, priority: int = 50, pool_size: int = 10, blend_mode: EffectBlender.BlendMode = EffectBlender.BlendMode.NORMAL) -> void:
	if not resource:
		_log_error(_format_error("invalid_resource", [effect_name]))
		return
		
	if _effect_resources.has(effect_name):
		_log_error(_format_error("duplicate_effect", [effect_name]))
		return
		
	if priority < 0:
		_log_error(_format_error("invalid_priority", [priority]))
		return
		
	if pool_size <= 0:
		_log_error(_format_error("invalid_pool_size", [pool_size]))
		return
		
	if blend_mode < 0 or blend_mode >= EffectBlender.BlendMode.size():
		_log_error(_format_error("invalid_blend_mode", [blend_mode]))
		return
		
	_effect_resources[effect_name] = resource
	_effect_priorities[effect_name] = priority
	
	_effect_pool.initialize_pool(effect_name, resource, pool_size)
	_effect_blender.set_blend_mode(effect_name, blend_mode)
	
	_log_debug("エフェクト '%s' を登録しました（優先度: %d, プールサイズ: %d, ブレンドモード: %d）" % [effect_name, priority, pool_size, blend_mode])

# エフェクトの再生
func play_effect(effect_name: String, params: Dictionary = {}) -> void:
	if effect_name.is_empty():
		push_warning("EffectBus: Attempted to play effect with empty name")
		return
	
	var priority = _effect_priorities.get(effect_name, 50)
	print("Debug: Playing effect '%s' with priority %d" % [effect_name, priority])
	
	# 優先度キューに追加
	_priority_queue.push(effect_name, priority)
	
	# 現在再生中のエフェクトの優先度をチェック
	var current_effect = _priority_queue.pop()
	if current_effect:
		_play_effect_internal(current_effect.name, params)
		
		# 低優先度のエフェクトを停止
		_stop_lower_priority_effects(priority)

# エフェクトの内部再生処理
func _play_effect_internal(effect_name: String, params: Dictionary) -> void:
	var effect = _effect_pool.get_effect(effect_name)
	if not effect:
		push_error("EffectBus: Failed to get effect from pool: %s" % effect_name)
		return
	
	if not _effect_pool._active_effects.has(effect_name):
		_effect_pool._active_effects[effect_name] = []
	
	# パラメータの検証
	if params.has("duration") and params["duration"] <= 0:
		push_warning("EffectBus: Invalid duration specified for effect '%s': %f" % [effect_name, params["duration"]])
		params["duration"] = 1.0  # デフォルト値に設定
	
	if params.has("num_effects") and params["num_effects"] <= 0:
		push_warning("EffectBus: Invalid number of effects specified for '%s': %d" % [effect_name, params["num_effects"]])
		params["num_effects"] = 1  # デフォルト値に設定
	
	# パラメータの設定
	if effect.has_method("set_params"):
		effect.set_params(params)
	
	# エフェクトの再生
	if effect.has_method("play"):
		effect.play()
		print("Debug: Started playing effect '%s'" % effect_name)
		effect_triggered.emit(effect_name, params)  # シグナルを発火

# 低優先度のエフェクトを停止
func _stop_lower_priority_effects(current_priority: int) -> void:
	for active_effect in _effect_pool._active_effects.values():
		for effect in active_effect:
			var effect_priority = _effect_priorities.get(effect.name, 50)
			if effect_priority < current_priority and effect.has_method("stop"):
				effect.stop()
				print("Debug: Stopped lower priority effect '%s' (priority: %d)" % [effect.name, effect_priority])

# ブレンド完了時の処理
func _on_blend_completed(effect_name: String) -> void:
	print("Debug: Blend completed for effect: %s" % effect_name)
	# 必要に応じて追加の処理を実装

# エフェクトの中断
func pause_effect(effect_name: String) -> void:
	if not _effect_pool._active_effects.has(effect_name):
		push_warning("EffectBus: No active effects found for '%s'" % effect_name)
		return
		
	var effect = _effect_pool.get_effect(effect_name)
	if effect and effect.has_method("pause"):
		effect.pause()
		effect_paused.emit(effect_name)
		print("Debug: Paused effect '%s'" % effect_name)
	else:
		push_error("EffectBus: Effect '%s' does not support pausing" % effect_name)

# エフェクトの再開
func resume_effect(effect_name: String) -> void:
	if not _effect_pool._active_effects.has(effect_name):
		push_warning("EffectBus: No active effects found for '%s'" % effect_name)
		return
		
	var effect = _effect_pool.get_effect(effect_name)
	if effect and effect.has_method("resume"):
		effect.resume()
		effect_resumed.emit(effect_name)
		print("Debug: Resumed effect '%s'" % effect_name)
	else:
		push_error("EffectBus: Effect '%s' does not support resuming" % effect_name)

# エフェクト完了シグナルハンドラ
func _on_effect_completed(effect_name: String, effect: Node) -> void:
	_log_debug("エフェクト '%s' が完了しました" % effect_name)
	
	# エフェクトをアクティブリストから削除
	if _effect_pool._active_effects.has(effect_name):
		_effect_pool._active_effects[effect_name].erase(effect)
		_log_debug("Removed effect from active list: %s" % effect.name)
		
		# エフェクトの状態をリセット
		if effect.has_method("reset"):
			effect.reset()
			_log_debug("Reset effect state: %s" % effect.name)
	
	# エフェクトをプールに戻す
	release_effect(effect_name, effect)
	_log_debug("Released effect to pool: %s" % effect.name)

# 全てのアクティブなエフェクトを一時停止
func pause_all_effects() -> void:
	_log_debug("全てのエフェクトを一時停止します")
	for effect_name in _effect_pool._active_effects:
		for effect in _effect_pool._active_effects[effect_name]:
			if effect and effect.has_method("pause"):
				effect.pause()
				effect_paused.emit(effect_name) # シグナルはエフェクトごとに発火

# 全てのアクティブなエフェクトを再開
func resume_all_effects() -> void:
	_log_debug("全てのエフェクトを再開します")
	for effect_name in _effect_pool._active_effects:
		for effect in _effect_pool._active_effects[effect_name]:
			if effect and effect.has_method("resume"):
				effect.resume()
				effect_resumed.emit(effect_name) # シグナルはエフェクトごとに発火

# エフェクトの解放
func release_effect(effect_name: String, effect: Node) -> void:
	_effect_pool.release_effect(effect_name, effect)

# プールの状態を取得
func get_pool_status(effect_name: String) -> Dictionary:
	return _effect_pool.get_pool_status(effect_name)

# ブレンドモードの設定
func set_blend_mode(effect_name: String, mode: EffectBlender.BlendMode) -> void:
	_effect_blender.set_blend_mode(effect_name, mode)

# イベントハンドラ
func _on_player_damaged(amount: float, source: Node) -> void:
	if not source:
		_log_error(_format_error("invalid_damage_source"))
		return
		
	play_effect("player_hit", {
		"amount": amount,
		"position": source.global_position,
		"num_effects": 2,
		"base_alpha": 1.0,
		"blend_alpha": 0.7
	})

func _on_enemy_damaged(enemy: Node, amount: float, source: Node) -> void:
	if not enemy or not source:
		_log_error(_format_error("invalid_enemy_source"))
		return
		
	play_effect("enemy_hit", {
		"amount": amount,
		"position": enemy.global_position,
		"num_effects": 2,
		"base_alpha": 1.0,
		"blend_alpha": 0.7
	})

func _on_player_died() -> void:
	play_effect("player_death", {
		"num_effects": 3,
		"base_alpha": 1.0,
		"blend_alpha": 0.5
	})

func _on_enemy_died(enemy: Node) -> void:
	if not enemy:
		_log_error(_format_error("invalid_enemy"))
		return
		
	play_effect("enemy_death", {
		"position": enemy.global_position,
		"num_effects": 3,
		"base_alpha": 1.0,
		"blend_alpha": 0.5
	})

# デバッグログ
func _log_debug(message: String) -> void:
	if _debug_mode:
		print("[EffectBus] %s" % message)

func _log_error(message: String) -> void:
	push_error("[EffectBus] %s" % message)
	error_occurred.emit(message)

# デバッグモードの設定
func set_debug_mode(enabled: bool) -> void:
	_debug_mode = enabled
	_log_debug("デバッグモードを%sに設定しました" % ("有効" if enabled else "無効"))

# エラーメッセージの生成
func _format_error(template_key: String, args: Array = []) -> String:
	var template = ERROR_TEMPLATES.get(template_key, "不明なエラー")
	return template % args

# エフェクトの優先度を更新
func update_effect_priority(effect_name: String, effect: Node) -> void:
	if not _effect_resources.has(effect_name):
		_log_error(_format_error("invalid_effect", [effect_name]))
		return
		
	if not effect or not effect.has_method("get_priority"):
		_log_error("無効なエフェクトインスタンス")
		return
		
	var new_priority = effect.priority
	if new_priority < 0:
		_log_error(_format_error("invalid_priority", [new_priority]))
		return
		
	_effect_priorities[effect_name] = new_priority
	_log_debug("エフェクト '%s' の優先度を %d に更新しました" % [effect_name, new_priority])

# 優先度の設定
func set_effect_priority(effect_name: String, priority: int) -> void:
	if effect_name.is_empty():
		push_warning("EffectBus: Attempted to set priority with empty effect name")
		return
	
	_effect_priorities[effect_name] = priority
	print("Debug: Set priority for effect '%s' to %d" % effect_name, priority)

# クリーンアップ
func cleanup() -> void:
	# イベントリスナーの切断
	if _event_bus:
		if _event_bus.has_signal("player_damaged"):
			_event_bus.player_damaged.disconnect(_on_player_damaged)
		if _event_bus.has_signal("enemy_damaged"):
			_event_bus.enemy_damaged.disconnect(_on_enemy_damaged)
		if _event_bus.has_signal("player_died"):
			_event_bus.player_died.disconnect(_on_player_died)
		if _event_bus.has_signal("enemy_died"):
			_event_bus.enemy_died.disconnect(_on_enemy_died)
	
	_priority_queue.clear()
	_effect_pool.cleanup_all_pools()
	print("Debug: EffectBus cleaned up")
