---
title: システム間連携
version: 0.2.0
status: draft
updated: 2024-03-23
tags:
    - Core
    - System
    - Integration
    - Architecture
linked_docs:
    - "[[12_01_mvvm_rx_architecture|MVVM+RXアーキテクチャ]]"
    - "[[12_02_basic_design|MVVM+RX基本設計書]]"
    - "[[12_03_detailed_design/01_core_components/04_event_bus|EventBus実装詳細]]"
---

# システム間連携

## 目次

1. [概要](#1-概要)
2. [コアシステム](#2-コアシステム)
3. [連携パターン](#3-連携パターン)
4. [エラー処理](#4-エラー処理)
5. [パフォーマンス最適化](#5-パフォーマンス最適化)
6. [テスト](#6-テスト)
7. [ベストプラクティス](#7-ベストプラクティス)
8. [制限事項](#8-制限事項)
9. [変更履歴](#9-変更履歴)

## 1. 概要

本ドキュメントは、ゲームシステム間の連携方法と実装パターンを定義します。各システムは独立して動作しつつ、必要な情報を効率的に共有・連携できる設計を目指します。

## 2. コアシステム

### 2.1 GameManager

```csharp
public class GameManager : IDisposable
{
    private readonly IEventBus _eventBus;
    private readonly CompositeDisposable _disposables = new();

    public GameManager(IEventBus eventBus)
    {
        _eventBus = eventBus;
        InitializeSystems();
    }

    private void InitializeSystems()
    {
        // システムの初期化と連携の設定
        var inputSystem = new InputSystem(_eventBus);
        var stateManager = new StateManager(_eventBus);
        var resourceManager = new ResourceManager(_eventBus);

        // システム間の連携を設定
        SetupSystemConnections(inputSystem, stateManager, resourceManager);
    }

    private void SetupSystemConnections(
        InputSystem inputSystem,
        StateManager stateManager,
        ResourceManager resourceManager)
    {
        // 入力システムと状態管理システムの連携
        _disposables.Add(
            inputSystem.GetInputStream()
                .Subscribe(input => stateManager.HandleInput(input))
        );

        // 状態管理システムとリソース管理システムの連携
        _disposables.Add(
            stateManager.GetStateStream()
                .Subscribe(state => resourceManager.HandleStateChange(state))
        );
    }
}
```

### 2.2 EventSystem

```csharp
public class EventSystem : IEventBus
{
    private readonly Subject<IEvent> _subject = new();
    private readonly Dictionary<Type, object> _streams = new();

    public void Publish<T>(T event) where T : IEvent
    {
        _subject.OnNext(event);
    }

    public IObservable<T> GetEventStream<T>() where T : IEvent
    {
        var type = typeof(T);
        if (!_streams.TryGetValue(type, out var stream))
        {
            stream = _subject.OfType<T>().Publish().RefCount();
            _streams[type] = stream;
        }
        return (IObservable<T>)stream;
    }
}
```

### 2.3 StateManager

```csharp
public class StateManager
{
    private readonly IEventBus _eventBus;
    private readonly ReactiveProperty<GameState> _currentState = new(GameState.Initial);

    public StateManager(IEventBus eventBus)
    {
        _eventBus = eventBus;
        SetupStateTransitions();
    }

    private void SetupStateTransitions()
    {
        _currentState.ValueChanged
            .Subscribe(state => _eventBus.Publish(new GameStateChangedEvent(state)));
    }

    public void HandleInput(InputEvent input)
    {
        // 入力に基づいて状態を更新
        var nextState = DetermineNextState(_currentState.Value, input);
        _currentState.Value = nextState;
    }
}
```

### 2.4 ResourceManager

```csharp
public class ResourceManager
{
    private readonly IEventBus _eventBus;
    private readonly Dictionary<string, object> _resources = new();

    public ResourceManager(IEventBus eventBus)
    {
        _eventBus = eventBus;
        SetupResourceHandling();
    }

    private void SetupResourceHandling()
    {
        _eventBus.GetEventStream<ResourceRequestEvent>()
            .Subscribe(HandleResourceRequest);
    }

    public void HandleStateChange(GameState state)
    {
        // 状態に応じてリソースを管理
        switch (state)
        {
            case GameState.Loading:
                PreloadResources();
                break;
            case GameState.Playing:
                LoadGameResources();
                break;
        }
    }
}
```

## 3. システム間連携パターン

### 3.1 イベント駆動連携

```csharp
public class SystemConnector
{
    private readonly IEventBus _eventBus;

    public SystemConnector(IEventBus eventBus)
    {
        _eventBus = eventBus;
        SetupEventConnections();
    }

    private void SetupEventConnections()
    {
        // システムAからシステムBへの連携
        _eventBus.GetEventStream<SystemAEvent>()
            .Subscribe(evt => HandleSystemAEvent(evt));

        // システムBからシステムCへの連携
        _eventBus.GetEventStream<SystemBEvent>()
            .Subscribe(evt => HandleSystemBEvent(evt));
    }
}
```

### 3.2 状態共有連携

```csharp
public class SharedStateManager
{
    private readonly ReactiveProperty<SharedState> _state = new();
    private readonly List<IDisposable> _subscriptions = new();

    public void ConnectSystem(ISystem system)
    {
        _subscriptions.Add(
            _state.ValueChanged
                .Subscribe(state => system.UpdateState(state))
        );
    }

    public void UpdateState(SharedState newState)
    {
        _state.Value = newState;
    }
}
```

## 4. エラーハンドリング

### 4.1 システム間エラー処理

```csharp
public class SystemErrorHandler
{
    private readonly IEventBus _eventBus;
    private readonly ILogger _logger;

    public SystemErrorHandler(IEventBus eventBus, ILogger logger)
    {
        _eventBus = eventBus;
        _logger = logger;
        SetupErrorHandling();
    }

    private void SetupErrorHandling()
    {
        _eventBus.GetEventStream<SystemErrorEvent>()
            .Subscribe(HandleSystemError);
    }

    private void HandleSystemError(SystemErrorEvent error)
    {
        _logger.LogError($"System error: {error.Message}");
        // エラー回復処理
        RecoverFromError(error);
    }
}
```

### 4.2 リカバリー処理

```csharp
public class SystemRecovery
{
    private readonly IEventBus _eventBus;
    private readonly Dictionary<SystemType, IRecoveryStrategy> _strategies;

    public SystemRecovery(IEventBus eventBus)
    {
        _eventBus = eventBus;
        _strategies = InitializeRecoveryStrategies();
    }

    private void RecoverFromError(SystemErrorEvent error)
    {
        if (_strategies.TryGetValue(error.SystemType, out var strategy))
        {
            strategy.Recover(error);
        }
    }
}
```

## 5. パフォーマンス最適化

### 5.1 メモリ管理

```csharp
public class SystemMemoryManager
{
    private readonly IEventBus _eventBus;
    private readonly Dictionary<SystemType, MemoryProfile> _profiles;

    public SystemMemoryManager(IEventBus eventBus)
    {
        _eventBus = eventBus;
        _profiles = InitializeMemoryProfiles();
        SetupMemoryMonitoring();
    }

    private void SetupMemoryMonitoring()
    {
        _eventBus.GetEventStream<MemoryWarningEvent>()
            .Subscribe(HandleMemoryWarning);
    }
}
```

### 5.2 更新最適化

```csharp
public class SystemUpdateOptimizer
{
    private readonly IEventBus _eventBus;
    private readonly Dictionary<SystemType, UpdateStrategy> _strategies;

    public SystemUpdateOptimizer(IEventBus eventBus)
    {
        _eventBus = eventBus;
        _strategies = InitializeUpdateStrategies();
        SetupUpdateOptimization();
    }

    private void SetupUpdateOptimization()
    {
        _eventBus.GetEventStream<SystemUpdateEvent>()
            .Subscribe(OptimizeUpdate);
    }
}
```

## 6. テスト

### 6.1 単体テスト

```csharp
[Test]
public void SystemConnector_ConnectsSystems_Correctly()
{
    var eventBus = new EventBus();
    var connector = new SystemConnector(eventBus);
    var systemA = new Mock<ISystem>();
    var systemB = new Mock<ISystem>();

    connector.ConnectSystems(systemA.Object, systemB.Object);
    eventBus.Publish(new SystemAEvent());

    systemA.Verify(s => s.HandleEvent(It.IsAny<SystemAEvent>()));
    systemB.Verify(s => s.HandleEvent(It.IsAny<SystemBEvent>()));
}
```

### 6.2 統合テスト

```csharp
[Test]
public async Task SystemIntegration_HandlesComplexScenario()
{
    var eventBus = new EventBus();
    var gameManager = new GameManager(eventBus);
    var results = new List<string>();

    eventBus.GetEventStream<GameStateChangedEvent>()
        .Subscribe(evt => results.Add(evt.State.ToString()));

    await gameManager.StartGame();
    await gameManager.PauseGame();
    await gameManager.ResumeGame();

    Assert.That(results, Is.EquivalentTo(new[]
    {
        "Loading",
        "Playing",
        "Paused",
        "Playing"
    }));
}
```

## 7. ベストプラクティス

### 7.1 システム設計

-   システム間の依存関係を最小限に保つ
-   明確な責任分担を設定する
-   拡張性を考慮した設計を行う

### 7.2 パフォーマンス

-   不要な通信を避ける
-   適切なキャッシュ戦略を採用する
-   リソース使用量を監視する

### 7.3 メンテナンス性

-   ログを適切に記録する
-   エラー処理を統一する
-   ドキュメントを維持する

## 8. 制限事項

### 8.1 メモリ管理

-   システム間の参照に注意
-   リソースの適切な解放
-   循環参照の防止

### 8.2 パフォーマンス

-   システム間の通信コスト
-   更新頻度の最適化
-   リソース使用量の制御

### 8.3 スレッドセーフ

-   マルチスレッド環境での同期
-   デッドロックの防止
-   スレッド間通信の制御

## 9. 変更履歴

| バージョン | 更新日     | 変更内容                                                                                         |
| ---------- | ---------- | ------------------------------------------------------------------------------------------------ |
| 0.2.0      | 2024-03-23 | 機能拡張<br>- システム間連携パターンの追加<br>- エラー処理の強化<br>- パフォーマンス最適化の追加 |
| 0.1.0      | 2024-03-21 | 初版作成<br>- 基本設計の追加<br>- コアシステムの定義<br>- 連携方法の定義                         |
