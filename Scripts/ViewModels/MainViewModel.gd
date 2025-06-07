class_name MainViewModel
const ObservableProperty = preload("res://Scripts/Core/ObservableProperty.gd")
extends Node

@onready var message: ObservableProperty = ObservableProperty.new("")

func set_message(text: String) -> void:
    message.value = text
