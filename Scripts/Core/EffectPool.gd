class_name EffectPool
extends Node

const TestEffect = preload("res://Scripts/Examples/TestEffect.gd")

# プールの設定
const DEFAULT_POOL_SIZE = 10
const MAX_POOL_SIZE = 100

# プールの状態
var _pools: Dictionary = {}  # エフェクト名 -> プール配列
var _active_effects: Dictionary = {}  # エフェクト名 -> アクティブなエフェクトの配列
var _effect_resources: Dictionary = {} # エフェクト名 -> PackedSceneリソース

# シグナル
signal pool_created(effect_name: String, size: int)
signal pool_expanded(effect_name: String, new_size: int)
signal effect_recycled(effect_name: String)

func _init() -> void:
	name = "EffectPool"
	print("Debug: EffectPool initialized")

# プールの初期化
func initialize_pool(effect_name: String, resource: Resource, initial_size: int = DEFAULT_POOL_SIZE) -> void:
	if _pools.has(effect_name):
		push_warning("EffectPool: プール '%s' は既に存在します" % effect_name)
		return
	
	if not resource is PackedScene:
		push_error("EffectPool: リソース '%s' はPackedSceneではありません" % effect_name)
		return
	
	_effect_resources[effect_name] = resource
	_pools[effect_name] = []
	_active_effects[effect_name] = []
	
	for i in range(initial_size):
		var instance = _create_effect_instance(resource)
		if instance:
			instance.name = effect_name
			_pools[effect_name].append(instance)
			add_child(instance)
			instance.hide()
	
	print("Debug: Initialized pool for '%s' with %d effects" % [effect_name, initial_size])
	pool_created.emit(effect_name, initial_size)

# エフェクトの取得
func get_effect(effect_name: String) -> Node:
	if not _pools.has(effect_name):
		push_error("EffectPool: プール '%s' が存在しません" % effect_name)
		return null
	
	if not _effect_resources.has(effect_name):
		push_error("EffectPool: リソース '%s' が登録されていません" % effect_name)
		return null
	
	if not _active_effects.has(effect_name):
		_active_effects[effect_name] = []
	
	var pool = _pools[effect_name]
	var active = _active_effects[effect_name]
	
	print("Debug: EffectPool.get_effect - Pool status for ", effect_name)
	print("Debug: - Pool size: ", pool.size())
	print("Debug: - Active effects: ", active.size())
	
	# 未使用のエフェクトを探す
	for effect in pool:
		if not active.has(effect):
			print("Debug: Found available effect in pool: ", effect.name)
			active.append(effect)
			return effect
	
	# プールが満杯の場合、新しいエフェクトを作成
	if pool.size() < MAX_POOL_SIZE:
		print("Debug: Creating new effect instance for pool")
		var new_effect = _create_effect_instance(_effect_resources[effect_name])
		if new_effect:
			new_effect.name = effect_name
			pool.append(new_effect)
			active.append(new_effect)
			add_child(new_effect)
			print("Debug: Added new effect to pool: ", new_effect.name)
			pool_expanded.emit(effect_name, pool.size())
			return new_effect
	
	# プールが満杯の場合、最も古いエフェクトを再利用
	if not active.is_empty():
		var oldest_effect = active[0]
		print("Debug: Reusing oldest effect: ", oldest_effect.name)
		active.remove_at(0)
		active.append(oldest_effect)
		effect_recycled.emit(effect_name)
		if oldest_effect.has_method("reset"):
			oldest_effect.reset()
		return oldest_effect
	
	return null

# エフェクトの解放
func release_effect(effect_name: String, effect: Node) -> void:
	if not _pools.has(effect_name):
		push_warning("EffectPool: Attempted to release effect to non-existent pool: %s" % effect_name)
		return
	
	if not _active_effects.has(effect_name):
		_active_effects[effect_name] = []
	
	if not effect:
		push_warning("EffectPool: Attempted to release null effect")
		return
	
	# エフェクトの状態をリセット
	if effect.has_method("reset"):
		effect.reset()
	
	# アクティブリストから削除
	if _active_effects[effect_name].has(effect):
		_active_effects[effect_name].erase(effect)
		print("Debug: Removed effect from active list: ", effect.name)
	
	# プールに戻す
	if not _pools[effect_name].has(effect):
		_pools[effect_name].append(effect)
		print("Debug: Added effect back to pool: ", effect.name)
	
	print("Debug: Released effect '%s' to pool" % effect_name)
	print("Debug: - Pool size: %d" % _pools[effect_name].size())
	print("Debug: - Active effects: %d" % _active_effects[effect_name].size())

# エフェクトインスタンスの作成
func _create_effect_instance(resource: PackedScene) -> Node:
	if not resource:
		push_error("EffectPool: Attempted to create effect from null resource")
		return null
	
	if not resource is PackedScene:
		push_error("EffectPool: Attempted to instantiate a non-PackedScene resource")
		return null

	var instance = resource.instantiate()
	if instance:
		instance.process_mode = Node.PROCESS_MODE_ALWAYS
		print("Debug: Created new effect instance: ", instance.name)
	return instance

# プールのクリーンアップ
func cleanup_pool(effect_name: String) -> void:
	if not _pools.has(effect_name):
		push_warning("EffectPool: Attempted to cleanup non-existent pool: %s" % effect_name)
		return
	
	# アクティブなエフェクトの停止
	if _active_effects.has(effect_name):
		for effect in _active_effects[effect_name]:
			if effect and effect.has_method("stop"):
				effect.stop()
		_active_effects[effect_name].clear()
		print("Debug: Stopped and cleared active effects for '%s'" % effect_name)
	
	# プール内のエフェクトの解放
	for effect in _pools[effect_name]:
		if is_instance_valid(effect):
			effect.queue_free()
			print("Debug: Freed effect instance: ", effect.name)
	
	_pools.erase(effect_name)
	_active_effects.erase(effect_name)
	_effect_resources.erase(effect_name)
	
	print("Debug: Cleaned up pool for '%s'" % effect_name)

# 全プールのクリーンアップ
func cleanup_all_pools() -> void:
	for effect_name in _pools.keys():
		cleanup_pool(effect_name)
	print("Debug: Cleaned up all pools")

# プールの状態を取得
func get_pool_status(effect_name: String) -> Dictionary:
	if not _pools.has(effect_name):
		push_warning("EffectPool: Attempted to get status for non-existent pool: %s" % effect_name)
		return {}
	
	var active_count = 0
	if _active_effects.has(effect_name):
		active_count = _active_effects[effect_name].size()
	
	var status = {
		"total": _pools[effect_name].size(),
		"active": active_count,
		"available": _pools[effect_name].size() - active_count
	}
	
	print("Debug: Pool status for '%s': %s" % [effect_name, status])
	return status

# プールサイズを変更するメソッドを追加（必要であれば）
# func set_pool_size(effect_name: String, new_size: int) -> void:
# 	# 実装に応じてプールサイズの変更ロジックを追加
# 	pass 

# エラーログ出力
func _log_error(message: String) -> void:
	push_error("EffectPool: " + message) 

func _log_debug(message: String) -> void:
	print("Debug: " + message) 
