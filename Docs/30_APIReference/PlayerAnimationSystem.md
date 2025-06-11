---
title: Player Animation System
version: 0.1
status: draft
updated: 2024-03-21
tags:
    - Player
    - Animation
    - System
    - Gameplay
linked_docs:
    - "[[PlayerSystem]]"
    - "[[PlayerStateSystem]]"
---

# Player Animation System

## 目次

1. [概要](#概要)
2. [アニメーションタイプ](#アニメーションタイプ)
3. [アニメーション遷移](#アニメーション遷移)
4. [使用方法](#使用方法)
5. [制限事項](#制限事項)
6. [変更履歴](#変更履歴)

## 概要

Player Animation System は、プレイヤーキャラクターのアニメーションを管理するシステムです。このシステムは以下の機能を提供します：

-   アニメーションの再生制御
-   アニメーション遷移の管理
-   アニメーションイベントの処理
-   アニメーション状態の管理

## アニメーションタイプ

### 1. 基本アニメーション

-   **Idle（待機）**

    -   フレーム数: 8
    -   再生速度: 1.0
    -   ループ: 有効
    -   優先度: 低

-   **Walk（歩行）**

    -   フレーム数: 12
    -   再生速度: 1.2
    -   ループ: 有効
    -   優先度: 中

-   **Run（走行）**
    -   フレーム数: 12
    -   再生速度: 1.5
    -   ループ: 有効
    -   優先度: 中

### 2. 戦闘アニメーション

-   **Attack（攻撃）**

    -   フレーム数: 16
    -   再生速度: 1.0
    -   ループ: 無効
    -   優先度: 高

-   **Defend（防御）**

    -   フレーム数: 8
    -   再生速度: 1.0
    -   ループ: 有効
    -   優先度: 高

-   **Hit（被弾）**
    -   フレーム数: 6
    -   再生速度: 1.0
    -   ループ: 無効
    -   優先度: 最高

### 3. 特殊アニメーション

-   **Jump（ジャンプ）**

    -   フレーム数: 8
    -   再生速度: 1.0
    -   ループ: 無効
    -   優先度: 高

-   **Dash（ダッシュ）**

    -   フレーム数: 6
    -   再生速度: 1.5
    -   ループ: 無効
    -   優先度: 高

-   **Death（死亡）**
    -   フレーム数: 12
    -   再生速度: 0.8
    -   ループ: 無効
    -   優先度: 最高

## アニメーション遷移

### 1. 遷移ルール

-   **即時遷移**

    -   条件: 優先度が高い
    -   ブレンド時間: 0.0 秒
    -   適用: 被弾、死亡

-   **通常遷移**

    -   条件: 優先度が同じ
    -   ブレンド時間: 0.1 秒
    -   適用: 移動、攻撃

-   **スムーズ遷移**
    -   条件: 優先度が低い
    -   ブレンド時間: 0.2 秒
    -   適用: 待機、歩行

### 2. 遷移制限

-   **遷移不可**

    -   死亡中は他のアニメーションに遷移不可
    -   被弾中は移動アニメーションに遷移不可
    -   攻撃中は防御アニメーションに遷移不可

-   **遷移条件**
    -   移動速度が 0 以上で歩行アニメーション
    -   移動速度が 5 以上で走行アニメーション
    -   ジャンプ中は落下アニメーション

## 使用方法

### 1. アニメーションの再生

```gdscript
# アニメーションの再生
player_animation_system.play_animation("idle")

# アニメーションの停止
player_animation_system.stop_animation()

# アニメーションの一時停止
player_animation_system.pause_animation()
```

### 2. アニメーション状態の確認

```gdscript
# 現在のアニメーションを取得
var current_animation = player_animation_system.get_current_animation()

# アニメーションの再生状態を確認
var is_playing = player_animation_system.is_playing()

# アニメーションのループ状態を確認
var is_looping = player_animation_system.is_looping()
```

### 3. アニメーションイベントの購読

```gdscript
# アニメーション開始イベント
player_animation_system.animation_started.connect(_on_animation_started)

# アニメーション終了イベント
player_animation_system.animation_finished.connect(_on_animation_finished)

func _on_animation_started(animation_name: String) -> void:
    print("Animation started: ", animation_name)

func _on_animation_finished(animation_name: String) -> void:
    print("Animation finished: ", animation_name)
```

## 制限事項

1. 同時に再生可能なアニメーションは 1 つまで
2. アニメーションの優先度は固定
3. アニメーションの遷移は自動的に制御
4. アニメーションイベントは非同期で発行

## 変更履歴

| バージョン | 更新日     | 変更内容 |
| ---------- | ---------- | -------- |
| 0.1.0      | 2024-03-21 | 初版作成 |
