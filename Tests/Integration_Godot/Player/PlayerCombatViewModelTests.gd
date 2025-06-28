extends GutTest

# PlayerCombatViewModelTests.gd
# GUT版のプレイヤー戦闘ViewModelテスト

var event_bus: GameEventBus
var combat_model: PlayerCombatModel
var combat_view_model: PlayerCombatViewModel

func before_each():
	event_bus = GameEventBus.new()
	combat_model = PlayerCombatModel.new(event_bus)
	combat_view_model = PlayerCombatViewModel.new(combat_model, event_bus)

func after_each():
	combat_view_model.free()
	combat_model.free()
	event_bus.free()

func test_initialize_default_health_max():
	# 準備・実行
	combat_view_model.initialize()
	
	# 検証
	assert_eq(combat_view_model.health.value, 100)
	assert_eq(combat_view_model.max_health.value, 100)

func test_attack_publishes_attack_event():
	# 準備
	var received_event = null
	event_bus.get_event_stream("CombatAttackEvent").subscribe(
		func(event): received_event = event
	)
	combat_view_model.initialize()
	
	# 実行
	combat_view_model.attack("BasicAttack")
	
	# 少し待機してイベント処理を完了させる
	await get_tree().process_frame
	
	# 検証
	assert_not_null(received_event)
	assert_eq(received_event.action_name, "BasicAttack")

func test_attack_invalid_action_publishes_error():
	# 準備
	var error_received = false
	event_bus.get_event_stream("ErrorEvent").subscribe(
		func(event): error_received = true
	)
	combat_view_model.initialize()
	
	# 実行
	combat_view_model.attack("InvalidAction")
	
	# 少し待機してイベント処理を完了させる
	await get_tree().process_frame
	
	# 検証
	assert_true(error_received, "Error event should be published for invalid action")

func test_take_damage_reduces_health_and_publishes():
	# 準備
	var received_event = null
	event_bus.get_event_stream("CombatDamageEvent").subscribe(
		func(event): received_event = event
	)
	combat_view_model.initialize()
	var initial_health = combat_view_model.health.value
	
	# 実行
	combat_view_model.take_damage(20)
	
	# 少し待機してイベント処理を完了させる
	await get_tree().process_frame
	
	# 検証
	assert_not_null(received_event)
	assert_eq(received_event.damage, 20)
	assert_eq(combat_view_model.health.value, initial_health - 20)

func test_heal_restores_health_and_publishes():
	# 準備
	var received_event = null
	event_bus.get_event_stream("CombatHealEvent").subscribe(
		func(event): received_event = event
	)
	combat_view_model.initialize()
	combat_view_model.take_damage(30) # ダメージを与える
	var health_before_heal = combat_view_model.health.value
	
	# 実行
	combat_view_model.heal(15)
	
	# 少し待機してイベント処理を完了させる
	await get_tree().process_frame
	
	# 検証
	assert_not_null(received_event)
	assert_eq(received_event.heal_amount, 15)
	assert_eq(combat_view_model.health.value, health_before_heal + 15)

func test_health_cannot_exceed_max_health():
	# 準備
	combat_view_model.initialize()
	
	# 実行
	combat_view_model.heal(50) # 最大HPを超える回復
	
	# 検証
	assert_eq(combat_view_model.health.value, combat_view_model.max_health.value)

func test_health_cannot_go_below_zero():
	# 準備
	combat_view_model.initialize()
	
	# 実行
	combat_view_model.take_damage(200) # 最大HPを超えるダメージ
	
	# 検証
	assert_eq(combat_view_model.health.value, 0)

func test_update_combat_state():
	# 準備
	combat_view_model.initialize()
	
	# 実行
	combat_view_model.update_combat()
	
	# 検証（エラーが発生しないことを確認）
	# GUTでは例外が発生した場合自動的にテストが失敗する

func test_combat_state_changes():
	# 準備
	combat_view_model.initialize()
	
	# 実行
	combat_view_model.set_combat_state("Attacking")
	
	# 検証
	assert_eq(combat_view_model.combat_state.value, "Attacking") 