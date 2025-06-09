---
title: テストガイドライン
version: 1.1.0
status: active
updated: 2025-06-09
tags:
    - UserGuide
    - Test
    - Guideline
linked_docs:
    - "[[20_UserGuides/TestExecutionGuide|テスト実行ガイド]]"
    - "[[99_Reference/AI_Agent_TestWorkflow|AIエージェント向けテスト実行ワークフロー]]"
---

# テストガイドライン

## 目次

1. [概要](#概要)
2. [テスト戦略](#テスト戦略)
3. [テストの種類と使い分け](#テストの種類と使い分け)
4. [テストの書き方](#テストの書き方)
5. [推奨アプローチ](#推奨アプローチ)
6. [変更履歴](#変更履歴)

## 概要

このドキュメントでは、プロジェクトのテスト戦略と実装ガイドラインを説明します。
具体的な実行手順については、[テスト実行ガイド](TestExecutionGuide.md)を参照してください。

## テスト戦略

1. **基本方針**

    - すべての変更はコミット前にテストを実行
    - テストカバレッジを維持・向上
    - テストの自動化を推進

2. **テストの優先順位**
    - 重要なビジネスロジック
    - 頻繁に変更されるコード
    - 複雑なロジック
    - エッジケース

## テストの種類と使い分け

1. **C#テスト（NUnit）**

    - ビジネスロジックのテスト
    - ユーティリティクラスのテスト
    - データ構造のテスト
    - 非同期処理のテスト

2. **GUT テスト**
    - Godot エンジン機能のテスト
    - シーンとノードのテスト
    - ゲーム特有の機能テスト
    - 物理演算やアニメーションのテスト

## テストの書き方

### C#テストの例

```csharp
[Test]
public void ValueChange_NotifiesSubscribers()
{
    var property = new ReactiveProperty<int>(0);
    int notifiedValue = -1;
    using (property.Subscribe(v => notifiedValue = v))
    {
        property.Value = 42;
    }
    Assert.AreEqual(42, notifiedValue);
}
```

### GUT テストの例

```gdscript
extends GutTest

var bus
var received

func _on_event(data: Dictionary) -> void:
    received = data

func before_each() -> void:
    bus = EventBus.new()
    add_child(bus)
    received = null

func after_each() -> void:
    bus.free()

func test_emit_and_subscribe() -> void:
    bus.Subscribe("TestEvent", Callable(self, "_on_event"))
    var data := {"value": 42}
    bus.EmitEvent("TestEvent", data)
    assert_eq(received, data)
```

## 推奨アプローチ

1. **ハイブリッドアプローチ**

    - ビジネスロジック → C#テスト
    - ゲームエンジン機能 → GUT テスト
    - 統合テスト → 必要に応じて両方を使用

2. **テストの粒度**

    - ユニットテスト: 個々の機能をテスト
    - インテグレーションテスト: 複数の機能の連携をテスト
    - エンドツーエンドテスト: 実際の使用シナリオをテスト

3. **テストの命名規則**

    - C#テスト: `[Test]`属性を使用
    - GUT テスト: `test_`プレフィックスを使用

4. **テストの構造**
    - 準備（Arrange）
    - 実行（Act）
    - 検証（Assert）

## テストケースの説明

-   各テストには目的と期待結果をコメントで明確に記述します
-   異常系や境界値のテストも網羅し、理由を併記します

## テストデータ生成方法

-   テストデータは `SetUp` メソッドやヘルパークラスで生成します
-   ランダムデータが必要な場合は `System.Random` を使用し、再現性のためシード値を固定します
-   大量データが必要な場合は `Enumerable.Range` を活用し、メモリ消費に注意します

## 変更履歴

| バージョン | 更新日     | 変更内容 |
| ---------- | ---------- | -------- |
| 1.0.0      | 2025-06-07 | 初版作成 |
| 1.1.0      | 2025-06-09 | テストケース説明とデータ生成方法を追加 |
