extends GutTest

var buffer

func before_each() -> void:
    buffer = InputBuffer.new()
    add_child(buffer)

func after_each() -> void:
    buffer.free()

func test_enqueue_dequeue() -> void:
    var event_a := InputEventKey.new()
    event_a.keycode = KEY_A
    var event_b := InputEventKey.new()
    event_b.keycode = KEY_B

    buffer.Enqueue(event_a)
    buffer.Enqueue(event_b)

    var result_a = buffer.Dequeue()
    assert_true(result_a is InputEventKey)
    assert_eq(result_a.keycode, KEY_A)

    var result_b = buffer.Dequeue()
    assert_true(result_b is InputEventKey)
    assert_eq(result_b.keycode, KEY_B)

    assert_eq(buffer.Dequeue(), null)

func test_clear() -> void:
    var event := InputEventKey.new()
    event.keycode = KEY_A
    buffer.Enqueue(event)
    buffer.Clear()
    assert_eq(buffer.Dequeue(), null)

func test_retention_time() -> void:
    buffer.RetentionTime = 0.0
    var event := InputEventKey.new()
    event.keycode = KEY_A
    buffer.Enqueue(event)
    buffer._Process(0)
    assert_eq(buffer.Dequeue(), null)

func test_input_observer() -> void:
    var observer = InputObserver.new()
    observer.Buffer = buffer
    add_child(observer)
    var event := InputEventKey.new()
    event.keycode = KEY_B
    observer._UnhandledInput(event)
    var result = buffer.Dequeue()
    assert_true(result is InputEventKey)
    assert_eq(result.keycode, KEY_B)
    observer.free()
