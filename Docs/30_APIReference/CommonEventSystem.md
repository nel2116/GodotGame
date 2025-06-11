---
title: Common Event System API Reference
version: 0.1
status: draft
updated: 2024-03-21
tags:
    - API
    - Common
    - Event
    - Systems
    - Reference
---

# Common Event System API Reference

## 目次

1. [概要](#概要)
2. [イベントクラス](#イベントクラス)
3. [使用方法](#使用方法)
4. [制限事項](#制限事項)
5. [変更履歴](#変更履歴)

## 概要

Common Event System は、ゲームの各サブシステムで使用される具体的なイベントを定義するシステムです。Core Event System を基盤として、各システム固有のイベント処理を提供します。

## イベントクラス

### StateEvents

状態変更に関するイベントを定義するクラスです。

```csharp
public static class StateEvents
{
    // 状態変更イベント
    public static event Action<StateChangeEventArgs> OnStateChanged;

    // 状態初期化イベント
    public static event Action<StateInitEventArgs> OnStateInitialized;
}
```

### ResourceEvents

リソース操作に関するイベントを定義するクラスです。

```csharp
public static class ResourceEvents
{
    // リソース取得イベント
    public static event Action<ResourceAcquireEventArgs> OnResourceAcquired;

    // リソース解放イベント
    public static event Action<ResourceReleaseEventArgs> OnResourceReleased;
}
```

### MovementEvents

移動に関するイベントを定義するクラスです。

```csharp
public static class MovementEvents
{
    // 移動開始イベント
    public static event Action<MovementStartEventArgs> OnMovementStarted;

    // 移動停止イベント
    public static event Action<MovementStopEventArgs> OnMovementStopped;

    // 位置変更イベント
    public static event Action<PositionChangeEventArgs> OnPositionChanged;
}
```

### CombatEvents

戦闘に関するイベントを定義するクラスです。

```csharp
public static class CombatEvents
{
    // 攻撃イベント
    public static event Action<AttackEventArgs> OnAttack;

    // ダメージイベント
    public static event Action<DamageEventArgs> OnDamage;
}
```

### AnimationEvents

アニメーションに関するイベントを定義するクラスです。

```csharp
public static class AnimationEvents
{
    // アニメーション開始イベント
    public static event Action<AnimationStartEventArgs> OnAnimationStarted;

    // アニメーション終了イベント
    public static event Action<AnimationEndEventArgs> OnAnimationEnded;
}
```

## 使用方法

### 1. イベントの購読

```csharp
// 状態変更イベントの購読
StateEvents.OnStateChanged += HandleStateChanged;

// 移動イベントの購読
MovementEvents.OnMovementStarted += HandleMovementStarted;
```

### 2. イベントの発行

```csharp
// 状態変更イベントの発行
StateEvents.OnStateChanged?.Invoke(new StateChangeEventArgs(newState));

// 移動イベントの発行
MovementEvents.OnMovementStarted?.Invoke(new MovementStartEventArgs(direction, speed));
```

### 3. イベントの購読解除

```csharp
// イベントの購読解除
StateEvents.OnStateChanged -= HandleStateChanged;
MovementEvents.OnMovementStarted -= HandleMovementStarted;
```

## 制限事項

1. **イベントハンドラの管理**

    - イベントハンドラは適切なタイミングで購読解除する必要があります
    - 複数のイベントを購読する場合は、`CompositeDisposable` を使用して管理することを推奨

2. **null チェック**

    - イベントの発行時は必ず null チェックを行う必要があります
    - 購読者が存在しない場合のエラーを防ぐため

3. **例外処理**

    - イベントハンドラ内での例外は適切に処理する必要があります
    - イベントの発行元で例外をキャッチし、適切に処理してください

4. **パフォーマンス**
    - 頻繁に発生するイベントは、適切な間隔で発行する必要があります
    - 不要なイベントの発行は避けてください

## 変更履歴

| バージョン | 更新日     | 変更内容 |
| ---------- | ---------- | -------- |
| 0.1.0      | 2024-03-21 | 初版作成 |
