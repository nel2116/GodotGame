---
title: Player Combat System
version: 0.1
status: draft
updated: 2024-03-21
tags:
    - Player
    - Combat
    - System
    - Gameplay
linked_docs:
    - "[[PlayerSystem]]"
    - "[[PlayerStateSystem]]"
---

# Player Combat System

## 目次

1. [概要](#概要)
2. [戦闘システム](#戦闘システム)
3. [攻撃システム](#攻撃システム)
4. [防御システム](#防御システム)
5. [使用方法](#使用方法)
6. [制限事項](#制限事項)
7. [変更履歴](#変更履歴)

## 概要

Player Combat System は、プレイヤーの戦闘関連の機能を管理するシステムです。このシステムは以下の機能を提供します：

-   攻撃の実行と判定
-   防御の管理
-   ダメージの計算
-   戦闘状態の管理

## 戦闘システム

### 1. 基本パラメータ

-   **体力（HP）**

    -   最大値: 100
    -   回復速度: 1.0/秒
    -   回復条件: 非戦闘時

-   **スタミナ**

    -   最大値: 100
    -   回復速度: 5.0/秒
    -   回復条件: 非戦闘時

-   **攻撃力**
    -   基本値: 10
    -   成長率: 1.0/レベル
    -   上限: 100

### 2. 戦闘状態

-   **通常状態**

    -   移動可能
    -   攻撃可能
    -   防御可能

-   **戦闘状態**

    -   移動速度低下
    -   攻撃力上昇
    -   防御力上昇

-   **気絶状態**
    -   移動不可
    -   攻撃不可
    -   防御不可

## 攻撃システム

### 1. 通常攻撃

-   **連続攻撃**

    -   最大コンボ: 3
    -   コンボ間隔: 0.5 秒
    -   ダメージ倍率: 1.0, 1.2, 1.5

-   **強攻撃**
    -   チャージ時間: 1.0 秒
    -   ダメージ倍率: 2.0
    -   スタミナ消費: 20

### 2. 特殊攻撃

-   **スキル攻撃**

    -   クールダウン: 5.0 秒
    -   ダメージ倍率: 3.0
    -   スタミナ消費: 50

-   **必殺技**
    -   クールダウン: 30.0 秒
    -   ダメージ倍率: 5.0
    -   スタミナ消費: 100

### 3. 攻撃判定

-   **判定範囲**

    -   前方: 2.0 units
    -   左右: 1.0 units
    -   高さ: 2.0 units

-   **判定タイミング**
    -   開始: アニメーションフレーム 5
    -   終了: アニメーションフレーム 10
    -   持続時間: 0.2 秒

## 防御システム

### 1. 通常防御

-   **防御姿勢**

    -   ダメージ軽減: 50%
    -   スタミナ消費: 5/秒
    -   移動速度: 50%

-   **パリィ**
    -   判定時間: 0.1 秒
    -   成功時: 無敵時間 0.5 秒
    -   スタミナ消費: 20

### 2. 特殊防御

-   **カウンター**

    -   判定時間: 0.2 秒
    -   成功時: ダメージ 2 倍
    -   スタミナ消費: 30

-   **回避**
    -   無敵時間: 0.3 秒
    -   移動距離: 2.0 units
    -   スタミナ消費: 15

## 使用方法

### 1. 攻撃の実行

```gdscript
# 通常攻撃
player_combat_system.attack()

# 強攻撃
player_combat_system.strong_attack()

# スキル攻撃
player_combat_system.skill_attack()
```

### 2. 防御の実行

```gdscript
# 通常防御
player_combat_system.defend()

# パリィ
player_combat_system.parry()

# 回避
player_combat_system.dodge()
```

### 3. 戦闘イベントの購読

```gdscript
# ダメージイベント
player_combat_system.damage_taken.connect(_on_damage_taken)

# 攻撃ヒットイベント
player_combat_system.attack_hit.connect(_on_attack_hit)

func _on_damage_taken(amount: float) -> void:
    print("Damage taken: ", amount)

func _on_attack_hit(target: Node, damage: float) -> void:
    print("Hit target: ", target, " with damage: ", damage)
```

## 制限事項

1. 同時に実行可能な攻撃は 1 つまで
2. 防御中は攻撃不可
3. スタミナが 0 の場合は特殊技使用不可
4. 戦闘状態は自動的に切り替わる

## 変更履歴

| バージョン | 更新日     | 変更内容 |
| ---------- | ---------- | -------- |
| 0.1.0      | 2024-03-21 | 初版作成 |
