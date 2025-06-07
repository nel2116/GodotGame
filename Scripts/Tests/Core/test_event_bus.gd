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
	assert_eq(received, data)
	var history = bus.GetEventHistory("TestEvent")
	assert_eq(history.size(), 1)
	assert_eq(history[0], data)

func test_unsubscribe() -> void:
	bus.Subscribe("TestEvent", Callable(self, "_on_event"))
	bus.Unsubscribe("TestEvent", Callable(self, "_on_event"))
	bus.EmitEvent("TestEvent", {})
	assert_eq(received, null)
