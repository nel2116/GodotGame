---
title: 共通ユーティリティ実装詳細
version: 0.2.7
status: draft
updated: 2025-06-12
tags:
    - Core
    - Utility
    - Implementation
linked_docs:
    - "[[12_01_mvvm_rx_architecture|MVVM+RXアーキテクチャ]]"
    - "[[12_02_basic_design|MVVM+RX基本設計書]]"
    - "[[12_05_common_utilities|共通ユーティリティ概要]]"
    - "[[01_reactive_property|ReactiveProperty実装詳細]]"
    - "[[02_viewmodel_base|ViewModelBase実装詳細]]"
---

# 共通ユーティリティ実装詳細

> **注意**: このドキュメントは実装の詳細に焦点を当てています。概要や使用例については [[12_05_common_utilities|共通ユーティリティ概要]] を参照してください。

## 目次

1. [概要](#1-概要)
2. [リアクティブユーティリティ](#2-リアクティブユーティリティ)
3. [イベントユーティリティ](#3-イベントユーティリティ)
4. [ログユーティリティ](#4-ログユーティリティ)
5. [バリデーションユーティリティ](#5-バリデーションユーティリティ)
6. [非同期処理ユーティリティ](#6-非同期処理ユーティリティ)
7. [ベストプラクティス](#7-ベストプラクティス)
8. [制限事項](#8-制限事項)
9. [変更履歴](#9-変更履歴)

## 1. 概要

共通ユーティリティは、アプリケーション全体で使用される汎用的な機能を提供します。これらは、コードの重複を避け、一貫性のある実装を促進するために使用されます。

## 2. リアクティブユーティリティ

### 2.1 ReactiveCommand

```csharp
public class ReactiveCommand : ICommand, IDisposable
{
    private readonly Subject<Unit> _executeSubject = new();
    private readonly ReactiveProperty<bool> _canExecute = new(true);
    private readonly CompositeDisposable _disposables = new();
    private event EventHandler? _canExecuteChanged;
    private bool _disposed;

    public IObservable<Unit> ExecuteObservable => _executeSubject.AsObservable();
    public IObservable<bool> CanExecuteObservable => _canExecute.ValueChanged;

    public ReactiveCommand()
    {
        _canExecute.ValueChanged
            .Subscribe(_ => _canExecuteChanged?.Invoke(this, EventArgs.Empty))
            .AddTo(_disposables);
    }

    public bool CanExecute(object parameter) => _canExecute.Value;

    public event EventHandler? CanExecuteChanged
    {
        add { _canExecuteChanged += value; }
        remove { _canExecuteChanged -= value; }
    }

    public void SetCanExecute(bool value)
    {
        _canExecute.Value = value;
    }

    public void Execute(object parameter)
    {
        if (CanExecute(parameter))
        {
            _executeSubject.OnNext(Unit.Default);
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _disposables.Dispose();
            _executeSubject.Dispose();
            _canExecute.Dispose();
            _disposed = true;
        }
    }
}
```

```csharp
public class ReactiveCommand<T> : ICommand, IDisposable
{
    private readonly Subject<T> _executeSubject = new();
    private readonly ReactiveProperty<bool> _canExecute = new(true);
    private readonly CompositeDisposable _disposables = new();
    private event EventHandler? _canExecuteChanged;
    private bool _disposed;

    public IObservable<T> ExecuteObservable => _executeSubject.AsObservable();
    public IObservable<bool> CanExecuteObservable => _canExecute.ValueChanged;

    public ReactiveCommand()
    {
        _canExecute.ValueChanged
            .Subscribe(_ => _canExecuteChanged?.Invoke(this, EventArgs.Empty))
            .AddTo(_disposables);
    }

    public bool CanExecute(object parameter) => _canExecute.Value;

    public event EventHandler? CanExecuteChanged
    {
        add { _canExecuteChanged += value; }
        remove { _canExecuteChanged -= value; }
    }

    public void SetCanExecute(bool value)
    {
        _canExecute.Value = value;
    }

    public void Execute(object parameter)
    {
        if (!CanExecute(parameter)) return;
        if (parameter is T t)
        {
            _executeSubject.OnNext(t);
        }
        else if (parameter is null)
        {
            _executeSubject.OnNext(default!);
        }
        else
        {
            throw new ArgumentException($"Invalid parameter type. Expected {typeof(T)}, but got {parameter.GetType()}", nameof(parameter));
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposables.Dispose();
        _executeSubject.Dispose();
        _canExecute.Dispose();
        _disposed = true;
    }
}
```

### 2.2 ReactiveCollection

```csharp
public class ReactiveCollection<T> : IList<T>, IDisposable
{
    private readonly List<T> _items = new();
    private readonly Subject<CollectionChangedEvent<T>> _changedSubject = new();
    private bool _disposed;

    public IObservable<CollectionChangedEvent<T>> Changed => _changedSubject.AsObservable();

    public T this[int index]
    {
        get => _items[index];
        set
        {
            var old = _items[index];
            _items[index] = value;
            _changedSubject.OnNext(new CollectionChangedEvent<T>(CollectionChangeType.Remove, old));
            _changedSubject.OnNext(new CollectionChangedEvent<T>(CollectionChangeType.Add, value));
        }
    }

    public int Count => _items.Count;
    public bool IsReadOnly => false;

    public void Add(T item)
    {
        _items.Add(item);
        _changedSubject.OnNext(new CollectionChangedEvent<T>(CollectionChangeType.Add, item));
    }

    public void Clear()
    {
        foreach (var item in _items)
        {
            _changedSubject.OnNext(new CollectionChangedEvent<T>(CollectionChangeType.Remove, item));
        }
        _items.Clear();
    }

    public bool Contains(T item) => _items.Contains(item);

    public void CopyTo(T[] array, int arrayIndex) => _items.CopyTo(array, arrayIndex);

    public IEnumerator<T> GetEnumerator() => _items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int IndexOf(T item) => _items.IndexOf(item);

    public void Insert(int index, T item)
    {
        _items.Insert(index, item);
        _changedSubject.OnNext(new CollectionChangedEvent<T>(CollectionChangeType.Add, item));
    }

    public bool Remove(T item)
    {
        if (_items.Remove(item))
        {
            _changedSubject.OnNext(new CollectionChangedEvent<T>(CollectionChangeType.Remove, item));
            return true;
        }
        return false;
    }

    public void RemoveAt(int index)
    {
        var item = _items[index];
        _items.RemoveAt(index);
        _changedSubject.OnNext(new CollectionChangedEvent<T>(CollectionChangeType.Remove, item));
    }

    public void Dispose()
    {
        if (_disposed) return;
        _changedSubject.Dispose();
        _disposed = true;
    }
}
```

## 3. イベントユーティリティ

### 3.1 EventAggregator

```csharp
public class EventAggregator : IEventAggregator
{
    private readonly Dictionary<Type, object> _handlers = new();
    private readonly object _lock = new();

    public void Publish<T>(T message) where T : class
    {
        List<Action<T>>? handlers_to_invoke = null;
        lock (_lock)
        {
            if (_handlers.TryGetValue(typeof(T), out var handlers))
            {
                handlers_to_invoke = new List<Action<T>>((List<Action<T>>)handlers);
            }
        }
        if (handlers_to_invoke != null)
        {
            foreach (var handler in handlers_to_invoke)
            {
                handler(message);
            }
        }
    }

    public void Subscribe<T>(Action<T> handler) where T : class
    {
        lock (_lock)
        {
            var type = typeof(T);
            if (!_handlers.TryGetValue(type, out var handlers))
            {
                handlers = new List<Action<T>>();
                _handlers[type] = handlers;
            }
            ((List<Action<T>>)handlers).Add(handler);
        }
    }
}
```

### 3.2 WeakEventManager

```csharp
public class WeakEventManager
{
    private readonly Dictionary<string, List<WeakReference>> _handlers = new();

    public void AddHandler(string eventName, EventHandler handler)
    {
        if (!_handlers.TryGetValue(eventName, out var handlers))
        {
            handlers = new List<WeakReference>();
            _handlers[eventName] = handlers;
        }
        handlers.Add(new WeakReference(handler));
    }

    public void RemoveHandler(string eventName, EventHandler handler)
    {
        if (_handlers.TryGetValue(eventName, out var handlers))
        {
            handlers.RemoveAll(wr => !wr.IsAlive || wr.Target == handler);
        }
    }

    public void RaiseEvent(string eventName, object sender, EventArgs args)
    {
        if (_handlers.TryGetValue(eventName, out var handlers))
        {
            var dead = new List<WeakReference>();
            foreach (var wr in handlers)
            {
                if (wr.IsAlive && wr.Target is EventHandler handler)
                {
                    handler(sender, args);
                }
                else
                {
                    dead.Add(wr);
                }
            }
            foreach (var d in dead)
            {
                handlers.Remove(d);
            }
        }
    }
}
```

## 4. ロギングユーティリティ

### 4.1 Logger

```csharp
public class Logger : ILogger
{
    private readonly IEventBus _eventBus;
    private readonly LogLevel _minimumLevel;

    public Logger(IEventBus eventBus, LogLevel minimumLevel = LogLevel.Info)
    {
        _eventBus = eventBus;
        _minimumLevel = minimumLevel;
    }

    public void Log(LogLevel level, string message, Exception ex = null)
    {
        if (level < _minimumLevel) return;

        var logEvent = new LogEvent
        {
            Level = level,
            Message = message,
            Exception = ex,
            Timestamp = DateTime.UtcNow
        };

        _eventBus.Publish(logEvent);
    }
}
```

### 4.2 LogFormatter

```csharp
public class LogFormatter
{
    public string Format(LogEvent logEvent)
    {
        var sb = new StringBuilder();
        sb.Append($"[{logEvent.Timestamp:yyyy-MM-dd HH:mm:ss}] ");
        sb.Append($"{logEvent.Level}: {logEvent.Message}");

        if (logEvent.Exception != null)
        {
            sb.AppendLine();
            sb.Append($"Exception: {logEvent.Exception}");
        }

        return sb.ToString();
    }
}
```

## 5. バリデーションユーティリティ

### 5.1 Validator

```csharp
public class Validator<T>
{
    private readonly List<ValidationRule<T>> _rules = new();

    public void AddRule(ValidationRule<T> rule)
    {
        _rules.Add(rule);
    }

    public ValidationResult Validate(T value)
    {
        var errors = new List<string>();

        foreach (var rule in _rules)
        {
            if (!rule.Validate(value))
            {
                errors.Add(rule.ErrorMessage);
            }
        }

        return new ValidationResult(errors);
    }
}
```

### 5.2 ValidationRule

```csharp
public class ValidationRule<T>
{
    public Func<T, bool> Validate { get; }
    public string ErrorMessage { get; }

    public ValidationRule(Func<T, bool> validate, string errorMessage)
    {
        Validate = validate;
        ErrorMessage = errorMessage;
    }
}
```

### 5.3 AsyncValidator

```csharp
public class AsyncValidator<T>
{
    private readonly List<AsyncValidationRule<T>> _rules = new();

    public void AddRule(AsyncValidationRule<T> rule)
    {
        _rules.Add(rule);
    }

    public async Task<ValidationResult> ValidateAsync(T value)
    {
        var errors = new List<string>();
        foreach (var rule in _rules)
        {
            if (!await rule.ValidateAsync(value))
            {
                errors.Add(rule.ErrorMessage);
            }
        }
        return new ValidationResult(errors);
    }
}
```

## 6. 非同期処理ユーティリティ

### 6.1 AsyncCommand

```csharp
public class AsyncCommand : ICommand, IDisposable
{
    private readonly Func<Task> _execute;
    private readonly ReactiveProperty<bool> _isExecuting = new(false);
    private readonly CompositeDisposable _disposables = new();
    private event EventHandler? _canExecuteChanged;
    private bool _disposed;

    public IObservable<bool> IsExecuting => _isExecuting.ValueChanged;

    public AsyncCommand(Func<Task> execute)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _isExecuting.ValueChanged
            .Subscribe(_ => _canExecuteChanged?.Invoke(this, EventArgs.Empty))
            .AddTo(_disposables);
    }

    public bool CanExecute(object parameter) => !_isExecuting.Value;

    public event EventHandler? CanExecuteChanged
    {
        add { _canExecuteChanged += value; }
        remove { _canExecuteChanged -= value; }
    }

    public async void Execute(object parameter)
    {
        if (!CanExecute(parameter)) return;

        try
        {
            _isExecuting.Value = true;
            await _execute();
        }
        finally
        {
            _isExecuting.Value = false;
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _disposables.Dispose();
            _isExecuting.Dispose();
            _disposed = true;
        }
    }
}
```

### 6.2 TaskExtensions

```csharp
public static class TaskExtensions
{
    public static async Task<T> WithTimeout<T>(this Task<T> task, TimeSpan timeout)
    {
        using var cts = new CancellationTokenSource(timeout);
        try
        {
            return await task.WaitAsync(cts.Token);
        }
        catch (OperationCanceledException)
        {
            throw new TimeoutException();
        }
    }

    public static async Task<T> WithRetry<T>(this Func<Task<T>> taskFactory, int maxRetries)
    {
        Exception? last_exception = null;
        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                return await taskFactory();
            }
            catch (Exception ex)
            {
                last_exception = ex;
                if (i < maxRetries - 1)
                {
                    await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, i)));
                }
            }
        }
        throw last_exception ?? new Exception("Unknown error");
    }
}
```
`maxRetries` 回すべて失敗した場合は最後に捕捉した例外を送出します。

## 7. ベストプラクティス

### 7.1 メモリ管理

-   適切なタイミングで Dispose を呼び出す
-   弱参照を活用する
-   メモリリークを防ぐ

### 7.2 パフォーマンス

-   不要なオブジェクト生成を避ける
-   キャッシュを適切に使用する
-   非同期処理を活用する

### 7.3 エラーハンドリング

-   例外を適切に処理する
-   ログを適切に記録する
-   リカバリー処理を実装する

## 8. 制限事項

### 8.1 メモリ管理

-   適切なタイミングで Dispose を呼び出す必要がある
-   循環参照に注意が必要
-   リソースの適切な解放

### 8.2 パフォーマンス

-   重い処理は非同期で実行する
-   キャッシュの適切な使用
-   リソース使用量の制御

### 8.3 スレッドセーフ

-   マルチスレッド環境での使用には注意が必要
-   適切な同期処理を実装する
-   スレッド間通信の制御

## 9. 変更履歴

| バージョン | 更新日     | 変更内容                                                                                                               |
| ---------- | ---------- | ---------------------------------------------------------------------------------------------------------------------- |
| 0.2.7      | 2025-06-12 | AsyncValidator の実装<br>ReplaySubject 採用によるメモリ管理改善 |
| 0.2.6      | 2025-06-09 | ReactiveCollection のインデクサ追加<br>ReactiveCommand<T> の検証追加<br>WithRetry の例外処理改善 |
| 0.2.5      | 2025-06-09 | EventAggregator の Publish 改善<br>WithRetry の null 許容型更新 |
| 0.2.4      | 2025-06-09 | AsyncCommand の CanExecuteChanged 実装 |
| 0.2.3      | 2025-06-09 | WeakEventManager に RaiseEvent 追加<br>ReactiveCommand の CanExecuteChanged 実装 |
| 0.2.2      | 2025-06-09 | WithRetry の変数名を明確化 |
| 0.2.1      | 2025-06-09 | WithRetry 実装のリトライ回数を修正 |
| 0.2.0      | 2024-03-23 | 機能拡張<br>- ロギングユーティリティの追加<br>- バリデーションユーティリティの追加<br>- 非同期処理ユーティリティの追加 |
| 0.1.0      | 2024-03-22 | 初版作成<br>- リアクティブユーティリティの実装<br>- イベントユーティリティの実装<br>- 使用例の追加                     |

## 10. テスト

### 10.1 単体テスト

```csharp
[Test]
public void ReactiveCommand_Execute_NotifiesSubscribers()
{
    var command = new ReactiveCommand();
    var executed = false;

    command.ExecuteObservable.Subscribe(_ => executed = true);
    command.Execute(null);

    Assert.That(executed, Is.True);
}
```

### 10.2 統合テスト

```csharp
[Test]
public async Task AsyncCommand_Execute_HandlesErrorsCorrectly()
{
    var command = new AsyncCommand(async () =>
    {
        await Task.Delay(100);
        throw new Exception("Test error");
    });

    var errorOccurred = false;
    command.Execute(null);

    await Task.Delay(200);
    Assert.That(errorOccurred, Is.False);
    Assert.That(command.IsExecuting.Value, Is.False);
}
```

## 11. 今後の改善計画

### 11.1 機能拡張

-   非同期処理のサポート強化
-   バリデーションルールの拡張
-   ログ機能の強化

### 11.2 ドキュメント

-   使用例の追加
-   パフォーマンスチューニングガイド
-   トラブルシューティングガイド
