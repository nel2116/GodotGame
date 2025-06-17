---
title: Common System API Reference
version: 0.1
status: draft
updated: 2024-03-21
tags:
    - API
    - Common
    - Systems
    - Reference
---

# Common System API Reference

## 目次

1. [概要](#概要)
2. [State System](#state-system)
3. [Resource System](#resource-system)
4. [Movement System](#movement-system)
5. [Event System](#event-system)
6. [制限事項](#制限事項)
7. [変更履歴](#変更履歴)

## 概要

Common System は、ゲームの基本的な機能を提供する共通システム群です。以下の主要なサブシステムで構成されています：

-   State System: ゲームオブジェクトの状態管理
-   Resource System: リソースの管理とプーリング
-   Movement System: 移動と位置の制御
-   Event System: システム間のイベント通信

## State System

### インターフェース

#### IStateSystem

```csharp
public interface IStateSystem
{
    void Initialize();
    void Update();
    void Cleanup();
}
```

### 主要クラス

#### CommonStateModel

状態のデータモデルを管理します。

#### CommonStateView

状態の視覚的表現を担当します。

#### CommonStateViewModel

Model と View の間のデータバインディングを管理します。

## Resource System

### インターフェース

#### IResourceSystem

```csharp
public interface IResourceSystem
{
    void Initialize();
    void Update();
    void Cleanup();
}
```

### 主要クラス

#### ResourceData

リソースの基本データ構造を定義します。

#### ResourcePool

リソースのプーリング機能を提供します。

#### CommonResourceModel

リソースのデータモデルを管理します。

#### CommonResourceView

リソースの視覚的表現を担当します。

#### CommonResourceViewModel

Model と View の間のデータバインディングを管理します。

## Movement System

### インターフェース

#### IMovementSystem

```csharp
public interface IMovementSystem
{
    void Initialize();
    void Update();
    void Cleanup();
}
```

### 主要クラス

#### CommonMovementModel

移動に関するデータモデルを管理します。

#### CommonMovementView

移動の視覚的表現を担当します。

#### CommonMovementViewModel

Model と View の間のデータバインディングを管理します。

## Event System

### イベントクラス

#### StateEvents

状態変更に関するイベントを定義します。

#### ResourceEvents

リソース操作に関するイベントを定義します。

#### MovementEvents

移動に関するイベントを定義します。

#### CombatEvents

戦闘に関するイベントを定義します。

#### AnimationEvents

アニメーションに関するイベントを定義します。

## 制限事項

1. 各システムは独立して動作しますが、イベントシステムを通じて連携します。
2. システムの初期化順序は重要です。依存関係に注意してください。
3. リソースプールのサイズは適切に設定する必要があります。

## 変更履歴

| バージョン | 更新日     | 変更内容 |
| ---------- | ---------- | -------- |
| 0.1.0      | 2024-03-21 | 初版作成 |
