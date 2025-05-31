class_name GameSystems
extends Node

# シングルトンインスタンス
static var instance: GameSystems

# サブシステム
var event_bus: EventBus
var effect_bus: EffectBus
var telemetry: Telemetry

# 初期化フラグ
var _is_initialized: bool = false

func _init() -> void:
    if instance != null:
        push_error("GameSystems: 複数のインスタンスが作成されようとしています")
        return
    instance = self

func _enter_tree() -> void:
    if _is_initialized:
        return

    # サブシステムの初期化
    _initialize_subsystems()
    _is_initialized = true

    # デバッグログ
    print("GameSystems: 初期化完了")

func _exit_tree() -> void:
    if instance == self:
        instance = null

# サブシステムの初期化
func _initialize_subsystems() -> void:
    # EventBusの初期化
    event_bus = EventBus.new()
    add_child(event_bus)

    # EffectBusの初期化
    effect_bus = EffectBus.new()
    add_child(effect_bus)

    # Telemetryの初期化
    telemetry = Telemetry.new()
    add_child(telemetry)

    # サブシステム間の依存関係の設定
    _setup_dependencies()

# サブシステム間の依存関係の設定
func _setup_dependencies() -> void:
    # TelemetryはEventBusを監視
    telemetry.setup_event_listeners(event_bus)

    # EffectBusはEventBusを監視
    effect_bus.setup_event_listeners(event_bus)

# エラーハンドリング
func _handle_error(error: String) -> void:
    push_error("GameSystems: " + error)
    # エラーをTelemetryに記録
    if telemetry:
        telemetry.log_error(error)
