---
title: ゲームプレイAPI
version: 0.1.0
status: draft
updated: 2025-06-01
tags:
    - API
    - Gameplay
    - System
linked_docs:
    - "[[30_APIReference/00_index]]"
    - "[[30_APIReference/CoreSystemAPI]]"
    - "[[10_CoreDocs/00_index]]"
---

# ゲームプレイ API

## 目次

1. [概要](#概要)
2. [API 一覧](#api一覧)
3. [使用方法](#使用方法)
4. [制限事項](#制限事項)
5. [変更履歴](#変更履歴)

## 概要

ゲームプレイ API は、ゲームの核となる機能を提供する API 群です。
プレイヤー管理、スキルシステム、戦闘システムなどの機能を含みます。

## API 一覧

### プレイヤー管理

#### プレイヤー制御

```gdscript
# プレイヤーの移動
func move_player(direction: Vector2, speed: float) -> void

# プレイヤーの回転
func rotate_player(angle: float) -> void

# プレイヤーの状態取得
func get_player_state() -> Dictionary
```

#### プレイヤーステータス

```gdscript
# ステータスの更新
func update_player_stats(stats: Dictionary) -> void

# ステータスの取得
func get_player_stats() -> Dictionary

# 経験値の追加
func add_player_exp(amount: int) -> void
```

### スキルシステム

#### スキル管理

```gdscript
# スキルの習得
func learn_skill(skill_id: String) -> bool

# スキルの使用
func use_skill(skill_id: String, target: Node) -> void

# スキルのレベルアップ
func level_up_skill(skill_id: String) -> bool
```

#### スキル効果

```gdscript
# 効果の適用
func apply_skill_effect(target: Node, effect: Dictionary) -> void

# 効果の解除
func remove_skill_effect(target: Node, effect_id: String) -> void

# 効果の確認
func has_skill_effect(target: Node, effect_id: String) -> bool
```

### 戦闘システム

#### 戦闘管理

```gdscript
# 戦闘の開始
func start_combat(enemy: Node) -> void

# 戦闘の終了
func end_combat() -> void

# 戦闘状態の取得
func get_combat_state() -> Dictionary
```

#### ダメージ計算

```gdscript
# ダメージの計算
func calculate_damage(attacker: Node, defender: Node, skill: Dictionary) -> int

# 防御力の計算
func calculate_defense(defender: Node) -> int

# クリティカル判定
func check_critical(attacker: Node) -> bool
```

## 使用方法

### 基本的な使用例

```gdscript
# プレイヤーの移動
func _process(delta: float) -> void:
    var direction = Input.get_vector("ui_left", "ui_right", "ui_up", "ui_down")
    if direction != Vector2.ZERO:
        move_player(direction, 5.0)

# スキルの使用
func _on_skill_button_pressed(skill_id: String) -> void:
    var target = get_current_target()
    if target != null:
        use_skill(skill_id, target)
```

### 戦闘システムの使用例

```gdscript
# 戦闘の開始と管理
func start_battle(enemy: Node) -> void:
    start_combat(enemy)
    var combat_state = get_combat_state()

    # 戦闘状態に応じた処理
    if combat_state.is_active:
        setup_combat_ui()
        connect_combat_signals()
```

## 制限事項

-   プレイヤーの移動は物理演算に依存します
-   スキルの使用にはクールダウン時間があります
-   戦闘システムは特定のシーンでのみ使用可能です

## 変更履歴

| バージョン | 更新日     | 変更内容 |
| ---------- | ---------- | -------- |
| 0.1.0      | 2025-06-01 | 初版作成 |
