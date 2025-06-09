---
title: ReactiveProperty実装詳細
version: 0.3.0
status: draft
updated: 2025-06-09
tags:
    - Core
    - Reactive
    - Property
    - Implementation
linked_docs:
    - "[[12_01_mvvm_rx_architecture|MVVM+RXアーキテクチャ]]"
    - "[[12_02_basic_design|MVVM+RX基本設計書]]"
    - "[[02_viewmodel_base|ViewModelBase実装詳細]]"
---

# ReactiveProperty 実装詳細

## 目次

1. [概要](#1-概要)
2. [基本実装](#2-基本実装)
3. [拡張機能](#3-拡張機能)
4. [パフォーマンス最適化](#4-パフォーマンス最適化)
5. [使用例](#5-使用例)
6. [ベストプラクティス](#6-ベストプラクティス)
7. [テスト](#7-テスト)
8. [制限事項](#8-制限事項)
9. [変更履歴](#9-変更履歴)

## 1. 概要

ReactiveProperty は、プロパティの変更を監視し、通知を行うためのヘルパークラスです。MVVM パターンとリアクティブプログラミングを組み合わせた実装をサポートします。

## 2. 基本実装

### 2.1 クラス定義

```csharp
public class ReactiveProperty<T>
{
    private T _value;
    private readonly Subject<T> _subject = new();
    private readonly List<IDisposable> _disposables = new();

    public T Value
    {
        get => _value;
        set
        {
            if (_validator != null && !_validator(value))
            {
                throw new ArgumentException("Validation failed", nameof(value));
            }
            if (!EqualityComparer<T>.Default.Equals(_value, value))
            {
                _value = value;
                _subject.OnNext(value);
            }
        }
    }

    public IObservable<T> ValueChanged => _subject.AsObservable();

    public ReactiveProperty(T initialValue = default)
    {
        _value = initialValue;
    }
}
```

### 2.2 基本的な使用例

```csharp
public class PlayerViewModel
{
    public ReactiveProperty<int> Health { get; } = new(100);
    public ReactiveProperty<string> Status { get; } = new("Normal");

    public PlayerViewModel()
    {
        // 値の変更を監視
        Health.ValueChanged
            .Where(h => h <= 0)
            .Subscribe(_ => Status.Value = "Dead");
    }
}
```

## 3. 拡張機能

### 3.1 値の検証

```csharp
public class ReactiveProperty<T>
{
    private Func<T, bool> _validator;

    public void SetValidator(Func<T, bool> validator)
    {
        _validator = validator;
    }

    public bool Validate(T value)
    {
        return _validator?.Invoke(value) ?? true;
    }
}
```

`SetValidator` で登録した関数が `false` を返した場合、値の設定時に `ArgumentException` が送出されます。

### 3.2 値の変換

```csharp
public class ReactiveProperty<T>
{
    public ReactiveProperty<R> Select<R>(Func<T, R> selector)
    {
        var result = new ReactiveProperty<R>(selector(_value));
        _subject.Select(selector).Subscribe(v => result.Value = v);
        return result;
    }
}
```

## 4. パフォーマンス最適化

### 4.1 メモリ管理

```csharp
public class ReactiveProperty<T> : IDisposable
{
    private bool _disposed;

    public void Dispose()
    {
        if (!_disposed)
        {
            _subject.Dispose();
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
            _disposables.Clear();
            _disposed = true;
        }
    }
}
```

### 4.2 バッチ更新

```csharp
public class ReactiveProperty<T>
{
    private bool _isUpdating;

    public void BeginUpdate()
    {
        _isUpdating = true;
    }

    public void EndUpdate()
    {
        _isUpdating = false;
        _subject.OnNext(_value);
    }
}
```

## 5. 使用例

### 5.1 ゲーム状態の管理

```csharp
public class GameStateViewModel
{
    public ReactiveProperty<GameState> CurrentState { get; } = new(GameState.Menu);
    public ReactiveProperty<int> Score { get; } = new(0);
    public ReactiveProperty<bool> IsPaused { get; } = new(false);

    public GameStateViewModel()
    {
        // 状態変更の監視
        CurrentState.ValueChanged
            .Where(s => s == GameState.Playing)
            .Subscribe(_ => IsPaused.Value = false);

        // スコアの監視
        Score.ValueChanged
            .Where(s => s >= 1000)
            .Subscribe(_ => OnHighScoreAchieved());
    }
}
```

### 5.2 UI の更新

```csharp
public class PlayerStatusViewModel
{
    public ReactiveProperty<string> HealthText { get; }
    public ReactiveProperty<string> ManaText { get; }

    public PlayerStatusViewModel(PlayerModel model)
    {
        HealthText = model.Health.Select(h => $"HP: {h}");
        ManaText = model.Mana.Select(m => $"MP: {m}");
    }
}
```

## 6. ベストプラクティス

### 6.1 メモリリークの防止

-   適切なタイミングで Dispose を呼び出す
-   不要なサブスクリプションを解除する
-   循環参照を避ける

### 6.2 パフォーマンス

-   必要な場合のみ値の更新を行う
-   バッチ更新を活用する
-   重い処理は非同期で実行する

### 6.3 エラーハンドリング

-   値の検証を適切に行う
-   例外を適切に処理する
-   エラー状態を通知する

## 7. テスト

### 7.1 単体テスト

```csharp
[Test]
public void ReactiveProperty_ValueChange_NotifiesSubscribers()
{
    var property = new ReactiveProperty<int>(0);
    int notifiedValue = -1;

    property.ValueChanged.Subscribe(v => notifiedValue = v);
    property.Value = 42;

    Assert.That(notifiedValue, Is.EqualTo(42));
}
```

### 7.2 統合テスト

```csharp
[Test]
public void ReactiveProperty_Integration_WorksWithViewModel()
{
    var viewModel = new PlayerViewModel();
    bool statusChanged = false;

    viewModel.Status.ValueChanged.Subscribe(_ => statusChanged = true);
    viewModel.Health.Value = 0;

    Assert.That(statusChanged, Is.True);
    Assert.That(viewModel.Status.Value, Is.EqualTo("Dead"));
}
```

## 8. 制限事項

### 8.1 メモリ管理

-   適切なタイミングで Dispose を呼び出す必要がある
-   循環参照に注意が必要
-   大量のサブスクリプションは避ける

### 8.2 パフォーマンス

-   頻繁な値の更新は避ける
-   重い処理は非同期で実行する
-   バッチ更新を活用する

### 8.3 スレッドセーフ

-   マルチスレッド環境での使用には注意が必要
-   適切な同期処理を実装する
-   スレッド間の値の受け渡しに注意

## 9. 変更履歴

| バージョン | 更新日     | 変更内容                                                                                           |
| ---------- | ---------- | -------------------------------------------------------------------------------------------------- |
| 0.3.0      | 2025-06-09 | バリデーション失敗時に例外を送出するよう変更 |
| 0.2.0      | 2024-03-23 | パフォーマンス最適化の追加<br>- メモリ管理の改善<br>- バッチ更新機能の追加<br>- 値の検証機能の強化 |
| 0.1.0      | 2024-03-22 | 初版作成<br>- 基本実装の定義<br>- 拡張機能の実装<br>- 使用例の追加                                 |
