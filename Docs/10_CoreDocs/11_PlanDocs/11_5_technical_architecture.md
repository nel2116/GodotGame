---
title: 技術アーキテクチャ設計書
version: 0.1.0
status: draft
updated: 2024-03-21
tags:
    - Architecture
    - Technical
    - Core
    - Project
linked_docs:
    - "[[11_1_plan|プロジェクト計画書]]"
    - "[[12_01_mvvm_rx_architecture|MVVM+RXアーキテクチャ]]"
    - "[[00_index|計画ドキュメント]]"
---

# 技術アーキテクチャ設計書

## 1. 概要

### 1.1 目的

本ドキュメントは、Shrine of the Lost Ones の技術アーキテクチャを定義し、以下の目的を達成することを目指します：

-   MVVM + リアクティブプログラミングの適用範囲の明確化
-   ゲームシステムの実装方針の確立
-   開発効率と保守性の向上

### 1.2 適用範囲

-   ゲームコアシステム
-   UI/UX システム
-   データ管理システム
-   イベントシステム

## 2. アーキテクチャ概要

### 2.1 基本構造

#### 2.1.1 レイヤー構成

```
[View Layer]
    ↓
[ViewModel Layer]
    ↓
[Model Layer]
    ↓
[Service Layer]
```

#### 2.1.2 各レイヤーの責務

| レイヤー  | 責務               | 実装例               |
| --------- | ------------------ | -------------------- |
| View      | UI 表示・入力受付  | Godot Node           |
| ViewModel | 状態管理・変換     | ReactiveProperty     |
| Model     | ビジネスロジック   | ゲームロジッククラス |
| Service   | 外部連携・共通機能 | リソース管理         |

### 2.2 リアクティブプログラミングの適用

#### 2.2.1 適用対象

1. **ゲーム状態管理**

    - プレイヤーステータス
    - スキルツリー状態
    - インベントリ

2. **イベント処理**

    - 戦闘イベント
    - スキル発動
    - アイテム使用

3. **UI 更新**
    - ステータス表示
    - スキルツリー表示
    - インベントリ表示

## 3. 実装詳細

### 3.1 コアシステム

#### 3.1.1 プレイヤーシステム

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
```

#### 3.1.2 スキルシステム

```csharp
// Model
public class SkillModel
{
    public ReactiveProperty<bool> IsUnlocked { get; } = new();
    public ReactiveProperty<int> Level { get; } = new();
    public ReactiveProperty<float> Cooldown { get; } = new();
}

// ViewModel
public class SkillViewModel
{
    private readonly SkillModel _model;
    public ReactiveProperty<string> StatusText { get; } = new();
    public ReactiveProperty<bool> IsAvailable { get; } = new();

    public SkillViewModel(SkillModel model)
    {
        _model = model;
        _model.IsUnlocked.Subscribe(UpdateStatus);
        _model.Cooldown.Subscribe(UpdateAvailability);
    }
}
```

### 3.2 データフロー

#### 3.2.1 一方向データフロー

```
[User Input] → [View] → [ViewModel] → [Model] → [Service]
```

#### 3.2.2 イベントバス

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

## 4. 実装ガイドライン

### 4.1 命名規則

-   Model: `[機能名]Model`
-   ViewModel: `[機能名]ViewModel`
-   View: `[機能名]View`
-   Service: `[機能名]Service`

### 4.2 ディレクトリ構造

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
