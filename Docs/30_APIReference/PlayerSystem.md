---
title: プレイヤーシステム
version: 0.1.0
status: draft
updated: 2024-03-21
tags:
    - API
    - Player
    - Core
    - State
    - Movement
    - Combat
    - Animation
    - Input
    - Progression
linked_docs:
    - "[[PlayerStateSystem]]"
    - "[[PlayerMovementSystem]]"
    - "[[PlayerCombatSystem]]"
    - "[[PlayerAnimationSystem]]"
    - "[[PlayerInputSystem]]"
    - "[[PlayerProgressionSystem]]"
    - "[[ReactiveSystem]]"
    - "[[ViewModelSystem]]"
---

# プレイヤーシステム

## 目次

1. [概要](#概要)
2. [システム構成](#システム構成)
3. [主要コンポーネント](#主要コンポーネント)
4. [使用例](#使用例)
5. [制限事項](#制限事項)
6. [変更履歴](#変更履歴)

## 概要

プレイヤーシステムは、ゲーム内のプレイヤーキャラクターを管理するための包括的なシステムです。以下の機能を提供します：

-   プレイヤーの状態管理
-   移動制御
-   戦闘処理
-   アニメーション制御
-   入力処理
-   進行管理

## システム構成

プレイヤーシステムは以下のサブシステムで構成されています：

1. 状態管理システム（[[PlayerStateSystem|プレイヤー状態システム]]）
2. 移動システム（[[PlayerMovementSystem|プレイヤー移動システム]]）
3. 戦闘システム（[[PlayerCombatSystem|プレイヤー戦闘システム]]）
4. アニメーションシステム（[[PlayerAnimationSystem|プレイヤーアニメーションシステム]]）
5. 入力システム（[[PlayerInputSystem|プレイヤー入力システム]]）
6. 進行システム（[[PlayerProgressionSystem|プレイヤー進行システム]]）

## 主要コンポーネント

### PlayerController

プレイヤーの制御を担当するメインコンポーネントです。

```csharp
public class PlayerController : MonoBehaviour
{
    private readonly CompositeDisposable _disposables = new();
    private readonly IGameEventBus _eventBus;
    private readonly PlayerStateManager _stateManager;
    private readonly PlayerMovementController _movementController;
    private readonly PlayerCombatController _combatController;
    private readonly PlayerAnimationController _animationController;
    private readonly PlayerInputController _inputController;
    private readonly PlayerProgressionController _progressionController;

    public void Initialize();
    public void Dispose();
    protected virtual void OnEnable();
    protected virtual void OnDisable();
}
```

### PlayerStateManager

プレイヤーの状態を管理するコンポーネントです。

```csharp
public class PlayerStateManager
{
    private readonly ReactiveProperty<PlayerState> _currentState;
    public IReactiveProperty<PlayerState> CurrentState => _currentState;

    public void ChangeState(PlayerState newState);
    public bool CanChangeState(PlayerState newState);
}
```

### PlayerMovementController

プレイヤーの移動を制御するコンポーネントです。

```csharp
public class PlayerMovementController
{
    private readonly ReactiveProperty<Vector3> _position;
    private readonly ReactiveProperty<Vector3> _velocity;
    public IReactiveProperty<Vector3> Position => _position;
    public IReactiveProperty<Vector3> Velocity => _velocity;

    public void Move(Vector3 direction);
    public void Stop();
}
```

### PlayerCombatController

プレイヤーの戦闘を制御するコンポーネントです。

```csharp
public class PlayerCombatController
{
    private readonly ReactiveProperty<int> _health;
    private readonly ReactiveProperty<int> _maxHealth;
    public IReactiveProperty<int> Health => _health;
    public IReactiveProperty<int> MaxHealth => _maxHealth;

    public void TakeDamage(int damage);
    public void Heal(int amount);
}
```

### PlayerAnimationController

プレイヤーのアニメーションを制御するコンポーネントです。

```csharp
public class PlayerAnimationController
{
    private readonly Animator _animator;
    private readonly ReactiveProperty<string> _currentAnimation;
    public IReactiveProperty<string> CurrentAnimation => _currentAnimation;

    public void PlayAnimation(string animationName);
    public void StopAnimation();
}
```

### PlayerInputController

プレイヤーの入力を制御するコンポーネントです。

```csharp
public class PlayerInputController
{
    private readonly ReactiveProperty<Vector2> _moveInput;
    private readonly ReactiveProperty<bool> _isAttacking;
    public IReactiveProperty<Vector2> MoveInput => _moveInput;
    public IReactiveProperty<bool> IsAttacking => _isAttacking;

    public void UpdateInput();
}
```

### PlayerProgressionController

プレイヤーの進行を管理するコンポーネントです。

```csharp
public class PlayerProgressionController
{
    private readonly ReactiveProperty<int> _level;
    private readonly ReactiveProperty<int> _experience;
    public IReactiveProperty<int> Level => _level;
    public IReactiveProperty<int> Experience => _experience;

    public void AddExperience(int amount);
    public void LevelUp();
}
```

## 使用例

### プレイヤーの初期化

```csharp
public class GameManager : MonoBehaviour
{
    private PlayerController _playerController;

    private void Start()
    {
        _playerController = GetComponent<PlayerController>();
        _playerController.Initialize();
    }

    private void OnDestroy()
    {
        _playerController?.Dispose();
    }
}
```

### プレイヤーの状態変更

```csharp
public class PlayerStateHandler : MonoBehaviour
{
    [SerializeField] private PlayerController _playerController;

    private void OnEnable()
    {
        _playerController.StateManager.CurrentState
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
-   プレイヤーの状態変更は、必ず`PlayerStateManager`を通じて行ってください
-   移動処理は、必ず`PlayerMovementController`を通じて行ってください
-   戦闘処理は、必ず`PlayerCombatController`を通じて行ってください
-   アニメーション処理は、必ず`PlayerAnimationController`を通じて行ってください
-   入力処理は、必ず`PlayerInputController`を通じて行ってください
-   進行処理は、必ず`PlayerProgressionController`を通じて行ってください

## 変更履歴

| バージョン | 更新日     | 変更内容 |
| ---------- | ---------- | -------- |
| 0.1.0      | 2024-03-21 | 初版作成 |
