---
title: 共通インターフェース仕様
version: 1.0.0
status: draft
updated: 2024-03-21
tags:
    - API
    - Interface
    - Core
---

# 共通インターフェース仕様

## 目次

1. [GameState](#gamestate)
2. [IEventBus](#ieventbus)
3. [IRecoveryStrategy](#irecoverystrategy)
4. [その他の共通インターフェース](#その他の共通インターフェース)

## GameState

### 概要

ゲームの状態を管理するための基本インターフェースです。

### 定義

```csharp
public interface IGameState
{
    GameStateType CurrentState { get; }
    void TransitionTo(GameStateType newState);
    bool CanTransitionTo(GameStateType targetState);
}
```

### 責務

-   ゲームの現在の状態を保持
-   状態遷移の制御
-   状態遷移の妥当性検証

### 使用例

```csharp
public class GameStateManager : IGameState
{
    private GameStateType _currentState;

    public GameStateType CurrentState => _currentState;

    public void TransitionTo(GameStateType newState)
    {
        if (CanTransitionTo(newState))
        {
            _currentState = newState;
            // 状態変更時の処理
        }
    }
}
```

## IEventBus

### 概要

イベント駆動型の通信を実現するためのイベントバスインターフェースです。

### 定義

```csharp
public interface IEventBus
{
    void Subscribe<T>(Action<T> handler) where T : IEvent;
    void Unsubscribe<T>(Action<T> handler) where T : IEvent;
    void Publish<T>(T event) where T : IEvent;
}
```

### 責務

-   イベントの購読管理
-   イベントの発行
-   イベントハンドラの登録解除

### 使用例

```csharp
public class GameEventBus : IEventBus
{
    private readonly Dictionary<Type, List<Delegate>> _handlers = new();

    public void Subscribe<T>(Action<T> handler) where T : IEvent
    {
        // 実装
    }
}
```

## IRecoveryStrategy

### 概要

エラー発生時の回復戦略を定義するインターフェースです。

### 定義

```csharp
public interface IRecoveryStrategy
{
    Task<bool> AttemptRecovery(Exception error);
    bool CanHandle(Exception error);
}
```

### 責務

-   エラー状態の回復処理
-   回復可能なエラーの判定
-   回復処理の結果報告

### 使用例

```csharp
public class DefaultRecoveryStrategy : IRecoveryStrategy
{
    public async Task<bool> AttemptRecovery(Exception error)
    {
        // 回復処理の実装
        return true;
    }
}
```

## その他の共通インターフェース

### ISaveable

セーブ可能なオブジェクトを定義するインターフェース

### IInitializable

初期化可能なオブジェクトを定義するインターフェース

### IDisposable

リソース解放が必要なオブジェクトを定義するインターフェース
