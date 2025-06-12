---
title: プレイヤー状態システム
version: 0.1.0
status: draft
updated: 2024-03-21
tags:
    - API
    - Player
    - State
    - Core
    - Reactive
    - Event
linked_docs:
    - "[[PlayerSystem]]"
    - "[[ReactiveSystem]]"
    - "[[ViewModelSystem]]"
    - "[[CoreEventSystem]]"
    - "[[CommonEventSystem]]"
---

# プレイヤー状態システム

## 目次

1. [概要](#概要)
2. [状態定義](#状態定義)
3. [主要コンポーネント](#主要コンポーネント)
4. [使用例](#使用例)
5. [制限事項](#制限事項)
6. [変更履歴](#変更履歴)

## 概要

プレイヤー状態システムは、プレイヤーの状態を管理し、状態遷移を制御するシステムです。以下の機能を提供します：

-   プレイヤーの状態管理
-   状態遷移の制御
-   状態に応じた動作の制御
-   イベント通知

## 状態定義

### PlayerState

プレイヤーの状態を表す列挙型です。

```csharp
public enum PlayerState
{
    Idle,
    Walking,
    Running,
    Jumping,
    Falling,
    Attacking,
    Damaged,
    Dead
}
```

### PlayerStateTransition

状態遷移の条件を定義するクラスです。

```csharp
public class PlayerStateTransition
{
    public PlayerState FromState { get; }
    public PlayerState ToState { get; }
    public Func<bool> Condition { get; }
    public Action OnTransition { get; }

    public PlayerStateTransition(
        PlayerState fromState,
        PlayerState toState,
        Func<bool> condition,
        Action onTransition = null)
    {
        FromState = fromState;
        ToState = toState;
        Condition = condition;
        OnTransition = onTransition;
    }
}
```

## 主要コンポーネント

### PlayerStateManager

プレイヤーの状態を管理するコンポーネントです。

```csharp
public class PlayerStateManager
{
    private readonly ReactiveProperty<PlayerState> _currentState;
    private readonly List<PlayerStateTransition> _transitions;
    private readonly IGameEventBus _eventBus;

    public IReactiveProperty<PlayerState> CurrentState => _currentState;

    public void ChangeState(PlayerState newState);
    public bool CanChangeState(PlayerState newState);
    public void AddTransition(PlayerStateTransition transition);
    public void RemoveTransition(PlayerStateTransition transition);
    public void Update();
}
```

### PlayerStateHandler

プレイヤーの状態変更を処理するコンポーネントです。

```csharp
public class PlayerStateHandler : MonoBehaviour
{
    private readonly CompositeDisposable _disposables = new();
    private readonly PlayerStateManager _stateManager;

    private void OnEnable();
    private void OnDisable();
    private void Update();
    private void OnStateChanged(PlayerState newState);
}
```

## 使用例

### 状態遷移の定義

```csharp
public class PlayerStateInitializer : MonoBehaviour
{
    [SerializeField] private PlayerStateManager _stateManager;

    private void Start()
    {
        // アイドル状態から歩行状態への遷移
        _stateManager.AddTransition(new PlayerStateTransition(
            PlayerState.Idle,
            PlayerState.Walking,
            () => Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0
        ));

        // 歩行状態から走行状態への遷移
        _stateManager.AddTransition(new PlayerStateTransition(
            PlayerState.Walking,
            PlayerState.Running,
            () => Input.GetKey(KeyCode.LeftShift)
        ));

        // 地上状態からジャンプ状態への遷移
        _stateManager.AddTransition(new PlayerStateTransition(
            PlayerState.Idle,
            PlayerState.Jumping,
            () => Input.GetKeyDown(KeyCode.Space) && IsGrounded()
        ));
    }

    private bool IsGrounded()
    {
        // 接地判定の実装
        return true;
    }
}
```

### 状態変更の監視

```csharp
public class PlayerStateObserver : MonoBehaviour
{
    [SerializeField] private PlayerStateManager _stateManager;

    private void OnEnable()
    {
        _stateManager.CurrentState
            .Subscribe(OnStateChanged)
            .AddTo(_disposables);
    }

    private void OnStateChanged(PlayerState newState)
    {
        Debug.Log($"Player state changed to: {newState}");
    }
}
```

## 制限事項

-   スレッドセーフな実装が必要な箇所では、必ず提供されている同期メカニズムを使用してください
-   リソースの解放は適切なタイミングで行ってください
-   イベントの購読は必要最小限に抑えてください
-   非同期処理の実行時は、必ず`ExecuteAsync`メソッドを使用してください
-   状態遷移の条件は、必ず`PlayerStateTransition`を通じて定義してください
-   状態変更は、必ず`PlayerStateManager`を通じて行ってください
-   状態変更時の処理は、必ず`PlayerStateHandler`を通じて行ってください

## 変更履歴

| バージョン | 更新日     | 変更内容 |
| ---------- | ---------- | -------- |
| 0.1.0      | 2024-03-21 | 初版作成 |
