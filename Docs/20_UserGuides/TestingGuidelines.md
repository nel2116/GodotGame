---
title: テストガイドライン
version: 0.1.0
status: draft
updated: 2025-06-07
tags:
    - Testing
    - Guidelines
    - UserGuide
linked_docs:
    - "[[10_CoreDocs/15_ImplementationSpecs/15.6_Testing|テスト実装仕様書]]"
---

# テストガイドライン

## 目次
1. [概要](#概要)
2. [ディレクトリ構成](#ディレクトリ構成)
3. [命名規則](#命名規則)
4. [テストの書き方](#テストの書き方)
5. [ベストプラクティス](#ベストプラクティス)
6. [変更履歴](#変更履歴)

## 概要

このドキュメントでは、Gut フレームワークを用いたテストスクリプトの記述方法をまとめます。
既存のサンプルテストを参考に、ユニットテストおよび統合テストを効率良く作成するための指針を提供します。

## ディレクトリ構成

```
Scripts/
└── Tests/
    ├── Core/
    ├── Performance/
    └── Integration/
```

`.gutconfig.json` の `configured_dirs` に記載されたディレクトリへテストを配置します。

## 命名規則

- テストファイルは `test_` で始め、拡張子は `.gd` とします。例: `test_player.gd`
- クラス名は PascalCase で `Test` を接尾辞とします。例: `PlayerTest`

## テストの書き方

1. `extends GutTest` を宣言し、`class_name` を設定します。
2. `before_each()` で初期化、`after_each()` でクリーンアップを行います。
3. 非同期処理のテストでは `await` を使用し、十分な待機時間を確保します。
4. アサーションには `assert_eq`, `assert_true`, `assert_false` などを使用します。

```gdscript
class_name PlayerTest
extends GutTest

var player: Player

func before_each() -> void:
    player = Player.new()
    add_child(player)

func after_each() -> void:
    player.queue_free()

func test_player_health() -> void:
    assert_eq(player.health, 100)

func test_take_damage() -> void:
    player.take_damage(10)
    assert_eq(player.health, 90)
```

### 非同期テストの例

```gdscript
func test_async_event() -> void:
    game.start()
    await get_tree().create_timer(0.1).timeout
    assert_true(game.is_running)
```

## ベストプラクティス

- 各テストは互いに依存させず、独立して実行可能にする。
- 正常系だけでなく異常系や境界値も網羅する。
- モックが必要な場合はシンプルなノードを用いて再現する。
- CI では `godot --headless --script res://addons/gut/gut_cmdln.gd` を使用して自動実行する。

## 変更履歴

| バージョン | 更新日     | 変更内容 |
| ---------- | ---------- | -------- |
| 0.1.0      | 2025-06-01 | 初版作成 |
