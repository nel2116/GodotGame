---
title: テストガイドライン
version: 0.1.4
status: draft
updated: 2025-06-07
tags:
    - UserGuide
    - Test
    - Guideline
linked_docs:
    - "[[20_UserGuides/TestExecutionGuide|テスト実行ガイド]]"
    - "[[99_Reference/AI_Agent_ImplementationWorkflow.md]]"
---

# テストガイドライン

## 目次
1. [概要](#概要)
2. [基本方針](#基本方針)
3. [テスト実行手順](#テスト実行手順)
4. [テストの書き方](#テストの書き方)
5. [注意事項](#注意事項)
6. [変更履歴](#変更履歴)

## 概要

このドキュメントでは、プロジェクトでテストを実施する際の基本方針と手順をまとめます。

## 基本方針

- すべての変更はコミット前にテストを実行して結果を確認します。
- テストケースは GUT フレームワークを使用して記述します。
- C# スクリプトを追加・変更した際は `godot --headless --path . --build-solutions --quit` を実行し DLL を生成します。

## テスト実行手順

1. `sudo apt-get update` を実行し、パッケージリストを最新化します。
2. `setup_godot_cli.sh` を実行すると `.NET SDK` と Godot CLI がまとめて導入され、アセットインポートとソリューションビルドまで自動で行われます。
3. 下記コマンドでテストを実行します。

```bash
godot --headless --path . -s addons/gut/gut_cmdln.gd -gconfig=.gutconfig.json
```

## テストの書き方

Scripts/Tests/Core 内の `.gd` ファイルは GUT を利用した基本的なテスト例です。
各テストファイルは `test_*.gd` の名前で保存し、以下の形式で記述します。

```gdscript
extends GutTest

var bus
var received

func _on_event(data: Dictionary) -> void:
    received = data

func before_each() -> void:
    bus = EventBus.new()
    add_child(bus) # テスト対象をツリーに追加
    received = null

func after_each() -> void:
    bus.free() # 生成したノードを解放

func test_emit_and_subscribe() -> void:
    bus.Subscribe("TestEvent", Callable(self, "_on_event"))
    var data := {"value": 42}
    bus.EmitEvent("TestEvent", data)
    assert_eq(received, data)
```

`before_each()` と `after_each()` でテストの準備と後処理を行います。テスト関数は
`test_` から始まる名前にし、`assert_eq` などのアサーションで期待値を検証します。

`test_input_buffer.gd` では入力イベントのキュー操作を、`test_state_manager.gd`
では状態管理の履歴検証を行っています。既存ファイルを参考に、1 つのテストでは 1
つの機能を確認するようにしてください。

## 注意事項

- `gut_cmdln.gd` の設定は `.gutconfig.json` に記述されています。
- テスト結果は `res://Scripts/Tests/test-results_*.xml` に出力されます。
- テストが失敗した場合は原因を特定し、再度テストを通過させてからコミットしてください。

## 変更履歴

| バージョン | 更新日     | 変更内容 |
| ---------- | ---------- | -------- |
| 0.1.4      | 2025-06-07 | テストの書き方セクションを追加 |
| 0.1.3      | 2025-06-07 | setup_godot_cli.sh の自動化に伴い手順を簡素化 |
| 0.1.2      | 2025-06-06 | setup_godot_cli.sh 実行前の apt 更新と .NET インストールを追記 |
| 0.1.1      | 2025-06-06 | インポート手順を追記 |
| 0.1.0      | 2025-06-06 | 初版作成 |

