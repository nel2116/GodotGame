extends GutTest

var bus
var received

func before_each() -> void:
    bus = EventBus.new()
    add_child(bus)
    received = null

func after_each() -> void:
    bus.free()

func _on_event(data: Dictionary) -> void:
    received = data

func test_emit_and_subscribe() -> void:
    bus.Subscribe("TestEvent", Callable(self, "_on_event"))
    var data := {"value": 42}
    bus.EmitEvent("TestEvent", data)
    bus._Process(0)
    assert_eq(received, data)
    var history = bus.GetEventHistory("TestEvent")
    assert_eq(history.size(), 1)
    assert_eq(history[0], data)

func test_unsubscribe() -> void:
    bus.Subscribe("TestEvent", Callable(self, "_on_event"))
    bus.Unsubscribe("TestEvent", Callable(self, "_on_event"))
    bus.EmitEvent("TestEvent", {})
    bus._Process(0)
    assert_eq(received, null)

func test_priority() -> void:
    bus.Subscribe("TestEvent", Callable(self, "_on_event"))
    var low := {"value": 1}
    var high := {"value": 2}
    bus.EmitEvent("TestEvent", low, 0)
    bus.EmitEvent("TestEvent", high, 1)
    bus._Process(0)
    assert_eq(received, high)

func _filter(data: Dictionary) -> bool:
    return data.get("ok", false)

func test_filter() -> void:
    bus.Subscribe("TestEvent", Callable(self, "_on_event"), Callable(self, "_filter"))
    bus.EmitEvent("TestEvent", {"ok": false})
    bus.EmitEvent("TestEvent", {"ok": true})
    bus._Process(0)
    assert_ne(received, null)
    assert_true(received["ok"])

func test_history_cleanup() -> void:
    for i in range(105):
        bus.EmitEvent("TestEvent", {"i": i})
    bus._Process(0)
    var history = bus.GetEventHistory("TestEvent")
    assert_eq(history.size(), 100)
