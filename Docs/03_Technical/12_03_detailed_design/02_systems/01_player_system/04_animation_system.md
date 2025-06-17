---
title: プレイヤーアニメーションシステム実装詳細
version: 0.3.0
status: draft
updated: 2024-03-24
tags:
    - Architecture
    - MVVM
    - Reactive
    - System
    - Implementation
    - Player
    - Animation
linked_docs:
    - "[[12_03_detailed_design/02_systems/01_player_system/index|プレイヤーシステム実装詳細]]"
    - "[[12_03_detailed_design/02_systems/00_common_systems/02_animation_system|共通アニメーションシステム実装詳細]]"
    - "[[12_03_detailed_design/01_core_components/06_player_system_base|プレイヤーシステム基底クラス実装詳細]]"
---

# プレイヤーアニメーションシステム実装詳細

## 目次

1. [概要](#1-概要)
2. [クラス図](#2-クラス図)
3. [シーケンス図](#3-シーケンス図)
4. [実装詳細](#4-実装詳細)
5. [パフォーマンス最適化](#5-パフォーマンス最適化)
6. [テスト戦略](#6-テスト戦略)
7. [変更履歴](#7-変更履歴)

## 1. 概要

### 1.1 目的

本ドキュメントは、プレイヤーアニメーションシステムの実装詳細を定義し、以下の目的を達成することを目指します：

-   プレイヤー固有のアニメーション処理の実装
-   共通アニメーションシステムの拡張
-   アニメーション状態の管理
-   開発チーム間での実装の一貫性確保

### 1.2 適用範囲

-   プレイヤー固有のアニメーション処理
-   プレイヤー固有のアニメーションパラメータ
-   プレイヤー固有のアニメーション状態
-   プレイヤー固有のアニメーションイベント

## 2. クラス図

```mermaid
classDiagram
    class PlayerSystemBase {
        +CompositeDisposable Disposables
        +IEventBus EventBus
        +PlayerStateManager StateManager
        +void Initialize()
        +void Update()
        +void Dispose()
    }

    class PlayerAnimationModel {
        +Dictionary<string, AnimationClip> Clips
        +float TransitionSpeed
        +void PlayAnimation()
        +void BlendAnimation()
    }

    class PlayerAnimationViewModel {
        +ReactiveProperty<string> CurrentAnimation
        +ReactiveProperty<float> Speed
        +ReactiveProperty<bool> IsPlaying
        +void UpdateAnimation()
        +void HandleAnimation()
    }

    class PlayerAnimationView {
        +void OnAnimationUpdated()
        +void UpdateAnimationDisplay()
    }

    PlayerSystemBase <|-- PlayerAnimationModel
    PlayerAnimationModel --> PlayerAnimationViewModel : uses
    PlayerAnimationViewModel --> PlayerAnimationView : observes
```

## 3. シーケンス図

```mermaid
sequenceDiagram
    participant View as PlayerAnimationView
    participant ViewModel as PlayerAnimationViewModel
    participant Model as PlayerAnimationModel
    participant Base as PlayerSystemBase
    participant State as PlayerStateManager
    participant Event as EventBus

    View->>ViewModel: アニメーション要求
    ViewModel->>Model: アニメーション処理
    Model->>Base: 基本処理
    Base->>State: 状態確認
    Base->>Event: イベント発行
    Model->>ViewModel: 状態更新
    ViewModel->>View: 表示更新
```

## 4. 実装詳細

### 4.1 モデル層

```csharp
public class PlayerAnimationModel : PlayerSystemBase
{
    private readonly Dictionary<string, AnimationClip> _clips;
    private float _transitionSpeed;
    private bool _isPlaying;

    public PlayerAnimationModel(IEventBus eventBus) : base(eventBus)
    {
        _clips = new Dictionary<string, AnimationClip>();
        _transitionSpeed = 0.25f;
    }

    public override void Initialize()
    {
        try
        {
            // プレイヤー固有のアニメーションパラメータの初期化
            LoadAnimationClips();
            CurrentAnimation = "Idle";
            Speed = 1.0f;
            _isPlaying = true;

            // 状態の登録
            StateManager.RegisterState("Animation", new AnimationState());
            StateManager.RegisterTransition("Animation", "Playing", () => _isPlaying);
            StateManager.RegisterTransition("Animation", "Paused", () => !_isPlaying);
        }
        catch (Exception ex)
        {
            HandleError("Initialize", ex);
        }
    }

    private void LoadAnimationClips()
    {
        try
        {
            using (var resourceLoader = new ResourceLoader())
            {
                _clips["Idle"] = resourceLoader.Load<AnimationClip>("Animations/Player/Idle");
                _clips["Walk"] = resourceLoader.Load<AnimationClip>("Animations/Player/Walk");
                _clips["Run"] = resourceLoader.Load<AnimationClip>("Animations/Player/Run");
                _clips["Jump"] = resourceLoader.Load<AnimationClip>("Animations/Player/Jump");
                _clips["Attack"] = resourceLoader.Load<AnimationClip>("Animations/Player/Attack");
            }
        }
        catch (Exception ex)
        {
            HandleError("LoadAnimationClips", ex);
        }
    }

    public override void Update()
    {
        try
        {
            if (_isPlaying)
            {
                // アニメーション更新処理
                UpdateAnimation();
            }
        }
        catch (Exception ex)
        {
            HandleError("Update", ex);
        }
    }

    public void PlayAnimation(string animationName)
    {
        try
        {
            if (!_clips.ContainsKey(animationName))
            {
                throw new ArgumentException($"Invalid animation name: {animationName}");
            }

            CurrentAnimation = animationName;
            Play();
            EventBus.Publish(new AnimationPlayedEvent(animationName));
        }
        catch (Exception ex)
        {
            HandleError("PlayAnimation", ex);
        }
    }

    public void BlendAnimation(string fromAnimation, string toAnimation, float blendTime)
    {
        try
        {
            if (!_clips.ContainsKey(fromAnimation) || !_clips.ContainsKey(toAnimation))
            {
                throw new ArgumentException("Invalid animation names for blending");
            }

            // アニメーションブレンド処理
            StartCoroutine(BlendAnimationCoroutine(fromAnimation, toAnimation, blendTime));
            EventBus.Publish(new AnimationBlendStartedEvent(fromAnimation, toAnimation, blendTime));
        }
        catch (Exception ex)
        {
            HandleError("BlendAnimation", ex);
        }
    }

    private IEnumerator BlendAnimationCoroutine(string fromAnimation, string toAnimation, float blendTime)
    {
        float elapsedTime = 0f;
        while (elapsedTime < blendTime)
        {
            float blend = elapsedTime / blendTime;
            // ブレンド処理
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        CurrentAnimation = toAnimation;
        EventBus.Publish(new AnimationBlendCompletedEvent(toAnimation));
    }
}
```

### 4.2 ビューモデル層

```csharp
public class PlayerAnimationViewModel : ViewModelBase
{
    private readonly PlayerAnimationModel _model;
    private readonly ReactiveProperty<string> _currentAnimation;
    private readonly ReactiveProperty<float> _speed;
    private readonly ReactiveProperty<bool> _isPlaying;

    public PlayerAnimationViewModel(PlayerAnimationModel model)
    {
        _model = model;
        _currentAnimation = new ReactiveProperty<string>();
        _speed = new ReactiveProperty<float>();
        _isPlaying = new ReactiveProperty<bool>();

        // アニメーション状態の購読
        _currentAnimation.Subscribe(OnAnimationChanged).AddTo(Disposables);
        _speed.Subscribe(OnSpeedChanged).AddTo(Disposables);
        _isPlaying.Subscribe(OnPlayingChanged).AddTo(Disposables);
    }

    public void UpdateAnimation()
    {
        _model.Update();
        UpdateAnimationState();
    }

    public void HandleAnimation(string animationName)
    {
        _model.PlayAnimation(animationName);
    }

    private void UpdateAnimationState()
    {
        _currentAnimation.Value = _model.CurrentAnimation;
        _speed.Value = _model.Speed;
        _isPlaying.Value = _model.IsPlaying;
    }

    private void OnAnimationChanged(string animation)
    {
        EventBus.Publish(new AnimationChangedEvent(animation));
    }

    private void OnSpeedChanged(float speed)
    {
        EventBus.Publish(new AnimationSpeedChangedEvent(speed));
    }

    private void OnPlayingChanged(bool isPlaying)
    {
        EventBus.Publish(new AnimationPlayingChangedEvent(isPlaying));
    }
}
```

### 4.3 ビュー層

```csharp
public class PlayerAnimationView : MonoBehaviour
{
    private PlayerAnimationViewModel _viewModel;

    private void Start()
    {
        var eventBus = new EventBus();
        var model = new PlayerAnimationModel(eventBus);
        _viewModel = new PlayerAnimationViewModel(model);
        _viewModel.Initialize();
    }

    private void Update()
    {
        _viewModel.UpdateAnimation();
    }

    private void OnDestroy()
    {
        _viewModel.Dispose();
    }
}
```

## 5. パフォーマンス最適化

### 5.1 メモリ管理

-   アニメーションデータのキャッシュ
-   イベントの最適化
-   リソースの適切な解放

### 5.2 更新最適化

-   アニメーション処理の優先順位付け
-   不要な更新の回避
-   バッチ処理の活用

## 6. テスト戦略

### 6.1 単体テスト

```csharp
[Test]
public void TestPlayerAnimation()
{
    var eventBus = new EventBus();
    var model = new PlayerAnimationModel(eventBus);
    var viewModel = new PlayerAnimationViewModel(model);

    // アニメーションのテスト
    viewModel.UpdateAnimation();
    Assert.That(viewModel.CurrentAnimation.Value, Is.EqualTo("Idle"));
}
```

### 6.2 統合テスト

```csharp
[Test]
public void TestPlayerAnimationToStateIntegration()
{
    var eventBus = new EventBus();
    var animationSystem = new PlayerAnimationModel(eventBus);
    var stateSystem = new PlayerStateManager();

    // アニメーションから状態への連携テスト
    animationSystem.Update();
    Assert.That(stateSystem.IsValidTransition("Animation", "Playing"), Is.True);
}
```

## 7. 変更履歴

| バージョン | 更新日     | 変更内容                                                                                   |
| ---------- | ---------- | ------------------------------------------------------------------------------------------ |
| 0.3.0      | 2024-03-24 | 基底クラスの導入<br>- エラーハンドリングの統一<br>- 状態管理の統合<br>- イベント処理の改善 |
| 0.2.0      | 2024-03-23 | 共通システムとの連携を追加<br>- アニメーション処理の最適化<br>- ブレンドシステムの統合     |
| 0.1.0      | 2024-03-21 | 初版作成                                                                                   |
