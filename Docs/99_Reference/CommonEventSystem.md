---
title: 共通イベントシステム
version: 0.1.0
status: draft
updated: 2024-03-21
tags:
    - API
    - Event
    - Core
    - Reactive
linked_docs:
    - "[[CoreEventSystem]]"
    - "[[ReactiveSystem]]"
    - "[[ReactiveProperty]]"
    - "[[CompositeDisposable]]"
---

# 共通イベントシステム

## 目次

1. [概要](#概要)
2. [イベント定義](#イベント定義)
3. [主要コンポーネント](#主要コンポーネント)
4. [使用例](#使用例)
5. [制限事項](#制限事項)
6. [変更履歴](#変更履歴)

## 概要

共通イベントシステムは、ゲーム内で共通して使用されるイベントを定義・管理するシステムです。以下の機能を提供します：

-   共通イベントの定義
-   イベントの発行と購読
-   イベントのフィルタリング
-   イベントのバッファリング

## イベント定義

### GameStateEvent

ゲームの状態変更を通知するイベントです。

```csharp
public class GameStateEvent : GameEventBase
{
    public GameState PreviousState { get; }
    public GameState CurrentState { get; }

    public GameStateEvent(object source, GameState previousState, GameState currentState)
        : base("GameStateChanged", source)
    {
        PreviousState = previousState;
        CurrentState = currentState;
    }
}

public enum GameState
{
    None,
    Title,
    Playing,
    Paused,
    GameOver
}
```

### SceneEvent

シーンの変更を通知するイベントです。

```csharp
public class SceneEvent : GameEventBase
{
    public string PreviousScene { get; }
    public string CurrentScene { get; }

    public SceneEvent(object source, string previousScene, string currentScene)
        : base("SceneChanged", source)
    {
        PreviousScene = previousScene;
        CurrentScene = currentScene;
    }
}
```

## 主要コンポーネント

### ICommonEventBus

共通イベントバスのインターフェースです。

```csharp
public interface ICommonEventBus : IGameEventBus
{
    void PublishGameStateChanged(GameState previousState, GameState currentState);
    void PublishSceneChanged(string previousScene, string currentScene);
}
```

### CommonEventBus

共通イベントバスの実装クラスです。

```csharp
public class CommonEventBus : GameEventBus, ICommonEventBus
{
    public void PublishGameStateChanged(GameState previousState, GameState currentState);
    public void PublishSceneChanged(string previousScene, string currentScene);
}
```

## 使用例

### ゲーム状態の変更

```csharp
public class GameManager : MonoBehaviour
{
    [SerializeField] private ICommonEventBus _eventBus;
    private GameState _currentState = GameState.None;

    public void ChangeState(GameState newState)
    {
        var previousState = _currentState;
        _currentState = newState;
        _eventBus.PublishGameStateChanged(previousState, _currentState);
    }
}
```

### シーンの変更

```csharp
public class SceneManager : MonoBehaviour
{
    [SerializeField] private ICommonEventBus _eventBus;
    private string _currentScene;

    public void LoadScene(string sceneName)
    {
        var previousScene = _currentScene;
        _currentScene = sceneName;
        _eventBus.PublishSceneChanged(previousScene, _currentScene);
    }
}
```

### イベントの購読

```csharp
public class GameUI : MonoBehaviour
{
    [SerializeField] private ICommonEventBus _eventBus;
    [SerializeField] private GameObject _pauseMenu;
    private readonly CompositeDisposable _disposables = new();

    private void Start()
    {
        _eventBus.Subscribe<GameStateEvent>(OnGameStateChanged)
            .AddTo(_disposables);
    }

    private void OnGameStateChanged(GameStateEvent evt)
    {
        _pauseMenu.SetActive(evt.CurrentState == GameState.Paused);
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
