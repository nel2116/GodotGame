---
title: プレイヤー移動システム
version: 0.1.0
status: draft
updated: 2024-03-21
tags:
    - API
    - Player
    - Movement
    - Core
    - Reactive
    - Event
linked_docs:
    - "[[PlayerSystem]]"
    - "[[PlayerStateSystem]]"
    - "[[ReactiveSystem]]"
    - "[[ViewModelSystem]]"
    - "[[CoreEventSystem]]"
    - "[[CommonEventSystem]]"
---

# プレイヤー移動システム

## 目次

1. [概要](#概要)
2. [移動パラメータ](#移動パラメータ)
3. [主要コンポーネント](#主要コンポーネント)
4. [使用例](#使用例)
5. [制限事項](#制限事項)
6. [変更履歴](#変更履歴)

## 概要

プレイヤー移動システムは、プレイヤーの移動を制御するシステムです。以下の機能を提供します：

-   移動制御
-   衝突判定
-   アニメーション連携
-   イベント通知

## 移動パラメータ

### MovementParameters

移動に関するパラメータを定義するクラスです。

```csharp
public class MovementParameters
{
    public float WalkSpeed { get; set; } = 5f;
    public float RunSpeed { get; set; } = 8f;
    public float JumpForce { get; set; } = 5f;
    public float Gravity { get; set; } = 9.81f;
    public float GroundCheckDistance { get; set; } = 0.1f;
    public LayerMask GroundLayer { get; set; }
}
```

## 主要コンポーネント

### PlayerMovementController

プレイヤーの移動を制御するコンポーネントです。

```csharp
public class PlayerMovementController
{
    private readonly ReactiveProperty<Vector3> _position;
    private readonly ReactiveProperty<Vector3> _velocity;
    private readonly ReactiveProperty<bool> _isGrounded;
    private readonly MovementParameters _parameters;
    private readonly IGameEventBus _eventBus;

    public IReactiveProperty<Vector3> Position => _position;
    public IReactiveProperty<Vector3> Velocity => _velocity;
    public IReactiveProperty<bool> IsGrounded => _isGrounded;

    public void Move(Vector3 direction);
    public void Stop();
    public void Jump();
    public void Update();
}
```

### PlayerMovementHandler

プレイヤーの移動を処理するコンポーネントです。

```csharp
public class PlayerMovementHandler : MonoBehaviour
{
    private readonly CompositeDisposable _disposables = new();
    private readonly PlayerMovementController _movementController;

    private void OnEnable();
    private void OnDisable();
    private void Update();
    private void OnPositionChanged(Vector3 newPosition);
    private void OnVelocityChanged(Vector3 newVelocity);
    private void OnGroundedChanged(bool isGrounded);
}
```

## 使用例

### 移動の制御

```csharp
public class PlayerMovementInput : MonoBehaviour
{
    [SerializeField] private PlayerMovementController _movementController;

    private void Update()
    {
        // 移動入力の取得
        var horizontal = Input.GetAxisRaw("Horizontal");
        var vertical = Input.GetAxisRaw("Vertical");
        var direction = new Vector3(horizontal, 0, vertical).normalized;

        // 移動の実行
        if (direction != Vector3.zero)
        {
            _movementController.Move(direction);
        }
        else
        {
            _movementController.Stop();
        }

        // ジャンプの実行
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _movementController.Jump();
        }
    }
}
```

### 移動状態の監視

```csharp
public class PlayerMovementObserver : MonoBehaviour
{
    [SerializeField] private PlayerMovementController _movementController;

    private void OnEnable()
    {
        _movementController.Position
            .Subscribe(OnPositionChanged)
            .AddTo(_disposables);

        _movementController.Velocity
            .Subscribe(OnVelocityChanged)
            .AddTo(_disposables);

        _movementController.IsGrounded
            .Subscribe(OnGroundedChanged)
            .AddTo(_disposables);
    }

    private void OnPositionChanged(Vector3 newPosition)
    {
        Debug.Log($"Player position changed to: {newPosition}");
    }

    private void OnVelocityChanged(Vector3 newVelocity)
    {
        Debug.Log($"Player velocity changed to: {newVelocity}");
    }

    private void OnGroundedChanged(bool isGrounded)
    {
        Debug.Log($"Player grounded state changed to: {isGrounded}");
    }
}
```

## 制限事項

-   スレッドセーフな実装が必要な箇所では、必ず提供されている同期メカニズムを使用してください
-   リソースの解放は適切なタイミングで行ってください
-   イベントの購読は必要最小限に抑えてください
-   非同期処理の実行時は、必ず`ExecuteAsync`メソッドを使用してください
-   移動パラメータは、必ず`MovementParameters`を通じて設定してください
-   移動制御は、必ず`PlayerMovementController`を通じて行ってください
-   移動処理は、必ず`PlayerMovementHandler`を通じて行ってください

## 変更履歴

| バージョン | 更新日     | 変更内容 |
| ---------- | ---------- | -------- |
| 0.1.0      | 2024-03-21 | 初版作成 |
