---
title: EventBus実装詳細
version: 0.2.0
status: draft
updated: 2024-03-23
tags:
    - Core
    - Event
    - Bus
    - Implementation
linked_docs:
    - "[[12_01_mvvm_rx_architecture|MVVM+RXアーキテクチャ]]"
    - "[[12_02_basic_design|MVVM+RX基本設計書]]"
    - "[[12_04_system_integration|システム間連携]]"
---

# EventBus 実装詳細

## 目次

1. [概要](#1-概要)
2. [基本実装](#2-基本実装)
3. [拡張機能](#3-拡張機能)
4. [パフォーマンス最適化](#4-パフォーマンス最適化)
5. [使用例](#5-使用例)
6. [ベストプラクティス](#6-ベストプラクティス)
7. [テスト](#7-テスト)
8. [制限事項](#8-制限事項)
9. [変更履歴](#9-変更履歴)

## 1. 概要

イベントバスは、システム間の疎結合な通信を実現するためのメッセージングシステムです。Reactive Extensions (Rx) を活用し、イベントの購読と発行を効率的に管理します。

## 2. 基本実装

### 2.1 インターフェース定義

```csharp
public interface IEventBus
{
    void Publish<T>(T event) where T : IEvent;
    IObservable<T> GetEventStream<T>() where T : IEvent;
    void Dispose();
}
```

### 2.2 基本実装

```csharp
public class EventBus : IEventBus, IDisposable
{
    private readonly Subject<IEvent> _subject = new();
    private readonly Dictionary<Type, object> _streams = new();
    private bool _disposed;

    public void Publish<T>(T event) where T : IEvent
    {
        if (_disposed) return;
        _subject.OnNext(event);
    }

    public IObservable<T> GetEventStream<T>() where T : IEvent
    {
        if (_disposed) return Observable.Empty<T>();

        var type = typeof(T);
        if (!_streams.TryGetValue(type, out var stream))
        {
            stream = _subject.OfType<T>().Publish().RefCount();
            _streams[type] = stream;
        }

        return (IObservable<T>)stream;
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _subject.Dispose();
            _streams.Clear();
            _disposed = true;
        }
    }
}
```

## 3. 拡張機能

### 3.1 イベントフィルタリング

```csharp
public class EventBus : IEventBus, IDisposable
{
    public IObservable<T> GetFilteredEventStream<T>(Func<T, bool> predicate) where T : IEvent
    {
        return GetEventStream<T>().Where(predicate);
    }
}
```

### 3.2 イベントバッファリング

```csharp
public class EventBus : IEventBus, IDisposable
{
    public IObservable<IList<T>> GetBufferedEventStream<T>(TimeSpan bufferTime) where T : IEvent
    {
        return GetEventStream<T>().Buffer(bufferTime);
    }
}
```

## 4. パフォーマンス最適化

### 4.1 メモリ管理

```csharp
public class EventBus : IEventBus, IDisposable
{
    private readonly CompositeDisposable _disposables = new();

    public void Dispose()
    {
        if (!_disposed)
        {
            _disposables.Dispose();
            _subject.Dispose();
            _streams.Clear();
            _disposed = true;
        }
    }
}
```

### 4.2 スレッドセーフ

```csharp
public class EventBus : IEventBus, IDisposable
{
    private readonly object _lock = new();

    public void Publish<T>(T event) where T : IEvent
    {
        if (_disposed) return;
        lock (_lock)
        {
            _subject.OnNext(event);
        }
    }
}
```

## 5. 使用例

### 5.1 ゲームイベントの処理

```csharp
public class GameEventBus
{
    private readonly IEventBus _eventBus;

    public GameEventBus(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public void PublishPlayerDamage(float amount)
    {
        _eventBus.Publish(new PlayerDamageEvent(amount));
    }

    public IObservable<PlayerDamageEvent> GetPlayerDamageStream()
    {
        return _eventBus.GetEventStream<PlayerDamageEvent>();
    }
}
```

### 5.2 イベントの購読

```csharp
public class PlayerViewModel : ViewModelBase
{
    public PlayerViewModel(IEventBus eventBus)
    {
        Disposables.Add(
            eventBus.GetEventStream<PlayerDamageEvent>()
                .Subscribe(OnPlayerDamage)
        );
    }

    private void OnPlayerDamage(PlayerDamageEvent evt)
    {
        Health.Value -= evt.Amount;
    }
}
```

## 6. ベストプラクティス

### 6.1 イベント設計

-   イベントは不変（immutable）にする
-   イベント名は過去形で命名する
-   必要な情報のみを含める

### 6.2 パフォーマンス

-   不要なイベント発行を避ける
-   適切なバッファリングを使用する
-   メモリリークに注意する

### 6.3 エラーハンドリング

-   イベント処理の例外を適切に処理する
-   デッドロックを防ぐ
-   リソースの解放を確実に行う

## 7. テスト

### 7.1 単体テスト

```csharp
[Test]
public void EventBus_Publish_NotifiesSubscribers()
{
    var eventBus = new EventBus();
    var received = false;

    eventBus.GetEventStream<TestEvent>()
        .Subscribe(_ => received = true);

    eventBus.Publish(new TestEvent());

    Assert.That(received, Is.True);
}
```

### 7.2 統合テスト

```csharp
[Test]
public async Task EventBus_AsyncOperation_ProcessesEventsCorrectly()
{
    var eventBus = new EventBus();
    var results = new List<int>();

    eventBus.GetEventStream<TestEvent>()
        .Select(e => e.Value)
        .Subscribe(v => results.Add(v));

    await Task.WhenAll(
        Task.Run(() => eventBus.Publish(new TestEvent(1))),
        Task.Run(() => eventBus.Publish(new TestEvent(2)))
    );

    Assert.That(results, Is.EquivalentTo(new[] { 1, 2 }));
}
```

## 8. 制限事項

### 8.1 メモリ管理

-   適切なタイミングで Dispose を呼び出す必要がある
-   循環参照に注意が必要
-   大量のイベントは避ける

### 8.2 パフォーマンス

-   イベントの頻度に注意
-   重い処理は非同期で実行する
-   バッファリングを活用する

### 8.3 スレッドセーフ

-   マルチスレッド環境での使用には注意が必要
-   適切な同期処理を実装する
-   イベントの順序保証に注意

## 9. 変更履歴

| バージョン | 更新日     | 変更内容                                                                                                             |
| ---------- | ---------- | -------------------------------------------------------------------------------------------------------------------- |
| 0.2.0      | 2024-03-23 | パフォーマンス最適化の追加<br>- メモリ管理の改善<br>- スレッドセーフティの強化<br>- イベントバッファリング機能の追加 |
| 0.1.0      | 2024-03-22 | 初版作成<br>- 基本実装の定義<br>- イベントフィルタリング機能の追加<br>- 使用例の追加                                 |
