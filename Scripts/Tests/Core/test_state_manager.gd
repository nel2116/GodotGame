extends GutTest

const StateManagerClass = preload("res://Scripts/Core/StateManager.cs")
var manager
var received

func before_each() -> void:
    manager = StateManagerClass.new()
    add_child(manager)
    received = null

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

func _observer(data: Dictionary) -> void:
    received = data

func test_transition_and_observer() -> void:
    manager.RegisterTransition("mode", 0, 1)
    manager.Observe("mode", Callable(self, "_observer"))
    manager.SetState("mode", 0)
    manager.SetState("mode", 1)
    assert_eq(received["from"], 0)
    assert_eq(received["to"], 1)
    manager.SetState("mode", 0) # invalid back transition
    assert_eq(manager.GetState("mode"), 1)
    manager.Unobserve("mode", Callable(self, "_observer"))

func test_persistence() -> void:
    manager.SetState("save", 123)
    var path := "user://state_test.json"
    manager.SaveAll(path)
    var other = StateManagerClass.new()
    add_child(other)
    other.LoadAll(path)
    assert_eq(other.GetState("save"), 123)
    other.free()
    if FileAccess.file_exists(path):
        DirAccess.remove_absolute(path)
