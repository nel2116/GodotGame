---
title: プレイヤー入力システム
version: 0.1.0
status: draft
updated: 2024-03-21
tags:
    - API
    - Player
    - Input
    - Core
    - Reactive
    - Event
linked_docs:
    - "[[PlayerSystem]]"
    - "[[PlayerStateSystem]]"
    - "[[PlayerMovementSystem]]"
    - "[[PlayerCombatSystem]]"
    - "[[PlayerAnimationSystem]]"
    - "[[ReactiveSystem]]"
    - "[[ViewModelSystem]]"
    - "[[CoreEventSystem]]"
    - "[[CommonEventSystem]]"
---

# プレイヤー入力システム

## 目次

1. [概要](#概要)
2. [入力定義](#入力定義)
3. [主要コンポーネント](#主要コンポーネント)
4. [使用例](#使用例)
5. [制限事項](#制限事項)
6. [変更履歴](#変更履歴)

## 概要

プレイヤー入力システムは、プレイヤーの入力を制御するシステムです。以下の機能を提供します：

-   入力検出
-   入力マッピング
-   イベント通知
-   入力状態管理

## 入力定義

### InputDefinition

入力の定義を管理するクラスです。

```csharp
public class InputDefinition
{
    public string Name { get; set; }
    public InputType Type { get; set; }
    public string Key { get; set; }
    public float DeadZone { get; set; } = 0.1f;
    public bool Invert { get; set; } = false;
    public float Sensitivity { get; set; } = 1.0f;
}

public enum InputType
{
    Button,
    Axis,
    Vector2,
    Vector3
}
```

## 主要コンポーネント

### PlayerInputController

プレイヤーの入力を制御するコンポーネントです。

```csharp
public class PlayerInputController
{
    private readonly ReactiveProperty<Vector2> _movementInput;
    private readonly ReactiveProperty<bool> _jumpInput;
    private readonly ReactiveProperty<bool> _attackInput;
    private readonly ReactiveProperty<bool> _defendInput;
    private readonly Dictionary<string, InputDefinition> _inputDefinitions;
    private readonly IGameEventBus _eventBus;

    public IReactiveProperty<Vector2> MovementInput => _movementInput;
    public IReactiveProperty<bool> JumpInput => _jumpInput;
    public IReactiveProperty<bool> AttackInput => _attackInput;
    public IReactiveProperty<bool> DefendInput => _defendInput;

    public void AddInputDefinition(InputDefinition definition);
    public void RemoveInputDefinition(string inputName);
    public void UpdateInput();
}
```

### PlayerInputHandler

プレイヤーの入力を処理するコンポーネントです。

```csharp
public class PlayerInputHandler : MonoBehaviour
{
    private readonly CompositeDisposable _disposables = new();
    private readonly PlayerInputController _inputController;

    private void OnEnable();
    private void OnDisable();
    private void Update();
    private void OnMovementInputChanged(Vector2 newInput);
    private void OnJumpInputChanged(bool newInput);
    private void OnAttackInputChanged(bool newInput);
    private void OnDefendInputChanged(bool newInput);
}
```

## 使用例

### 入力の制御

```csharp
public class PlayerInputManager : MonoBehaviour
{
    [SerializeField] private PlayerInputController _inputController;

    private void Start()
    {
        // 移動入力の定義
        var movementInput = new InputDefinition
        {
            Name = "Movement",
            Type = InputType.Vector2,
            Key = "Horizontal,Vertical",
            DeadZone = 0.1f,
            Sensitivity = 1.0f
        };
        _inputController.AddInputDefinition(movementInput);

        // ジャンプ入力の定義
        var jumpInput = new InputDefinition
        {
            Name = "Jump",
            Type = InputType.Button,
            Key = "Space",
            DeadZone = 0.0f
        };
        _inputController.AddInputDefinition(jumpInput);

        // 攻撃入力の定義
        var attackInput = new InputDefinition
        {
            Name = "Attack",
            Type = InputType.Button,
            Key = "Mouse0",
            DeadZone = 0.0f
        };
        _inputController.AddInputDefinition(attackInput);

        // 防御入力の定義
        var defendInput = new InputDefinition
        {
            Name = "Defend",
            Type = InputType.Button,
            Key = "Mouse1",
            DeadZone = 0.0f
        };
        _inputController.AddInputDefinition(defendInput);
    }

    private void Update()
    {
        _inputController.UpdateInput();
    }
}
```

### 入力状態の監視

```csharp
public class PlayerInputObserver : MonoBehaviour
{
    [SerializeField] private PlayerInputController _inputController;

    private void OnEnable()
    {
        _inputController.MovementInput
            .Subscribe(OnMovementInputChanged)
            .AddTo(_disposables);

        _inputController.JumpInput
            .Subscribe(OnJumpInputChanged)
            .AddTo(_disposables);

        _inputController.AttackInput
            .Subscribe(OnAttackInputChanged)
            .AddTo(_disposables);

        _inputController.DefendInput
            .Subscribe(OnDefendInputChanged)
            .AddTo(_disposables);
    }

    private void OnMovementInputChanged(Vector2 newInput)
    {
        Debug.Log($"Player movement input changed to: {newInput}");
    }

    private void OnJumpInputChanged(bool newInput)
    {
        Debug.Log($"Player jump input changed to: {newInput}");
    }

    private void OnAttackInputChanged(bool newInput)
    {
        Debug.Log($"Player attack input changed to: {newInput}");
    }

    private void OnDefendInputChanged(bool newInput)
    {
        Debug.Log($"Player defend input changed to: {newInput}");
    }
}
```

## 制限事項

-   スレッドセーフな実装が必要な箇所では、必ず提供されている同期メカニズムを使用してください
-   リソースの解放は適切なタイミングで行ってください
-   イベントの購読は必要最小限に抑えてください
-   非同期処理の実行時は、必ず`ExecuteAsync`メソッドを使用してください
-   入力定義は、必ず`InputDefinition`を通じて設定してください
-   入力制御は、必ず`PlayerInputController`を通じて行ってください
-   入力処理は、必ず`PlayerInputHandler`を通じて行ってください

## 変更履歴

| バージョン | 更新日     | 変更内容 |
| ---------- | ---------- | -------- |
| 0.1.0      | 2024-03-21 | 初版作成 |
