extends GutTest

var manager

func before_each() -> void:
    manager = StateManager.new()
    add_child(manager)

func after_each() -> void:
    manager.free()

func test_set_get_has_remove() -> void:
    manager.SetState("score", 10)
    assert_true(manager.HasState("score"))
    assert_eq(manager.GetState("score"), 10)

    manager.SetState("score", 20)
    var history = manager.GetStateHistory("score")
    assert_eq(history.size(), 2)
    assert_eq(history[0], 10)
    assert_eq(history[1], 20)

    manager.RemoveState("score")
    assert_false(manager.HasState("score"))
    assert_eq(manager.GetState("score"), null)

func test_get_state_default() -> void:
    assert_eq(manager.GetState("unknown"), null)
