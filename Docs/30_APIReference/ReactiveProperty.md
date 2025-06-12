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
    - "[[CompositeDisposable]]"
    - "[[CoreEventSystem]]"
    - "[[CommonEventSystem]]"
---

# リアクティブプロパティ

## 目次

1. [概要](#概要)
2. [プロパティ定義](#プロパティ定義)
3. [主要コンポーネント](#主要コンポーネント)
4. [使用例](#使用例)
5. [制限事項](#制限事項)
6. [変更履歴](#変更履歴)

## 概要

リアクティブプロパティは、値の変更を監視し、変更時に通知を行うプロパティシステムです。以下の機能を提供します：

-   値の変更通知
-   値の検証
-   値の変換
-   値のバッファリング

## プロパティ定義

### IReactiveProperty

リアクティブプロパティのインターフェースです。

```csharp
public interface IReactiveProperty<T>
{
    T Value { get; set; }
    IObservable<T> OnValueChanged { get; }
    bool HasValue { get; }
    void SetValueAndForceNotify(T value);
}
```

### ReactiveProperty

リアクティブプロパティの基本クラスです。

```csharp
public class ReactiveProperty<T> : IReactiveProperty<T>
{
    private T _value;
    private readonly Subject<T> _valueChangedSubject;

    public T Value
    {
        get => _value;
        set
        {
            if (!EqualityComparer<T>.Default.Equals(_value, value))
            {
                _value = value;
                _valueChangedSubject.OnNext(value);
            }
        }
    }

    public IObservable<T> OnValueChanged => _valueChangedSubject;
    public bool HasValue => _value != null;

    public ReactiveProperty(T initialValue = default)
    {
        _value = initialValue;
        _valueChangedSubject = new Subject<T>();
    }

    public void SetValueAndForceNotify(T value)
    {
        _value = value;
        _valueChangedSubject.OnNext(value);
    }
}
```

## 主要コンポーネント

### ReactivePropertyExtensions

リアクティブプロパティの拡張メソッドを提供するクラスです。

```csharp
public static class ReactivePropertyExtensions
{
    public static IDisposable Subscribe<T>(this IReactiveProperty<T> property, Action<T> onNext);
    public static IDisposable Subscribe<T>(this IReactiveProperty<T> property, Action<T> onNext, Action<Exception> onError);
    public static IDisposable Subscribe<T>(this IReactiveProperty<T> property, Action<T> onNext, Action onCompleted);
    public static IDisposable Subscribe<T>(this IReactiveProperty<T> property, Action<T> onNext, Action<Exception> onError, Action onCompleted);
}
```

## 使用例

### 基本的な使用

```csharp
public class PlayerStats : MonoBehaviour
{
    private readonly ReactiveProperty<int> _health = new(100);
    private readonly CompositeDisposable _disposables = new();

    private void Start()
    {
        _health.Subscribe(OnHealthChanged)
            .AddTo(_disposables);
    }

    private void OnHealthChanged(int newHealth)
    {
        Debug.Log($"Health changed to: {newHealth}");
    }

    public void TakeDamage(int damage)
    {
        _health.Value = Mathf.Max(0, _health.Value - damage);
    }

    private void OnDestroy()
    {
        _disposables.Dispose();
    }
}
```

### 値の検証

```csharp
public class ValidatedReactiveProperty<T> : ReactiveProperty<T>
{
    private readonly Func<T, bool> _validator;

    public ValidatedReactiveProperty(T initialValue, Func<T, bool> validator)
        : base(initialValue)
    {
        _validator = validator;
    }

    public new T Value
    {
        get => base.Value;
        set
        {
            if (_validator(value))
            {
                base.Value = value;
            }
            else
            {
                throw new ArgumentException("Invalid value");
            }
        }
    }
}

public class PlayerStats : MonoBehaviour
{
    private readonly ValidatedReactiveProperty<int> _health;

    public PlayerStats()
    {
        _health = new ValidatedReactiveProperty<int>(100, value => value >= 0 && value <= 100);
    }
}
```

## 制限事項

-   スレッドセーフな実装が必要な箇所では、必ず提供されている同期メカニズムを使用してください
-   リソースの解放は適切なタイミングで行ってください
-   値の変更通知は必要最小限に抑えてください
-   値の検証は、必ず`ValidatedReactiveProperty`を使用してください
-   値の変換は、必ず`Select`メソッドを使用してください
-   値のバッファリングは、必ず`Buffer`メソッドを使用してください
-   値の購読は、必ず`IDisposable`を保持して適切に解放してください

## 変更履歴

| バージョン | 更新日     | 変更内容 |
| ---------- | ---------- | -------- |
| 0.1.0      | 2024-03-21 | 初版作成 |
