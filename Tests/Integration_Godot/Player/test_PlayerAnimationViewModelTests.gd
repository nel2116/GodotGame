extends GutTest

# PlayerAnimationViewModelTests.gd
# GUT版のプレイヤーアニメーションViewModelテスト
# ※C#クラスを直接参照しているため、GDScriptからは実行不可。
#   テストを有効化するにはC#ノードのGDScriptラッパーまたはテスト用ダミーが必要です。

# var event_bus: NewGameProject.GameEventBus
# var animation_model: NewGameProject.PlayerAnimationModel
# var animation_view_model: NewGameProject.PlayerAnimationViewModel

# func before_each():
# 	event_bus = NewGameProject.GameEventBus.new()
# 	animation_model = NewGameProject.PlayerAnimationModel.new(event_bus)
# 	animation_view_model = NewGameProject.PlayerAnimationViewModel.new(animation_model, event_bus)

# func after_each():
# 	animation_view_model.free()
# 	animation_model.free()
# 	event_bus.free()

# func test_initialize_default_animation_idle():
# 	animation_view_model.initialize()
# 	assert_eq(animation_view_model.current_animation.value, "Idle")

# func test_play_animation_valid_name_publishes_events():
# 	var received_event = null
# 	event_bus.get_event_stream("AnimationPlayEvent").subscribe(
# 		func(event): received_event = event
# 	)
# 	animation_view_model.initialize()
# 	animation_view_model.play_animation("Jump")
# 	await get_tree().process_frame
# 	assert_not_null(received_event)
# 	assert_eq(received_event.animation_name, "Jump")
# 	assert_eq(animation_view_model.current_animation.value, "Jump")

# func test_play_animation_invalid_name_publishes_error():
# 	var error_received = false
# 	event_bus.get_event_stream("ErrorEvent").subscribe(
# 		func(event): error_received = true
# 	)
# 	animation_view_model.initialize()
# 	animation_view_model.play_animation("InvalidAnimation")
# 	await get_tree().process_frame
# 	assert_true(error_received, "Error event should be published for invalid animation")

# func test_blend_animation_publishes_events():
# 	var received_event = null
# 	event_bus.get_event_stream("AnimationBlendEvent").subscribe(
# 		func(event): received_event = event
# 	)
# 	animation_view_model.initialize()
# 	animation_view_model.blend_animation("Walk", 0.5)
# 	await get_tree().process_frame
# 	assert_not_null(received_event)
# 	assert_eq(received_event.animation_name, "Walk")
# 	assert_eq(received_event.blend_weight, 0.5)

# func test_update_animation_state():
# 	animation_view_model.initialize()
# 	animation_view_model.update()

# func test_stop_animation_publishes_event():
# 	var received_event = null
# 	event_bus.get_event_stream("AnimationStopEvent").subscribe(
# 		func(event): received_event = event
# 	)
# 	animation_view_model.initialize()
# 	animation_view_model.stop_animation()
# 	await get_tree().process_frame
# 	assert_not_null(received_event)
# 	assert_eq(animation_view_model.current_animation.value, "Idle") 