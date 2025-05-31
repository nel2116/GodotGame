---
title: データ管理API
version: 0.1.0
status: draft
updated: 2025-06-01
tags:
    - API
    - Data
    - Management
linked_docs:
    - "[[30_APIReference/00_index]]"
    - "[[30_APIReference/CoreSystemAPI]]"
    - "[[10_CoreDocs/00_index]]"
---

# データ管理 API

## 目次

1. [概要](#概要)
2. [API 一覧](#api一覧)
3. [使用方法](#使用方法)
4. [制限事項](#制限事項)
5. [変更履歴](#変更履歴)

## 概要

データ管理 API は、ゲーム内のデータを管理するための API 群です。
セーブ/ロード、設定管理、リソース管理などの機能を提供します。

## API 一覧

### セーブ/ロード

#### セーブデータ管理

```gdscript
# ゲームデータの保存
func save_game_data(slot: int = 0) -> bool

# ゲームデータの読み込み
func load_game_data(slot: int = 0) -> bool

# セーブデータの削除
func delete_save_data(slot: int) -> bool
```

#### セーブデータ情報

```gdscript
# セーブデータの存在確認
func has_save_data(slot: int) -> bool

# セーブデータの情報取得
func get_save_data_info(slot: int) -> Dictionary

# セーブデータの一覧取得
func get_save_data_list() -> Array
```

### 設定管理

#### ゲーム設定

```gdscript
# 設定の保存
func save_settings(settings: Dictionary) -> void

# 設定の読み込み
func load_settings() -> Dictionary

# 設定のリセット
func reset_settings() -> void
```

#### キー設定

```gdscript
# キー設定の保存
func save_key_bindings(bindings: Dictionary) -> void

# キー設定の読み込み
func load_key_bindings() -> Dictionary

# キー設定のリセット
func reset_key_bindings() -> void
```

### リソース管理

#### リソース操作

```gdscript
# リソースの読み込み
func load_resource(path: String) -> Resource

# リソースの保存
func save_resource(resource: Resource, path: String) -> bool

# リソースの解放
func unload_resource(resource: Resource) -> void
```

#### リソース情報

```gdscript
# リソースの存在確認
func has_resource(path: String) -> bool

# リソースの情報取得
func get_resource_info(path: String) -> Dictionary

# リソースの一覧取得
func get_resource_list() -> Array
```

## 使用方法

### 基本的な使用例

```gdscript
# ゲームデータの保存
func save_game() -> void:
    var save_data = {
        "player": get_player_data(),
        "inventory": get_inventory_data(),
        "quests": get_quest_data()
    }

    if save_game_data(save_data):
        print("ゲームデータを保存しました")
    else:
        print("保存に失敗しました")

# 設定の管理
func update_settings() -> void:
    var current_settings = load_settings()
    current_settings["volume"] = 0.8
    current_settings["fullscreen"] = true
    save_settings(current_settings)
```

### リソース管理の使用例

```gdscript
# リソースの読み込みと管理
func load_game_resources() -> void:
    var resources = {
        "player_texture": "res://assets/textures/player.png",
        "enemy_texture": "res://assets/textures/enemy.png",
        "background": "res://assets/textures/background.png"
    }

    for key in resources:
        var resource = load_resource(resources[key])
        if resource != null:
            resource_cache[key] = resource
```

## 制限事項

-   セーブデータは最大 10 スロットまで保存可能です
-   リソースの読み込みは非同期で行われる場合があります
-   設定の変更は即時反映されない場合があります

## 変更履歴

| バージョン | 更新日     | 変更内容 |
| ---------- | ---------- | -------- |
| 0.1.0      | 2025-06-01 | 初版作成 |
