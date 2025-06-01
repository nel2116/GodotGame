extends GutTest

var event_types: EventTypes

func before_each() -> void:
	event_types = EventTypes.new()

func after_each() -> void:
	event_types = null

# イベントタイプの検証テスト
func test_validate_event_type() -> void:
	# 有効なイベントタイプと引数のテスト
	assert_true(EventTypes.validate_event_type("game_started", []))
	
	# Nodeインスタンスを作成してテスト
	var test_node = Node.new()
	add_child(test_node)
	assert_true(EventTypes.validate_event_type("player_damaged", [10.0, test_node]))
	assert_true(EventTypes.validate_event_type("game_ended", [100, "victory"]))
	
	# 無効なイベントタイプのテスト
	assert_false(EventTypes.validate_event_type("invalid_event", []))
	
	# 引数の型が間違っている場合のテスト
	assert_false(EventTypes.validate_event_type("player_damaged", ["invalid", test_node]))
	assert_false(EventTypes.validate_event_type("game_ended", ["invalid", 123]))
	
	# 引数の数が間違っている場合のテスト
	assert_false(EventTypes.validate_event_type("player_damaged", [10.0]))
	assert_false(EventTypes.validate_event_type("game_ended", [100]))
	
	# テスト用のNodeを解放
	test_node.queue_free()
	await test_node.tree_exited

# イベントタイプの取得テスト
func test_get_event_type() -> void:
	# 有効なイベントタイプの取得テスト
	var game_started = EventTypes.get_event_type("game_started")
	assert_not_null(game_started)
	assert_eq(game_started.name, "game_started")
	assert_eq(game_started.category, EventTypes.Category.GAME_STATE)
	
	# 無効なイベントタイプの取得テスト
	assert_null(EventTypes.get_event_type("invalid_event"))

# イベントタイプの一覧取得テスト
func test_get_all_event_types() -> void:
	var types = EventTypes.get_all_event_types()
	assert_true(types.size() > 0)
	
	# 各イベントタイプの基本構造を確認
	for type in types:
		assert_true(type is EventTypes.EventType)
		assert_true(type.name is String)
		assert_true(type.category is int)
		assert_true(type.parameters is Dictionary)
		assert_true(type.description is String)

# イベントカテゴリのテスト
func test_event_categories() -> void:
	var types = EventTypes.get_all_event_types()
	var categories = {}
	
	# 各カテゴリのイベント数をカウント
	for type in types:
		if not categories.has(type.category):
			categories[type.category] = 0
		categories[type.category] += 1
	
	# 各カテゴリに少なくとも1つのイベントがあることを確認
	assert_true(categories.has(EventTypes.Category.GAME_STATE))
	assert_true(categories.has(EventTypes.Category.PLAYER))
	assert_true(categories.has(EventTypes.Category.ENEMY))
	assert_true(categories.has(EventTypes.Category.ROOM))

# イベントパラメータのテスト
func test_event_parameters() -> void:
	# パラメータなしのイベント
	var game_started = EventTypes.get_event_type("game_started")
	assert_eq(game_started.parameters.size(), 0)
	
	# パラメータありのイベント
	var player_damaged = EventTypes.get_event_type("player_damaged")
	assert_eq(player_damaged.parameters.size(), 2)
	assert_eq(player_damaged.parameters["amount"], TYPE_FLOAT)
	assert_eq(player_damaged.parameters["source"], TYPE_OBJECT)
	
	# 複数のパラメータを持つイベント
	var enemy_damaged = EventTypes.get_event_type("enemy_damaged")
	assert_eq(enemy_damaged.parameters.size(), 3)
	assert_eq(enemy_damaged.parameters["enemy"], TYPE_OBJECT)
	assert_eq(enemy_damaged.parameters["amount"], TYPE_FLOAT)
	assert_eq(enemy_damaged.parameters["source"], TYPE_OBJECT)
	
	# 参照を解放
	game_started = null
	player_damaged = null
	enemy_damaged = null 
