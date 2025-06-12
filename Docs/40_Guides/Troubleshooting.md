---
title: トラブルシューティングガイド
version: 0.1.0
status: draft
updated: 2024-03-24
tags:
    - Guide
    - Troubleshooting
    - Player
    - System
linked_docs:
    - "[[PlayerSystem]]"
    - "[[PlayerInputSystem]]"
    - "[[PlayerStateSystem]]"
    - "[[PlayerMovementSystem]]"
    - "[[PlayerCombatSystem]]"
    - "[[PlayerAnimationSystem]]"
---

# トラブルシューティングガイド

## 目次

1. [概要](#概要)
2. [一般的な問題](#一般的な問題)
3. [入力システムの問題](#入力システムの問題)
4. [状態システムの問題](#状態システムの問題)
5. [移動システムの問題](#移動システムの問題)
6. [戦闘システムの問題](#戦闘システムの問題)
7. [アニメーションシステムの問題](#アニメーションシステムの問題)
8. [パフォーマンスの問題](#パフォーマンスの問題)
9. [エラー処理の問題](#エラー処理の問題)
10. [変更履歴](#変更履歴)

## 一般的な問題

### システムの初期化に失敗する

**症状**:

-   システムの初期化時に例外が発生する
-   サブシステムが正しく動作しない

**考えられる原因**:

1. イベントバスが正しく初期化されていない
2. 必要なリソースが読み込まれていない
3. 依存関係の順序が正しくない

**解決方法**:

```csharp
// 1. イベントバスの初期化を確認
var eventBus = new GameEventBus();
if (eventBus == null)
{
    Debug.LogError("イベントバスの初期化に失敗しました");
    return;
}

// 2. リソースの読み込みを確認
if (!ResourceLoader.Exists("res://Resources/Player/player_config.tres"))
{
    Debug.LogError("必要なリソースが見つかりません");
    return;
}

// 3. 初期化順序の確認
// 正しい順序: イベントバス → 入力 → 状態 → 移動 → 戦闘 → アニメーション
var inputSystem = new PlayerInputSystem(eventBus);
var stateSystem = new PlayerStateSystem(eventBus);
var movementSystem = new PlayerMovementSystem(eventBus);
var combatSystem = new PlayerCombatSystem(eventBus);
var animationSystem = new PlayerAnimationSystem(eventBus);
```

### イベントが発火しない

**症状**:

-   イベントハンドラーが呼び出されない
-   システム間の連携が機能しない

**考えられる原因**:

1. イベントの購読が正しく設定されていない
2. イベントの発火タイミングが適切でない
3. イベントの型が一致していない

**解決方法**:

```csharp
// 1. イベントの購読を確認
eventBus.GetEventStream<PlayerStateChangedEvent>()
    .Subscribe(evt => {
        Debug.Log($"状態が変更されました: {evt.PreviousState} → {evt.NewState}");
    })
    .AddTo(_disposables);

// 2. イベントの発火を確認
public void ChangeState(PlayerState newState)
{
    var previousState = _currentState;
    _currentState = newState;

    // イベントの発火を確認
    _eventBus.Publish(new PlayerStateChangedEvent(previousState, newState));
    Debug.Log($"状態変更イベントを発火: {previousState} → {newState}");
}

// 3. イベントの型を確認
public class PlayerStateChangedEvent
{
    public PlayerState PreviousState { get; }
    public PlayerState NewState { get; }

    public PlayerStateChangedEvent(PlayerState previousState, PlayerState newState)
    {
        PreviousState = previousState;
        NewState = newState;
    }
}
```

## 入力システムの問題

### 入力が検出されない

**症状**:

-   キー入力が反応しない
-   入力イベントが発火しない

**考えられる原因**:

1. 入力アクションが正しく登録されていない
2. 入力の有効化/無効化が適切でない
3. 入力の優先順位が正しく設定されていない

**解決方法**:

```csharp
// 1. 入力アクションの登録を確認
public void RegisterInputActions()
{
    // 移動入力の登録
    RegisterInputAction(new InputAction("Move", Key.W, Key.S, Key.A, Key.D));

    // 攻撃入力の登録
    RegisterInputAction(new InputAction("Attack", Key.Space));

    // 防御入力の登録
    RegisterInputAction(new InputAction("Block", Key.LeftShift));
}

// 2. 入力の有効化/無効化を確認
public void EnableInput()
{
    _isInputEnabled = true;
    Debug.Log("入力システムを有効化しました");
}

public void DisableInput()
{
    _isInputEnabled = false;
    Debug.Log("入力システムを無効化しました");
}

// 3. 入力の優先順位を確認
public void SetInputPriority(string actionName, int priority)
{
    if (_inputActions.TryGetValue(actionName, out var action))
    {
        action.Priority = priority;
        Debug.Log($"入力アクション '{actionName}' の優先順位を {priority} に設定しました");
    }
}
```

### 入力の遅延

**症状**:

-   入力の反応が遅い
-   入力の処理に時間がかかる

**考えられる原因**:

1. 入力処理の最適化が不十分
2. 不要な入力チェックが行われている
3. 入力のバッファリングが適切でない

**解決方法**:

```csharp
// 1. 入力処理の最適化
public void ProcessInput()
{
    if (!_isInputEnabled) return;

    // 必要な入力のみをチェック
    var currentInput = GetCurrentInput();
    if (currentInput == null) return;

    // 入力の処理
    ProcessInputAction(currentInput);
}

// 2. 入力チェックの最適化
private InputAction GetCurrentInput()
{
    // 優先順位の高い入力からチェック
    return _inputActions.Values
        .OrderByDescending(a => a.Priority)
        .FirstOrDefault(a => a.IsTriggered());
}

// 3. 入力のバッファリング
private readonly Queue<InputAction> _inputBuffer = new Queue<InputAction>();
private const int MAX_BUFFER_SIZE = 5;

public void BufferInput(InputAction input)
{
    if (_inputBuffer.Count >= MAX_BUFFER_SIZE)
    {
        _inputBuffer.Dequeue();
    }
    _inputBuffer.Enqueue(input);
}
```

## 状態システムの問題

### 状態遷移が機能しない

**症状**:

-   状態が変更されない
-   状態遷移の条件が満たされても遷移しない

**考えられる原因**:

1. 状態遷移の条件が正しく設定されていない
2. 状態のロックが適切でない
3. 状態遷移のイベントが正しく発火していない

**解決方法**:

```csharp
// 1. 状態遷移の条件を確認
public bool CanTransitionTo(PlayerState newState)
{
    // 現在の状態から遷移可能かチェック
    if (!_stateTransitions.ContainsKey(_currentState))
    {
        Debug.LogError($"現在の状態 '{_currentState}' からの遷移が定義されていません");
        return false;
    }

    var allowedTransitions = _stateTransitions[_currentState];
    if (!allowedTransitions.Contains(newState))
    {
        Debug.LogError($"状態 '{_currentState}' から '{newState}' への遷移は許可されていません");
        return false;
    }

    return true;
}

// 2. 状態のロックを確認
public void LockState(PlayerState state)
{
    _lockedStates.Add(state);
    Debug.Log($"状態 '{state}' をロックしました");
}

public void UnlockState(PlayerState state)
{
    _lockedStates.Remove(state);
    Debug.Log($"状態 '{state}' のロックを解除しました");
}

// 3. 状態遷移のイベントを確認
public void ChangeState(PlayerState newState)
{
    if (!CanTransitionTo(newState))
    {
        Debug.LogError($"状態 '{_currentState}' から '{newState}' への遷移が失敗しました");
        return;
    }

    var previousState = _currentState;
    _currentState = newState;

    // 状態遷移イベントの発火
    _eventBus.Publish(new PlayerStateChangedEvent(previousState, newState));
    Debug.Log($"状態を '{previousState}' から '{newState}' に変更しました");
}
```

### 状態の競合

**症状**:

-   複数の状態が同時に有効になる
-   状態の優先順位が正しく機能しない

**考えられる原因**:

1. 状態の優先順位が正しく設定されていない
2. 状態の排他制御が不十分
3. 状態の更新タイミングが適切でない

**解決方法**:

```csharp
// 1. 状態の優先順位を設定
public void SetStatePriority(PlayerState state, int priority)
{
    _statePriorities[state] = priority;
    Debug.Log($"状態 '{state}' の優先順位を {priority} に設定しました");
}

// 2. 状態の排他制御
public bool IsStateExclusive(PlayerState state)
{
    return _exclusiveStates.Contains(state);
}

public void SetStateExclusive(PlayerState state, bool isExclusive)
{
    if (isExclusive)
    {
        _exclusiveStates.Add(state);
    }
    else
    {
        _exclusiveStates.Remove(state);
    }
}

// 3. 状態の更新タイミング
public void UpdateState()
{
    if (_isUpdating) return;
    _isUpdating = true;

    try
    {
        // 状態の更新処理
        UpdateCurrentState();
    }
    finally
    {
        _isUpdating = false;
    }
}
```

## 移動システムの問題

### 移動が機能しない

**症状**:

-   キャラクターが移動しない
-   移動速度が正しく適用されない

**考えられる原因**:

1. 移動の有効化/無効化が適切でない
2. 移動速度の計算が正しくない
3. 移動の制限が適切でない

**解決方法**:

```csharp
// 1. 移動の有効化/無効化を確認
public void EnableMovement()
{
    _isMovementEnabled = true;
    Debug.Log("移動システムを有効化しました");
}

public void DisableMovement()
{
    _isMovementEnabled = false;
    Debug.Log("移動システムを無効化しました");
}

// 2. 移動速度の計算を確認
public Vector2 CalculateMovementSpeed(Vector2 input)
{
    var speed = _baseSpeed;

    // 状態に応じた速度調整
    if (_currentState == PlayerState.Running)
    {
        speed *= _runMultiplier;
    }
    else if (_currentState == PlayerState.Walking)
    {
        speed *= _walkMultiplier;
    }

    // 入力方向の正規化
    if (input.magnitude > 0)
    {
        input.Normalize();
    }

    return input * speed;
}

// 3. 移動の制限を確認
public bool CanMove()
{
    // 移動可能な状態かチェック
    if (!_movableStates.Contains(_currentState))
    {
        Debug.Log($"現在の状態 '{_currentState}' では移動できません");
        return false;
    }

    // 移動の制限をチェック
    if (_movementRestrictions.Any(r => r.IsRestricted()))
    {
        Debug.Log("移動が制限されています");
        return false;
    }

    return true;
}
```

### 移動の滑らかさ

**症状**:

-   移動が滑らかでない
-   移動の加速/減速が不自然

**考えられる原因**:

1. 移動の補間が適切でない
2. 加速度の設定が不適切
3. 移動の更新頻度が低い

**解決方法**:

```csharp
// 1. 移動の補間
public Vector2 InterpolateMovement(Vector2 current, Vector2 target, float deltaTime)
{
    return Vector2.Lerp(current, target, _movementSmoothness * deltaTime);
}

// 2. 加速度の設定
public void SetAcceleration(float acceleration)
{
    _acceleration = acceleration;
    Debug.Log($"加速度を {acceleration} に設定しました");
}

public void SetDeceleration(float deceleration)
{
    _deceleration = deceleration;
    Debug.Log($"減速度を {deceleration} に設定しました");
}

// 3. 移動の更新頻度
public void UpdateMovement(float deltaTime)
{
    if (!_isMovementEnabled) return;

    // 移動の更新
    var currentVelocity = _currentVelocity;
    var targetVelocity = CalculateMovementSpeed(_inputDirection);

    // 加速度/減速度の適用
    if (targetVelocity.magnitude > 0)
    {
        _currentVelocity = Vector2.MoveTowards(
            currentVelocity,
            targetVelocity,
            _acceleration * deltaTime
        );
    }
    else
    {
        _currentVelocity = Vector2.MoveTowards(
            currentVelocity,
            Vector2.zero,
            _deceleration * deltaTime
        );
    }

    // 位置の更新
    _position += _currentVelocity * deltaTime;
}
```

## 戦闘システムの問題

### 攻撃が機能しない

**症状**:

-   攻撃が発動しない
-   ダメージが適用されない

**考えられる原因**:

1. 攻撃の有効化/無効化が適切でない
2. 攻撃の判定が正しくない
3. ダメージ計算が正しくない

**解決方法**:

```csharp
// 1. 攻撃の有効化/無効化を確認
public void EnableCombat()
{
    _isCombatEnabled = true;
    Debug.Log("戦闘システムを有効化しました");
}

public void DisableCombat()
{
    _isCombatEnabled = false;
    Debug.Log("戦闘システムを無効化しました");
}

// 2. 攻撃の判定を確認
public bool CanAttack()
{
    // 戦闘可能な状態かチェック
    if (!_combatEnabledStates.Contains(_currentState))
    {
        Debug.Log($"現在の状態 '{_currentState}' では攻撃できません");
        return false;
    }

    // 攻撃の制限をチェック
    if (_attackRestrictions.Any(r => r.IsRestricted()))
    {
        Debug.Log("攻撃が制限されています");
        return false;
    }

    return true;
}

// 3. ダメージ計算を確認
public float CalculateDamage(float baseDamage)
{
    var damage = baseDamage;

    // 攻撃力の補正
    damage *= _attackPower;

    // クリティカル判定
    if (IsCriticalHit())
    {
        damage *= _criticalMultiplier;
    }

    // 防御力の計算
    damage = Mathf.Max(0, damage - _defense);

    return damage;
}
```

### 戦闘のバランス

**症状**:

-   戦闘が簡単すぎる/難しすぎる
-   ダメージのバランスが取れていない

**考えられる原因**:

1. パラメータの設定が不適切
2. 戦闘の難易度調整が不十分
3. バランス調整の仕組みが不十分

**解決方法**:

```csharp
// 1. パラメータの設定
public void SetCombatParameters(CombatParameters parameters)
{
    _baseAttackDamage = parameters.BaseAttackDamage;
    _baseDefense = parameters.BaseDefense;
    _criticalRate = parameters.CriticalRate;
    _criticalMultiplier = parameters.CriticalMultiplier;

    Debug.Log("戦闘パラメータを更新しました");
}

// 2. 難易度調整
public void SetDifficultyLevel(DifficultyLevel level)
{
    switch (level)
    {
        case DifficultyLevel.Easy:
            _damageMultiplier = 0.8f;
            _defenseMultiplier = 1.2f;
            break;
        case DifficultyLevel.Normal:
            _damageMultiplier = 1.0f;
            _defenseMultiplier = 1.0f;
            break;
        case DifficultyLevel.Hard:
            _damageMultiplier = 1.2f;
            _defenseMultiplier = 0.8f;
            break;
    }

    Debug.Log($"難易度を {level} に設定しました");
}

// 3. バランス調整
public void AdjustBalance(float damageMultiplier, float defenseMultiplier)
{
    _damageMultiplier = damageMultiplier;
    _defenseMultiplier = defenseMultiplier;

    Debug.Log($"戦闘バランスを調整: 攻撃力 {damageMultiplier}, 防御力 {defenseMultiplier}");
}
```

## アニメーションシステムの問題

### アニメーションが再生されない

**症状**:

-   アニメーションが開始しない
-   アニメーションの遷移が機能しない

**考えられる原因**:

1. アニメーションの初期化が不適切
2. アニメーションの再生条件が正しくない
3. アニメーションの遷移条件が正しくない

**解決方法**:

```csharp
// 1. アニメーションの初期化を確認
public void InitializeAnimation()
{
    // アニメーションクリップの読み込み
    foreach (var clip in _animationClips)
    {
        if (!LoadAnimationClip(clip))
        {
            Debug.LogError($"アニメーションクリップ '{clip.Name}' の読み込みに失敗しました");
            continue;
        }
    }

    // アニメーションの初期状態を設定
    SetInitialAnimation();
}

// 2. アニメーションの再生条件を確認
public bool CanPlayAnimation(string animationName)
{
    // アニメーションが存在するかチェック
    if (!_animationClips.ContainsKey(animationName))
    {
        Debug.LogError($"アニメーション '{animationName}' が見つかりません");
        return false;
    }

    // 再生条件をチェック
    var clip = _animationClips[animationName];
    if (!clip.CanPlay())
    {
        Debug.Log($"アニメーション '{animationName}' の再生条件が満たされていません");
        return false;
    }

    return true;
}

// 3. アニメーションの遷移条件を確認
public bool CanTransitionTo(string targetAnimation)
{
    // 現在のアニメーションから遷移可能かチェック
    if (!_animationTransitions.ContainsKey(_currentAnimation))
    {
        Debug.LogError($"現在のアニメーション '{_currentAnimation}' からの遷移が定義されていません");
        return false;
    }

    var allowedTransitions = _animationTransitions[_currentAnimation];
    if (!allowedTransitions.Contains(targetAnimation))
    {
        Debug.LogError($"アニメーション '{_currentAnimation}' から '{targetAnimation}' への遷移は許可されていません");
        return false;
    }

    return true;
}
```

### アニメーションの同期

**症状**:

-   アニメーションと他のシステムの同期が取れない
-   アニメーションのタイミングがずれる

**考えられる原因**:

1. アニメーションの更新タイミングが適切でない
2. イベントの同期が不十分
3. アニメーションの補間が不適切

**解決方法**:

```csharp
// 1. アニメーションの更新タイミング
public void UpdateAnimation(float deltaTime)
{
    if (!_isAnimationEnabled) return;

    // アニメーションの更新
    _currentAnimationTime += deltaTime;

    // アニメーションの更新処理
    UpdateCurrentAnimation();

    // アニメーションの完了チェック
    CheckAnimationCompletion();
}

// 2. イベントの同期
public void SynchronizeWithEvent(GameEvent evt)
{
    switch (evt)
    {
        case PlayerStateChangedEvent stateEvent:
            OnStateChanged(stateEvent);
            break;
        case PlayerMovementEvent movementEvent:
            OnMovementChanged(movementEvent);
            break;
        case PlayerCombatEvent combatEvent:
            OnCombatChanged(combatEvent);
            break;
    }
}

// 3. アニメーションの補間
public void InterpolateAnimation(float targetTime, float deltaTime)
{
    var currentTime = _currentAnimationTime;
    var interpolatedTime = Mathf.Lerp(currentTime, targetTime, _animationSmoothness * deltaTime);

    SetAnimationTime(interpolatedTime);
}
```

## パフォーマンスの問題

### メモリ使用量の増加

**症状**:

-   メモリ使用量が徐々に増加する
-   パフォーマンスが低下する

**考えられる原因**:

1. リソースの解放が不適切
2. イベントの購読解除が漏れている
3. オブジェクトプールの使用が不適切

**解決方法**:

```csharp
// 1. リソースの解放
public void Dispose()
{
    // リソースの解放
    foreach (var resource in _resources)
    {
        resource.Dispose();
    }
    _resources.Clear();

    // イベントの購読解除
    _disposables.Dispose();

    // オブジェクトプールのクリア
    _objectPool.Clear();
}

// 2. イベントの購読解除
public void UnsubscribeFromEvents()
{
    _disposables.Clear();
    Debug.Log("すべてのイベント購読を解除しました");
}

// 3. オブジェクトプールの使用
public T GetFromPool<T>() where T : class, new()
{
    if (_objectPool.TryGetValue(typeof(T), out var pool))
    {
        return pool.Get() as T;
    }
    return new T();
}

public void ReturnToPool<T>(T obj) where T : class
{
    if (_objectPool.TryGetValue(typeof(T), out var pool))
    {
        pool.Return(obj);
    }
}
```

### CPU 使用率の増加

**症状**:

-   CPU 使用率が高い
-   フレームレートが低下する

**考えられる原因**:

1. 不要な更新処理が行われている
2. 計算処理が最適化されていない
3. スレッドの使用が不適切

**解決方法**:

```csharp
// 1. 更新処理の最適化
public void Update(float deltaTime)
{
    if (!_isEnabled) return;

    // 必要な更新のみを実行
    if (_needsUpdate)
    {
        UpdateSystems(deltaTime);
        _needsUpdate = false;
    }
}

// 2. 計算処理の最適化
public Vector2 CalculateMovement(Vector2 input)
{
    // キャッシュを使用
    if (_cachedInput == input)
    {
        return _cachedResult;
    }

    _cachedInput = input;
    _cachedResult = OptimizedMovementCalculation(input);
    return _cachedResult;
}

// 3. スレッドの使用
public async Task ProcessHeavyComputation()
{
    await Task.Run(() =>
    {
        // 重い計算処理
        PerformHeavyComputation();
    });
}
```

## エラー処理の問題

### 症状

-   エラーが適切に処理されない
-   エラー状態からの回復ができない
-   エラーログが不十分

### 考えられる原因

-   エラーハンドリングの実装が不適切
-   エラー状態の検出が不十分
-   エラーログの出力が不適切

### 解決方法

1. エラーハンドリングの実装を確認
2. エラー状態の検出を強化
3. エラーログの出力を改善

## 変更履歴

| バージョン | 更新日     | 変更内容                                                                         |
| ---------- | ---------- | -------------------------------------------------------------------------------- |
| 0.1.0      | 2024-03-24 | 初版作成<br>- 一般的な問題の解決方法を追加<br>- 各システムの問題と解決方法を追加 |
