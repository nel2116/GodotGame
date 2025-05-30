extends Node

# エフェクトシグナル定義
signal effect_triggered(effect_name: String, params: Dictionary)
signal vfx_played(vfx_name: String, position: Vector3)
signal se_played(se_name: String)
signal camera_shake(intensity: float, duration: float)

# エフェクトリソースの管理
var _effect_resources: Dictionary = {}

# EventBusへの参照
var _event_bus: EventBus

func _init() -> void:
    name = "EffectBus"

# EventBusのリスナー設定
func setup_event_listeners(event_bus: EventBus) -> void:
    _event_bus = event_bus

    # イベントとエフェクトの紐付け
    _event_bus.player_damaged.connect(_on_player_damaged)
    _event_bus.enemy_damaged.connect(_on_enemy_damaged)
    _event_bus.player_died.connect(_on_player_died)
    _event_bus.enemy_died.connect(_on_enemy_died)

# エフェクトリソースの登録
func register_effect(effect_name: String, resource: Resource) -> void:
    _effect_resources[effect_name] = resource

# エフェクトの再生
func play_effect(effect_name: String, params: Dictionary = {}) -> void:
    if _effect_resources.has(effect_name):
        effect_triggered.emit(effect_name, params)
        # エフェクトの再生処理
        _play_effect_internal(effect_name, params)
    else:
        push_error("EffectBus: 未登録のエフェクト '%s' が呼び出されました" % effect_name)

# 内部的なエフェクト再生処理
func _play_effect_internal(effect_name: String, params: Dictionary) -> void:
    var resource = _effect_resources[effect_name]
    # TODO: エフェクトの具体的な再生処理を実装

# イベントハンドラ
func _on_player_damaged(amount: float, source: Node) -> void:
    play_effect("player_hit", {
        "amount": amount,
        "position": source.global_position
    })

func _on_enemy_damaged(enemy: Node, amount: float, source: Node) -> void:
    play_effect("enemy_hit", {
        "amount": amount,
        "position": enemy.global_position
    })

func _on_player_died() -> void:
    play_effect("player_death")

func _on_enemy_died(enemy: Node) -> void:
    play_effect("enemy_death", {
        "position": enemy.global_position
    })
