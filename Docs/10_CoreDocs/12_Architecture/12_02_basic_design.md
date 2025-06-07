---
title: MVVM + リアクティブプログラミング 基本設計書
version: 0.1.0
status: draft
updated: 2024-03-21
tags:
    - Architecture
    - MVVM
    - Reactive
    - Design
    - Core
linked_docs:
    - "[[12_01_mvvm_rx_architecture|MVVM+RXアーキテクチャ]]"
    - "[[11_5_technical_architecture|技術アーキテクチャ設計書]]"
    - "[[99_Reference/DocumentManagementRules|ドキュメント管理ルール]]"
---

# MVVM + リアクティブプログラミング 基本設計書

## 1. 概要

### 1.1 目的

本ドキュメントは、Shrine of the Lost Ones における MVVM + リアクティブプログラミングの基本設計を定義し、以下の目的を達成することを目指します：

-   アーキテクチャの具体的な実装方針の確立
-   開発チーム間での実装の一貫性確保
-   保守性と拡張性の高いコードベースの構築

### 1.2 適用範囲

-   ゲームコアシステム
-   UI/UX システム
-   データ管理システム
-   イベントシステム

## 2. 基本設計

### 2.1 レイヤー構造

#### 2.1.1 レイヤー構成

```
[View Layer (Godot Node)]
    ↓
[ViewModel Layer (ReactiveProperty)]
    ↓
[Model Layer (Business Logic)]
    ↓
[Service Layer (External Resources)]
```

#### 2.1.2 各レイヤーの責務

| レイヤー  | 責務               | 実装例               |
| --------- | ------------------ | -------------------- |
| View      | UI 表示・入力受付  | Godot Node           |
| ViewModel | 状態管理・変換     | ReactiveProperty     |
| Model     | ビジネスロジック   | ゲームロジッククラス |
| Service   | 外部連携・共通機能 | リソース管理         |

### 2.2 データフロー

#### 2.2.1 一方向データフロー

```
[User Input] → [View] → [ViewModel] → [Model] → [Service]
```

#### 2.2.2 双方向バインディング

-   フォーム入力などの特定のケースでのみ使用
-   基本的には一方向データフローを推奨

## 3. 実装ガイドライン

### 3.1 命名規則

#### 3.1.1 クラス命名

-   Model: `[機能名]Model`
-   ViewModel: `[機能名]ViewModel`
-   View: `[機能名]View`
-   Service: `[機能名]Service`

#### 3.1.2 プロパティ命名

-   リアクティブプロパティ: `[プロパティ名]`
-   通常のプロパティ: `[プロパティ名]`
-   プライベートフィールド: `_[フィールド名]`

### 3.2 ディレクトリ構造

```
Assets/
├── Scripts/
│   ├── Models/
│   │   ├── Player/
│   │   ├── Skills/
│   │   └── Items/
│   ├── ViewModels/
│   │   ├── Player/
│   │   ├── Skills/
│   │   └── UI/
│   ├── Views/
│   │   ├── Player/
│   │   ├── Skills/
│   │   └── UI/
│   └── Services/
│       ├── Resource/
│       ├── Save/
│       └── Event/
```

## 4. 実装例

### 4.1 基本クラス構造

```csharp
// Model
public class PlayerModel
{
    public ReactiveProperty<float> Health { get; } = new();
    public ReactiveProperty<float> MaxHealth { get; } = new();
    public ReactiveProperty<int> ShadowFragments { get; } = new();
}

// ViewModel
public class PlayerViewModel
{
    private readonly PlayerModel _model;
    public ReactiveProperty<string> HealthText { get; } = new();
    public ReactiveProperty<float> HealthPercentage { get; } = new();

    public PlayerViewModel(PlayerModel model)
    {
        _model = model;
        _model.Health.Subscribe(UpdateHealthDisplay);
    }

    private void UpdateHealthDisplay(float health)
    {
        HealthText.Value = $"HP: {health}/{_model.MaxHealth.Value}";
        HealthPercentage.Value = health / _model.MaxHealth.Value;
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

### 4.2 イベント処理

```csharp
public static class GameEventBus
{
    public static Subject<GameEvent> Events { get; } = new();

    public static void Publish(GameEvent gameEvent)
    {
        Events.OnNext(gameEvent);
    }
}
```

## 5. パフォーマンス最適化

### 5.1 メモリ管理

-   不要なサブスクリプションの解除
-   オブジェクトプーリングの活用
-   リソースの適切な解放

### 5.2 更新最適化

-   バッチ処理の実装
-   更新頻度の制御
-   不要な更新の防止

## 6. テスト戦略

### 6.1 単体テスト

-   Model: ビジネスロジックの検証
-   ViewModel: 状態変換の検証
-   Service: 機能の検証

### 6.2 統合テスト

-   レイヤー間の連携
-   イベント処理
-   データフロー

## 7. 変更履歴

| バージョン | 更新日     | 変更内容 |
| ---------- | ---------- | -------- |
| 0.1.0      | 2024-03-21 | 初版作成 |
