---
title: State System API Reference
version: 0.1
status: draft
updated: 2024-03-21
tags:
    - API
    - State
    - Systems
    - Reference
---

# State System API Reference

## 目次

1. [概要](#概要)
2. [インターフェース](#インターフェース)
3. [主要クラス](#主要クラス)
4. [使用方法](#使用方法)
5. [制限事項](#制限事項)
6. [変更履歴](#変更履歴)

## 概要

State System は、ゲームオブジェクトの状態管理を担当するシステムです。MVVM パターンに基づいて実装されており、状態の変更を効率的に管理し、UI との連携を提供します。

## インターフェース

### IStateSystem

```csharp
public interface IStateSystem
{
    void Initialize();
    void Update();
    void Cleanup();
}
```

#### メソッド

-   `Initialize()`: システムの初期化を行います
-   `Update()`: システムの状態を更新します
-   `Cleanup()`: システムのリソースを解放します

## 主要クラス

### CommonStateModel

状態のデータモデルを管理するクラスです。

```csharp
public class CommonStateModel
{
    // 状態データの管理
    // 状態の変更通知
    // 状態の永続化
}
```

### CommonStateView

状態の視覚的表現を担当するクラスです。

```csharp
public class CommonStateView
{
    // UI要素の更新
    // アニメーション制御
    // 視覚的フィードバック
}
```

### CommonStateViewModel

Model と View の間のデータバインディングを管理するクラスです。

```csharp
public class CommonStateViewModel
{
    // データバインディング
    // コマンド処理
    // 状態変更の通知
}
```

## 使用方法

### 1. システムの初期化

```csharp
var stateSystem = new CommonStateViewModel();
stateSystem.Initialize();
```

### 2. 状態の更新

```csharp
// 状態の変更
stateSystem.UpdateState(newState);

// 状態の取得
var currentState = stateSystem.GetCurrentState();
```

### 3. イベントの購読

```csharp
// 状態変更イベントの購読
stateSystem.OnStateChanged += HandleStateChanged;
```

## 制限事項

1. 状態の変更は必ず ViewModel を通じて行う必要があります
2. 状態の永続化は Model クラスで管理されます
3. 複数の状態を同時に変更する場合は、適切な同期処理が必要です

## 変更履歴

| バージョン | 更新日     | 変更内容 |
| ---------- | ---------- | -------- |
| 0.1.0      | 2024-03-21 | 初版作成 |
