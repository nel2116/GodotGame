class_name EventTypes
extends RefCounted

# イベントカテゴリの定義
enum Category {
	GAME_STATE,    # ゲーム状態関連
	PLAYER,        # プレイヤー関連
	ENEMY,         # 敵関連
	ROOM,          # 部屋関連
	UI,            # UI関連
	SYSTEM,        # システム関連
	TEST           # テスト用
}

# イベントタイプの定義
class EventType:
	var name: String
	var category: Category
	var parameters: Dictionary  # パラメータ名と型のマッピング
	var description: String
	
	func _init(p_name: String, p_category: Category, p_parameters: Dictionary, p_description: String) -> void:
		name = p_name
		category = p_category
		parameters = p_parameters
		description = p_description

# イベントタイプのインスタンスを保持する辞書
static var _event_types: Dictionary = {}

# 初期化済みフラグ
static var _initialized: bool = false

# イベントタイプの初期化処理本体
static func _do_init() -> void:
	# テスト用イベント
	_event_types["test_event"] = EventType.new(
		"test_event",
		Category.TEST,
		{},
		"テスト用イベント"
	)

	_event_types["test_event_async"] = EventType.new(
		"test_event_async",
		Category.TEST,
		{},
		"非同期テスト用イベント"
	)

	# ゲーム状態関連イベント
	_event_types["game_started"] = EventType.new(
		"game_started",
		Category.GAME_STATE,
		{},
		"ゲームが開始された時に発火"
	)

	_event_types["game_paused"] = EventType.new(
		"game_paused",
		Category.GAME_STATE,
		{},
		"ゲームが一時停止された時に発火"
	)

	_event_types["game_resumed"] = EventType.new(
		"game_resumed",
		Category.GAME_STATE,
		{},
		"ゲームが再開された時に発火"
	)

	_event_types["game_ended"] = EventType.new(
		"game_ended",
		Category.GAME_STATE,
		{
			"score": TYPE_INT,
			"reason": TYPE_STRING
		},
		"ゲームが終了した時に発火"
	)

	# プレイヤー関連イベント
	_event_types["player_damaged"] = EventType.new(
		"player_damaged",
		Category.PLAYER,
		{
			"amount": TYPE_FLOAT,
			"source": TYPE_OBJECT
		},
		"プレイヤーがダメージを受けた時に発火"
	)

	_event_types["player_healed"] = EventType.new(
		"player_healed",
		Category.PLAYER,
		{
			"amount": TYPE_FLOAT,
			"source": TYPE_OBJECT
		},
		"プレイヤーが回復した時に発火"
	)

	_event_types["player_died"] = EventType.new(
		"player_died",
		Category.PLAYER,
		{
			"killer": TYPE_OBJECT
		},
		"プレイヤーが死亡した時に発火"
	)

	# 敵関連イベント
	_event_types["enemy_damaged"] = EventType.new(
		"enemy_damaged",
		Category.ENEMY,
		{
			"enemy": TYPE_OBJECT,
			"amount": TYPE_FLOAT,
			"source": TYPE_OBJECT
		},
		"敵がダメージを受けた時に発火"
	)

	_event_types["enemy_died"] = EventType.new(
		"enemy_died",
		Category.ENEMY,
		{
			"enemy": TYPE_OBJECT,
			"killer": TYPE_OBJECT
		},
		"敵が死亡した時に発火"
	)

	# 部屋関連イベント
	_event_types["room_entered"] = EventType.new(
		"room_entered",
		Category.ROOM,
		{
			"room_id": TYPE_STRING,
			"player": TYPE_OBJECT
		},
		"プレイヤーが部屋に入った時に発火"
	)

	_event_types["room_cleared"] = EventType.new(
		"room_cleared",
		Category.ROOM,
		{
			"room_id": TYPE_STRING,
			"clear_time": TYPE_FLOAT
		},
		"部屋がクリアされた時に発火"
	)

# イベントタイプの初期化
static func _init() -> void:
	# 初期化を実行
	_do_init()
	_initialized = true

# イベントタイプの検証
static func validate_event_type(event_name: String, args: Array) -> bool:
	# イベントタイプの取得（キャッシュされた値を返す）
	var event_type = get_event_type(event_name)
	if event_type == null:
		return false
	
	# 引数の数が一致するか確認（高速チェック）
	if event_type.parameters.size() != args.size():
		return false
	
	# パラメータの型を検証（最適化版）
	var param_names = event_type.parameters.keys()
	for i in range(args.size()):
		var expected_type = event_type.parameters[param_names[i]]
		var actual_value = args[i]
		
		# TYPE_OBJECTの場合は、nullまたはObject型のインスタンスであることを確認
		if expected_type == TYPE_OBJECT:
			if actual_value != null and not (actual_value is Object):
				return false
		# その他の型は typeof() で直接比較
		elif typeof(actual_value) != expected_type:
			return false
	
	return true

# イベントタイプの取得
static func get_event_type(event_name: String) -> EventType:
	# 初期化が行われていない場合は初期化を実行
	if not _initialized:
		_do_init()
		_initialized = true
	return _event_types.get(event_name)

# イベントタイプの一覧を取得
static func get_all_event_types() -> Array:
	# 初期化が行われていない場合は初期化を実行
	if not _initialized:
		_do_init()
		_initialized = true
	return _event_types.values()

# デバッグ用：登録されているイベントタイプを出力
static func debug_event_types() -> void:
	print("--- Debug: Registered Event Types ---")
	if _event_types.is_empty():
		print("No event types registered.")
	else:
		for name in _event_types.keys():
			var et = _event_types[name]
			var param_info = []
			for param_name in et.parameters.keys():
				param_info.append("%s: %s" % [param_name, typeof(et.parameters[param_name])])
			print("- ", name, " (Category: ", Category.keys()[et.category] if et.category < Category.keys().size() else "Unknown", "): ", et.description, " [Params: ", ", ".join(param_info), "]")
	print("------------------------------------") 