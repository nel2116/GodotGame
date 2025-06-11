---
title: Player State System
version: 0.1
status: draft
updated: 2024-03-21
tags:
    - Player
    - State
    - System
    - Gameplay
linked_docs:
    - "[[StateSystem]]"
    - "[[PlayerSystem]]"
---

# Player State System

## 目次

1. [概要](#概要)
2. [状態一覧](#状態一覧)
3. [状態遷移](#状態遷移)
4. [使用方法](#使用方法)
5. [制限事項](#制限事項)
6. [変更履歴](#変更履歴)

## 概要

Player State System は、プレイヤーキャラクターの状態を管理し、状態に応じた動作を制御するシステムです。このシステムは以下の機能を提供します：

-   プレイヤーの状態管理
-   状態遷移の制御
-   状態に応じた動作の制御
-   状態変更イベントの発行

## 状態一覧

### 基本状態

1. **Idle（待機）**

    - プレイヤーが何もしていない状態
    - 入力待ち状態
    - デフォルトの状態

2. **Moving（移動）**

    - プレイヤーが移動している状態
    - 方向キー入力中
    - 移動アニメーション再生中

3. **Jumping（ジャンプ）**

    - プレイヤーがジャンプしている状態
    - ジャンプアニメーション再生中
    - 着地判定待ち

4. **Falling（落下）**
    - プレイヤーが落下している状態
    - 落下アニメーション再生中
    - 着地判定待ち

### 戦闘状態

1. **Attacking（攻撃）**

    - プレイヤーが攻撃している状態
    - 攻撃アニメーション再生中
    - 攻撃判定発生中

2. **Defending（防御）**

    - プレイヤーが防御している状態
    - 防御アニメーション再生中
    - ダメージ軽減効果発生中

3. **Stunned（気絶）**
    - プレイヤーが気絶している状態
    - 気絶アニメーション再生中
    - 入力無効化中

## 状態遷移

### 遷移ルール

1. **Idle → Moving**

    - 条件: 方向キー入力
    - 優先度: 中

2. **Moving → Idle**

    - 条件: 方向キー入力解除
    - 優先度: 中

3. **Idle/Moving → Jumping**

    - 条件: ジャンプキー入力
    - 優先度: 高

4. **Jumping → Falling**

    - 条件: 上昇速度が 0 以下
    - 優先度: 中

5. **Falling → Idle**
    - 条件: 着地判定
    - 優先度: 高

### 遷移制限

1. 気絶中は他の状態に遷移できない
2. 攻撃中は移動状態に遷移できない
3. 防御中はジャンプ状態に遷移できない

## 使用方法

### 1. 状態の変更

```gdscript
# 状態の変更
player_state_system.change_state("idle")
```

### 2. 状態の確認

```gdscript
# 現在の状態を取得
var current_state = player_state_system.get_current_state()

# 特定の状態かどうかを確認
var is_moving = player_state_system.is_state("moving")
```

### 3. 状態変更イベントの購読

```gdscript
# 状態変更イベントの購読
player_state_system.state_changed.connect(_on_state_changed)

func _on_state_changed(new_state: String) -> void:
    print("State changed to: ", new_state)
```

## 制限事項

1. 同時に複数の状態を持つことはできません
2. 状態遷移は優先度に基づいて決定されます
3. 一部の状態は特定の条件下でのみ遷移可能です
4. 状態変更イベントは非同期で発行されます

## 変更履歴

| バージョン | 更新日     | 変更内容 |
| ---------- | ---------- | -------- |
| 0.1.0      | 2024-03-21 | 初版作成 |
