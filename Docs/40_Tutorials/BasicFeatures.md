---
title: 基本機能
version: 0.2.0
status: draft
updated: 2025-06-01
tags:
    - Tutorial
    - Basic
    - Features
linked_docs:
    - "[[40_Tutorials/00_index]]"
    - "[[40_Tutorials/GettingStarted]]"
    - "[[30_APIReference/GameplayAPI]]"
---

# 基本機能

## 目次

1. [概要](#概要)
2. [キャラクター操作](#キャラクター操作)
3. [スキル使用](#スキル使用)
4. [アイテム管理](#アイテム管理)
5. [戦闘システム](#戦闘システム)
6. [クエストシステム](#クエストシステム)
7. [制限事項](#制限事項)
8. [変更履歴](#変更履歴)

## 概要

このドキュメントでは、ゲームの基本機能について説明します。
キャラクター操作、スキル使用、アイテム管理などの基本的な機能を詳細に解説します。

## キャラクター操作

### 移動操作

1. 基本移動

    - W: 前進
    - S: 後退
    - A: 左移動
    - D: 右移動
    - Shift: ダッシュ
    - Space: ジャンプ

2. カメラ操作

    - マウス移動: 視点変更
    - マウスホイール: ズーム
    - 右クリック: カメラリセット

3. アクション操作
    - E: インタラクション
    - F: アイテム拾得
    - Q: スキルメニュー

### 戦闘操作

1. 基本アクション

    - 左クリック: 通常攻撃
    - 右クリック: 防御
    - 数字キー: スキル選択

2. ターゲット操作
    - Tab: ターゲット切り替え
    - マウスホバー: ターゲット表示
    - 中クリック: ターゲットロック

## スキル使用

### スキルの取得

1. レベルアップ

    ```gdscript
    # レベルアップ時のスキルポイント付与
    func on_level_up(new_level: int) -> void:
        var skill_points = calculate_skill_points(new_level)
        player.add_skill_points(skill_points)
    ```

2. クエスト報酬

    ```gdscript
    # クエスト完了時のスキル報酬
    func on_quest_complete(quest: Quest) -> void:
        if quest.has_skill_reward:
            player.learn_skill(quest.skill_reward)
    ```

3. 特殊イベント
    ```gdscript
    # 特殊イベントでのスキル獲得
    func on_special_event(event: SpecialEvent) -> void:
        if event.has_skill_reward:
            player.learn_skill(event.skill_reward)
    ```

### スキルの使用

1. クイックスロット

    ```gdscript
    # スキルのクイックスロット設定
    func set_quick_slot(slot: int, skill: Skill) -> void:
        if is_valid_slot(slot):
            player.quick_slots[slot] = skill
    ```

2. スキル選択

    ```gdscript
    # スキルの選択
    func select_skill(skill: Skill) -> void:
        if player.can_use_skill(skill):
            player.selected_skill = skill
    ```

3. スキル発動
    ```gdscript
    # スキルの発動
    func use_skill(skill: Skill, target: Node) -> void:
        if player.can_use_skill(skill):
            skill.execute(player, target)
            player.start_cooldown(skill)
    ```

### スキル効果

1. ダメージスキル

    ```gdscript
    # ダメージスキルの実装
    class_name DamageSkill
    extends Skill

    func execute(caster: Node, target: Node) -> void:
        var damage = calculate_damage(caster, target)
        target.take_damage(damage)
    ```

2. バフ/デバフ

    ```gdscript
    # バフスキルの実装
    class_name BuffSkill
    extends Skill

    func execute(caster: Node, target: Node) -> void:
        var buff = create_buff()
        target.add_buff(buff)
    ```

3. ユーティリティ

    ```gdscript
    # ユーティリティスキルの実装
    class_name UtilitySkill
    extends Skill

    func execute(caster: Node, target: Node) -> void:
        apply_utility_effect(caster, target)
    ```

## アイテム管理

### インベントリ

1. アイテムの整理

    ```gdscript
    # インベントリの整理
    func organize_inventory() -> void:
        # カテゴリ別にソート
        inventory.sort_by_category()

        # 自動スタック
        inventory.stack_items()

        # 重複アイテムの統合
        inventory.merge_duplicates()
    ```

2. アイテムの使用

    ```gdscript
    # アイテムの使用
    func use_item(item: Item) -> void:
        if item.is_consumable:
            item.consume(player)
        elif item.is_equipment:
            player.equip_item(item)
        elif item.is_material:
            item.add_to_materials()
    ```

3. アイテムの移動
    ```gdscript
    # アイテムの移動
    func move_item(item: Item, from_slot: int, to_slot: int) -> void:
        if is_valid_move(from_slot, to_slot):
            inventory.move_item(item, from_slot, to_slot)
    ```

### 装備管理

1. 装備の変更

    ```gdscript
    # 装備の変更
    func change_equipment(slot: String, item: Equipment) -> void:
        if player.can_equip(item):
            var old_item = player.unequip(slot)
            player.equip(slot, item)
            inventory.add_item(old_item)
    ```

2. 装備スロット

    ```gdscript
    # 装備スロットの確認
    func check_equipment_slots() -> Dictionary:
        return {
            "weapon": player.equipment_slots.weapon,
            "armor": player.equipment_slots.armor,
            "accessory": player.equipment_slots.accessory
        }
    ```

3. 装備強化
    ```gdscript
    # 装備の強化
    func enhance_equipment(item: Equipment) -> void:
        if player.has_enough_materials():
            var success = item.enhance()
            if success:
                player.consume_materials()
    ```

### 装備効果

1. ステータス効果

    ```gdscript
    # 装備のステータス効果
    func calculate_equipment_stats() -> Dictionary:
        var stats = {}
        for item in player.equipment_slots.values():
            stats.merge(item.stats)
        return stats
    ```

2. セット効果

    ```gdscript
    # セット効果の確認
    func check_set_effects() -> Array:
        var set_effects = []
        for set_name in get_equipped_sets():
            set_effects.append(get_set_effect(set_name))
        return set_effects
    ```

3. 特殊効果
    ```gdscript
    # 特殊効果の適用
    func apply_special_effects() -> void:
        for item in player.equipment_slots.values():
            if item.has_special_effect:
                item.special_effect.apply(player)
    ```

## 戦闘システム

### 基本戦闘

1. 攻撃

    ```gdscript
    # 攻撃の実装
    func attack(attacker: Node, target: Node) -> void:
        var damage = calculate_damage(attacker, target)
        target.take_damage(damage)
    ```

2. 防御

    ```gdscript
    # 防御の実装
    func defend(defender: Node) -> void:
        defender.start_defense()
        defender.apply_defense_buff()
    ```

3. 回避
    ```gdscript
    # 回避の実装
    func dodge(character: Node) -> void:
        if character.can_dodge():
            character.start_dodge()
            character.apply_dodge_buff()
    ```

### 戦闘状態

1. 状態管理

    ```gdscript
    # 戦闘状態の管理
    func update_combat_state() -> void:
        for character in combatants:
            character.update_status()
            character.update_effects()
            character.check_conditions()
    ```

2. 効果管理

    ```gdscript
    # 効果の管理
    func manage_effects() -> void:
        for character in combatants:
            character.update_buffs()
            character.update_debuffs()
            character.update_dots()
    ```

3. 条件判定
    ```gdscript
    # 条件の判定
    func check_conditions() -> void:
        for character in combatants:
            if character.is_dead:
                handle_death(character)
            if character.is_stunned:
                handle_stun(character)
    ```

## クエストシステム

### クエスト管理

1. クエストの受注

    ```gdscript
    # クエストの受注
    func accept_quest(quest: Quest) -> void:
        if player.can_accept_quest(quest):
            player.active_quests.append(quest)
            quest.start()
    ```

2. クエストの進行

    ```gdscript
    # クエストの進行
    func update_quest_progress(quest: Quest) -> void:
        quest.update_objectives()
        if quest.is_completed:
            complete_quest(quest)
    ```

3. クエストの完了
    ```gdscript
    # クエストの完了
    func complete_quest(quest: Quest) -> void:
        player.receive_rewards(quest.rewards)
        player.active_quests.erase(quest)
        player.completed_quests.append(quest)
    ```

### クエスト報酬

1. 経験値

    ```gdscript
    # 経験値の付与
    func grant_experience(amount: int) -> void:
        player.add_experience(amount)
        check_level_up()
    ```

2. アイテム

    ```gdscript
    # アイテムの付与
    func grant_items(items: Array) -> void:
        for item in items:
            player.inventory.add_item(item)
    ```

3. スキル
    ```gdscript
    # スキルの付与
    func grant_skill(skill: Skill) -> void:
        player.learn_skill(skill)
    ```

## 制限事項

-   スキル使用にはクールダウン時間があります
-   アイテム所持数には上限があります
-   装備の変更は特定の場所でのみ可能です
-   クエストは同時に 5 つまで受注可能です
-   戦闘中は一部のアクションが制限されます

## 変更履歴

| バージョン | 更新日     | 変更内容                                   |
| ---------- | ---------- | ------------------------------------------ |
| 0.2.0      | 2025-06-01 | 戦闘システムとクエストシステムの詳細を追加 |
| 0.1.0      | 2025-06-01 | 初版作成                                   |
