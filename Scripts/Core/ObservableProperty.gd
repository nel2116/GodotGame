class_name ObservableProperty
extends RefCounted

var _value
var _observers: Array = []

func _init(initial_value = null) -> void:
    _value = initial_value

func _get_value():
    return _value

func _set_value(value):
    if _value != value:
        _value = value
        for obs in _observers:
            obs.call(value)

var value:
    get: _get_value
    set: _set_value

func subscribe(observer: Callable) -> void:
    _observers.append(observer)

func unsubscribe(observer: Callable) -> void:
    _observers.erase(observer)
