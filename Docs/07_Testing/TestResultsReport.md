---
title: テスト結果レポート
version: 1.0.0
status: active
updated: 2025-06-29
tags:
    - Testing
    - Results
    - Report
    - CoreTests
    - GUT
linked_docs:
    - "[[index|テスト戦略]]"
    - "[[TestingEnvironment|テスト環境]]"
    - "[[../99_Reference/TestExecutionGuide|テスト実行ガイド]]"
---

# テスト結果レポート

## 目次

1. [概要](#概要)
2. [Core テスト実行結果](#coreテスト実行結果)
3. [修正内容詳細](#修正内容詳細)
4. [懸念点と対策](#懸念点と対策)
5. [今後の改善計画](#今後の改善計画)
6. [変更履歴](#変更履歴)

## 概要

このドキュメントでは、プロジェクトのテスト実行結果、修正内容、懸念点を詳細に記録します。
テストの安定性向上と品質保証のため、継続的に更新されます。

## Core テスト実行結果

### 最終実行結果（2025-06-29）

| 項目           | 結果   |
| -------------- | ------ |
| **総テスト数** | 88     |
| **成功数**     | 88     |
| **失敗数**     | 0      |
| **スキップ数** | 0      |
| **実行時間**   | 2.5 秒 |
| **警告数**     | 7 件   |

### テスト分類別結果

#### ✅ 成功したテストカテゴリ

-   **Events**: GameEventBusTests（イベント発行・購読・エラー処理）
-   **ViewModels**: ViewModelBaseTests（基底クラス機能）
-   **Reactive**: ReactivePropertyTests（リアクティブプロパティ）
-   **Utilities**: AsyncValidatorTests, CommandTests（ユーティリティ）
-   **State**: CommonStateViewModelTests（状態管理）
-   **Resource**: CommonResourceViewModelTests（リソース管理）

#### ⚠️ 警告内容

-   **CS8785**: ScriptPathAttributeGenerator の警告（Godot 関連、無視可能）
-   **CS8625/CS8600**: null 非許容型の警告（テストコード、動作に影響なし）

### 実行環境

-   **.NET Version**: 8.0
-   **NUnit Version**: 3.13.3
-   **OS**: Windows 10.0.26100
-   **実行場所**: `Tests/Core/`

## 修正内容詳細

### 1. ViewModelBaseTests 修正

#### 問題

-   `ViewModelBase.SubscribeToEvent<T>(Action<T>)`が protected でアクセス不可
-   `CompositeDisposable`に Count プロパティがない

#### 修正内容

```csharp
// TestViewModelにpublicラッパー追加
public IDisposable PublicSubscribeToEvent<T>(Action<T> onNext = null) where T : GameEvent
{
    return SubscribeToEvent(onNext);
}

// DisposablesのCountアクセサ修正
public int DisposableCount => Disposables.DisposableCount;
```

#### 結果

-   ✅ テスト成功
-   ✅ 適切な抽象化レベルを維持

### 2. GameEventBusTests 修正

#### 問題

-   イベントバッファリング（16ms）により、Thread.Sleep(10)では不十分
-   イベントが通知されず、カウントが 0 になる

#### 修正内容

```csharp
// Thread.Sleepを10ms→20msに延長
Thread.Sleep(20); // イベント処理の遅延を考慮（バッファリング16ms + 余裕）
```

#### 結果

-   ✅ イベント通知が正常に動作
-   ✅ バッファリング遅延を適切に考慮

### 3. CommonStateViewModelTests 修正

#### 問題

-   EventBus や ViewModel のインスタンスが未定義
-   イベント購読の順序が不適切

#### 修正内容

```csharp
// 明示的なインスタンス生成
var bus = new GameEventBus();
var model = new CommonStateModel();
var vm = new CommonStateViewModel(model, bus);
vm.Initialize();

// イベント購読→状態変更→遅延→検証の順序
StateChangedEvent receivedEvent = null;
bus.GetEventStream<StateChangedEvent>().Subscribe(e => receivedEvent = e);
vm.ChangeState("NewState");
Thread.Sleep(20);
```

#### 結果

-   ✅ イベント発行・購読が正常に動作
-   ✅ テストの独立性を確保

### 4. CommonResourceViewModelTests 修正

#### 問題

-   ResourceCacheEvent 型名が間違い（正しくは ResourceCacheChangedEvent）
-   EventBus や ViewModel のインスタンスが未定義

#### 修正内容

```csharp
// 正しい型名に修正
ResourceCacheChangedEvent receivedEvent = null;
bus.GetEventStream<ResourceCacheChangedEvent>().Subscribe(e => receivedEvent = e);
```

#### 結果

-   ✅ 型エラーが解消
-   ✅ イベント処理が正常に動作

## 懸念点と対策

### 1. イベントバッファリングの遅延

#### 懸念点

-   GameEventBus の 16ms バッファリングにより、テストで 20ms の遅延が必要
-   テスト実行時間が増加
-   タイミング依存のテストが不安定になる可能性

#### 対策

-   ✅ バッファリング遅延を考慮した適切な Thread.Sleep 設定
-   🔄 今後の改善：テスト用にバッファリングを無効化するオプション検討

### 2. Godot 依存テストの分離

#### 懸念点

-   Godot 依存テストが.NET テストランナーでクラッシュ
-   統合テストの実行環境が複雑

#### 対策

-   ✅ Godot 依存テストを GUT に移行
-   ✅ Core テストと Godot 依存テストの明確な分離
-   🔄 今後の改善：GUT テストの実行環境整備

### 3. テストの保守性

#### 懸念点

-   イベント駆動システムのテストが複雑
-   タイミング依存のテストが増加

#### 対策

-   ✅ テスト実装ガイドラインの整備
-   ✅ 本質を保つ修正の原則の確立
-   🔄 今後の改善：テストヘルパークラスの整備

### 4. パフォーマンス

#### 懸念点

-   大量のイベント発行時のパフォーマンス
-   メモリ使用量の増加

#### 対策

-   ✅ パフォーマンステストの実装
-   ✅ メモリリークテストの実装
-   🔄 今後の改善：パフォーマンス監視の強化

## 今後の改善計画

### 短期（1-2 週間）

1. **GUT テスト環境の整備**

    - Godot エディタでの GUT テスト実行手順確立
    - CI/CD での GUT テスト実行環境構築

2. **テストヘルパーの整備**
    - イベント駆動テスト用ヘルパークラス作成
    - タイミング依存テスト用ユーティリティ作成

### 中期（1-2 ヶ月）

1. **テストカバレッジの向上**

    - 未テスト領域の特定とテスト追加
    - エッジケースのテスト追加

2. **パフォーマンステストの強化**
    - 負荷テストの自動化
    - メモリリーク検出の自動化

### 長期（3-6 ヶ月）

1. **テスト戦略の最適化**

    - テスト実行時間の短縮
    - テスト保守性の向上

2. **品質保証の強化**
    - 継続的テスト実行の確立
    - テスト結果の自動分析

## 2026-07-17 追記: 除外テストの復元とCI導入

`Tests/Core/CoreTests.csproj`の`Compile Remove`で長期間ビルド対象外になっていたテストファイル（Player/Animation, Combat, State, Progression, Movement, Input, ErrorHandling等）を精査した。

-   実際にGodot依存があったのは`Player/Input`配下の3ファイルのみ。原因は`TestBase`を継承していなかったため`GodotMock.SetTestEnvironment(true)`が呼ばれず、`InputState.Update()`が実際の`Godot.Input`ネイティブAPIを呼んでテストホストをクラッシュさせていたこと。`TestBase`継承を追加して解消した。
-   除外期間中に実装側が変更され、テストが実装とズレていた例を複数発見（`CommonMovementModel`のVelocity/VerticalVelocity分離、`PlayerStateViewModel.HandleStateChange`と`PlayerMovementViewModel.HandleDash`の状態反映漏れ、`GameEventBus`のログ出力無効化など）。実装側の不整合は修正し、設計判断が必要なものは`[Ignore]`に理由を明記して残した。
-   `Performance/LongRunningTests.cs`（数十秒〜分単位のstabilityテスト）は`Compile Remove`ではなく`[Category("LongRunning")]`に変更し、コンパイルは通しつつ通常実行から除外するようにした。
-   `.github/workflows/tests.yml`を追加し、`dotnet test`（コア）・GUT・LongRunningの3ジョブをCIに接続した（従来は`Godot_CI.yml`がexportのみでテストは一切実行していなかった）。
-   `coverlet.collector`を追加しカバレッジ計測を有効化した。

## 変更履歴

| バージョン | 更新日     | 変更内容                                  |
| ---------- | ---------- | ----------------------------------------- |
| 1.1.0      | 2026-07-17 | 除外テストの復元、CI導入、カバレッジ計測追加を記録 |
| 1.0.0      | 2025-06-29 | 初版作成、Core テスト結果と修正内容を記録 |

---

**注意事項**

-   このレポートは定期的に更新し、テストの品質向上に活用してください
-   懸念点は早期に対策を実施し、プロジェクトの安定性を確保してください
-   改善計画は進捗に応じて調整し、継続的な品質向上を図ってください
