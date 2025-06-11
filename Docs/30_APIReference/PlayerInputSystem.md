---
title: Player Input System
version: 0.1
status: draft
updated: 2024-03-21
tags:
    - Player
    - Input
    - System
    - Gameplay
linked_docs:
    - "[[PlayerSystem]]"
    - "[[CoreEventSystem]]"
---

# Player Input System

## 目次

1. [概要](#概要)
2. [入力マッピング](#入力マッピング)
3. [入力処理](#入力処理)
4. [使用方法](#使用方法)
5. [制限事項](#制限事項)
6. [変更履歴](#変更履歴)

## 概要

Player Input System は、プレイヤーの入力処理を管理するシステムです。このシステムは以下の機能を提供します：

-   キーボードとゲームパッドの入力処理
-   入力マッピングの管理
-   入力イベントの発行
-   入力状態の管理

## 入力マッピング

### 1. 基本操作

-   **移動**

    -   キーボード: WASD / 矢印キー
    -   ゲームパッド: 左スティック
    -   デッドゾーン: 0.2

-   **ジャンプ**

    -   キーボード: スペース
    -   ゲームパッド: A ボタン
    -   長押し判定: 0.1 秒

-   **ダッシュ**
    -   キーボード: Shift
    -   ゲームパッド: B ボタン
    -   長押し判定: 0.2 秒

### 2. 戦闘操作

-   **通常攻撃**

    -   キーボード: J
    -   ゲームパッド: X ボタン
    -   連続入力: 0.3 秒

-   **強攻撃**

    -   キーボード: K
    -   ゲームパッド: Y ボタン
    -   チャージ時間: 1.0 秒

-   **防御**
    -   キーボード: L
    -   ゲームパッド: LB ボタン
    -   長押し判定: 0.1 秒

### 3. 特殊操作

-   **スキル 1**

    -   キーボード: Q
    -   ゲームパッド: RB ボタン
    -   クールダウン: 5.0 秒

-   **スキル 2**
    -   キーボード: E
    -   ゲームパッド: RT ボタン
    -   クールダウン: 8.0 秒

## 入力処理

### 1. 入力検出

-   **即時入力**

    -   キー押下: フレーム単位
    -   キー解放: フレーム単位
    -   キー長押し: 時間ベース

-   **組み合わせ入力**
    -   同時押し: 2 キーまで
    -   順次押し: 0.5 秒以内
    -   キャンセル: 0.2 秒以内

### 2. 入力バッファ

-   **バッファサイズ**: 4 フレーム
-   **有効時間**: 0.2 秒
-   **優先順位**: 最新の入力

### 3. 入力フィルタリング

-   **デッドゾーン**

    -   アナログスティック: 0.2
    -   トリガー: 0.1
    -   方向キー: 0.0

-   **入力スムージング**
    -   移動: 0.8
    -   回転: 0.6
    -   カメラ: 0.4

## 使用方法

### 1. 入力の取得

```gdscript
# 移動入力の取得
var movement_input = player_input_system.get_movement_input()

# ジャンプ入力の取得
var jump_pressed = player_input_system.is_jump_pressed()

# 攻撃入力の取得
var attack_pressed = player_input_system.is_attack_pressed()
```

### 2. 入力イベントの購読

```gdscript
# 入力イベントの購読
player_input_system.input_received.connect(_on_input_received)

func _on_input_received(input_type: String, value: float) -> void:
    print("Input received: ", input_type, " with value: ", value)
```

### 3. 入力マッピングの変更

```gdscript
# 入力マッピングの変更
player_input_system.remap_input("jump", "space", "gamepad_a")

# 入力マッピングの保存
player_input_system.save_input_mapping()
```

## 制限事項

1. 同時に入力可能なキーの数は 4 つまで
2. 入力バッファは 4 フレームまで保持
3. 入力マッピングの変更は実行時のみ可能
4. ゲームパッドの接続/切断は自動検出

## 変更履歴

| バージョン | 更新日     | 変更内容 |
| ---------- | ---------- | -------- |
| 0.1.0      | 2024-03-21 | 初版作成 |
