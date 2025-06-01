extends GutTest

const EffectBusScript = preload("res://Scripts/Core/EffectBus.gd")

# テスト用のモックエフェクトクラス
class MockEffect extends Node2D:
	var _is_playing: bool = false
	var duration: float = 1.0
	var effect_name: String = ""
	var priority: int = 50  # 優先度プロパティを追加
	var _stop_requested: bool = false
	var _timer: SceneTreeTimer = null
	
	signal effect_started
	signal effect_completed
	signal effect_paused
	signal effect_resumed
	signal effect_stopped
	
	func get_priority() -> int:
		return priority
	
	func play() -> void:
		if _is_playing:
			print("Debug: Effect is already playing, skipping play call")
			return
			
		_is_playing = true
		_stop_requested = false
		print("Debug: Starting effect: ", name)
		print("Debug: - Duration: ", duration)
		print("Debug: - Priority: ", priority)
		effect_started.emit()
		
		# 非同期で再生を開始
		_timer = get_tree().create_timer(duration)
		_timer.timeout.connect(_on_timer_timeout)
	
	func _on_timer_timeout() -> void:
		# 停止が要求されていない場合のみ完了を通知
		if not _stop_requested:
			_is_playing = false
			print("Debug: Effect completed normally: ", name)
			effect_completed.emit()
	
	func pause() -> void:
		if _is_playing:
			print("Debug: Pausing effect: ", name)
			effect_paused.emit()
	
	func resume() -> void:
		if _is_playing:
			print("Debug: Resuming effect: ", name)
			effect_resumed.emit()
	
	func stop() -> void:
		if not _is_playing:
			print("Debug: Effect is not playing, skipping stop call: ", name)
			return
			
		print("Debug: Stopping effect: ", name)
		_stop_requested = true
		_is_playing = false
		
		# タイマーをキャンセル
		if _timer != null:
			_timer.timeout.disconnect(_on_timer_timeout)
			_timer = null
		
		# エフェクトの状態をリセット
		visible = false
		_stop_requested = false
		effect_stopped.emit()
		
		# 完了通知を送信
		effect_completed.emit()
	
	func is_playing() -> bool:
		return _is_playing
		
	func hide_effect() -> void:
		visible = false
		
	func show_effect() -> void:
		visible = true
		
	func reset() -> void:
		_is_playing = false
		_stop_requested = false
		visible = false
		if _timer != null:
			_timer.timeout.disconnect(_on_timer_timeout)
			_timer = null

var _effect_bus: EffectBus
var _high_priority_effect: MockEffect
var _low_priority_effect: MockEffect
var _medium_priority_effect: MockEffect
var _mock_scene: PackedScene

func before_each():
	_effect_bus = EffectBusScript.new()
	add_child_autofree(_effect_bus)
	
	# モックシーンの作成
	_mock_scene = PackedScene.new()
	var root = MockEffect.new()
	_mock_scene.pack(root)
	
	# エフェクトの登録
	_effect_bus.register_effect("high_priority", _mock_scene, 100, 1)
	_effect_bus.register_effect("low_priority", _mock_scene, 0, 1)
	_effect_bus.register_effect("medium_priority", _mock_scene, 50, 1)
	
	_high_priority_effect = MockEffect.new()
	add_child_autofree(_high_priority_effect)
	_high_priority_effect.name = "HighPriorityEffect"
	_high_priority_effect.priority = 100  # 優先度を設定
	
	_low_priority_effect = MockEffect.new()
	add_child_autofree(_low_priority_effect)
	_low_priority_effect.name = "LowPriorityEffect"
	_low_priority_effect.priority = 0  # 優先度を設定
	
	_medium_priority_effect = MockEffect.new()
	add_child_autofree(_medium_priority_effect)
	_medium_priority_effect.name = "MediumPriorityEffect"
	_medium_priority_effect.priority = 50  # 優先度を設定

func test_effect_priority_registration():
	# 優先度の確認
	assert_eq(_effect_bus._effect_priorities["high_priority"], 100, "高優先度エフェクトの優先度が正しく設定されていない")
	assert_eq(_effect_bus._effect_priorities["low_priority"], 0, "低優先度エフェクトの優先度が正しく設定されていない")
	assert_eq(_effect_bus._effect_priorities["medium_priority"], 50, "中優先度エフェクトの優先度が正しく設定されていない")

func test_effect_playback_order():
	# エフェクトの再生
	_effect_bus.play_effect("low_priority", {"effect": _low_priority_effect})
	await get_tree().process_frame  # フレームを待機して再生を確実に開始
	
	_effect_bus.play_effect("medium_priority", {"effect": _medium_priority_effect})
	await get_tree().process_frame  # フレームを待機して再生を確実に開始
	
	_effect_bus.play_effect("high_priority", {"effect": _high_priority_effect})
	await get_tree().process_frame  # フレームを待機して再生を確実に開始
	
	# 再生順序の確認
	assert_true(_high_priority_effect.is_playing(), "高優先度エフェクトが再生中であるべき")
	assert_false(_medium_priority_effect.is_playing(), "中優先度エフェクトは停止しているべき")
	assert_false(_low_priority_effect.is_playing(), "低優先度エフェクトは停止しているべき")

func test_same_priority_effects():
	# エフェクトの登録
	_effect_bus.register_effect("medium_priority1", _mock_scene, 50, 1)
	_effect_bus.register_effect("medium_priority2", _mock_scene, 50, 1)
	
	# 同じ優先度のエフェクトを再生
	_effect_bus.play_effect("medium_priority1", {"effect": _medium_priority_effect})
	var another_medium = MockEffect.new()
	add_child_autofree(another_medium)
	another_medium.name = "AnotherMediumPriorityEffect"
	_effect_bus.play_effect("medium_priority2", {"effect": another_medium})
	
	# 両方のエフェクトが再生中であることを確認
	assert_true(_medium_priority_effect.is_playing(), "最初のエフェクトは再生中であるべき")
	assert_true(another_medium.is_playing(), "2番目のエフェクトも再生中であるべき")

func test_high_priority_effect_interrupts_lower_priority():
	# 低優先度エフェクトを再生
	_effect_bus.play_effect("low_priority", {"effect": _low_priority_effect})
	await get_tree().process_frame  # フレームを待機して再生を確実に開始
	assert_true(_low_priority_effect.is_playing(), "低優先度エフェクトが再生中であるべき")
	
	# 高優先度エフェクトを再生
	_effect_bus.play_effect("high_priority", {"effect": _high_priority_effect})
	await get_tree().process_frame  # フレームを待機して再生を確実に開始
	assert_true(_high_priority_effect.is_playing(), "高優先度エフェクトが再生中であるべき")
	assert_false(_low_priority_effect.is_playing(), "低優先度エフェクトは停止しているべき")

func test_priority_queue_management():
	# 複数のエフェクトを再生
	_effect_bus.play_effect("low_priority", {"effect": _low_priority_effect})
	await get_tree().process_frame  # フレームを待機して再生を確実に開始
	
	_effect_bus.play_effect("medium_priority", {"effect": _medium_priority_effect})
	await get_tree().process_frame  # フレームを待機して再生を確実に開始
	
	_effect_bus.play_effect("high_priority", {"effect": _high_priority_effect})
	await get_tree().process_frame  # フレームを待機して再生を確実に開始
	
	# 高優先度エフェクトが再生中で、他は停止していることを確認
	assert_true(_high_priority_effect.is_playing(), "高優先度エフェクトが再生中であるべき")
	assert_false(_medium_priority_effect.is_playing(), "中優先度エフェクトは停止しているべき")
	assert_false(_low_priority_effect.is_playing(), "低優先度エフェクトは停止しているべき")
	
	# 高優先度エフェクトを停止
	_high_priority_effect.stop()
	await get_tree().process_frame  # フレームを待機して停止を確実に反映
	
	# 中優先度エフェクトが再生を開始することを確認
	assert_true(_medium_priority_effect.is_playing(), "中優先度エフェクトが再生中であるべき")
	assert_false(_low_priority_effect.is_playing(), "低優先度エフェクトは停止しているべき")

func test_priority_change_during_playback():
	# 低優先度エフェクトを再生
	_effect_bus.play_effect("low_priority", {"effect": _low_priority_effect})
	await get_tree().process_frame  # フレームを待機して再生を確実に開始
	assert_true(_low_priority_effect.is_playing(), "低優先度エフェクトが再生中であるべき")
	
	# 優先度を変更
	_low_priority_effect.priority = 100
	_effect_bus.update_effect_priority("low_priority", _low_priority_effect)
	
	# 新しい中優先度エフェクトを再生
	var new_medium = MockEffect.new()
	new_medium.priority = 50
	new_medium.name = "NewMediumPriorityEffect"
	add_child_autofree(new_medium)
	
	# 新しいエフェクトを登録
	_effect_bus.register_effect("new_medium", _mock_scene, 50, 1)
	_effect_bus.play_effect("new_medium", {"effect": new_medium})
	await get_tree().process_frame  # フレームを待機して再生を確実に開始
	
	# 優先度が変更されたエフェクトが再生中であることを確認
	assert_true(_low_priority_effect.is_playing(), "優先度が変更されたエフェクトが再生中であるべき")
	assert_false(new_medium.is_playing(), "新しい中優先度エフェクトは停止しているべき") 
