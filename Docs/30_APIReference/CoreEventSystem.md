---
title: Core Event System API Reference
version: 0.1
status: draft
updated: 2024-03-21
tags:
    - API
    - Core
    - Event
    - Systems
    - Reference
---

# Core Event System API Reference

## 目次

1. [概要](#概要)
2. [インターフェース](#インターフェース)
3. [主要クラス](#主要クラス)
4. [使用方法](#使用方法)
5. [制限事項](#制限事項)
6. [変更履歴](#変更履歴)

## 概要

Core Event System は、ゲーム全体で使用される基本的なイベント処理の基盤を提供するシステムです。スレッドセーフな実装と型安全なイベント処理を特徴とし、システム間の疎結合な通信を実現します。

## インターフェース

### IGameEvent

```csharp
public interface IGameEvent
{
    DateTime Timestamp { get; }
}
```

#### プロパティ

-   `Timestamp`: イベントが発生した時刻（UTC）

### IGameEventBus

```csharp
public interface IGameEventBus
{
    void Publish<T>(T evt) where T : GameEvent;
    IObservable<T> GetEventStream<T>() where T : GameEvent;
}
```

#### メソッド

-   `Publish<T>`: イベントを発行します
-   `GetEventStream<T>`: イベントの購読ストリームを取得します

## 主要クラス

### GameEvent

```csharp
public abstract class GameEvent : IGameEvent
{
    public DateTime Timestamp { get; } = DateTime.UtcNow;
}
```

### GameEventBus

```csharp
public class GameEventBus : IGameEventBus
{
    private readonly ConcurrentDictionary<Type, ISubject<GameEvent>> _subjects = new();

    public void Publish<T>(T evt) where T : GameEvent;
    public IObservable<T> GetEventStream<T>() where T : GameEvent;
}
```

## 使用方法

### 1. イベントの定義

```csharp
public class PlayerDamagedEvent : GameEvent
{
    public int Damage { get; }
    public PlayerDamagedEvent(int damage) => Damage = damage;
}

public class PlayerHealedEvent : GameEvent
{
    public int Amount { get; }
    public PlayerHealedEvent(int amount) => Amount = amount;
}
```

### 2. イベントの発行と購読

```csharp
// イベントバスの作成
var eventBus = new GameEventBus();

// イベントの購読
var subscription = eventBus.GetEventStream<PlayerDamagedEvent>()
    .Subscribe(evt => Debug.Log($"Player took {evt.Damage} damage"));

// イベントの発行
eventBus.Publish(new PlayerDamagedEvent(10));

// リソースの解放
subscription.Dispose();
```

### 3. ViewModel での使用例

```csharp
public class PlayerViewModel : ViewModelBase
{
    public PlayerViewModel(IGameEventBus eventBus) : base(eventBus)
    {
        // イベントの購読
        SubscribeToEvent<PlayerDamagedEvent>(OnPlayerDamaged);
        SubscribeToEvent<PlayerHealedEvent>(OnPlayerHealed);
    }

    private void OnPlayerDamaged(PlayerDamagedEvent evt)
    {
        // ダメージ処理
        Health.Value -= evt.Damage;
    }

    private void OnPlayerHealed(PlayerHealedEvent evt)
    {
        // 回復処理
        Health.Value += evt.Amount;
    }
}
```

## 制限事項

1. **スレッドセーフ**

    - イベントの発行と購読はスレッドセーフです
    - イベントハンドラ内での処理は適切に同期化してください
    - 長時間実行されるイベントハンドラは避けてください

2. **メモリ管理**

    - 購読は明示的に解除する必要があります
    - `CompositeDisposable` を使用して複数の購読を管理することを推奨
    - 循環参照に注意してください

3. **パフォーマンス**

    - 大量のイベント発行時は注意が必要です
    - 不要な購読は早期に解除してください
    - イベントハンドラは軽量に保ってください

4. **例外処理**
    - イベントハンドラ内での例外は適切に処理してください
    - イベント発行時の例外は発行元で処理してください
    - 購読処理内での例外は購読者で処理してください

## 変更履歴

| バージョン | 更新日     | 変更内容 |
| ---------- | ---------- | -------- |
| 0.1.0      | 2024-03-21 | 初版作成 |
