class_name Telemetry
extends Node

# テレメトリデータの保存先
const TELEMETRY_DIR = "user://telemetry"
const TELEMETRY_FILE = "telemetry.csv"

# メトリクスの定義
enum MetricType {
    PLAYER_DAMAGE,
    ENEMY_DAMAGE,
    ROOM_CLEAR_TIME,
    GAME_SESSION_TIME,
    ERROR_COUNT
}

# メトリクスデータの管理
var _metrics: Dictionary = {}
var _session_start_time: float
var _csv_file: FileAccess

func _init() -> void:
    name = "Telemetry"
    _initialize_metrics()

func _enter_tree() -> void:
    _session_start_time = Time.get_unix_time_from_system()
    _setup_file_logging()

func _exit_tree() -> void:
    if _csv_file:
        _csv_file.close()

# メトリクスの初期化
func _initialize_metrics() -> void:
    for metric in MetricType.values():
        _metrics[metric] = 0.0

# ファイルロギングの設定
func _setup_file_logging() -> void:
    var dir = DirAccess.open("user://")
    if not dir.dir_exists(TELEMETRY_DIR):
        dir.make_dir(TELEMETRY_DIR)

    var file_path = TELEMETRY_DIR.path_join(TELEMETRY_FILE)
    _csv_file = FileAccess.open(file_path, FileAccess.WRITE_READ)
    if _csv_file == null:
        push_error("Telemetry: ファイルのオープンに失敗しました: %s" % file_path)
        return

    # CSVヘッダーの書き込み
    _csv_file.store_line("timestamp,metric_type,value,context")

# EventBusのリスナー設定
func setup_event_listeners(event_bus: EventBus) -> void:
    event_bus.player_damaged.connect(_on_player_damaged)
    event_bus.enemy_damaged.connect(_on_enemy_damaged)
    event_bus.room_cleared.connect(_on_room_cleared)

# メトリクスの記録
func record_metric(metric_type: MetricType, value: float, context: Dictionary = {}) -> void:
    _metrics[metric_type] += value
    _write_to_csv(metric_type, value, context)

# エラーの記録
func log_error(error_message: String) -> void:
    record_metric(MetricType.ERROR_COUNT, 1.0, {"message": error_message})

# CSVへの書き込み
func _write_to_csv(metric_type: MetricType, value: float, context: Dictionary) -> void:
    if _csv_file:
        var timestamp = Time.get_unix_time_from_system()
        var context_str = JSON.stringify(context)
        var line = "%f,%d,%f,%s" % [timestamp, metric_type, value, context_str]
        _csv_file.store_line(line)

# イベントハンドラ
func _on_player_damaged(amount: float, source: Node) -> void:
    record_metric(MetricType.PLAYER_DAMAGE, amount, {
        "source": source.name if source else "unknown"
    })

func _on_enemy_damaged(enemy: Node, amount: float, source: Node) -> void:
    record_metric(MetricType.ENEMY_DAMAGE, amount, {
        "enemy": enemy.name if enemy else "unknown",
        "source": source.name if source else "unknown"
    })

func _on_room_cleared(room_id: String) -> void:
    var clear_time = Time.get_unix_time_from_system() - _session_start_time
    record_metric(MetricType.ROOM_CLEAR_TIME, clear_time, {
        "room_id": room_id
    })
