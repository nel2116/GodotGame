# Main.gd
extends Node3D

const MAIN_VIEW_MODEL = preload("res://Scripts/ViewModels/MainViewModel.gd")

var _view_model: MainViewModel

func _ready() -> void:
    _view_model = MAIN_VIEW_MODEL.new()
    _view_model.message.subscribe(_on_message_changed)
    _view_model.set_message("ゲームの初期化処理を実行します。")

func _on_message_changed(text) -> void:
    print(text)

func _exit_tree() -> void:
    _view_model.message.unsubscribe(_on_message_changed)
