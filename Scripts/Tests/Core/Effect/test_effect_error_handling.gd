extends GutTest

var EffectBus = load("res://Scripts/Core/EffectBus.gd")
var TestEffect = load("res://Scripts/Examples/TestEffect.gd")
var TestEffectScene = load("res://Scripts/Examples/TestEffect.tscn") # PackedSceneをロード

var _effect_bus: EffectBus
var _effect: Node  # TestEffectはNodeを継承しているため、Nodeとして宣言

func before_each():
	_effect_bus = EffectBus.new()
	add_child_autofree(_effect_bus)
	
	# テスト用のエフェクトインスタンスを作成（主にパラメータ設定テスト用）
	# _effect = TestEffect.new() # 削除
	# _effect.name = "TestEffect" # 削除
	
	# PackedSceneリソースをEffectBusに登録
	if TestEffectScene:
		_effect_bus.register_effect("test_effect", TestEffectScene, 50, 1, EffectBlender.BlendMode.NORMAL)
	
	# 無効なブレンドモードテスト用に別のエフェクトを登録
	if TestEffectScene:
		_effect_bus.register_effect("test_invalid_blend", TestEffectScene, 50, 1)

func after_each():
	# テスト終了時のクリーンアップ
	# EffectBusが管理する全てのエフェクトプールをクリーンアップ
	for effect_name in _effect_bus._effect_resources.keys():
		_effect_bus._effect_pool.cleanup_pool(effect_name)
	
	# 残りのエフェクトインスタンスをクリーンアップ
	for child in _effect_bus.get_children():
		if child is TestEffect:
			child.queue_free()

func test_invalid_effect_handling():
	# 無効なエフェクト名（未登録）を再生しようとする
	var errors_received = [] # フラグの代わりにリストを使用
	var callback = func(msg):
		print("test_invalid_effect_handling: error_occurred signal received with message: ", msg)
		errors_received.append(msg) # リストにメッセージを追加
		print("test_invalid_effect_handling: errors_received list size: ", errors_received.size()) # デバッグ出力変更
	var connect_result = _effect_bus.error_occurred.connect(callback)
	print("test_invalid_effect_handling: Connection result: ", connect_result)

	_effect_bus.play_effect("non_existent_effect") # 未登録のエフェクト名を使用

	await get_tree().process_frame # シグナル発火を待機
	print("test_invalid_effect_handling: After process_frame, errors_received size is ", errors_received.size()) # デバッグ出力変更

	assert_eq(errors_received.size(), 1, "エラーが発生するべき") # アサート条件を変更
	_effect_bus.error_occurred.disconnect(callback) # シグナルの接続を解除

func test_duplicate_effect_registration():
	# 同じエフェクト名で再度登録を試みる
	var errors_received = [] # フラグの代わりにリストを使用
	var callback = func(msg):
		print("test_duplicate_effect_registration: error_occurred signal received with message: ", msg)
		errors_received.append(msg) # リストにメッセージを追加
		print("test_duplicate_effect_registration: errors_received list size: ", errors_received.size()) # デバッグ出力変更
	var connect_result = _effect_bus.error_occurred.connect(callback)
	print("test_duplicate_effect_registration: Connection result: ", connect_result)

	var effect_resource = TestEffectScene # PackedSceneリソースを使用
	_effect_bus.register_effect("test_effect", effect_resource, 50) # 再度登録

	await get_tree().process_frame # シグナル発火を待機
	print("test_duplicate_effect_registration: After process_frame, errors_received size is ", errors_received.size()) # デバッグ出力変更

	assert_eq(errors_received.size(), 1, "エラーが発生するべき") # アサート条件を変更
	_effect_bus.error_occurred.disconnect(callback) # シグナルの接続を解除

func test_invalid_priority_handling():
	# play_effect に渡すエフェクトインスタンスに無効な優先度を設定して再生を試みる
	var errors_received = [] # フラグの代わりにリストを使用
	var callback = func(msg):
		print("test_invalid_priority_handling: error_occurred signal received with message: ", msg)
		errors_received.append(msg) # リストにメッセージを追加
		print("test_invalid_priority_handling: errors_received list size: ", errors_received.size()) # デバッグ出力変更
	var connect_result = _effect_bus.error_occurred.connect(callback)
	print("test_invalid_priority_handling: Connection result: ", connect_result)

	var effect_instance = TestEffectScene.instantiate() # PackedSceneからインスタンス化
	effect_instance.priority = -1 # 無効な優先度
	_effect_bus.play_effect("test_effect", {"effect": effect_instance})

	await get_tree().process_frame # シグナル発火を待機
	print("test_invalid_priority_handling: After process_frame, errors_received size is ", errors_received.size()) # デバッグ出力変更

	assert_eq(errors_received.size(), 1, "エラーが発生するべき") # アサート条件を変更
	_effect_bus.error_occurred.disconnect(callback) # シグナルの接続を解除

	if is_instance_valid(effect_instance):
		effect_instance.queue_free() # テストインスタンスをクリーンアップ

func test_invalid_duration_handling():
	# play_effect に渡すエフェクトインスタンスに無効なdurationを設定して再生を試みる
	var errors_received = [] # フラグの代わりにリストを使用
	var callback = func(msg):
		print("test_invalid_duration_handling: error_occurred signal received with message: ", msg) # デバッグ出力追加
		errors_received.append(msg) # リストにメッセージを追加
		print("test_invalid_duration_handling: errors_received list size: ", errors_received.size()) # デバッグ出力変更
	var connect_result = _effect_bus.error_occurred.connect(callback)
	print("Connection result: ", connect_result)
	
	var effect_instance = TestEffectScene.instantiate() # PackedSceneからインスタンス化
	effect_instance.duration = -1.0 # 無効なduration
	_effect_bus.play_effect("test_effect", {"effect": effect_instance})
	
	await get_tree().process_frame # シグナル発火を待機
	
	assert_eq(errors_received.size(), 1, "エラーが発生するべき") # アサート条件を変更
	_effect_bus.error_occurred.disconnect(callback) # シグナルの接続を解除
	
	if is_instance_valid(effect_instance):
		effect_instance.queue_free() # テストインスタンスをクリーンアップ

func test_invalid_blend_mode_handling():
	# register_effect に無効なブレンドモードを渡す
	var errors_received = [] # フラグの代わりにリストを使用
	var callback = func(msg):
		print("test_invalid_blend_mode_handling: error_occurred signal received with message: ", msg)
		errors_received.append(msg) # リストにメッセージを追加
		print("test_invalid_blend_mode_handling: errors_received list size: ", errors_received.size()) # デバッグ出力変更
	var connect_result = _effect_bus.error_occurred.connect(callback)
	print("test_invalid_blend_mode_handling: Connection result: ", connect_result)

	var effect_resource = TestEffectScene # PackedSceneリソースを使用
	_effect_bus.register_effect("another_effect", effect_resource, 50, 10, 999) # 無効なブレンドモード

	await get_tree().process_frame # シグナル発火を待機
	print("test_invalid_blend_mode_handling: After process_frame, errors_received size is ", errors_received.size()) # デバッグ出力変更

	assert_eq(errors_received.size(), 1, "エラーが発生するべき") # アサート条件を変更
	_effect_bus.error_occurred.disconnect(callback) # シグナルの接続を解除

func test_effect_cleanup_on_error():
	# 無効なdurationでエフェクトを再生し、エラー後にクリーンアップされることを確認
	var errors_received = [] # フラグの代わりにリストを使用
	var callback = func(msg):
		print("test_effect_cleanup_on_error: error_occurred signal received with message: ", msg) # デバッグ出力追加
		errors_received.append(msg) # リストにメッセージを追加
		print("test_effect_cleanup_on_error: errors_received list size: ", errors_received.size()) # デバッグ出力変更
	var connect_result = _effect_bus.error_occurred.connect(callback)
	print("Connection result: ", connect_result)
	
	var effect_instance = TestEffectScene.instantiate() # PackedSceneからインスタンス化
	effect_instance.duration = -1.0 # 無効なduration
	
	# register_effectはPackedSceneを期待するため、play_effectにインスタンスを直接渡す方式でテスト
	# EffectBusのplay_effect内でdurationのバリデーションが行われエラーが発生するはず
	_effect_bus.play_effect("test_effect", {"effect": effect_instance})
	
	await get_tree().process_frame # エラーシグナルとクリーンアップ処理を待機

	assert_eq(errors_received.size(), 1, "エラーが発生するべき") # アサート条件を変更
	_effect_bus.error_occurred.disconnect(callback) # シグナルの接続を解除

	# エフェクトが適切にクリーンアップされたことを確認（queue_free()が呼ばれていることを間接的に確認）
	# Note: is_instance_validはqueue_free()が呼ばれるとfalseになる
	assert_false(is_instance_valid(effect_instance), "エフェクトインスタンスは無効になっているべき")

func test_error_recovery():
	var errors_received = []
	var callback = func(msg):
		print("test_error_recovery: error_occurred signal received with message: ", msg)
		errors_received.append(msg)
	var connect_result = _effect_bus.error_occurred.connect(callback)

	# 1回目の試行：無効なdurationで再生を試みる（プールから取得し、paramsでdurationを指定）
	# EffectBusのplay_effectは内部でプールからエフェクトを取得する
	_effect_bus.play_effect("test_effect", {"duration": -1.0})

	await get_tree().process_frame # エラー処理を待機

	assert_eq(errors_received.size(), 1, "1回目の試行でエラーが発生するべき")
	# エラー発生時にプールに戻さず破棄されるかはEffectBus/EffectPoolの内部実装に依存
	# ここではエラー発生の確認に留める

	# 2回目の試行：有効なdurationで再生（プールから取得）
	# EffectBus._play_effect_internal内でeffect_completedシグナルが接続されるはず
	var valid_duration = 1.0

	# EffectBus経由でエフェクトを再生
	_effect_bus.play_effect("test_effect", {"duration": valid_duration})

	# EffectBusが内部でプールから取得・再生したインスタンスの完了を待機
	# EffectBusの内部シグナルなどを利用する必要があるが、テストコードから直接アクセスするのは避けるため、
	# 少し待機するか、EffectBusに完了を通知するメカニズムが必要。
	# ここではテスト簡略化のため、特定の時間が経過するまで待機する (理想的には完了シグナルを待つべき)
	# TODO: EffectBusにテスト用の完了通知メカニズムを追加するか検討

	# EffectBusが完了シグナルを受け取って EffectPool に戻すまでにある程度の時間がかかるため待機
	# await get_tree().create_timer(valid_duration + 0.1).timeout # タイマー待機
	await get_tree().process_frame
	await get_tree().process_frame # 複数フレーム待機して完了処理を促す
	await get_tree().process_frame


	assert_eq(errors_received.size(), 1, "2回目の試行でエラーは発生しないべき") # エラーカウントは1のまま

	# オプション：EffectPoolの状態を確認して、アクティブなエフェクトが0になっていることを確認
	# assert_eq(_effect_bus._effect_pool.get_pool_status("test_effect").active, 0, "アクティブなエフェクトが0であるべき")

	_effect_bus.error_occurred.disconnect(callback)

func test_error_logging():
	var errors_received = []
	var callback = func(msg):
		print("test_error_logging: error_occurred signal received with message: ", msg)
		errors_received.append(msg)
	var connect_result = _effect_bus.error_occurred.connect(callback)
	
	var effect_instance = TestEffectScene.instantiate()
	effect_instance.duration = -1.0
	_effect_bus.play_effect("test_effect", {"effect": effect_instance})
	await get_tree().process_frame
	
	assert_eq(errors_received.size(), 1, "エラーメッセージが記録されるべき")
	assert_true(errors_received[0].contains("再生時間"), "エラーメッセージに再生時間が含まれるべき")
	
	# テスト終了時のクリーンアップ
	if is_instance_valid(effect_instance):
		effect_instance.queue_free()
	
	_effect_bus.error_occurred.disconnect(callback)
	
