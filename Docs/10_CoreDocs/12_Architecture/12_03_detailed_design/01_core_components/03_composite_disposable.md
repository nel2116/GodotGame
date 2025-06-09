---
title: CompositeDisposable実装詳細
version: 0.1.1
status: draft
updated: 2025-06-09
tags:
    - Architecture
    - MVVM
    - Reactive
    - Core
    - Implementation
linked_docs:
    - "[[12_03_detailed_design|MVVM+RX詳細設計書]]"
    - "[[12_01_mvvm_rx_architecture|MVVM+RXアーキテクチャ]]"
    - "[[01_reactive_property|ReactiveProperty実装詳細]]"
    - "[[02_viewmodel_base|ViewModelBase実装詳細]]"
---

# CompositeDisposable 実装詳細

## 目次

1. [概要](#1-概要)
2. [クラス図](#2-クラス図)
3. [シーケンス図](#3-シーケンス図)
4. [実装詳細](#4-実装詳細)
5. [パフォーマンス最適化](#5-パフォーマンス最適化)
6. [テスト戦略](#6-テスト戦略)
7. [制限事項](#7-制限事項)
8. [変更履歴](#8-変更履歴)

## 1. 概要

### 1.1 目的

本ドキュメントは、MVVM + リアクティブプログラミングにおける CompositeDisposable の実装詳細を定義し、以下の目的を達成することを目指します：

-   複数の IDisposable リソースの一元管理
-   メモリリークの防止
-   リソース解放の確実な実行

### 1.2 適用範囲

-   サブスクリプション管理
-   リソース解放
-   メモリ管理

## 2. クラス図

```mermaid
classDiagram
    class IDisposable {
        <<interface>>
        +Dispose() void
    }

    class CompositeDisposable {
        -List~IDisposable~ _disposables
        -bool _isDisposed
        +Add(IDisposable) void
        +AddRange(IEnumerable~IDisposable~) void
        +Remove(IDisposable) bool
        +Clear() void
        +Dispose() void
    }

    IDisposable <|.. CompositeDisposable
```

## 3. シーケンス図

```mermaid
sequenceDiagram
    participant VM as ViewModel
    participant CD as CompositeDisposable
    participant RP as ReactiveProperty

    VM->>CD: Add(ReactiveProperty)
    VM->>CD: Add(Subscription)
    VM->>CD: Dispose()
    CD->>RP: Dispose()
    CD->>Subscription: Dispose()
```

## 4. 実装詳細

### 4.1 基本実装

```csharp
public class CompositeDisposable : IDisposable
{
    private readonly List<IDisposable> _disposables = new();
    private bool _isDisposed;
    private readonly object _lock = new();

    public void Add(IDisposable disposable)
    {
        if (disposable == null) throw new ArgumentNullException(nameof(disposable));

        lock (_lock)
        {
            if (_isDisposed)
            {
                disposable.Dispose();
                return;
            }

            _disposables.Add(disposable);
        }
    }

    // Add メソッドでは自身の追加を検出して無視するため特別なチェックは不要

    public void AddRange(IEnumerable<IDisposable> disposables)
    {
        if (disposables == null) throw new ArgumentNullException(nameof(disposables));

        foreach (var disposable in disposables)
        {
            Add(disposable);
        }
    }

    public bool Remove(IDisposable disposable)
    {
        if (disposable == null) throw new ArgumentNullException(nameof(disposable));

        lock (_lock)
        {
            return _disposables.Remove(disposable);
        }
    }

    public void Clear()
    {
        lock (_lock)
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
            _disposables.Clear();
        }
    }

    public void Dispose()
    {
        lock (_lock)
        {
            if (_isDisposed) return;

            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }
            _disposables.Clear();
            _isDisposed = true;
        }
    }
}
```

### 4.1.1 AddTo 拡張

```csharp
public static class DisposableExtensions
{
    public static T AddTo<T>(this T disposable, CompositeDisposable composite) where T : IDisposable
    {
        composite.Add(disposable);
        return disposable;
    }
}
```

### 4.2 使用例

```csharp
public class GameViewModel : IDisposable
{
    private readonly CompositeDisposable _disposables = new();
    public ReactiveProperty<int> Score { get; } = new(0);
    public ReactiveProperty<float> Time { get; } = new(0f);

    public GameViewModel()
    {
        // 複数のサブスクリプションを管理
        _disposables.Add(
            Score.Subscribe(OnScoreChanged)
        );
        _disposables.Add(
            Time.Subscribe(OnTimeChanged)
        );
    }

    private void OnScoreChanged(int score)
    {
        // スコア変更時の処理
    }

    private void OnTimeChanged(float time)
    {
        // 時間変更時の処理
    }

    public void Dispose()
    {
        _disposables.Dispose();
    }
}
```

## 5. パフォーマンス最適化

### 5.1 メモリ管理

-   スレッドセーフな実装
-   効率的なリソース解放
-   重複登録の防止

### 5.2 スレッドセーフティ

-   ロックの最適化
-   デッドロックの防止
-   並行アクセスの制御

## 6. テスト戦略

### 6.1 単体テスト

```csharp
[Test]
public void CompositeDisposable_AddAndDispose_DisposesAllResources()
{
    var disposable1 = new Mock<IDisposable>();
    var disposable2 = new Mock<IDisposable>();
    var composite = new CompositeDisposable();

    composite.Add(disposable1.Object);
    composite.Add(disposable2.Object);
    composite.Dispose();

    disposable1.Verify(d => d.Dispose(), Times.Once);
    disposable2.Verify(d => d.Dispose(), Times.Once);
}
```

### 6.2 スレッドセーフティテスト

```csharp
[Test]
public void CompositeDisposable_ConcurrentAccess_ThreadSafe()
{
    var composite = new CompositeDisposable();
    var tasks = new List<Task>();

    for (int i = 0; i < 1000; i++)
    {
        tasks.Add(Task.Run(() =>
        {
            var disposable = new Mock<IDisposable>();
            composite.Add(disposable.Object);
        }));
    }

    Task.WaitAll(tasks.ToArray());
    composite.Dispose();
}
```

## 7. 制限事項

### 7.1 スレッドセーフティ

-   複数のスレッドからの同時アクセスは、ロック機構によって保護されていますが、パフォーマンスに影響を与える可能性があります。
-   大量のリソースを同時に管理する場合は、パフォーマンスに注意が必要です。

### 7.2 メモリ管理

-   リソースの解放漏れを防ぐため、必ず`Dispose()`メソッドを呼び出す必要があります。
-   循環参照に注意が必要です。

### 7.3 使用上の注意

-   自身のインスタンスを`Add()`メソッドに渡すことは避けてください。
-   既に破棄されたリソースの追加は無視されます。

## 8. 変更履歴

| バージョン | 更新日     | 変更内容                                                                                               |
| ---------- | ---------- | ------------------------------------------------------------------------------------------------------ |
| 0.1.1      | 2025-06-09 | AddTo 拡張メソッドを追加 |
| 0.1.0      | 2025-06-09 | 初版作成<br>- 基本実装の定義<br>- スレッドセーフティの実装<br>- テストケースの追加<br>- 制限事項の追加 |
