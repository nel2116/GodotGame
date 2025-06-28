extends GutTest

# PlayerInputViewModelTests.gd
# GUT版のプレイヤー入力ViewModelテスト

var event_bus: GameEventBus
var input_model: PlayerInputModel
var input_view_model: PlayerInputViewModel

func before_each():
	event_bus = GameEventBus.new()
	input_model = PlayerInputModel.new(event_bus)
	input_view_model = PlayerInputViewModel.new(input_model, event_bus)

func after_each():
	input_view_model.free()
	input_model.free()
	event_bus.free()

func test_initialize_default_state_is_enabled():
	# 準備・実行
	input_view_model.initialize()
	
	# 検証
	assert_true(input_view_model.is_enabled.value)

func test_update_input_publishes_state_event():
	# 準備
	var received_event = null
	event_bus.get_event_stream("InputStateChangedEvent").subscribe(
		func(event): received_event = event
	)
	input_view_model.initialize()
	
	# 実行
	input_view_model.update_input()
	
	# 少し待機してイベント処理を完了させる
	await get_tree().process_frame
	
	# 検証
	assert_not_null(received_event)
	assert_not_null(received_event.state)

func test_initialize_publishes_enabled_event():
	# 準備
	var received_event = null
	event_bus.get_event_stream("InputEnabledChangedEvent").subscribe(
		func(event): received_event = event
	)
	
	# 実行
	input_view_model.initialize()
	
	# 少し待機してイベント処理を完了させる
	await get_tree().process_frame
	
	# 検証
	assert_not_null(received_event)
	assert_true(received_event.enabled) 