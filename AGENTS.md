---
title: AI エージェント向けプロジェクト運用ルール
version: 1.0.0
status: active
updated: 2025-01-27
tags:
    - AI
    - Agent
    - Guidelines
    - Development
linked_docs:
    - "[[Docs/99_Reference/ProjectRules.md|プロジェクトルール]]"
    - "[[Docs/08_DocRules/DevelopmentGuidelines.md|開発ガイドライン]]"
    - "[[Docs/03_Technical/12_01_mvvm_rx_architecture.md|MVVM+RXアーキテクチャ]]"
---

# AI エージェント向けプロジェクト運用ルール

## プロジェクト概要

このプロジェクトは **Godot 4.x + C#** を使用したゲーム開発プロジェクトです。
**MVVM + リアクティブプログラミング** アーキテクチャを採用し、保守性と拡張性を重視した設計方針を取っています。

## 技術スタック

- **ゲームエンジン**: Godot 4.x
- **プログラミング言語**: C# (Godot .NET)
- **アーキテクチャ**: MVVM + リアクティブプログラミング
- **テストフレームワーク**:
  - C#テスト: NUnit
  - GDScriptテスト: GUT (Godot Unit Test)
- **開発環境**: Cursor + GitHub
- **ドキュメント**: Markdown + YAML メタデータ

## コーディング規約

### 基本原則

- **可読性と保守性を最優先**: クリーンで理解しやすいコードを書く
- **日本語コメント**: すべての関数に日本語で簡潔な説明コメントを付ける
- **再利用性**: 共通化可能な処理は積極的に分離する
- **理由と目的の明確化**: なぜその実装を選んだかをコメントで説明する

### 命名規則

- **クラス名**: パスカルケース (例: `PlayerMovementViewModel`)
- **メソッド名**: パスカルケース (例: `UpdatePlayerPosition`)
- **プロパティ名**: パスカルケース (例: `PlayerHealth`)
- **パブリック変数**: パスカルケース (例: `MaxHealth`)
- **プライベート変数**: キャメルケース (例: `playerHealth`)
- **ローカル変数**: キャメルケース (例: `currentPosition`)
- **定数**: 大文字スネークケース (例: `MAX_HEALTH`)
- **列挙型**: パスカルケース (例: `PlayerState`)
- **列挙型の値**: パスカルケース (例: `Idle`, `Moving`, `Attacking`)
- **インターフェース名**: パスカルケース、Iプレフィックス (例: `IPlayerSystem`)
- **名前空間**: パスカルケース (例: `Player.Systems.Movement`)
- **ファイル名**: クラス名と同じパスカルケース (例: `PlayerMovementViewModel.cs`)

詳細な命名規則は `Docs/08_DocRules/DevelopmentGuidelines.md` を参照してください。

### アーキテクチャ準拠

新規機能やシステム設計を行う際は、以下のアーキテクチャ原則に従ってください：

#### MVVM + リアクティブプログラミング

```csharp
// Model: ビジネスロジックとデータ構造
public class PlayerModel
{
    public ReactiveProperty<int> Health { get; } = new(100);
    public ReactiveProperty<Vector2> Position { get; } = new(Vector2.Zero);

    public void TakeDamage(int amount)
    {
        Health.Value = Math.Max(0, Health.Value - amount);
    }
}

// ViewModel: Modelの状態をView用に変換
public class PlayerViewModel : ViewModelBase
{
    private readonly PlayerModel _model;
    public ReactiveProperty<string> DisplayHealth { get; } = new("");
    public ReactiveProperty<bool> IsDead { get; } = new(false);

    public PlayerViewModel(PlayerModel model)
    {
        _model = model;
        _model.Health.Subscribe(OnHealthChanged).AddTo(this);
    }

    private void OnHealthChanged(int newHealth)
    {
        DisplayHealth.Value = newHealth.ToString();
        IsDead.Value = newHealth <= 0;
    }
}

// View: Godot Node、ViewModelを購読して描画
public partial class PlayerView : Node2D
{
    [Export] private Label _healthLabel;
    private PlayerViewModel _viewModel;

    public override void _Ready()
    {
        _viewModel = new PlayerViewModel(new PlayerModel());
        _viewModel.DisplayHealth.Subscribe(OnDisplayHealthChanged).AddTo(this);
        _viewModel.IsDead.Subscribe(OnDeathStateChanged).AddTo(this);
    }

    private void OnDisplayHealthChanged(string newText)
    {
        _healthLabel.Text = newText;
    }

    private void OnDeathStateChanged(bool isDead)
    {
        Modulate = new Color(Modulate, isDead ? 0.5f : 1.0f);
    }
}
```

詳細なアーキテクチャガイドラインは `Docs/03_Technical/12_01_mvvm_rx_architecture.md` を参照してください。

## ファイル構造

```
Scripts/
├── Core/                    # コアシステム
│   ├── Events/             # イベントシステム
│   ├── Interfaces/         # 共通インターフェース
│   ├── Reactive/           # リアクティブシステム
│   ├── Utilities/          # ユーティリティ
│   └── ViewModels/         # ベースViewModel
├── Systems/                # ゲームシステム
│   ├── Common/             # 共通システム
│   └── Player/             # プレイヤーシステム
└── Main.cs                 # エントリーポイント

Tests/
├── Core/                   # C#テスト (NUnit)
│   ├── Reactive/           # リアクティブシステムテスト
│   ├── Player/             # プレイヤーシステムテスト
│   └── Utilities/          # ユーティリティテスト
└── Performance/            # パフォーマンステスト

Docs/                       # ドキュメント
├── 03_Technical/           # 技術ドキュメント
├── 08_DocRules/            # ドキュメントルール
└── 99_Reference/           # リファレンス
```

## ドキュメント管理

### ドキュメント更新ルール

`Docs/` 以下の Markdown を更新する際は以下の指針に従います：

1. **YAML メタデータ必須**: 各ドキュメントの冒頭に以下を含める
   ```yaml
   ---
   title: ドキュメントタイトル
   version: 0.1.0
   status: draft/active/deprecated
   updated: YYYY-MM-DD
   tags:
       - tag1
       - tag2
   linked_docs:
       - "[[関連ドキュメント1]]"
       - "[[関連ドキュメント2]]"
   ---
   ```

2. **更新日**: `updated` は UTC の `YYYY-MM-DD` 形式で現在の日付を記載
3. **変更履歴**: ドキュメント末尾に変更履歴を記録

詳細なルールは `Docs/99_Reference/DocumentManagementRules.md` を参照してください。

### 主要ドキュメント

- **プロジェクト概要**: `Docs/README.md`
- **技術アーキテクチャ**: `Docs/03_Technical/12_01_mvvm_rx_architecture.md`
- **開発ガイドライン**: `Docs/08_DocRules/DevelopmentGuidelines.md`
- **プロジェクトルール**: `Docs/99_Reference/ProjectRules.md`

## テスト戦略

### テストの種類と使い分け

1. **C#テスト (NUnit)**
   - ビジネスロジックのテスト
   - ViewModel のテスト
   - ユーティリティクラスのテスト
   - パフォーマンステスト

2. **GUT テスト (GDScript)**
   - Godot エンジン機能のテスト
   - シーン統合テスト
   - 入力システムのテスト

### テスト実行

#### C#テスト実行
```bash
# プロジェクトルートで実行
dotnet test Tests/Core/CoreTests.csproj
```

#### GUT テスト実行
```bash
# ヘッドレスモードで実行
godot --headless --path . -s addons/gut/gut_cmdln.gd -gconfig=.gutconfig.json
```

詳細なテスト実行手順は `Docs/99_Reference/GodotTestCommand.md` を参照してください。

### テスト品質基準

- **カバレッジ**: 重要なビジネスロジックは90%以上
- **実行時間**: 単体テストは1秒以内
- **独立性**: テスト間の依存関係を避ける
- **可読性**: テスト名とアサーションは日本語で分かりやすく

## コミットとプルリクエスト

### コミットメッセージ

1. **1行目**: 変更内容の要約
2. **関連Issue**: 必要に応じて `Closes #<番号>` を明記
3. **Prefix ルール**:
   - `feat:` 新機能
   - `fix:` バグ修正
   - `docs:` ドキュメント更新
   - `test:` テスト追加・修正
   - `refactor:` リファクタリング
   - `style:` コードスタイル修正

詳細なフォーマットは `Docs/99_Reference/CommitMessageRules.md` を参照してください。

### プルリクエスト

1. **テンプレート使用**: `Docs/99_Reference/PRTemplate.md` をベースに記入
2. **日本語説明**: 変更点、テスト方法、関連Issueを日本語で説明
3. **関連ドキュメント**: 必要に応じてドキュメントも更新

詳細な手順は `Docs/99_Reference/PullRequestProcedure.md` を参照してください。

## 開発ワークフロー

### 基本的な開発フロー

1. **Issue作成**: 機能要件やバグを明確化
2. **ブランチ作成**: `feature/機能名` または `fix/バグ名`
3. **実装**: MVVM+RXアーキテクチャに従って実装
4. **テスト作成**: 新機能に対応するテストを追加
5. **テスト実行**: 既存テストと新規テストを実行
6. **ドキュメント更新**: 必要に応じてドキュメントを更新
7. **コミット**: 適切なメッセージでコミット
8. **プルリクエスト**: レビュー依頼

詳細なワークフローは `Docs/99_Reference/DevWorkflows.md` を参照してください。

## パフォーマンス最適化

### メモリ管理

- **CompositeDisposable**: リソースの適切な解放
- **オブジェクトプーリング**: 頻繁に生成・破棄されるオブジェクト
- **弱参照**: 循環参照を避ける

### リアクティブシステム最適化

- **必要な更新のみ**: 不要な通知を避ける
- **バッチ処理**: 複数の更新をまとめる
- **デバウンス**: 頻繁な更新を制御

## 制限事項と注意点

### 技術的制限

- **Godot 4.x**: 最新の安定版を使用
- **C# 8.0以上**: モダンなC#機能を活用
- **プラットフォーム**: Windows/Linux/macOS対応

### 開発方針

- **段階的実装**: 大きな機能は小さく分割
- **テスト駆動**: 重要なロジックはテストファースト
- **ドキュメント先行**: 設計を文書化してから実装

## AI エージェント向け特別ガイドライン

### コード生成時の注意点

1. **アーキテクチャ準拠**: MVVM+RXパターンに従う
2. **日本語コメント**: すべての関数に説明を追加
3. **テスト考慮**: テスト可能な設計にする
4. **エラーハンドリング**: 適切な例外処理を含める

### 推奨アプローチ

1. **小さな変更**: 一度に大きな変更は避ける
2. **段階的改善**: 既存コードを段階的に改善
3. **ドキュメント同期**: コード変更とドキュメント更新を同時に
4. **テスト実行**: 変更後は必ずテストを実行

### トラブルシューティング

- **コンパイルエラー**: まず基本的な構文エラーを確認
- **テスト失敗**: 既存テストの失敗原因を特定
- **パフォーマンス問題**: プロファイリングツールを使用
- **アーキテクチャ違反**: 設計ドキュメントを再確認

---

**注意**: このドキュメントは AI エージェントがプロジェクトの開発を支援する際の指針として作成されています。常に最新のプロジェクト状況に合わせて更新してください。
