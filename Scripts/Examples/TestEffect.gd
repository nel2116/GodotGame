class_name TestEffect
extends CanvasItem

# ブレンドモードの参照
const BlendMode = EffectBlender.BlendMode

# エフェクトの状態
enum State {
	STOPPED,
	PLAYING,
	PAUSED
}
var _state: State = State.STOPPED

# エフェクトの基本プロパティ
var priority: int = 50
var duration: float = 1.0:
	set(value):
		duration = value
		# アニメーションの長さはタイマーで制御するため、ここではアニメーション自体の長さを更新しない
var _alpha: float = 1.0
var _blend_mode: EffectBlender.BlendMode = BlendMode.NORMAL
var _is_playing: bool = false
var _effect_pool: EffectPool = null
var _elapsed_time: float = 0.0
var _timer: Timer = null
var _is_stopping: bool = false

# 対応するエフェクト名を保持
var effect_name: String = ""

# シグナル定義
signal effect_started
signal effect_completed
signal effect_stopped
signal effect_paused
signal effect_resumed

func _ready() -> void:
	modulate.a = _alpha
	_setup_timer()
	# アニメーションの完了シグナルは引き続き接続しておく
	if has_node("AnimationPlayer"):
		$AnimationPlayer.animation_finished.connect(_on_animation_finished)

func _setup_timer() -> void:
	print("Debug: Setting up timer for effect: ", name)
	
	# 既存のタイマーをクリーンアップ
	if is_instance_valid(_timer):
		if _timer.timeout.is_connected(_on_timer_timeout):
			_timer.timeout.disconnect(_on_timer_timeout)
		_timer.queue_free()
	
	# 新しいタイマーを作成
	_timer = Timer.new()
	_timer.one_shot = true
	_timer.autostart = false  # 明示的に自動開始を無効化
	_timer.timeout.connect(_on_timer_timeout)
	add_child(_timer)
	print("Debug: Timer setup complete for effect: ", name)
	
	# タイマーの状態を確認
	print("Debug: Timer configuration:")
	print("Debug: - One shot: ", _timer.one_shot)
	print("Debug: - Autostart: ", _timer.autostart)
	print("Debug: - Wait time: ", _timer.wait_time)
	print("Debug: - Is stopped: ", _timer.is_stopped())

func set_effect_pool(pool: EffectPool) -> void:
	if not is_instance_valid(pool):
		push_warning("TestEffect: Attempted to set invalid EffectPool")
		return
	_effect_pool = pool

func _change_state(new_state: State) -> void:
	if _state == new_state:
		return
	
	match new_state:
		State.PLAYING:
			if _is_stopping:
				return
		State.STOPPED:
			if not _is_playing:
				return
	
	_state = new_state

func play() -> void:
	print("Debug: TestEffect.play - Starting effect: ", name)
	print("Debug: - Duration: ", duration)
	print("Debug: - Current state: ", "playing" if _is_playing else "stopped")
	
	if _is_playing:
		print("Debug: Effect is already playing, skipping")
		return
	
	_is_playing = true
	_is_stopping = false
	_elapsed_time = 0.0
	visible = true  # エフェクトを表示状態にする
	
	# エフェクトプールの参照を設定
	if not _effect_pool:
		print("Debug: Searching for EffectPool in parent hierarchy")
		var parent = get_parent()
		while parent:
			if parent is EffectPool:
				_effect_pool = parent
				print("Debug: Found EffectPool in parent: ", parent.name)
				break
			parent = parent.get_parent()
	
	if not _effect_pool:
		print("Debug: Warning - EffectPool not found in parent hierarchy")
	
	# アニメーションの再生
	if has_node("AnimationPlayer"):
		var anim_player = get_node("AnimationPlayer")
		print("Debug: Starting AnimationPlayer")
		if anim_player.has_animation("effect"):
			print("Debug: Playing 'effect' animation")
			anim_player.play("effect")
		else:
			print("Debug: Warning - 'effect' animation not found in AnimationPlayer")
			# アニメーションが見つからない場合は、タイマーのみで制御
			print("Debug: Falling back to timer-only control")
	
	# タイマーの設定
	_setup_timer()
	if _timer:
		print("Debug: Setting up timer with duration: ", duration)
		_timer.wait_time = duration
		_timer.start()
		print("Debug: Timer started with wait_time: ", _timer.wait_time)
		print("Debug: Timer is stopped: ", _timer.is_stopped())
		print("Debug: Timer time left: ", _timer.time_left)
		
		# タイマーの接続を確認
		if not _timer.timeout.is_connected(_on_timer_timeout):
			_timer.timeout.connect(_on_timer_timeout)
			print("Debug: Connected timer timeout signal")
	
	_change_state(State.PLAYING)
	effect_started.emit()
	print("Debug: Effect play complete")

func stop() -> void:
	print("Debug: TestEffect.stop - Stopping effect: ", name)
	print("Debug: - Current state: ", "playing" if _is_playing else "stopped")
	print("Debug: - Is stopping: ", _is_stopping)
	
	if not _is_playing or _is_stopping:
		print("Debug: Effect is not playing or already stopping, skipping")
		return
	
	_is_stopping = true
	
	# アニメーションの停止
	if has_node("AnimationPlayer"):
		print("Debug: Stopping AnimationPlayer")
		get_node("AnimationPlayer").stop()
	
	# タイマーの停止
	if _timer:
		print("Debug: Stopping timer")
		_timer.stop()
	
	_is_playing = false
	_is_stopping = false
	
	# エフェクトプールへの解放
	if _effect_pool:
		print("Debug: Releasing effect to pool: ", _effect_pool.name)
		_effect_pool.release_effect(effect_name, self)
	else:
		_handle_pool_release_through_parent()
	
	_change_state(State.STOPPED)
	effect_stopped.emit()
	print("Debug: Effect stop complete")

func _handle_pool_release_through_parent() -> void:
	print("Debug: Warning - No EffectPool reference, searching in parent hierarchy")
	var parent = get_parent()
	while parent:
		if parent is EffectPool:
			print("Debug: Found EffectPool in parent: ", parent.name)
			parent.release_effect(effect_name, self)
			break
		parent = parent.get_parent()

func pause() -> void:
	if not _is_playing or _is_stopping:
		return
		
	if has_node("AnimationPlayer"):
		$AnimationPlayer.pause()
	if is_instance_valid(_timer):
		_timer.paused = true
	_change_state(State.PAUSED)
	effect_paused.emit()

func resume() -> void:
	if not _is_playing or _is_stopping:
		return
		
	if has_node("AnimationPlayer"):
		# 最後に再生したアニメーションを再開
		# ここでは "effect" を想定
		if $AnimationPlayer.has_animation("effect"):
			$AnimationPlayer.play("effect")
		else:
			push_warning("Animation 'effect' not found in AnimationPlayer on resume.")
	if is_instance_valid(_timer):
		_timer.paused = false
	_change_state(State.PLAYING)
	effect_resumed.emit()

func set_alpha(value: float) -> void:
	_alpha = value
	modulate.a = value

func set_blend_mode(mode: EffectBlender.BlendMode) -> void:
	_blend_mode = mode
	# ブレンドモードの設定（実際の実装はエフェクトの種類に応じて変更）
	if material == null:
		material = CanvasItemMaterial.new()
	
	if material:
		material = material.duplicate()
		match mode:
			BlendMode.ADDITIVE:
				material.blend_mode = CanvasItemMaterial.BLEND_MODE_ADD
			BlendMode.MULTIPLY:
				material.blend_mode = CanvasItemMaterial.BLEND_MODE_MUL
			BlendMode.SCREEN:
				material.blend_mode = CanvasItemMaterial.BLEND_MODE_ADD # スクリーン合成もADDで表現？確認が必要
			BlendMode.OVERLAY:
				material.blend_mode = CanvasItemMaterial.BLEND_MODE_MIX # オーバーレイ合成もMIXで表現？確認が必要
			_:
				material.blend_mode = CanvasItemMaterial.BLEND_MODE_MIX # デフォルトは通常のブレンド

func is_playing() -> bool:
	return _is_playing

func is_stopped() -> bool:
	return !_is_playing and not is_inside_tree() and not is_instance_valid(self) # インスタンスが無効またはツリーから削除されたら停止とみなす

# 優先度の取得
func get_priority() -> int:
	return priority

# エフェクトの状態をリセットし、プールに戻せるように準備する
func reset() -> void:
	print("Debug: TestEffect.reset - Resetting effect: ", name)

	# 再生中の場合は停止する
	if _is_playing or _is_stopping:
		stop()

	# 状態変数を初期値に戻す
	_state = State.STOPPED
	_is_playing = false
	_is_stopping = false
	_elapsed_time = 0.0
	modulate.a = 1.0 # アルファ値をリセット
	_blend_mode = BlendMode.NORMAL # ブレンドモードをリセット

	# タイマーを停止し、シグナルを切断
	_cleanup_timer()

	# アニメーションを停止し、シグナルを切断
	if has_node("AnimationPlayer"):
		var anim_player = get_node("AnimationPlayer")
		anim_player.stop()
		if anim_player.animation_finished.is_connected(_on_animation_finished):
			anim_player.animation_finished.disconnect(_on_animation_finished)

	# ノードを非表示にする
	hide()
	print("Debug: TestEffect.reset - Effect reset complete: ", name)

func _cleanup_timer() -> void:
	if is_instance_valid(_timer):
		if _timer.timeout.is_connected(_on_timer_timeout):
			_timer.timeout.disconnect(_on_timer_timeout)
		_timer.stop()
		_timer.queue_free()
		_timer = null

# アニメーション完了時の処理
func _on_animation_finished(anim_name: String) -> void:
	if anim_name == "effect" and _is_playing and not _is_stopping:
		stop()
		# アニメーション完了シグナルを切断して、二重停止を防ぐ
		if has_node("AnimationPlayer"):
			$AnimationPlayer.animation_finished.disconnect(_on_animation_finished)

# タイマー完了時の処理
func _on_timer_timeout() -> void:
	print("Debug: _on_timer_timeout called for effect: ", name)
	print("Debug: - Is stopping: ", _is_stopping)
	print("Debug: - Is playing: ", _is_playing)
	
	if not _is_stopping:
		_is_playing = false
		print("Debug: Effect completed normally: ", name)
		
		# エフェクトを非表示にする
		visible = false
		
		# エフェクトプールへの解放
		if _effect_pool:
			print("Debug: Releasing effect to pool: ", _effect_pool.name)
			if _effect_pool._active_effects.has(effect_name):
				var active_list = _effect_pool._active_effects[effect_name]
				if not active_list.is_empty() and active_list.has(self):
					active_list.erase(self)
					print("Debug: Removed effect from active list")
			_effect_pool.release_effect(effect_name, self)
		else:
			_handle_pool_release_through_parent()
		
		# 完了シグナルを発火
		print("Debug: Emitting effect_completed signal")
		effect_completed.emit()
		
		# タイマーをクリーンアップ
		_cleanup_timer()
		
		# 状態をリセット
		_reset_state()

func _reset_state() -> void:
	_is_stopping = false
	_elapsed_time = 0.0
	print("Debug: Effect state reset")

# エフェクトプールの検索
func _find_effect_pool() -> EffectPool:
	if _effect_pool:
		return _effect_pool
	
	var parent = get_parent()
	while parent:
		if parent is EffectPool:
			_effect_pool = parent
			return parent
		parent = parent.get_parent()
	
	return null

# プロセス処理
func _process(delta: float) -> void:
	# durationによる自動停止はタイマーに任せるため削除
	pass 
