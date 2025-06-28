extends GutTest

# PlayerAnimationViewModelTests.gd
# GUT版のプレイヤーアニメーションViewModelテスト

var event_bus: GameEventBus
var animation_model: PlayerAnimationModel
var animation_view_model: PlayerAnimationViewModel

func before_each():
	event_bus = GameEventBus.new()
	animation_model = PlayerAnimationModel.new(event_bus)
	animation_view_model = PlayerAnimationViewModel.new(animation_model, event_bus)

func after_each():
	animation_view_model.free()
	animation_model.free()
	event_bus.free()

func test_initialize_default_animation_idle():
	# 準備・実行
	animation_view_model.initialize()
	
	# 検証
	assert_eq(animation_view_model.current_animation.value, "Idle")

func test_play_animation_valid_name_publishes_events():
	# 準備
	var received_event = null
	event_bus.get_event_stream("AnimationPlayEvent").subscribe(
		func(event): received_event = event
	)
	animation_view_model.initialize()
	
	# 実行
	animation_view_model.play_animation("Jump")
	
	# 少し待機してイベント処理を完了させる
	await get_tree().process_frame
	
	# 検証
	assert_not_null(received_event)
	assert_eq(received_event.animation_name, "Jump")
	assert_eq(animation_view_model.current_animation.value, "Jump")

func test_play_animation_invalid_name_publishes_error():
	# 準備
	var error_received = false
	event_bus.get_event_stream("ErrorEvent").subscribe(
		func(event): error_received = true
	)
	animation_view_model.initialize()
	
	# 実行
	animation_view_model.play_animation("InvalidAnimation")
	
	# 少し待機してイベント処理を完了させる
	await get_tree().process_frame
	
	# 検証
	assert_true(error_received, "Error event should be published for invalid animation")

func test_blend_animation_publishes_events():
	# 準備
	var received_event = null
	event_bus.get_event_stream("AnimationBlendEvent").subscribe(
		func(event): received_event = event
	)
	animation_view_model.initialize()
	
	# 実行
	animation_view_model.blend_animation("Walk", 0.5)
	
	# 少し待機してイベント処理を完了させる
	await get_tree().process_frame
	
	# 検証
	assert_not_null(received_event)
	assert_eq(received_event.animation_name, "Walk")
	assert_eq(received_event.blend_weight, 0.5)

func test_update_animation_state():
	# 準備
	animation_view_model.initialize()
	
	# 実行
	animation_view_model.update()
	
	# 検証（エラーが発生しないことを確認）
	# GUTでは例外が発生した場合自動的にテストが失敗する

func test_stop_animation_publishes_event():
	# 準備
	var received_event = null
	event_bus.get_event_stream("AnimationStopEvent").subscribe(
		func(event): received_event = event
	)
	animation_view_model.initialize()
	
	# 実行
	animation_view_model.stop_animation()
	
	# 少し待機してイベント処理を完了させる
	await get_tree().process_frame
	
	# 検証
	assert_not_null(received_event)
	assert_eq(animation_view_model.current_animation.value, "Idle") 