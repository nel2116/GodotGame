---
title: 共通ユーティリティ概要
version: 0.2.1
status: draft
updated: 2025-06-13
tags:
    - Core
    - Utility
    - Overview
linked_docs:
    - "[[12_01_mvvm_rx_architecture|MVVM+RXアーキテクチャ]]"
    - "[[12_02_basic_design|MVVM+RX基本設計書]]"
    - "[[12_03_detailed_design/01_core_components/05_common_utilities|共通ユーティリティ実装詳細]]"
    - "[[12_04_system_integration|システム間連携]]"
---

# 共通ユーティリティ

## 目次

1. [概要](#1-概要)
2. [基本ユーティリティ](#2-基本ユーティリティ)
3. [拡張ユーティリティ](#3-拡張ユーティリティ)
4. [テストユーティリティ](#4-テストユーティリティ)
5. [パフォーマンス最適化](#5-パフォーマンス最適化)
6. [使用例](#6-使用例)
7. [制限事項](#7-制限事項)
8. [変更履歴](#8-変更履歴)

## 1. 概要

### 1.1 目的

本ドキュメントは、Shrine of the Lost Ones で使用される共通ユーティリティの設計と実装を定義します。
これらのユーティリティは、[コアコンポーネント](../01_core_components/05_common_utilities.md)を拡張し、
アプリケーション全体で再利用可能な機能を提供します。

### 1.2 適用範囲

-   ゲームロジック
-   UI/UX システム
-   データ管理
-   テスト環境

## 2. 基本ユーティリティ

### 2.1 リアクティブユーティリティ

```gdscript
class_name ReactiveUtils
extends Node

static func create_observable(initial_value) -> ReactiveProperty:
    return ReactiveProperty.new(initial_value)

static func combine_latest(properties: Array[ReactiveProperty]) -> ReactiveProperty:
    var result = ReactiveProperty.new({})
    for prop in properties:
        prop.changed.connect(func(_): _update_combined_value(result, properties))
    return result

static func _update_combined_value(result: ReactiveProperty, properties: Array[ReactiveProperty]) -> void:
    var values = {}
    for i in range(properties.size()):
        values["prop_%d" % i] = properties[i].value
    result.value = values
```

### 2.2 イベントユーティリティ

```gdscript
class_name EventUtils
extends Node

static func create_event_bus() -> EventBus:
    return EventBus.new()

static func subscribe_to_events(bus: EventBus, events: Array[String], callback: Callable) -> void:
    for event in events:
        bus.subscribe(event, callback)

static func unsubscribe_from_events(bus: EventBus, events: Array[String], callback: Callable) -> void:
    for event in events:
        bus.unsubscribe(event, callback)
```

## 3. 拡張ユーティリティ

### 3.1 アニメーション拡張

```gdscript
class_name AnimationUtils
extends Node

static func create_tween(node: Node, properties: Dictionary) -> Tween:
    var tween = node.create_tween()
    for prop in properties:
        tween.tween_property(node, prop, properties[prop], 0.3)
    return tween

static func create_sequence(node: Node, animations: Array[Dictionary]) -> Tween:
    var tween = node.create_tween()
    for anim in animations:
        tween.tween_property(node, anim.property, anim.value, anim.duration)
    return tween
```

### 3.2 UI 拡張

```gdscript
class_name UIUtils
extends Node

static func create_tooltip(node: Node, text: String) -> Control:
    var tooltip = Label.new()
    tooltip.text = text
    node.add_child(tooltip)
    return tooltip

static func create_loading_indicator(node: Node) -> Control:
    var indicator = ProgressBar.new()
    indicator.show_percentage = false
    node.add_child(indicator)
    return indicator
```

## 4. テストユーティリティ

### 4.1 モック生成

```gdscript
class_name MockUtils
extends Node

static func create_mock_view_model() -> GameViewModel:
    var mock = GameViewModel.new()
    mock.state = ReactiveProperty.new({"test": true})
    return mock

static func create_mock_model() -> GameModel:
    var mock = GameModel.new()
    mock.state = {"test": true}
    return mock
```

### 4.2 テストヘルパー

```gdscript
class_name TestUtils
extends Node

static func wait_for_signal(signal_emitter: Object, signal_name: String, timeout: float = 1.0) -> bool:
    var timeout_timer = Timer.new()
    timeout_timer.wait_time = timeout
    timeout_timer.one_shot = true
    signal_emitter.add_child(timeout_timer)

    var signal_received = false
    signal_emitter.connect(signal_name, func(): signal_received = true)
    timeout_timer.start()
    await timeout_timer.timeout

    return signal_received
```

## 5. パフォーマンス最適化

### 5.1 キャッシュ管理

```gdscript
class_name CacheUtils
extends Node

static var _cache: Dictionary = {}

static func get_cached(key: String, factory: Callable) -> Variant:
    if not _cache.has(key):
        _cache[key] = factory.call()
    return _cache[key]

static func clear_cache() -> void:
    _cache.clear()
```

### 5.2 メモリ管理

```gdscript
class_name MemoryUtils
extends Node

static func dispose_resources(node: Node) -> void:
    for child in node.get_children():
        dispose_resources(child)
    if node.has_method("dispose"):
        node.dispose()
```

## 6. 使用例

### 6.1 リアクティブな状態管理

```gdscript
# ViewModel
var health = ReactiveUtils.create_observable(100)
var mana = ReactiveUtils.create_observable(50)
var combined = ReactiveUtils.combine_latest([health, mana])

# View
func _ready() -> void:
    combined.changed.connect(_on_stats_changed)

func _on_stats_changed(new_stats: Dictionary) -> void:
    $HealthLabel.text = "HP: %d" % new_stats.prop_0
    $ManaLabel.text = "MP: %d" % new_stats.prop_1
```

### 6.2 イベント処理

```gdscript
# Game
var event_bus = EventUtils.create_event_bus()

func _ready() -> void:
    EventUtils.subscribe_to_events(event_bus, ["player_died", "game_over"], _on_game_end)

func _on_game_end(event_name: String) -> void:
    match event_name:
        "player_died":
            show_death_screen()
        "game_over":
            show_game_over_screen()
```

## 7. 制限事項

### 7.1 技術的制限

-   Godot の制約
-   パフォーマンスの考慮
-   メモリ使用量の制限

### 7.2 運用上の制限

-   チーム開発の効率
-   学習コスト
-   保守性の確保

## 8. 変更履歴

### v0.2.0 (2024-03-23)

-   テストユーティリティの追加
-   パフォーマンス最適化の改善
-   使用例の追加

## 10. 変更履歴

| バージョン | 更新日     | 変更内容                                                                             |
| ---------- | ---------- | ------------------------------------------------------------------------------------ |
| 0.2.1      | 2025-06-13 | 目次の更新と概要セクションの追加                                                     |
| 0.2.0      | 2024-03-23 | 機能拡張<br>- ユーティリティの追加<br>- 使用例の追加<br>- パフォーマンス最適化の追加 |
| 0.1.0      | 2024-03-21 | 初版作成<br>- 基本実装の追加<br>- エラー処理の定義<br>- ベストプラクティスの追加     |
