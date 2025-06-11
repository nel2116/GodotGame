---
title: Player Movement System
version: 0.1
status: draft
updated: 2024-03-21
tags:
    - Player
    - Movement
    - System
    - Gameplay
linked_docs:
    - "[[MovementSystem]]"
    - "[[PlayerSystem]]"
---

# Player Movement System

## 目次

1. [概要](#概要)
2. [移動タイプ](#移動タイプ)
3. [物理演算](#物理演算)
4. [使用方法](#使用方法)
5. [制限事項](#制限事項)
6. [変更履歴](#変更履歴)

## 概要

Player Movement System は、プレイヤーキャラクターの移動を制御するシステムです。このシステムは以下の機能を提供します：

-   基本的な移動制御
-   物理演算との連携
-   移動アニメーションの制御
-   移動状態の管理

## 移動タイプ

### 1. 地上移動

-   **通常移動**

    -   速度: 5.0 units/sec
    -   加速度: 10.0 units/sec²
    -   減速度: 15.0 units/sec²

-   **走り移動**
    -   速度: 8.0 units/sec
    -   加速度: 12.0 units/sec²
    -   減速度: 18.0 units/sec²

### 2. 空中移動

-   **ジャンプ**

    -   初速: 10.0 units/sec
    -   重力: 20.0 units/sec²
    -   最大高さ: 5.0 units

-   **二段ジャンプ**
    -   初速: 8.0 units/sec
    -   重力: 20.0 units/sec²
    -   最大高さ: 3.0 units

### 3. 特殊移動

-   **ダッシュ**

    -   速度: 15.0 units/sec
    -   持続時間: 0.5 sec
    -   クールダウン: 1.0 sec

-   **ウォールジャンプ**
    -   初速: 8.0 units/sec
    -   角度: 45 度
    -   最大回数: 1 回

## 物理演算

### 1. 衝突判定

-   プレイヤーのコリジョン形状
    -   幅: 1.0 units
    -   高さ: 2.0 units
    -   形状: カプセル

### 2. 物理レイヤー

-   プレイヤーレイヤー: 1
-   地面レイヤー: 2
-   敵レイヤー: 3
-   アイテムレイヤー: 4

### 3. 物理マテリアル

-   摩擦係数: 0.3
-   反発係数: 0.0
-   密度: 1.0

## 使用方法

### 1. 移動の制御

```gdscript
# 基本的な移動
player_movement_system.move(direction)

# 走り移動
player_movement_system.run(direction)

# ジャンプ
player_movement_system.jump()

# ダッシュ
player_movement_system.dash()
```

### 2. 移動状態の確認

```gdscript
# 移動速度の取得
var speed = player_movement_system.get_speed()

# 地面接地判定
var is_grounded = player_movement_system.is_grounded()

# 壁接触判定
var is_wall_sliding = player_movement_system.is_wall_sliding()
```

### 3. 移動イベントの購読

```gdscript
# 移動開始イベント
player_movement_system.movement_started.connect(_on_movement_started)

# 移動終了イベント
player_movement_system.movement_ended.connect(_on_movement_ended)

func _on_movement_started() -> void:
    print("Movement started")

func _on_movement_ended() -> void:
    print("Movement ended")
```

## 制限事項

1. 移動システムは 2D 空間での動作のみをサポート
2. 同時に複数の特殊移動を実行することはできない
3. 物理演算は Godot の組み込み物理エンジンに依存
4. 移動速度は状態に応じて自動的に調整される

## 変更履歴

| バージョン | 更新日     | 変更内容 |
| ---------- | ---------- | -------- |
| 0.1.0      | 2024-03-21 | 初版作成 |
