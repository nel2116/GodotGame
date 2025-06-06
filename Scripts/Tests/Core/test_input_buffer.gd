extends GutTest

var buffer

func before_each() -> void:
    buffer = InputBuffer.new()
    add_child(buffer)

func after_each() -> void:
    buffer.queue_free()

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
