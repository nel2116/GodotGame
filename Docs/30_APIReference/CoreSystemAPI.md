---
title: コアシステムAPI
version: 0.1.0
status: draft
updated: 2025-06-01
tags:
    - API
    - Core
    - System
linked_docs:
    - "[[30_APIReference/00_index]]"
    - "[[10_CoreDocs/00_index]]"
---

# コアシステム API

## 目次

1. [概要](#概要)
2. [API 一覧](#api一覧)
3. [使用方法](#使用方法)
4. [制限事項](#制限事項)
5. [変更履歴](#変更履歴)

## 概要

コアシステム API は、ゲームの基本的な機能を提供する API 群です。
ゲームエンジンとの連携、システム管理、ユーティリティ機能などを含みます。

## API 一覧

### ゲームエンジン関連

#### シーン管理

```gdscript
# シーンの読み込み
func load_scene(scene_path: String) -> void

# シーンの切り替え
func change_scene(scene_path: String, transition: String = "") -> void
```

#### リソース管理

```gdscript
# リソースの読み込み
func load_resource(resource_path: String) -> Resource

# リソースの解放
func unload_resource(resource: Resource) -> void
```

### システム管理

#### 設定管理

```gdscript
# 設定の読み込み
func load_settings() -> Dictionary

# 設定の保存
func save_settings(settings: Dictionary) -> void
```

#### ログ管理

```gdscript
# ログの出力
func log(message: String, level: String = "INFO") -> void

# ログのクリア
func clear_logs() -> void
```

### ユーティリティ機能

#### 時間管理

```gdscript
# ゲーム時間の取得
func get_game_time() -> float

# 時間の一時停止
func pause_time() -> void
```

#### 数学関数

```gdscript
# ランダム値の生成
func random_range(min: float, max: float) -> float

# 角度の正規化
func normalize_angle(angle: float) -> float
```

## 使用方法

### 基本的な使用例

```gdscript
# シーンの読み込み
func _ready() -> void:
    var scene_manager = get_node("/root/SceneManager")
    scene_manager.load_scene("res://scenes/main_menu.tscn")

# リソースの管理
func load_game_assets() -> void:
    var resource_manager = get_node("/root/ResourceManager")
    var texture = resource_manager.load_resource("res://assets/textures/player.png")
```

### エラーハンドリング

```gdscript
# エラーハンドリングの例
func safe_load_resource(path: String) -> Resource:
    var resource_manager = get_node("/root/ResourceManager")
    var resource = null

    try:
        resource = resource_manager.load_resource(path)
    except:
        log("Failed to load resource: " + path, "ERROR")

    return resource
```

## 制限事項

-   シーンの読み込みは非同期で行われるため、完了を待つ必要があります
-   リソースの読み込みはメモリ使用量に注意が必要です
-   時間管理はゲームの状態に依存します

## 変更履歴

| バージョン | 更新日     | 変更内容 |
| ---------- | ---------- | -------- |
| 0.1.0      | 2025-06-01 | 初版作成 |
