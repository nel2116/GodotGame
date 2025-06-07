# Godot + C# における MVVM + リアクティブプログラミング アーキテクチャ

## 1. 設計思想

### 1.1 MVVM とリアクティブプログラミングの統合

#### 基本概念

-   **MVVM (Model-View-ViewModel)**
    -   Model: ゲームのビジネスロジックとデータ構造
    -   View: Godot の Node をベースとした UI/ゲームオブジェクト
    -   ViewModel: Model と View の間の状態管理と変換層

#### リアクティブプログラミングの位置づけ

-   **データストリームとしてのゲーム状態**
    -   プレイヤーの入力
    -   ゲーム内のイベント
    -   時間経過
    -   物理演算の結果
        これらを全て Observable なストリームとして扱う

#### 統合のメリット

1. **状態管理の一元化**

    - 複雑なゲーム状態を宣言的に記述可能
    - 副作用の分離と制御が容易

2. **テスト容易性**

    - ViewModel のロジックを独立してテスト可能
    - モックやスタブの作成が容易

3. **保守性の向上**
    - 責務の明確な分離
    - コードの再利用性向上

### 1.2 ゲーム開発における有効性

#### リアルタイム処理との親和性

-   フレームレートに依存しないロジック実装
-   非同期処理の簡潔な記述
-   イベント駆動型のゲームロジック実装

#### パフォーマンス最適化

-   必要な更新のみを実行
-   メモリ使用量の最適化
-   バッチ処理の実装が容易

### 1.3 OOP との関係性

#### 共存アプローチ

-   **クラスベースの構造化**
    -   基本的なクラス設計は OOP の原則に従う
    -   継承とコンポジションの適切な使い分け

#### 思想的な違い

-   **命令型 vs 宣言型**
    -   OOP: 状態の変更を明示的に記述
    -   リアクティブ: 状態の変化をストリームとして扱う

## 2. レイヤー構造

### 2.1 各レイヤーの責務

#### Model

-   ゲームのビジネスロジック
-   データ構造の定義
-   外部サービスとの通信
-   永続化処理

#### ViewModel

-   Model の状態を View 用に変換
-   ユーザー入力の処理
-   状態の一時的な保持
-   バリデーションロジック

#### View

-   UI の表示
-   ユーザー入力の受付
-   アニメーション制御
-   サウンド再生

#### Service Layer

-   外部リソースの管理
-   共通機能の提供
-   依存性の注入

### 2.2 リアクティブ処理の対象

#### 非同期イベント

-   ネットワーク通信
-   ファイル I/O
-   外部 API 呼び出し

#### 時間ベース処理

-   アニメーション
-   パーティクルエフェクト
-   ゲームループ

#### ステート駆動アニメーション

-   キャラクターの状態遷移
-   UI のトランジション
-   エフェクトの制御

## 3. 構成単位と粒度

### 3.1 クラス設計

#### 命名規則

```
[機能名][レイヤー名]
例：
- PlayerModel
- GameStateViewModel
- MainMenuView
```

#### モジュール分割

-   機能ごとのディレクトリ構造
-   共通コンポーネントの分離
-   テストコードの配置

### 3.2 データフロー

#### 一方向データフロー

```
View → ViewModel → Model
```

#### 双方向バインディング

-   限定的な使用を推奨
-   フォーム入力など特定のケースでのみ使用

## 4. アーキテクチャパターン

### 4.1 プロジェクト構造

```
Assets/
├── Scripts/
│   ├── Models/
│   ├── ViewModels/
│   ├── Views/
│   ├── Services/
│   └── Common/
├── Scenes/
├── Resources/
└── Tests/
```

### 4.2 依存関係

-   ViewModel は Model に依存
-   View は ViewModel に依存
-   Service は共通レイヤーとして機能

## 5. 開発フロー

### 5.1 最小構成の実装

#### 基本クラス

```csharp
// Model
public class PlayerModel
{
    public ReactiveProperty<float> Health { get; } = new();
    public ReactiveProperty<Vector2> Position { get; } = new();
}

// ViewModel
public class PlayerViewModel
{
    private readonly PlayerModel _model;
    public ReactiveProperty<string> HealthText { get; } = new();

    public PlayerViewModel(PlayerModel model)
    {
        _model = model;
        _model.Health.Subscribe(health =>
            HealthText.Value = $"HP: {health}");
    }
}

// View
public partial class PlayerView : Node2D
{
    private PlayerViewModel _viewModel;

    public override void _Ready()
    {
        _viewModel = new PlayerViewModel(new PlayerModel());
        _viewModel.HealthText.Subscribe(text =>
            GetNode<Label>("HealthLabel").Text = text);
    }
}
```

### 5.2 チーム開発ガイドライン

#### コーディング規約

-   命名規則の統一
-   コメントの書き方
-   コミットメッセージの形式

#### ドキュメント

-   クラス設計書
-   シーケンス図
-   API 仕様書

### 5.3 テスト戦略

#### 単体テスト

-   Model: ビジネスロジックの検証
-   ViewModel: 状態変換の検証
-   View: UI 表示の検証

#### 統合テスト

-   レイヤー間の連携
-   エンドツーエンドの動作確認

## 6. 実装例

### 6.1 基本的な実装パターン

```csharp
// リアクティブプロパティの定義
public class ReactiveProperty<T>
{
    private T _value;
    private readonly Subject<T> _subject = new();

    public T Value
    {
        get => _value;
        set
        {
            _value = value;
            _subject.OnNext(value);
        }
    }

    public IDisposable Subscribe(Action<T> onNext)
    {
        return _subject.Subscribe(onNext);
    }
}

// ViewModelでの使用例
public class GameViewModel
{
    public ReactiveProperty<int> Score { get; } = new();
    public ReactiveProperty<bool> IsGameOver { get; } = new();

    public void UpdateScore(int points)
    {
        Score.Value += points;
        if (Score.Value >= 100)
        {
            IsGameOver.Value = true;
        }
    }
}
```

### 6.2 非同期処理の実装

```csharp
public class GameService
{
    public IObservable<GameState> LoadGameState()
    {
        return Observable.FromAsync(async () =>
        {
            // 非同期でゲーム状態を読み込む
            var state = await LoadStateFromFile();
            return state;
        });
    }
}
```

## 7. ベストプラクティス

### 7.1 パフォーマンス最適化

-   不要なサブスクリプションの解除
-   バッチ処理の活用
-   メモリリークの防止

### 7.2 エラーハンドリング

-   例外の適切な処理
-   エラー状態の伝播
-   リカバリー処理の実装

### 7.3 デバッグ

-   ログ出力の戦略
-   状態の可視化
-   パフォーマンスプロファイリング
