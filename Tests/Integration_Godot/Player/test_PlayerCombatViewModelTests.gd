extends GutTest

# PlayerCombatViewModelTests.gd
# GUT版のプレイヤー戦闘ViewModelテスト
# ※C#クラスを直接参照しているため、GDScriptからは実行不可。
#   テストを有効化するにはC#ノードのGDScriptラッパーまたはテスト用ダミーが必要です。

# var event_bus: NewGameProject.GameEventBus
# var combat_model: NewGameProject.PlayerCombatModel
# var combat_view_model: NewGameProject.PlayerCombatViewModel

var combat_node

func before_each():
	combat_node = preload("res://Scripts/Systems/Player/Combat/PlayerCombatViewModelNode.cs").new()
	add_child(combat_node)
	combat_node.Initialize()

func after_each():
	remove_child(combat_node)
	combat_node.queue_free()
	await get_tree().process_frame

func test_initialize_default_health_max():
	assert_eq(combat_node.CurrentHealth, 100.0)
	assert_eq(combat_node.MaxHealth, 100.0)

# func test_attack_publishes_attack_event():
# 	var received_event = null
# 	event_bus.get_event_stream("CombatAttackEvent").subscribe(
# 		func(event): received_event = event
# 	)
# 	combat_view_model.initialize()
# 	combat_view_model.attack("BasicAttack")
# 	await get_tree().process_frame
# 	assert_not_null(received_event)
# 	assert_eq(received_event.action_name, "BasicAttack")

# func test_attack_invalid_action_publishes_error():
# 	var error_received = false
# 	event_bus.get_event_stream("ErrorEvent").subscribe(
# 		func(event): error_received = true
# 	)
# 	combat_view_model.initialize()
# 	combat_view_model.attack("InvalidAction")
# 	await get_tree().process_frame
# 	assert_true(error_received, "Error event should be published for invalid action")

# func test_take_damage_reduces_health_and_publishes():
# 	var received_event = null
# 	event_bus.get_event_stream("CombatDamageEvent").subscribe(
# 		func(event): received_event = event
# 	)
# 	combat_view_model.initialize()
# 	var initial_health = combat_view_model.health.value
# 	combat_view_model.take_damage(20)
# 	await get_tree().process_frame
# 	assert_not_null(received_event)
# 	assert_eq(received_event.damage, 20)
# 	assert_eq(combat_view_model.health.value, initial_health - 20)

# func test_heal_restores_health_and_publishes():
# 	var received_event = null
# 	event_bus.get_event_stream("CombatHealEvent").subscribe(
# 		func(event): received_event = event
# 	)
# 	combat_view_model.initialize()
# 	combat_view_model.take_damage(30)
# 	var health_before_heal = combat_view_model.health.value
# 	combat_view_model.heal(15)
# 	await get_tree().process_frame
# 	assert_not_null(received_event)
# 	assert_eq(received_event.heal_amount, 15)
# 	assert_eq(combat_view_model.health.value, health_before_heal + 15)

# func test_health_cannot_exceed_max_health():
# 	combat_view_model.initialize()
# 	combat_view_model.heal(50)
# 	assert_eq(combat_view_model.health.value, combat_view_model.max_health.value)

# func test_health_cannot_go_below_zero():
# 	combat_view_model.initialize()
# 	combat_view_model.take_damage(200)
# 	assert_eq(combat_view_model.health.value, 0)

# func test_update_combat_state():
# 	combat_view_model.initialize()
# 	combat_view_model.update_combat()

# func test_combat_state_changes():
# 	combat_view_model.initialize()
# 	combat_view_model.set_combat_state("Attacking")
# 	assert_eq(combat_view_model.combat_state.value, "Attacking") 