---
title: プレイヤーアニメーションシステム
version: 0.1.0
status: draft
updated: 2024-03-21
tags:
    - API
    - Player
    - Animation
    - Core
    - Reactive
    - Event
linked_docs:
    - "[[PlayerSystem]]"
    - "[[PlayerStateSystem]]"
    - "[[PlayerMovementSystem]]"
    - "[[PlayerCombatSystem]]"
    - "[[ReactiveSystem]]"
    - "[[ViewModelSystem]]"
    - "[[CoreEventSystem]]"
    - "[[CommonEventSystem]]"
---

# プレイヤーアニメーションシステム

## 目次

1. [概要](#概要)
2. [アニメーション定義](#アニメーション定義)
3. [主要コンポーネント](#主要コンポーネント)
4. [使用例](#使用例)
5. [制限事項](#制限事項)
6. [変更履歴](#変更履歴)

## 概要

プレイヤーアニメーションシステムは、プレイヤーのアニメーションを制御するシステムです。以下の機能を提供します：

-   アニメーション再生
-   アニメーション遷移
-   イベント通知
-   アニメーション同期

## アニメーション定義

### AnimationDefinition

アニメーションの定義を管理するクラスです。

```csharp
public class AnimationDefinition
{
    public string Name { get; set; }
    public string ClipName { get; set; }
    public float Speed { get; set; } = 1.0f;
    public bool Loop { get; set; } = true;
    public float TransitionTime { get; set; } = 0.25f;
    public AnimationEvent[] Events { get; set; }
}

public class AnimationEvent
{
    public string Name { get; set; }
    public float Time { get; set; }
    public object Data { get; set; }
}
```

## 主要コンポーネント

### PlayerAnimationController

プレイヤーのアニメーションを制御するコンポーネントです。

```csharp
public class PlayerAnimationController
{
    private readonly ReactiveProperty<string> _currentAnimation;
    private readonly ReactiveProperty<float> _animationSpeed;
    private readonly Dictionary<string, AnimationDefinition> _animations;
    private readonly IGameEventBus _eventBus;

    public IReactiveProperty<string> CurrentAnimation => _currentAnimation;
    public IReactiveProperty<float> AnimationSpeed => _animationSpeed;

    public void PlayAnimation(string animationName);
    public void StopAnimation();
    public void SetAnimationSpeed(float speed);
    public void AddAnimation(AnimationDefinition definition);
    public void RemoveAnimation(string animationName);
}
```

### PlayerAnimationHandler

プレイヤーのアニメーションを処理するコンポーネントです。

```csharp
public class PlayerAnimationHandler : MonoBehaviour
{
    private readonly CompositeDisposable _disposables = new();
    private readonly PlayerAnimationController _animationController;
    private readonly Animator _animator;

    private void OnEnable();
    private void OnDisable();
    private void Update();
    private void OnAnimationChanged(string newAnimation);
    private void OnAnimationSpeedChanged(float newSpeed);
    private void OnAnimationEvent(AnimationEvent animationEvent);
}
```

## 使用例

### アニメーションの制御

```csharp
public class PlayerAnimationInput : MonoBehaviour
{
    [SerializeField] private PlayerAnimationController _animationController;

    private void Update()
    {
        // 移動アニメーション
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            _animationController.PlayAnimation("Walk");
        }
        else
        {
            _animationController.PlayAnimation("Idle");
        }

        // 攻撃アニメーション
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            _animationController.PlayAnimation("Attack");
        }

        // ジャンプアニメーション
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _animationController.PlayAnimation("Jump");
        }
    }
}
```

### アニメーション状態の監視

```csharp
public class PlayerAnimationObserver : MonoBehaviour
{
    [SerializeField] private PlayerAnimationController _animationController;

    private void OnEnable()
    {
        _animationController.CurrentAnimation
            .Subscribe(OnAnimationChanged)
            .AddTo(_disposables);

        _animationController.AnimationSpeed
            .Subscribe(OnAnimationSpeedChanged)
            .AddTo(_disposables);
    }

    private void OnAnimationChanged(string newAnimation)
    {
        Debug.Log($"Player animation changed to: {newAnimation}");
    }

    private void OnAnimationSpeedChanged(float newSpeed)
    {
        Debug.Log($"Player animation speed changed to: {newSpeed}");
    }
}
```

## 制限事項

-   スレッドセーフな実装が必要な箇所では、必ず提供されている同期メカニズムを使用してください
-   リソースの解放は適切なタイミングで行ってください
-   イベントの購読は必要最小限に抑えてください
-   非同期処理の実行時は、必ず`ExecuteAsync`メソッドを使用してください
-   アニメーション定義は、必ず`AnimationDefinition`を通じて設定してください
-   アニメーション制御は、必ず`PlayerAnimationController`を通じて行ってください
-   アニメーション処理は、必ず`PlayerAnimationHandler`を通じて行ってください

## 変更履歴

| バージョン | 更新日     | 変更内容 |
| ---------- | ---------- | -------- |
| 0.1.0      | 2024-03-21 | 初版作成 |
