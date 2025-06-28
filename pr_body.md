# テストファイル整理と ViewModelNode 追加、品質向上リファクタリング

## 概要

テストファイルの整理と新しい ViewModelNode ファイルの追加、およびレビューコメントに基づく品質向上リファクタリングを行いました。主に Integration_Godot テストファイルの命名規則統一、プレイヤーシステムの ViewModelNode 実装、保守性向上のためのリファクタリングを実施しています。

## 変更内容

### 🔧 リファクタリング

#### テストファイル整理

-   **Integration*Godot ディレクトリ内の.gd ファイルを test*プレフィックス付きに統一**
-   **不要ファイル削除**: 使用されていない.csproj ファイルの削除
-   **GUT 設定更新**: テスト設定ファイルの調整

#### 品質向上リファクタリング（レビューコメント対応）

-   **ViewModelNode の null チェック共通化**: 各 ViewModelNode に`EnsureViewModelInitialized()`メソッドを追加し、null チェックを統一
-   **Thread.Sleep の非同期化**: テストコードの`Thread.Sleep`を`await Task.Delay()`に変更し、テストメソッドを`async Task`に修正
-   **保守性向上**: コードの一貫性と保守性を大幅に向上

### ✨ 新機能追加

#### ViewModelNode 実装

-   **PlayerCombatViewModelNode.cs**: プレイヤー戦闘システムの ViewModelNode 実装
-   **PlayerInputViewModelNode.cs**: プレイヤー入力システムの ViewModelNode 実装
-   **PlayerMovementViewModelNode.cs**: プレイヤー移動システムの ViewModelNode 実装
-   **PlayerAnimationViewModelNode.cs**: プレイヤーアニメーションシステムの ViewModelNode 実装
-   **PlayerProgressionViewModelNode.cs**: プレイヤー進行システムの ViewModelNode 実装
-   **CommonMovementViewModelNode.cs**: 共通移動システムの ViewModelNode 実装

#### テスト実装

-   **統合テスト**: プレイヤーシステム全体の統合テスト
-   **ViewModelNode テスト**: 各 ViewModelNode の動作確認テスト
-   **パフォーマンステスト**: システムの性能確認テスト
-   **エラーハンドリングテスト**: 異常系の動作確認テスト

### 📝 その他の調整

-   **コアテストファイルの微調整**
-   **テストファイルの構造改善**
-   **エラーメッセージの統一化**

## 技術的詳細

### ファイル変更統計

-   **追加**: 7 ファイル
-   **削除**: 8 ファイル
-   **変更**: 20 ファイル
-   **総変更行数**: +1584 行, -788 行

### 影響範囲

-   **テスト実行環境（GUT）**
-   **プレイヤーシステムの ViewModel 層**
-   **Integration_Godot テストスイート**
-   **C#テストスイート**

### 品質向上の詳細

#### ViewModelNode の null チェック共通化

```csharp
// 修正前: 各メソッドで個別にnullチェック
public void UpdateMovement()
{
    if (_viewModel == null)
    {
        GD.PrintErr("ViewModel is not initialized. Call Initialize() first.");
        return;
    }
    _viewModel.UpdateMovement();
}

// 修正後: 共通メソッドでnullチェック
private bool EnsureViewModelInitialized()
{
    if (_viewModel == null)
    {
        GD.PrintErr("ViewModel is not initialized. Call Initialize() first.");
        return false;
    }
    return true;
}

public void UpdateMovement()
{
    if (!EnsureViewModelInitialized()) return;
    _viewModel.UpdateMovement();
}
```

#### Thread.Sleep の非同期化

```csharp
// 修正前: 同期的な待機
[Test]
public void TestMethod()
{
    // テスト処理
    Thread.Sleep(10);
    // 検証
}

// 修正後: 非同期待機
[Test]
public async Task TestMethod()
{
    // テスト処理
    await Task.Delay(10);
    // 検証
}
```

## テスト結果

### C#テスト結果

-   **総テスト数**: 88 件
-   **成功**: 88 件 ✅
-   **失敗**: 0 件 ✅
-   **実行時間**: 約 2.5 秒

### GUT テスト結果

-   **総テスト数**: 88 件
-   **成功**: 88 件 ✅
-   **失敗**: 0 件 ✅
-   **実行時間**: 2.117 秒

### テストカテゴリ別結果

#### C#テスト

-   **Core Systems**: ReactiveProperty, CompositeDisposable, GameEventBus, ViewModelBase
-   **Player Systems**: Input, Movement, Combat, Animation, State, Progression
-   **Integration Tests**: PlayerSystemIntegration, InputMovementIntegration
-   **Common Systems**: Movement, State, Resource

#### GUT テスト

-   **Common**: CommonMovementViewModelTests (17/17 passed)
-   **Player**: Animation, Combat, Input, Movement, SystemIntegration (全テスト成功)

## 品質向上効果

### 1. 保守性の向上

-   ViewModelNode の null チェックロジックが統一され、変更時の影響範囲が明確
-   エラーメッセージの一貫性が保たれる
-   コードの可読性と保守性が大幅に向上

### 2. 非同期処理の改善

-   テストでの同期的な待機が非同期化され、より効率的
-   将来的な CI/CD 環境でのテスト実行パフォーマンス向上
-   リソース使用効率の改善

### 3. コードの一貫性

-   全 ViewModelNode で同じパターンの null チェック実装
-   全テストで統一された非同期待機パターン
-   エラーハンドリングの標準化

## 関連 Issue

この変更に関連する Issue があれば記載してください。

## レビュー依頼

以下の点についてレビューをお願いします：

1. **ViewModelNode の設計**: 新しく追加された ViewModelNode の設計が適切か
2. **テストファイル整理**: ファイル名の変更が適切で、テスト実行に影響がないか
3. **命名規則**: test\_プレフィックスの統一が適切か
4. **null チェック共通化**: EnsureViewModelInitialized()メソッドの実装が適切か
5. **非同期化**: Thread.Sleep から Task.Delay への変更が適切か
6. **保守性向上**: リファクタリングによる保守性の向上が実現されているか

## スクリーンショット

該当する場合は、変更前後のスクリーンショットを添付してください。

## チェックリスト

-   [x] 既存のコアテストが正常に実行されることを確認
-   [x] 新しい ViewModelNode ファイルがコンパイルエラーなしで動作することを確認
-   [x] GUT テストの実行確認
-   [x] ViewModelNode の null チェック共通化の実装
-   [x] Thread.Sleep の非同期化の実装
-   [x] 全テスト（C#: 88 件 + GUT: 88 件 = 176 件）の成功確認
-   [x] コードの一貫性と保守性の向上確認
