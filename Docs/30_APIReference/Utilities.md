---
title: Utilities
version: 0.1.0
status: draft
updated: 2024-03-19
tags:
    - API
    - Utilities
    - Core
    - EventSystem
    - Logging
    - Validation
    - Reactive
linked_docs:
    - "[[EventSystem]]"
    - "[[ReactiveSystem]]"
---

# Utilities

## 目次

1. [概要](#概要)
2. [詳細](#詳細)
    - [イベント管理システム](#イベント管理システム)
    - [ロギングシステム](#ロギングシステム)
    - [バリデーションシステム](#バリデーションシステム)
    - [非同期処理](#非同期処理)
    - [リアクティブプログラミング](#リアクティブプログラミング)
3. [使用方法](#使用方法)
4. [制限事項](#制限事項)
5. [変更履歴](#変更履歴)

## 概要

Utilities は、アプリケーション全体で使用される共通機能を提供するユーティリティクラスのコレクションです。主に以下の機能を提供します：

-   イベント管理システム
-   ロギングシステム
-   バリデーションシステム
-   非同期処理
-   リアクティブプログラミング

## 詳細

### イベント管理システム

#### EventAggregator

シンプルなイベント集約クラスです。

```csharp
public class EventAggregator
{
    public void Publish<T>(T message) where T : class;
    public void Subscribe<T>(Action<T> handler) where T : class;
}
```

特徴：

-   ジェネリック型を使用した型安全なイベント処理
-   スレッドセーフな実装（内部でロックを使用）
-   メモリ効率の良い実装（Dictionary を使用）

### ロギングシステム

#### Logger

イベントバスへログを送信するロガーです。

```csharp
public class Logger
{
    public Logger(IGameEventBus eventBus, LogLevel minimumLevel = LogLevel.Info);
    public void Log(LogLevel level, string message, Exception? ex = null);
}
```

#### LogLevel

ログレベルを定義する列挙型です。

```csharp
public enum LogLevel
{
    Debug,
    Info,
    Warning,
    Error,
    Fatal
}
```

### バリデーションシステム

#### Validator

オブジェクトのバリデーションを行うクラスです。

```csharp
public class Validator<T>
{
    public void AddRule(ValidationRule<T> rule);
    public ValidationResult Validate(T value);
}
```

#### ValidationRule

バリデーションルールを定義するクラスです。

```csharp
public class ValidationRule<T>
{
    public ValidationRule(Func<T, bool> condition, string errorMessage);
    public bool Validate(T value);
    public string ErrorMessage { get; }
}
```

### 非同期処理

#### AsyncCommand

非同期コマンドを実装するクラスです。

```csharp
public class AsyncCommand : ICommand
{
    public AsyncCommand(Func<object, Task> execute, Func<object, bool> canExecute = null);
    public bool CanExecute(object parameter);
    public async Task ExecuteAsync(object parameter);
}
```

#### TaskExtensions

Task の拡張メソッドを提供します。

```csharp
public static class TaskExtensions
{
    public static async Task<T> WithTimeout<T>(this Task<T> task, TimeSpan timeout);
    public static async Task<T> WithRetry<T>(this Func<Task<T>> operation, int maxRetries);
}
```

### リアクティブプログラミング

#### ReactiveCommand

リアクティブコマンドを実装するクラスです。

```csharp
public class ReactiveCommand : ICommand
{
    public ReactiveCommand(IObservable<bool> canExecute = null);
    public IObservable<Unit> Execute();
    public IObservable<bool> CanExecute { get; }
}
```

#### ReactiveCollection

リアクティブコレクションを実装するクラスです。

```csharp
public class ReactiveCollection<T> : IList<T>, INotifyCollectionChanged
{
    public ReactiveCollection(IEnumerable<T> items = null);
    public IObservable<CollectionChangedEvent<T>> CollectionChanged { get; }
}
```

## 使用方法

### イベントの使用例

```csharp
// イベントの購読
eventAggregator.Subscribe<MyEvent>(OnMyEvent);

// イベントの発行
eventAggregator.Publish(new MyEvent());
```

### ロギングの使用例

```csharp
// ロガーの初期化
var logger = new Logger(eventBus, LogLevel.Info);

// ログの出力
logger.Log(LogLevel.Info, "アプリケーションが起動しました");
logger.Log(LogLevel.Error, "エラーが発生しました", exception);
```

### バリデーションの使用例

```csharp
var validator = new Validator<User>();
validator.AddRule(new ValidationRule<User>(
    user => !string.IsNullOrEmpty(user.Name),
    "ユーザー名は必須です"
));

var result = validator.Validate(user);
if (!result.IsValid)
{
    foreach (var error in result.Errors)
    {
        Console.WriteLine(error);
    }
}
```

## 制限事項

1. イベント管理

    - 大量のイベント購読はメモリ使用量に注意
    - イベント名の重複に注意

2. ロギング

    - 本番環境では適切なログレベルを設定
    - ログファイルのローテーションに注意

3. バリデーション

    - 複雑なバリデーションルールは別クラスに分離
    - パフォーマンスに影響する可能性のあるルールに注意

4. 非同期処理

    - デッドロックに注意
    - キャンセレーショントークンの適切な使用

5. リアクティブプログラミング
    - メモリリークに注意
    - サブスクリプションの管理に注意

## 変更履歴

| バージョン | 更新日     | 変更内容                                             |
| ---------- | ---------- | ---------------------------------------------------- |
| 0.1.0      | 2024-03-19 | 初版作成<br>- 基本機能の実装<br>- ドキュメントの作成 |
