---
title: リアクティブプロパティ
version: 0.1.0
status: draft
updated: 2024-03-21
tags:
    - API
    - Reactive
    - Core
    - Property
linked_docs:
    - "[[ReactiveSystem]]"
    - "[[ViewModelSystem]]"
    - "[[CompositeDisposable]]"
    - "[[00_index]]"
---

# リアクティブプロパティ

## 目次

1. [概要](#概要)
2. [インターフェース](#インターフェース)
3. [実装詳細](#実装詳細)
4. [使用方法](#使用方法)
5. [制限事項](#制限事項)
6. [変更履歴](#変更履歴)

## 概要

リアクティブプロパティは、値の変更を監視し、変更時に通知を行う機能を提供します。主に以下の機能を提供します：

-   値の変更通知
-   値の検証
-   値の変換
-   値の永続化
-   値の同期

## インターフェース

### IReactiveProperty

```csharp
public interface IReactiveProperty<T> : IObservable<T>, IDisposable
{
    T Value { get; set; }
    bool HasValue { get; }
    void ForceNotify();
}
```

## 実装詳細

### ReactiveProperty

```csharp
public class ReactiveProperty<T> : IReactiveProperty<T>
{
    private T _value;
    private readonly Subject<T> _subject = new();
    private readonly CompositeDisposable _disposables = new();
    private readonly object _sync_lock = new();

    public T Value
    {
        get => _value;
        set
        {
            if (EqualityComparer<T>.Default.Equals(_value, value))
                return;

            _value = value;
            _subject.OnNext(value);
        }
    }

    public bool HasValue { get; private set; }
    public IDisposable Subscribe(IObserver<T> observer);
    public void ForceNotify();
    public void Dispose();
}
```

主な特徴：

-   スレッドセーフな実装
    -   ロックベースの同期化
    -   アトミックな操作保証
-   効率的な通知
    -   値の変更時のみ通知
    -   強制通知のサポート
-   メモリ管理
    -   自動的なリソース解放
    -   循環参照の防止
-   拡張性
    -   カスタム検証のサポート
    -   値の変換機能

## 使用方法

### 基本的な使用例

```csharp
// プロパティの作成
var health = new ReactiveProperty<int>(100);

// 値の変更
health.Value = 80;

// 値の監視
var subscription = health.Subscribe(value =>
{
    Console.WriteLine($"Health changed: {value}");
});

// 強制通知
health.ForceNotify();

// リソースの解放
subscription.Dispose();
```

### ViewModel での使用例

```csharp
public class PlayerViewModel : ViewModelBase
{
    private readonly ReactiveProperty<int> _health;
    public IReactiveProperty<int> Health => _health;

    private readonly ReactiveProperty<string> _name;
    public IReactiveProperty<string> Name => _name;

    public PlayerViewModel(IGameEventBus eventBus) : base(eventBus)
    {
        _health = new ReactiveProperty<int>(100)
            .AddTo(Disposables);

        _name = new ReactiveProperty<string>("Player")
            .AddTo(Disposables);

        // 値の変更を監視
        _health.Subscribe(value =>
        {
            if (value <= 0)
            {
                // プレイヤーの死亡処理
                eventBus.Publish(new PlayerDiedEvent());
            }
        }).AddTo(Disposables);
    }
}
```

### 値の検証と変換

```csharp
public class ValidatedReactiveProperty<T> : ReactiveProperty<T>
{
    private readonly Func<T, bool> _validator;
    private readonly Func<T, T> _converter;

    public ValidatedReactiveProperty(
        T initialValue,
        Func<T, bool> validator,
        Func<T, T> converter = null)
        : base(initialValue)
    {
        _validator = validator;
        _converter = converter;
    }

    public new T Value
    {
        get => base.Value;
        set
        {
            var convertedValue = _converter?.Invoke(value) ?? value;
            if (_validator(convertedValue))
            {
                base.Value = convertedValue;
            }
        }
    }
}

// 使用例
var health = new ValidatedReactiveProperty<int>(
    initialValue: 100,
    validator: value => value >= 0 && value <= 100,
    converter: value => Mathf.Clamp(value, 0, 100)
);
```

## 制限事項

1. **スレッドセーフ**

    - 値の変更と通知はスレッドセーフです
    - 複数のスレッドからの同時操作は避けてください
    - 通知ハンドラ内での処理は適切に同期化してください

2. **メモリ管理**

    - 購読は明示的に解除する必要があります
    - `CompositeDisposable`を使用して複数の購読を管理することを推奨
    - 循環参照に注意してください

3. **パフォーマンス**

    - 大量の通知時は注意が必要です
    - 不要な購読は早期に解除してください
    - 通知ハンドラは軽量に保ってください

4. **例外処理**

    - 値の検証時の例外は適切に処理してください
    - 通知ハンドラ内での例外は購読者で処理してください
    - 値の変換時の例外は呼び出し元で処理してください

## 変更履歴

| バージョン | 更新日     | 変更内容 |
| ---------- | ---------- | -------- |
| 0.1.0      | 2024-03-21 | 初版作成 |
