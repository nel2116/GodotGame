---
title: MVVM+RXアーキテクチャ
version: 0.2.0
status: draft
updated: 2024-03-23
tags:
    - Core
    - Architecture
    - MVVM
    - Reactive
linked_docs:
    - "[[12_02_basic_design|MVVM+RX基本設計書]]"
    - "[[12_03_detailed_design|MVVM+RX詳細設計書]]"
    - "[[12_04_system_integration|システム間連携]]"
    - "[[12_05_common_utilities|共通ユーティリティ]]"
---

# MVVM+RX アーキテクチャ

## 目次

1. [概要](#1-概要)
2. [アーキテクチャ概要](#2-アーキテクチャ概要)
3. [レイヤー構成](#3-レイヤー構成)
4. [実装ガイドライン](#4-実装ガイドライン)
5. [パフォーマンス最適化](#5-パフォーマンス最適化)
6. [テスト戦略](#6-テスト戦略)
7. [ベストプラクティス](#7-ベストプラクティス)
8. [制限事項](#8-制限事項)
9. [変更履歴](#9-変更履歴)

## 1. 概要

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

## 2. アーキテクチャ概要

## 3. レイヤー構成

### 3.1 各レイヤーの責務

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

### 3.2 リアクティブ処理の対象

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

## 4. 実装ガイドライン

### 4.1 クラス設計

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

```plaintext
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
-   API リファレンス

## 6. パフォーマンス最適化

### 6.1 メモリ管理

-   オブジェクトプーリング
-   リソースの効率的な使用
-   メモリリークの防止

### 6.2 実行速度

-   バッチ処理の活用
-   非同期処理の最適化
-   キャッシュの適切な使用

## 7. テスト戦略

### 7.1 単体テスト

-   ViewModel のロジックテスト
-   Model のビジネスロジックテスト
-   ユーティリティクラスのテスト

### 7.2 統合テスト

-   システム間の連携テスト
-   エンドツーエンドテスト
-   パフォーマンステスト

## 8. 今後の展望

### 8.1 機能拡張

-   新しいパターンの導入
-   既存パターンの改善
-   ツールの開発

### 8.2 ドキュメント整備

-   チュートリアルの充実
-   ベストプラクティスの追加
-   サンプルコードの拡充

## 9. 変更履歴

| バージョン | 更新日     | 変更内容                         |
| ---------- | ---------- | -------------------------------- |
| 0.1.0      | 2024-03-21 | 初版作成                         |
| 0.2.0      | 2024-03-23 | 新しいリンクドキュメントを追加   |
| 0.3.0      | 2024-03-23 | 新規システムのテストケースを追加 |
| 0.4.0      | 2024-03-23 | ネットワークシステムの削除       |
| 0.5.0      | 2024-03-23 | ドキュメントの改善               |
