extends GutTest

var bus
var fsm
var received

func before_each() -> void:
    bus = EventBus.new()
    fsm = PlayerStateMachine.new()
    add_child(bus)
    add_child(fsm)
    fsm.EventBus = bus
    received = null
    bus.Subscribe("PlayerStateChanged", Callable(self, "_on_state_change"))

func after_each() -> void:
    fsm.free()
    bus.free()

func _on_state_change(data: Dictionary) -> void:
    received = data

func test_change_state_emit_event() -> void:
    fsm.ChangeState(1) # PlayerState.Moving
    bus._Process(0)
    assert_eq(fsm.CurrentState, 1)
    assert_not_null(received)
    assert_eq(received["from"], 0)
    assert_eq(received["to"], 1)

func test_no_event_when_same_state() -> void:
    fsm.ChangeState(0)
    bus._Process(0)
    assert_eq(received, null)

func test_invalid_transition() -> void:
    fsm.ChangeState(2) # Attacking from Idle allowed
    fsm.ChangeState(2) # Attacking again (not allowed)
    assert_eq(fsm.CurrentState, 2)

func test_timeout_cancel() -> void:
    fsm.ChangeState(2) # Attacking
    fsm.Timeouts[2] = 0.0
    fsm._Process(0)
    assert_eq(fsm.CurrentState, 0)

func test_state_manager_sync() -> void:
    var manager = StateManager.new()
    add_child(manager)
    fsm.StateManager = manager
    fsm.ChangeState(1)
    assert_eq(manager.GetState("PlayerState"), 1)
    manager.free()
