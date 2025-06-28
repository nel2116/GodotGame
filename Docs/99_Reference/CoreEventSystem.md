---
title: コアイベントシステム
version: 0.1.0
status: draft
updated: 2024-03-21
tags:
    - API
    - Event
    - Core
    - Reactive
linked_docs:
    - "[[ReactiveSystem]]"
    - "[[CommonEventSystem]]"
    - "[[ReactiveProperty]]"
    - "[[CompositeDisposable]]"
---

# コアイベントシステム

## 目次

1. [概要](#概要)
2. [イベント定義](#イベント定義)
3. [主要コンポーネント](#主要コンポーネント)
4. [使用例](#使用例)
5. [制限事項](#制限事項)
6. [変更履歴](#変更履歴)

## 概要

コアイベントシステムは、ゲーム内のイベントを管理する基盤となるシステムです。以下の機能を提供します：

-   イベント発行
-   イベント購読
-   イベントフィルタリング
-   イベントバッファリング

## イベント定義

### IGameEvent

ゲームイベントのインターフェースです。

```csharp
public interface IGameEvent
{
    string EventType { get; }
    DateTime Timestamp { get; }
    object Source { get; }
}
```

### GameEventBase

ゲームイベントの基本クラスです。

```csharp
public abstract class GameEventBase : IGameEvent
{
    public string EventType { get; }
    public DateTime Timestamp { get; }
    public object Source { get; }

    protected GameEventBase(string eventType, object source)
    {
        EventType = eventType;
        Timestamp = DateTime.UtcNow;
        Source = source;
    }
}
```

## 主要コンポーネント

### IGameEventBus

イベントバスのインターフェースです。

```csharp
public interface IGameEventBus
{
    IDisposable Subscribe<T>(Action<T> onNext) where T : IGameEvent;
    void Publish<T>(T gameEvent) where T : IGameEvent;
    void PublishAsync<T>(T gameEvent) where T : IGameEvent;
    void Clear();
}
```

### GameEventBus

イベントバスの実装クラスです。

```csharp
public class GameEventBus : IGameEventBus
{
    private readonly Subject<IGameEvent> _eventSubject;
    private readonly CompositeDisposable _disposables;

    public IDisposable Subscribe<T>(Action<T> onNext) where T : IGameEvent;
    public void Publish<T>(T gameEvent) where T : IGameEvent;
    public void PublishAsync<T>(T gameEvent) where T : IGameEvent;
    public void Clear();
}
```

## 使用例

### イベントの定義と発行

```csharp
public class PlayerDamagedEvent : GameEventBase
{
    public int Damage { get; }
    public int RemainingHealth { get; }

    public PlayerDamagedEvent(object source, int damage, int remainingHealth)
        : base("PlayerDamaged", source)
    {
        Damage = damage;
        RemainingHealth = remainingHealth;
    }
}

public class PlayerController : MonoBehaviour
{
    [SerializeField] private IGameEventBus _eventBus;
    private int _health = 100;

    public void TakeDamage(int damage)
    {
        _health -= damage;
        _eventBus.Publish(new PlayerDamagedEvent(this, damage, _health));
    }
}
```

### イベントの購読

```csharp
public class PlayerUI : MonoBehaviour
{
    [SerializeField] private IGameEventBus _eventBus;
    [SerializeField] private Text _healthText;
    private readonly CompositeDisposable _disposables = new();

    private void Start()
    {
        _eventBus.Subscribe<PlayerDamagedEvent>(OnPlayerDamaged)
            .AddTo(_disposables);
    }

    private void OnPlayerDamaged(PlayerDamagedEvent evt)
    {
        _healthText.text = $"HP: {evt.RemainingHealth}";
    }

    private void OnDestroy()
    {
        _disposables.Dispose();
    }
}
```

## 制限事項

-   スレッドセーフな実装が必要な箇所では、必ず提供されている同期メカニズムを使用してください
-   リソースの解放は適切なタイミングで行ってください
-   イベントの購読は必要最小限に抑えてください
-   非同期処理の実行時は、必ず`PublishAsync`メソッドを使用してください
-   イベントは、必ず`IGameEvent`インターフェースを実装してください
-   イベントの発行は、必ず`IGameEventBus`を通じて行ってください
-   イベントの購読は、必ず`IDisposable`を保持して適切に解放してください

## 変更履歴

| バージョン | 更新日     | 変更内容 |
| ---------- | ---------- | -------- |
| 0.1.0      | 2024-03-21 | 初版作成 |
