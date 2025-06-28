---
title: テスト・KPI・バランス調整
version: 0.3.0
status: draft
updated: 2025-06-29
tags:
    - Testing
    - KPI
    - Balance
    - Documentation
    - Index
linked_docs:
    - "[[00_index]]"
    - "[[TestingEnvironment|テスト環境]]"
    - "[[TestResultsReport|テスト結果レポート]]"
    - "[[../99_Reference/TestExecutionGuide|テスト実行ガイド]]"
    - "[[../99_Reference/GodotTestCommand|Godot依存テストの実行方法]]"
---

# テスト・KPI・バランス調整

## 目次

1. [概要](#概要)
2. [関連ドキュメント](#関連ドキュメント)
3. [テスト計画](#テスト計画)
4. [テスト実装ガイドライン](#テスト実装ガイドライン)
5. [本質を保つ修正の原則](#本質を保つ修正の原則)
6. [KPI 設定](#kpi設定)
7. [バランス調整](#バランス調整)
8. [注意事項](#注意事項)
9. [変更履歴](#変更履歴)

## 概要

このセクションでは、ゲームのテスト計画、KPI 設定、バランス調整に関するドキュメントを管理します。

## 関連ドキュメント

-   [[TestingEnvironment|テスト環境]]
-   [[TestResultsReport|テスト結果レポート]]
-   [[../99_Reference/TestExecutionGuide|テスト実行ガイド]]
-   [[../99_Reference/GodotTestCommand|Godot依存テストの実行方法]]

## テスト計画

1. 機能テスト

    - ユニットテスト
    - 統合テスト
    - システムテスト
    - 回帰テスト

2. パフォーマンステスト

    - 負荷テスト
    - ストレステスト
    - メモリリークテスト
    - フレームレートテスト

3. ユーザーテスト
    - アルファテスト
    - ベータテスト
    - ユーザビリティテスト
    - アクセシビリティテスト

## テスト実装ガイドライン

### 基本方針

テストは「本番の動作を正確に検証する」ことを目的とし、以下の原則に従って実装します。

### テスト実装時のチェックリスト

-   [ ] 本番と同じコンポーネント・設定を使用しているか
-   [ ] 実際の仕様・動作に合わせた期待値を設定しているか
-   [ ] タイミング依存の動作を考慮しているか
-   [ ] テストの目的が明確か（テストを通すためではなく、本番の動作を検証するため）

### イベント駆動システムのテスト

#### GameEventBus を使用するテスト

```csharp
// ✅ 正しい実装例
[Test]
public void EventDrivenTest_Example()
{
    var bus = new GameEventBus(); // 本番と同じインスタンス

    // イベント購読を先に実行
    SomeEvent? received = null;
    bus.GetEventStream<SomeEvent>().Subscribe(e => received = e);

    var model = new SomeModel(bus); // 明示的にバスを渡す
    model.Initialize();

    // バッファリング遅延を考慮して待機
    System.Threading.Thread.Sleep(20);

    // 実際の仕様に合わせた期待値
    Assert.AreEqual(expectedValue, actualValue);
}
```

#### 避けるべき実装

```csharp
// ❌ 避けるべき実装例
[Test]
public void BadTest_Example()
{
    var model = new SomeModel(); // デフォルトコンストラクタ（シングルトン使用）
    // 本番と異なる動作のテスト
    // 実際の仕様を無視した期待値
}
```

### タイミング依存テスト

#### フレームレート依存の動作

```csharp
// ✅ 正しい実装例
[Test]
public void FrameRateDependentTest_Example()
{
    // イベント発行
    model.ProcessInput();

    // バッファリング遅延を考慮（16ms = 1フレーム）
    System.Threading.Thread.Sleep(20);

    // 状態更新
    model.Update();

    // 実際の仕様に合わせた期待値（例：DAMPING_FACTOR考慮）
    Assert.AreEqual(expectedValue, model.Velocity);
}
```

## 本質を保つ修正の原則

### 1. 本番と同じコンポーネント・設定を使用

**原則**: テスト用に異なる実装を作らない

**理由**:

-   本番の動作を正確に検証できない
-   タイミング依存のバグを見逃すリスク
-   テストと本番の動作の乖離

**実装例**:

```csharp
// ✅ 正しい
var bus = new GameEventBus(); // 本番と同じ実装
var model = new PlayerInputModel(bus); // 明示的にバスを渡す

// ❌ 避ける
var model = new PlayerInputModel(); // シングルトン使用
var testBus = new TestGameEventBus(); // テスト用実装
```

### 2. 実際の仕様・動作に合わせた期待値

**原則**: システムの実際の動作を考慮した期待値を設定

**理由**:

-   仕様書や実装に基づいた正確な検証
-   システムの品質・信頼性向上

**実装例**:

```csharp
// ✅ 正しい（DAMPING_FACTOR = 0.9fを考慮）
Assert.AreEqual(new Vector2(4.5f, 0), movementModel.Velocity);

// ❌ 避ける（実際の動作を無視）
Assert.AreEqual(new Vector2(5.0f, 0), movementModel.Velocity);
```

### 3. タイミング依存の動作も考慮

**原則**: イベント伝播の遅延、フレーム間の状態変化を考慮

**理由**:

-   実際のゲームループでの動作を再現
-   タイミング依存のバグを検出

**実装例**:

```csharp
// ✅ 正しい
model.ProcessInput();
System.Threading.Thread.Sleep(20); // バッファリング遅延を考慮
model.Update();

// ❌ 避ける
model.ProcessInput();
model.Update(); // 即座に実行（本番と異なる動作）
```

### 4. テストの目的を明確化

**原則**: 「テストを通すため」ではなく「本番の動作を検証するため」

**理由**:

-   システムの品質・信頼性向上が目的
-   本質的な動作の検証

### 修正時のチェックポイント

#### 修正前

-   [ ] 本番の動作への影響を確認
-   [ ] 修正の必要性を明確化

#### 修正中

-   [ ] 本質的な動作が変わっていないか継続的にチェック
-   [ ] 本番と同じ条件での動作確認

#### 修正後

-   [ ] 本番と同じ条件での動作確認
-   [ ] 他のテストへの影響確認

### 避けるべき修正

1. **テスト用に異なる実装を作る**

    - 例: `TestGameEventBus`、`MockGameEventBus`

2. **実際の仕様を無視した期待値**

    - 例: DAMPING_FACTOR を無視した期待値

3. **本番では発生しない条件でのテスト**
    - 例: 即座のイベント伝播（バッファリングなし）

## KPI 設定

1. ゲームプレイ KPI

    - プレイ時間
    - リテンション率
    - クリア率
    - 難易度バランス

2. 技術 KPI

    - フレームレート
    - メモリ使用量
    - ロード時間
    - クラッシュ率

3. ビジネス KPI
    - ダウンロード数
    - アクティブユーザー数
    - 収益指標
    - ユーザー満足度

## バランス調整

1. ゲームバランス

    - 難易度調整
    - 報酬バランス
    - 進行速度
    - リソース管理

2. 経済バランス
    - 通貨バランス
    - アイテム価格
    - 報酬量
    - 課金バランス

## 注意事項

-   テスト結果は必ず記録し、改善に活用してください
-   KPI は定期的に見直し、必要に応じて調整してください
-   バランス調整は、プレイヤーフィードバックを重視してください
-   パフォーマンステストは、目標プラットフォームで実施してください
-   **Godot 依存の統合テストはクラッシュ回避のため分離して管理・実行してください**
-   **テスト設計・修正方針は本ドキュメントに明記し、今後も必ず遵守してください**
-   **テスト実装時は本質を保つ修正の原則を必ず参照してください**
-   **修正前・中・後のチェックポイントを必ず実行してください**

### Godot 依存テストについて

-   `Tests/Integration_Godot/`配下のテスト（例：PlayerIntegrationTests.cs）は、Godot のネイティブ機能（Node のライフサイクルメソッドやシーン API 等）に依存しています。
-   これらのテストは、**Godot が正しく初期化された環境でのみ実行可能**です。
-   通常の CI やローカルテストランナーでは失敗・クラッシュする場合があるため、**必ず Godot 環境で個別に実行してください**。
-   ユニットテスト（`Tests/Core/`配下）は Godot 依存がないため、通常の.NET テストランナーで実行可能です。

## 最新テスト結果（2025-06-29）

### Core テスト実行結果

-   **総テスト数**: 88
-   **成功数**: 88
-   **失敗数**: 0
-   **実行時間**: 2.5 秒
-   **警告数**: 7 件（動作に影響なし）

### 主要な修正内容

1. **ViewModelBaseTests**: protected メソッドの public ラッパー追加、Disposables アクセサ修正
2. **GameEventBusTests**: イベントバッファリング（16ms）を考慮した Thread.Sleep 延長（10ms→20ms）
3. **CommonStateViewModelTests**: EventBus・ViewModel の明示的インスタンス生成
4. **CommonResourceViewModelTests**: ResourceCacheChangedEvent 型名修正

### 懸念点と対策

1. **イベントバッファリング遅延**: テスト用バッファリング無効化オプション検討
2. **Godot 依存テスト分離**: GUT テスト環境整備が必要
3. **テスト保守性**: イベント駆動テスト用ヘルパークラス整備
4. **パフォーマンス**: 大量イベント発行時の監視強化

詳細は [[TestResultsReport|テスト結果レポート]] を参照してください。

## 変更履歴

| バージョン | 更新日     | 変更内容                                           |
| ---------- | ---------- | -------------------------------------------------- |
| 0.2.0      | 2024-12-19 | テスト実装ガイドライン、本質を保つ修正の原則を追加 |
| 0.1.0      | 2024-03-21 | 初版作成                                           |

# テスト戦略

## 1. テストの分類

-   **Core テスト**: .NET/NUnit ベース。純粋なロジック・アルゴリズム・イベントバス等、Godot エンジンに依存しない部分のみを対象とする。
-   **Godot 依存テスト**: Godot.Input や GodotMock、Godot 型（Vector2 等）を直接利用し、Godot のネイティブ API やシーンライフサイクルに依存するテスト。Input/Movement/Animation/Combat/State/Progression の ViewModel/Model テストや PlayerSystemIntegrationTests などが該当。

## 2. Godot 依存テストの管理方針

-   Godot 依存テストは `Tests/Integration_Godot/` 配下に移動し、GUT（Godot Unit Test）形式（GDScript ベース、Godot 4.x 対応）で記述・管理する。
-   既存の C#テストのロジック（イベント発行・状態遷移・エラー処理・統合動作など）は GUT テストに変換し、本質を維持する。
-   CoreTests.csproj からは Godot 依存テストを除外し、純粋なロジックのみを Core テストに残す。

## 3. 実行方法

-   Core テスト: `dotnet test` で実行。
-   Godot 依存テスト: Godot エディタまたは CLI で GUT を用いて実行。

---

**備考:**

-   Godot 依存テストは Godot の初期化やシーンライフサイクルに依存するため、.NET テストランナーでは実行できません。必ず GUT で管理・実行してください。
-   テスト分離により、クラッシュや不安定な挙動を防ぎ、CI/CD でも安定したテスト運用が可能になります。
