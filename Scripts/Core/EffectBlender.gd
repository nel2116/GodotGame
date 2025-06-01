class_name EffectBlender
extends Node

# ブレンドモード
enum BlendMode {
	NORMAL,     # 通常の再生
	ADDITIVE,   # 加算合成
	MULTIPLY,   # 乗算合成
	SCREEN,     # スクリーン合成
	OVERLAY     # オーバーレイ合成
}

# ブレンド設定
var _blend_settings: Dictionary = {}  # エフェクト名 -> ブレンド設定
var _default_blend_mode: BlendMode = BlendMode.NORMAL

# シグナル
signal blend_mode_changed(effect_name: String, mode: BlendMode)
signal blend_completed(effect_name: String)

func _init() -> void:
	name = "EffectBlender"
	# デフォルトのブレンドモードを設定
	_blend_settings["default"] = _default_blend_mode
	print("Debug: EffectBlender initialized with default blend mode: %d" % _default_blend_mode)

# ブレンドモードの設定
func set_blend_mode(effect_name: String, mode: BlendMode) -> void:
	if not effect_name.is_empty():
		_blend_settings[effect_name] = mode
		blend_mode_changed.emit(effect_name, mode)
		print("Debug: Blend mode set for '%s' to %d" % [effect_name, mode])
	else:
		push_warning("EffectBlender: Attempted to set blend mode with empty effect name")

# ブレンドモードの取得
func get_blend_mode(effect_name: String) -> BlendMode:
	if effect_name.is_empty():
		push_warning("EffectBlender: Attempted to get blend mode with empty effect name")
		return _default_blend_mode
	return _blend_settings.get(effect_name, _default_blend_mode)

# エフェクトのブレンド
func blend_effects(effects: Array[Node], params: Dictionary = {}) -> void:
	if effects.is_empty():
		push_warning("EffectBlender: Attempted to blend empty effects array")
		return
		
	var base_effect = effects[0]
	var blend_mode = get_blend_mode(base_effect.name)
	
	print("Debug: Blending effects for '%s' with mode %d" % [base_effect.name, blend_mode])
	
	match blend_mode:
		BlendMode.NORMAL:
			_blend_normal(effects, params)
		BlendMode.ADDITIVE:
			_blend_additive(effects, params)
		BlendMode.MULTIPLY:
			_blend_multiply(effects, params)
		BlendMode.SCREEN:
			_blend_screen(effects, params)
		BlendMode.OVERLAY:
			_blend_overlay(effects, params)
	
	# ブレンド完了を通知
	blend_completed.emit(base_effect.name)
	print("Debug: Blend completed for '%s'" % base_effect.name)

# 通常のブレンド
func _blend_normal(effects: Array[Node], params: Dictionary) -> void:
	for effect in effects:
		if effect.has_method("play"):
			effect.play()
			print("Debug: Playing effect '%s' in normal mode" % effect.name)

# 加算合成
func _blend_additive(effects: Array[Node], params: Dictionary) -> void:
	var base_alpha = params.get("base_alpha", 1.0)
	var blend_alpha = params.get("blend_alpha", 0.5)
	
	for i in range(effects.size()):
		var effect = effects[i]
		if effect.has_method("set_alpha"):
			effect.set_alpha(blend_alpha if i > 0 else base_alpha)
		if effect.has_method("play"):
			effect.play()
			print("Debug: Playing effect '%s' in additive mode with alpha %f" % [effect.name, blend_alpha if i > 0 else base_alpha])

# 乗算合成
func _blend_multiply(effects: Array[Node], params: Dictionary) -> void:
	var base_alpha = params.get("base_alpha", 1.0)
	var blend_alpha = params.get("blend_alpha", 0.5)
	
	for i in range(effects.size()):
		var effect = effects[i]
		if effect.has_method("set_alpha"):
			effect.set_alpha(blend_alpha if i > 0 else base_alpha)
		if effect.has_method("set_blend_mode"):
			effect.set_blend_mode(BlendMode.MULTIPLY)
		if effect.has_method("play"):
			effect.play()
			print("Debug: Playing effect '%s' in multiply mode with alpha %f" % [effect.name, blend_alpha if i > 0 else base_alpha])

# スクリーン合成
func _blend_screen(effects: Array[Node], params: Dictionary) -> void:
	var base_alpha = params.get("base_alpha", 1.0)
	var blend_alpha = params.get("blend_alpha", 0.5)
	
	for i in range(effects.size()):
		var effect = effects[i]
		if effect.has_method("set_alpha"):
			effect.set_alpha(blend_alpha if i > 0 else base_alpha)
		if effect.has_method("set_blend_mode"):
			effect.set_blend_mode(BlendMode.SCREEN)
		if effect.has_method("play"):
			effect.play()
			print("Debug: Playing effect '%s' in screen mode with alpha %f" % [effect.name, blend_alpha if i > 0 else base_alpha])

# オーバーレイ合成
func _blend_overlay(effects: Array[Node], params: Dictionary) -> void:
	var base_alpha = params.get("base_alpha", 1.0)
	var blend_alpha = params.get("blend_alpha", 0.5)
	
	for i in range(effects.size()):
		var effect = effects[i]
		if effect.has_method("set_alpha"):
			effect.set_alpha(blend_alpha if i > 0 else base_alpha)
		if effect.has_method("set_blend_mode"):
			effect.set_blend_mode(BlendMode.OVERLAY)
		if effect.has_method("play"):
			effect.play()
			print("Debug: Playing effect '%s' in overlay mode with alpha %f" % [effect.name, blend_alpha if i > 0 else base_alpha]) 
