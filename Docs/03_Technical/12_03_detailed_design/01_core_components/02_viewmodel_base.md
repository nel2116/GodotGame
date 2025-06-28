---
title: ViewModelBase実装詳細
version: 0.2.1
status: draft
updated: 2025-06-09
tags:
    - Core
    - MVVM
    - ViewModel
    - Implementation
linked_docs:
    - "[[12_01_mvvm_rx_architecture|MVVM+RXアーキテクチャ]]"
    - "[[12_02_basic_design|MVVM+RX基本設計書]]"
    - "[[01_reactive_property|ReactiveProperty実装詳細]]"
---

# ViewModelBase 実装詳細

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

ViewModelBase は、MVVM パターンにおける ViewModel の基底クラスです。リアクティブプログラミングの機能を統合し、メモリ管理やライフサイクル管理を提供します。

## 2. 基本実装

### 2.1 クラス定義

```csharp
public abstract class ViewModelBase : IDisposable
{
    protected readonly CompositeDisposable Disposables = new();
    private bool _disposed;

    protected ViewModelBase()
    {
        Initialize();
    }

    protected virtual void Initialize()
    {
        // サブクラスでオーバーライド可能な初期化処理
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            Disposables.Dispose();
            OnDispose();
            _disposed = true;
        }
    }

    protected virtual void OnDispose()
    {
        // サブクラスでオーバーライド可能な破棄処理
    }
}
```

### 2.2 基本的な使用例

```csharp
public class PlayerViewModel : ViewModelBase
{
    public ReactiveProperty<int> Health { get; } = new(100);
    public ReactiveProperty<string> Status { get; } = new("Normal");

    protected override void Initialize()
    {
        base.Initialize();

        // 値の変更を監視
        Disposables.Add(
            Health.ValueChanged
                .Where(h => h <= 0)
                .Subscribe(_ => Status.Value = "Dead")
        );
    }
}
```

## 3. 拡張機能

### 3.1 コマンドの実装

```csharp
public abstract class ViewModelBase : IDisposable
{
    protected ReactiveCommand CreateCommand()
    {
        return new ReactiveCommand().AddTo(Disposables);
    }

    protected ReactiveCommand<T> CreateCommand<T>()
    {
        return new ReactiveCommand<T>().AddTo(Disposables);
    }
}
```

### 3.2 非同期処理のサポート

```csharp
public abstract class ViewModelBase : IDisposable
{
    protected ReactiveProperty<bool> IsBusy { get; } = new(false).AddTo(Disposables);

    protected async Task ExecuteAsync(Func<Task> action)
    {
        try
        {
            IsBusy.Value = true;
            await action();
        }
        finally
        {
            IsBusy.Value = false;
        }
    }
}
```

## 4. パフォーマンス最適化

### 4.1 メモリ管理

```csharp
public abstract class ViewModelBase : IDisposable
{
    protected void AddDisposable(IDisposable disposable)
    {
        if (!_disposed)
        {
            Disposables.Add(disposable);
        }
        else
        {
            disposable.Dispose();
        }
    }
}
```

### 4.2 ライフサイクル管理

```csharp
public abstract class ViewModelBase : IDisposable
{
    public ReactiveProperty<ViewModelState> State { get; } = new(ViewModelState.Initial).AddTo(Disposables);

    protected virtual void OnActivate()
    {
        State.Value = ViewModelState.Active;
    }

    protected virtual void OnDeactivate()
    {
        State.Value = ViewModelState.Inactive;
    }
}
```

## 5. 使用例

### 5.1 ゲーム画面の ViewModel

```csharp
public class GameScreenViewModel : ViewModelBase
{
    public ReactiveProperty<GameState> CurrentState { get; } = new(GameState.Menu);
    public ReactiveCommand StartGameCommand { get; }
    public ReactiveCommand PauseGameCommand { get; }

    public GameScreenViewModel()
    {
        StartGameCommand = CreateCommand()
            .AddTo(Disposables);

        PauseGameCommand = CreateCommand()
            .AddTo(Disposables);

        Disposables.Add(
            StartGameCommand.Subscribe(_ => StartGame())
        );

        Disposables.Add(
            PauseGameCommand.Subscribe(_ => PauseGame())
        );
    }

    private void StartGame()
    {
        CurrentState.Value = GameState.Playing;
    }

    private void PauseGame()
    {
        CurrentState.Value = GameState.Paused;
    }
}
```

### 5.2 非同期処理を含む ViewModel

```csharp
public class DataLoadingViewModel : ViewModelBase
{
    public ReactiveProperty<List<Item>> Items { get; } = new(new List<Item>());
    public ReactiveCommand LoadDataCommand { get; }

    public DataLoadingViewModel()
    {
        LoadDataCommand = CreateCommand()
            .AddTo(Disposables);

        Disposables.Add(
            LoadDataCommand.Subscribe(async _ => await LoadDataAsync())
        );
    }

    private async Task LoadDataAsync()
    {
        await ExecuteAsync(async () =>
        {
            var data = await LoadItemsFromServer();
            Items.Value = data;
        });
    }
}
```

## 6. ベストプラクティス

### 6.1 メモリリークの防止

-   すべての Disposable を CompositeDisposable に追加する
-   適切なタイミングで Dispose を呼び出す
-   循環参照を避ける

### 6.2 パフォーマンス

-   必要な場合のみ値の更新を行う
-   重い処理は非同期で実行する
-   メモリ使用量を監視する

### 6.3 エラーハンドリング

-   例外を適切に処理する
-   エラー状態を通知する
-   リカバリー処理を実装する

## 7. テスト

### 7.1 単体テスト

```csharp
[Test]
public void ViewModelBase_Dispose_CleansUpResources()
{
    var viewModel = new TestViewModel();
    var weakRef = new WeakReference(viewModel);

    viewModel.Dispose();
    viewModel = null;
    GC.Collect();

    Assert.That(weakRef.IsAlive, Is.False);
}
```

### 7.2 統合テスト

```csharp
[Test]
public async Task ViewModelBase_AsyncOperation_UpdatesStateCorrectly()
{
    var viewModel = new DataLoadingViewModel();
    bool isBusyChanged = false;

    viewModel.IsBusy.ValueChanged.Subscribe(_ => isBusyChanged = true);
    await viewModel.LoadDataCommand.Execute();

    Assert.That(isBusyChanged, Is.True);
    Assert.That(viewModel.IsBusy.Value, Is.False);
}
```

## 8. 制限事項

### 8.1 メモリ管理

-   適切なタイミングで Dispose を呼び出す必要がある
-   循環参照に注意が必要
-   大量のコマンドは避ける

### 8.2 パフォーマンス

-   重い処理は非同期で実行する
-   コマンドの実行頻度に注意
-   状態更新の最適化

### 8.3 スレッドセーフ

-   マルチスレッド環境での使用には注意が必要
-   適切な同期処理を実装する
-   UI スレッドとの連携に注意

## 9. 変更履歴

| バージョン | 更新日     | 変更内容                                                                                                     |
| ---------- | ---------- | ------------------------------------------------------------------------------------------------------------ |
| 0.2.1      | 2025-06-09 | IsBusy と State を Disposables に登録 |
| 0.2.0      | 2025-06-09 | パフォーマンス最適化の追加<br>- メモリ管理の改善<br>- ライフサイクル管理の強化<br>- 非同期処理のサポート追加 |
| 0.1.0      | 2024-03-22 | 初版作成<br>- 基本実装の定義<br>- コマンド実装の追加<br>- 使用例の追加                                       |
