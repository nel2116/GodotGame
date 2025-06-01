extends GutTest

const EffectBusScript = preload("res://Scripts/Core/EffectBus.gd")

# テスト用のモックエフェクトクラス
class MockEffect extends Node2D:
	var _is_playing: bool = false
	var duration: float = 1.0
	var effect_name: String = ""
	var _is_paused: bool = false
	
	signal effect_started
	signal effect_completed
	signal effect_paused
	signal effect_resumed
	signal effect_stopped
	
	func play() -> void:
		print("MockEffect: play() called")
		if _is_playing:
			print("MockEffect: Already playing, stopping first")
			stop()
		_is_playing = true
		_is_paused = false
		effect_started.emit()
		await get_tree().create_timer(duration).timeout
		if _is_playing:  # 停止されていない場合のみ完了を発火
			_is_playing = false
			effect_completed.emit()
			print("MockEffect: play() completed")
	
	func pause() -> void:
		print("MockEffect: pause() called")
		if _is_playing and not _is_paused:
			_is_paused = true
			effect_paused.emit()
	
	func resume() -> void:
		print("MockEffect: resume() called")
		if _is_playing and _is_paused:
			_is_paused = false
			effect_resumed.emit()
	
	func stop() -> void:
		print("MockEffect: stop() called")
		if _is_playing:
			_is_playing = false
			_is_paused = false
			effect_stopped.emit()
	
	func is_playing() -> bool:
		return _is_playing
		
	func hide_effect() -> void:
		visible = false
		
	func show_effect() -> void:
		visible = true
		
	func _exit_tree() -> void:
		print("MockEffect: _exit_tree() called")
		queue_free()

var _effect_bus: EffectBus
var _effect: MockEffect
var _mock_scene: PackedScene

func before_each():
	print("\n=== Starting new test ===")
	_effect_bus = EffectBusScript.new()
	add_child(_effect_bus)
	
	# モックシーンの作成
	_mock_scene = PackedScene.new()
	var root = MockEffect.new()
	_mock_scene.pack(root)
	root.queue_free()  # ルートノードを解放
	
	# エフェクトの登録
	_effect_bus.register_effect("test_effect", _mock_scene, 50, 1)
	
	_effect = MockEffect.new()
	add_child(_effect)
	_effect.name = "TestEffect"
	_effect.duration = 2.0
	print("Test setup completed")

func after_each():
	print("=== Cleaning up test ===")
	if _effect and is_instance_valid(_effect):
		_effect.stop()
		_effect.queue_free()
	if _effect_bus and is_instance_valid(_effect_bus):
		_effect_bus.queue_free()
	if _mock_scene:
		_mock_scene = null  # シーンリソースを解放
	await get_tree().process_frame
	print("Test cleanup completed")

func test_effect_lifecycle_events():
	print("\n--- Testing effect lifecycle events ---")
	var state = {
		"started": false,
		"completed": false,
		"stopped": false
	}
	
	_effect.effect_started.connect(func(): 
		state.started = true
		print("Effect started signal received")
	)
	_effect.effect_completed.connect(func(): 
		state.completed = true
		print("Effect completed signal received")
	)
	_effect.effect_stopped.connect(func(): 
		state.stopped = true
		print("Effect stopped signal received")
	)
	
	# エフェクトを再生
	_effect.play()
	assert_true(state.started, "effect_startedシグナルが発火するべき")
	assert_false(state.completed, "effect_completedシグナルはまだ発火しないべき")
	assert_false(state.stopped, "effect_stoppedシグナルはまだ発火しないべき")
	
	# エフェクトの完了を待つ
	await get_tree().create_timer(2.1).timeout
	assert_true(state.completed, "effect_completedシグナルが発火するべき")
	assert_false(state.stopped, "effect_stoppedシグナルはまだ発火しないべき")

func test_effect_pause_resume_lifecycle():
	print("\n--- Testing effect pause/resume lifecycle ---")
	var state = {
		"paused": false,
		"resumed": false
	}
	
	_effect.effect_paused.connect(func(): 
		state.paused = true
		print("Effect paused signal received")
	)
	_effect.effect_resumed.connect(func(): 
		state.resumed = true
		print("Effect resumed signal received")
	)
	
	# エフェクトを再生
	_effect.play()
	
	# 一時停止
	_effect.pause()
	assert_true(state.paused, "effect_pausedシグナルが発火するべき")
	assert_false(state.resumed, "effect_resumedシグナルはまだ発火しないべき")
	
	# 再開
	_effect.resume()
	assert_true(state.resumed, "effect_resumedシグナルが発火するべき")
	
	# エフェクトの完了を待つ
	await get_tree().create_timer(2.1).timeout
	assert_false(_effect.is_playing(), "エフェクトは停止しているべき")

func test_effect_restart():
	print("\n--- Testing effect restart ---")
	var state = {
		"start_count": 0
	}
	
	_effect.effect_started.connect(func(): 
		state.start_count += 1
		print("Effect started signal received (count: %d)" % state.start_count)
	)
	
	# エフェクトを再生
	_effect.play()
	print("First play called")
	await get_tree().create_timer(0.1).timeout  # シグナルの発火を待つ
	assert_eq(state.start_count, 1, "effect_startedシグナルが1回発火するべき")
	
	# エフェクトを再起動
	_effect.play()
	print("Second play called")
	await get_tree().create_timer(0.1).timeout  # シグナルの発火を待つ
	assert_eq(state.start_count, 2, "effect_startedシグナルが2回発火するべき")
	
	# エフェクトの完了を待つ
	await get_tree().create_timer(2.1).timeout
	assert_false(_effect.is_playing(), "エフェクトは停止しているべき")

func test_effect_manual_stop():
	print("\n--- Testing effect manual stop ---")
	var state = {
		"started": false,
		"completed": false,
		"stopped": false
	}
	
	_effect.effect_started.connect(func(): 
		state.started = true
		print("Effect started signal received")
	)
	_effect.effect_completed.connect(func(): 
		state.completed = true
		print("Effect completed signal received")
	)
	_effect.effect_stopped.connect(func(): 
		state.stopped = true
		print("Effect stopped signal received")
	)
	
	# エフェクトを再生
	_effect.play()
	assert_true(state.started, "effect_startedシグナルが発火するべき")
	
	# エフェクトを手動で停止
	_effect.stop()
	assert_true(state.stopped, "effect_stoppedシグナルが発火するべき")
	assert_false(state.completed, "effect_completedシグナルは発火しないべき")

func test_effect_auto_cleanup():
	print("\n--- Testing effect auto cleanup ---")
	var state = {
		"deleted": false
	}
	
	# エフェクトを再生
	_effect.play()
	assert_true(_effect.is_playing(), "エフェクトが再生中であるべき")
	
	# エフェクトの完了を待つ
	await get_tree().create_timer(2.1).timeout
	assert_false(_effect.is_playing(), "エフェクトは停止しているべき")
	
	# エフェクトの自動削除を確認
	print("Checking if effect is in tree: %s" % str(_effect.is_inside_tree()))
	print("Effect parent: %s" % str(_effect.get_parent()))
	
	# エフェクトの削除を監視
	_effect.tree_exiting.connect(func():
		print("Effect is being removed from tree")
		state.deleted = true
	)
	
	# エフェクトを削除
	_effect.queue_free()
	
	# 削除が完了するまで待機
	var timeout = 0.0
	while not state.deleted and timeout < 1.0:
		await get_tree().process_frame
		timeout += 0.016  # 約60FPSのフレーム時間
	
	assert_true(state.deleted, "エフェクトは削除されるべき") 
