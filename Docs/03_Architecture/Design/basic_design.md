---
title: MVVM+RX基本設計書
version: 0.2.0
status: draft
updated: 2024-03-23
tags:
    - Core
    - Design
    - MVVM
    - Reactive
linked_docs:
    - "[[mvvm_rx_architecture|MVVM+RXアーキテクチャ]]"
    - "[[12_03_detailed_design|MVVM+RX詳細設計書]]"
    - "[[12_04_system_integration|システム間連携]]"
    - "[[12_05_common_utilities|共通ユーティリティ]]"
---

# MVVM+RX 基本設計書

## 目次

1. [概要](#1-概要)
2. [基本設計](#2-基本設計)
3. [コンポーネント設計](#3-コンポーネント設計)
4. [インターフェース設計](#4-インターフェース設計)
5. [データフロー](#5-データフロー)
6. [エラー処理](#6-エラー処理)
7. [ベストプラクティス](#7-ベストプラクティス)
8. [制限事項](#8-制限事項)
9. [変更履歴](#9-変更履歴)

## 1. 概要

### 1.1 目的

本ドキュメントは、Shrine of the Lost Ones における MVVM + リアクティブプログラミングの基本設計を定義し、以下の目的を達成することを目指します：

-   アーキテクチャの具体的な実装方針の確立
-   開発チーム間での実装の一貫性確保
-   保守性と拡張性の高いコードベースの構築

### 1.2 適用範囲

-   ゲームコアシステム
-   UI/UX システム
-   データ管理システム
-   イベントシステム

## 2. 基本設計

### 2.1 レイヤー構造

#### 2.1.1 レイヤー構成

```
[View Layer (Godot Node)]
    ↓
[ViewModel Layer (ReactiveProperty)]
    ↓
[Model Layer (Business Logic)]
    ↓
[Service Layer (External Resources)]
```

#### 2.1.2 各レイヤーの責務

| レイヤー  | 責務               | 実装例               |
| --------- | ------------------ | -------------------- |
| View      | UI 表示・入力受付  | Godot Node           |
| ViewModel | 状態管理・変換     | ReactiveProperty     |
| Model     | ビジネスロジック   | ゲームロジッククラス |
| Service   | 外部連携・共通機能 | リソース管理         |

### 2.2 データフロー

#### 2.2.1 一方向データフロー

```
[User Input] → [View] → [ViewModel] → [Model] → [Service]
```

#### 2.2.2 双方向バインディング

-   フォーム入力などの特定のケースでのみ使用
-   基本的には一方向データフローを推奨

## 3. コンポーネント設計

### 3.1 基本コンポーネント

#### 3.1.1 View

```gdscript
class_name GameView
extends Node2D

var view_model: GameViewModel

func _ready() -> void:
    view_model = GameViewModel.new()
    view_model.state_changed.connect(_on_state_changed)

func _on_state_changed(new_state: Dictionary) -> void:
    # UI更新処理
    pass
```

#### 3.1.2 ViewModel

```gdscript
class_name GameViewModel
extends Node

var state: ReactiveProperty = ReactiveProperty.new({})
var model: GameModel

func _init() -> void:
    model = GameModel.new()
    model.state_changed.connect(_on_model_state_changed)

func _on_model_state_changed(new_state: Dictionary) -> void:
    state.value = new_state
```

#### 3.1.3 Model

```gdscript
class_name GameModel
extends Node

var state: Dictionary = {}
signal state_changed(new_state: Dictionary)

func update_state(new_state: Dictionary) -> void:
    state = new_state
    state_changed.emit(state)
```

### 3.2 共通コンポーネント

#### 3.2.1 コマンド

```gdscript
class_name GameCommand
extends Node

var can_execute: ReactiveProperty = ReactiveProperty.new(true)
var is_executing: ReactiveProperty = ReactiveProperty.new(false)

func execute() -> void:
    if not can_execute.value:
        return
    is_executing.value = true
    # コマンド実行処理
    is_executing.value = false
```

#### 3.2.2 バリデーター

```gdscript
class_name GameValidator
extends Node

func validate_input(input: Dictionary) -> Array[String]:
    var errors: Array[String] = []
    # バリデーション処理
    return errors
```

## 4. インターフェース設計

### 4.1 基本インターフェース

#### 4.1.1 IViewModel

```gdscript
interface IViewModel:
    func initialize()
    func dispose()
    func update()
```

#### 4.1.2 IModel

```gdscript
interface IModel:
    func load()
    func save()
    func validate()
```

### 4.2 サービスインターフェース

#### 4.2.1 IDataService

```gdscript
interface IDataService:
    func get_data(key: String) -> Dictionary
    func set_data(key: String, value: Dictionary)
    func delete_data(key: String)
```

#### 4.2.2 IEventService

```gdscript
interface IEventService:
    func subscribe(event_name: String, callback: Callable)
    func publish(event_name: String, data: Dictionary)
    func unsubscribe(event_name: String, callback: Callable)
```

## 5. データフロー

### 5.1 基本的なデータフロー

#### 5.1.1 一方向データフロー

1. ユーザーアクション
2. View のイベント発火
3. ViewModel のコマンド実行
4. Model の状態更新
5. ViewModel の状態更新
6. View の更新

#### 5.1.2 双方向バインディング

```gdscript
# ViewModel
var input_text: ReactiveProperty = ReactiveProperty.new("")

# View
func _on_text_changed(new_text: String) -> void:
    view_model.input_text.value = new_text

func _ready() -> void:
    view_model.input_text.changed.connect(_on_input_text_changed)

func _on_input_text_changed(new_text: String) -> void:
    $InputField.text = new_text
```

### 5.2 非同期データフロー

#### 5.2.1 非同期処理

```gdscript
# ViewModel
func load_data() -> void:
    is_loading.value = true
    await model.load_data_async()
    is_loading.value = false

# Model
func load_data_async() -> void:
    await get_tree().create_timer(1.0).timeout
    state = {"loaded": true}
    state_changed.emit(state)
```

#### 5.2.2 キャッシュ戦略

```gdscript
# Service
var cache: Dictionary = {}

func get_cached_data(key: String) -> Dictionary:
    if cache.has(key):
        return cache[key]
    var data = load_data(key)
    cache[key] = data
    return data
```

## 6. エラー処理

### 6.1 エラーハンドリング

#### 6.1.1 例外処理

```gdscript
# ViewModel
func execute_command() -> void:
    try:
        model.execute()
    except Error as e:
        error_occurred.emit(e.message)
```

#### 6.1.2 エラー通知

```gdscript
# View
func _on_error_occurred(message: String) -> void:
    $ErrorLabel.text = message
    $ErrorLabel.show()
    await get_tree().create_timer(3.0).timeout
    $ErrorLabel.hide()
```

### 6.2 バリデーション

#### 6.2.1 入力バリデーション

```gdscript
# ViewModel
func validate_input(input: Dictionary) -> Array[String]:
    var errors: Array[String] = []
    if not input.has("name"):
        errors.append("名前は必須です")
    if input.get("age", 0) < 0:
        errors.append("年齢は0以上である必要があります")
    return errors
```

#### 6.2.2 ビジネスルール

```gdscript
# Model
func validate_business_rules() -> Array[String]:
    var errors: Array[String] = []
    if not can_perform_action():
        errors.append("アクションを実行できません")
    return errors
```

## 7. ベストプラクティス

### 7.1 コーディング規約

#### 7.1.1 命名規則

-   クラス名: PascalCase
-   メソッド名: snake_case
-   変数名: snake_case
-   定数: UPPER_SNAKE_CASE

#### 7.1.2 ファイル構成

-   1 ファイル 1 クラス
-   関連ファイルのグループ化
-   適切なディレクトリ構造

### 7.2 パフォーマンス最適化

#### 7.2.1 メモリ管理

```gdscript
# ViewModel
func dispose() -> void:
    # サブスクリプションの解除
    for subscription in subscriptions:
        subscription.dispose()
    subscriptions.clear()
```

#### 7.2.2 更新最適化

```gdscript
# View
var update_timer: float = 0.0
const UPDATE_INTERVAL: float = 0.1

func _process(delta: float) -> void:
    update_timer += delta
    if update_timer >= UPDATE_INTERVAL:
        update_timer = 0.0
        update_ui()
```

## 8. 制限事項

### 8.1 技術的制限

-   Godot の制約
-   パフォーマンスの考慮
-   メモリ使用量の制限

### 8.2 運用上の制限

-   チーム開発の効率
-   学習コスト
-   保守性の確保

## 9. 変更履歴

### v0.2.0 (2024-03-23)

-   実装例の追加
-   エラー処理の詳細化
-   パフォーマンス最適化の追加

## 9. 変更履歴

| バージョン | 更新日     | 変更内容                                                                                       |
| ---------- | ---------- | ---------------------------------------------------------------------------------------------- |
| 0.2.0      | 2024-03-23 | 機能拡張<br>- コンポーネント設計の追加<br>- インターフェース設計の追加<br>- データフローの追加 |
| 0.1.0      | 2024-03-21 | 初版作成<br>- 基本設計の追加<br>- エラー処理の定義<br>- ベストプラクティスの追加               |
