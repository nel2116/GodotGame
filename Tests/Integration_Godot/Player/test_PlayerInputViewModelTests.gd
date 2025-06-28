extends GutTest

# PlayerInputViewModelTests.gd
# GUT版のプレイヤー入力ViewModelテスト
# ※C#クラスを直接参照しているため、GDScriptからは実行不可。
#   テストを有効化するにはC#ノードのGDScriptラッパーまたはテスト用ダミーが必要です。

# var event_bus: NewGameProject.GameEventBus
# var input_model: NewGameProject.PlayerInputModel
# var input_view_model: NewGameProject.PlayerInputViewModel

var input_node

func before_each():
    input_node = preload("res://Scripts/Systems/Player/Input/PlayerInputViewModelNode.cs").new()
    add_child(input_node)
    input_node.Initialize()

func after_each():
    remove_child(input_node)
    input_node.queue_free()
    await get_tree().process_frame

func test_initialize_default_state_is_enabled():
    assert_true(input_node.IsEnabled)

# func test_update_input_publishes_state_event():
# 	var received_event = null
# 	event_bus.get_event_stream("InputStateChangedEvent").subscribe(
# 		func(event): received_event = event
# 	)
# 	input_view_model.initialize()
# 	input_view_model.update_input()
# 	await get_tree().process_frame
# 	assert_not_null(received_event)
# 	assert_not_null(received_event.state)

# func test_initialize_publishes_enabled_event():
# 	var received_event = null
# 	event_bus.get_event_stream("InputEnabledChangedEvent").subscribe(
# 		func(event): received_event = event
# 	)
# 	input_view_model.initialize()
# 	await get_tree().process_frame
# 	assert_not_null(received_event)
# 	assert_true(received_event.enabled) 