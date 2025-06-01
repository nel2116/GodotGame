extends GutTest

var effect_bus: EffectBus
var effect_pool: EffectPool
var effect_blender: EffectBlender
var mock_resource: Resource

func before_each() -> void:
	effect_bus = EffectBus.new()
	effect_pool = EffectPool.new()
	effect_blender = EffectBlender.new()
	add_child(effect_bus)
	add_child(effect_pool)
	add_child(effect_blender)
	
	# モックリソースの作成
	mock_resource = load("res://Scripts/Examples/TestEffect.tscn")
	assert_not_null(mock_resource, "テスト用エフェクトリソースの読み込みに失敗しました")

func after_each() -> void:
	effect_bus.queue_free()
	effect_pool.queue_free()
	effect_blender.queue_free()

func test_system_integration() -> void:
	# エフェクトの登録
	effect_bus.register_effect("test_effect", mock_resource, 80, 5, EffectBlender.BlendMode.ADDITIVE)
	
	# 各コンポーネントの初期状態を確認
	assert_true(effect_bus._effect_resources.has("test_effect"), "EffectBus: エフェクトが正しく登録されていません")
	var pool_status = effect_bus.get_pool_status("test_effect")
	assert_eq(pool_status.total, 5, "EffectPool: プールサイズが正しく設定されていません")
	assert_eq(effect_blender.get_blend_mode("test_effect"), EffectBlender.BlendMode.ADDITIVE, "EffectBlender: ブレンドモードが正しく設定されていません")
	
	# エフェクトの再生
	var params = {
		"position": Vector2(1, 1),
		"num_effects": 2,
		"base_alpha": 1.0,
		"blend_alpha": 0.5
	}
	effect_bus.play_effect("test_effect", params)
	
	# 再生後の各コンポーネントの状態を確認
	pool_status = effect_bus.get_pool_status("test_effect")
	assert_eq(pool_status.active, 2, "EffectBus: アクティブなエフェクトの数が正しくありません")

func test_component_interaction() -> void:
	# エフェクトの登録
	effect_bus.register_effect("test_effect", mock_resource, 50, 3, EffectBlender.BlendMode.ADDITIVE)
	
	# EffectBusとEffectPoolの連携テスト
	effect_bus.play_effect("test_effect")
	var pool_status = effect_bus.get_pool_status("test_effect")
	assert_eq(pool_status.active, 1, "EffectBusとEffectPoolの連携が正しく機能していません")
	
	# EffectPoolとEffectBlenderの連携テスト
	effect_blender.set_blend_mode("test_effect", EffectBlender.BlendMode.MULTIPLY)
	assert_eq(effect_blender.get_blend_mode("test_effect"), EffectBlender.BlendMode.MULTIPLY, "EffectPoolとEffectBlenderの連携が正しく機能していません")

func test_pool_management() -> void:
	# エフェクトの登録
	effect_bus.register_effect("test_effect", mock_resource, 50, 2)
	
	# プールサイズを超える再生
	var params = {"num_effects": 3}
	effect_bus.play_effect("test_effect", params)
	
	# プールの拡張確認
	var pool_status = effect_bus.get_pool_status("test_effect")
	assert_eq(pool_status.total, 3, "プールが正しく拡張されていません")
	assert_eq(pool_status.active, 3, "アクティブなエフェクトの数が正しくありません")

func test_blend_mode_changes() -> void:
	# エフェクトの登録
	effect_bus.register_effect("test_effect", mock_resource)
	
	# ブレンドモードの変更
	effect_bus.set_blend_mode("test_effect", EffectBlender.BlendMode.MULTIPLY)
	assert_eq(effect_blender.get_blend_mode("test_effect"), EffectBlender.BlendMode.MULTIPLY, "ブレンドモードが正しく変更されていません")
	
	# エフェクト再生時のブレンドモード適用確認
	effect_bus.play_effect("test_effect")
	var pool_status = effect_bus.get_pool_status("test_effect")
	assert_eq(pool_status.active, 1, "エフェクトが正しく再生されていません")

func test_error_handling() -> void:
	# 未登録のエフェクト
	effect_bus.play_effect("nonexistent_effect")
	var pool_status = effect_bus.get_pool_status("nonexistent_effect")
	assert_eq(pool_status.size(), 0, "未登録のエフェクトが処理されています")
	
	# 無効なリソース
	effect_bus.register_effect("invalid_effect", null)
	assert_false(effect_bus._effect_resources.has("invalid_effect"), "無効なリソースが登録されています") 
