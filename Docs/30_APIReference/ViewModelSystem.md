---
title: ViewModelシステム
version: 0.1.0
status: draft
updated: 2024-03-21
tags:
    - API
    - Core
    - ViewModel
    - Reactive
linked_docs:
    - "[[ReactiveSystem]]"
    - "[[ReactiveProperty]]"
    - "[[EventSystem]]"
    - "[[CompositeDisposable]]"
    - "[[00_index]]"
---

# ViewModel システム

## 目次

1. [概要](#概要)
2. [インターフェース](#インターフェース)
3. [実装詳細](#実装詳細)
4. [使用方法](#使用方法)
5. [制限事項](#制限事項)
6. [変更履歴](#変更履歴)

## 概要

ViewModel システムは、プレゼンテーションロジックとビジネスロジックを分離し、UI とデータの双方向バインディングを実現するための基盤を提供します。主に以下の機能を提供します：

-   データバインディング
-   コマンド実行
-   イベント処理
-   リソース管理
-   状態管理

## インターフェース

### IViewModel

```csharp
public interface IViewModel : IDisposable
{
    void Initialize();
    void Cleanup();
    IDisposable SubscribeToEvent<TEvent>(Action<TEvent> handler) where TEvent : IGameEvent;
}
```

## 実装詳細

### ViewModelBase

```csharp
public abstract class ViewModelBase : IViewModel
{
    protected readonly IGameEventBus _eventBus;
    protected readonly CompositeDisposable _disposables = new();
    private readonly object _sync_lock = new();
    private bool _is_initialized;
    private bool _is_disposed;

    protected ViewModelBase(IGameEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    public virtual void Initialize()
    {
        lock (_sync_lock)
        {
            if (_is_initialized)
                return;

            OnInitialize();
            _is_initialized = true;
        }
    }

    public virtual void Cleanup()
    {
        lock (_sync_lock)
        {
            if (!_is_initialized)
                return;

            OnCleanup();
            _is_initialized = false;
        }
    }

    protected virtual void OnInitialize() { }
    protected virtual void OnCleanup() { }

    protected IDisposable SubscribeToEvent<TEvent>(Action<TEvent> handler)
        where TEvent : IGameEvent
    {
        var subscription = _eventBus.Subscribe(handler);
        _disposables.Add(subscription);
        return subscription;
    }

    public void Dispose()
    {
        if (_is_disposed)
            return;

        Cleanup();
        _disposables.Dispose();
        _is_disposed = true;
    }
}
```

主な特徴：

-   スレッドセーフな実装
    -   ロックベースの同期化
    -   アトミックな操作保証
-   リソース管理
    -   自動的なリソース解放
    -   循環参照の防止
-   イベント処理
    -   イベントの購読管理
    -   自動的な購読解除
-   状態管理
    -   初期化状態の追跡

```csharp
public abstract class ViewModelBase : IDisposable
{
    protected readonly CompositeDisposable Disposables = new();
    protected readonly IGameEventBus EventBus;
    protected ReactiveProperty<bool> IsBusy { get; }
    public ReactiveProperty<ViewModelState> State { get; }

    protected ViewModelBase(IGameEventBus eventBus);
    public virtual void Dispose();
    protected void AddDisposable(IDisposable disposable);
    protected IDisposable SubscribeToEvent<T>(Action<T> onNext) where T : GameEvent;
    protected ReactiveCommand CreateCommand();
    protected ReactiveCommand<T> CreateCommand<T>();
    protected async Task ExecuteAsync(Func<Task> action);
    protected T GetValue<T>(IReactiveProperty<T> property);
    protected void SetValue<T>(IReactiveProperty<T> property, T value);
    public virtual void Activate();
    public virtual void Deactivate();
    protected virtual void OnActivate();
    protected virtual void OnDeactivate();
}
```

主な特徴：

-   リソース管理の自動化
-   イベント購読の簡易化
-   コマンド生成のヘルパーメソッド
-   非同期処理のサポート
-   アクティブ/非アクティブ状態の管理

### ViewModelState

```csharp
public enum ViewModelState
{
    Initial,
    Active,
    Inactive
}
```

## 使用方法

### 基本的な ViewModel の実装

```csharp
public class PlayerViewModel : ViewModelBase
{
    private readonly ReactiveProperty<int> _health;
    public IReactiveProperty<int> Health => _health;

    public PlayerViewModel(IGameEventBus eventBus) : base(eventBus)
    {
        _health = new ReactiveProperty<int>(100).AddTo(Disposables);
        SubscribeToEvent<PlayerDamagedEvent>(OnPlayerDamaged);
    }

    private void OnPlayerDamaged(PlayerDamagedEvent evt)
    {
        _health.Value -= evt.Damage;
    }

    protected override void OnActivate()
    {
        base.OnActivate();
        // アクティブ化時の処理
    }

    protected override void OnDeactivate()
    {
        base.OnDeactivate();
        // 非アクティブ化時の処理
    }
}
```

### コマンドの実装

```csharp
public class GameViewModel : ViewModelBase
{
    public ReactiveCommand SaveCommand { get; }
    public ReactiveCommand<int> LoadLevelCommand { get; }

    public GameViewModel(IGameEventBus eventBus) : base(eventBus)
    {
        SaveCommand = CreateCommand();
        LoadLevelCommand = CreateCommand<int>();

        SaveCommand.Subscribe(async () =>
        {
            await ExecuteAsync(async () =>
            {
                // セーブ処理
                await SaveGameAsync();
            });
        });

        LoadLevelCommand.Subscribe(async levelId =>
        {
            await ExecuteAsync(async () =>
            {
                // レベル読み込み処理
                await LoadLevelAsync(levelId);
            });
        });
    }
}
```

## 制限事項

1. **リソース管理**

    - すべての IDisposable リソースは Disposables に追加する必要があります
    - 明示的な Dispose 呼び出しは避けてください

2. **スレッドセーフ**

    - すべてのプロパティアクセスはスレッドセーフです
    - 非同期処理は ExecuteAsync を使用してください

3. **イベント処理**

    - イベントの購読は SubscribeToEvent を使用してください
    - イベントハンドラ内での例外は適切に処理してください

4. **状態管理**
    - 状態の変更は Activate/Deactivate メソッドを使用してください
    - 状態に依存する処理は適切な状態チェックを行ってください

## 変更履歴

| バージョン | 更新日     | 変更内容 |
| ---------- | ---------- | -------- |
| 0.1.0      | 2024-03-21 | 初版作成 |
