---
title: 共通ユーティリティ概要
version: 0.2.0
status: draft
updated: 2024-03-23
tags:
    - Core
    - Utility
    - Overview
linked_docs:
    - "[[12_01_mvvm_rx_architecture|MVVM+RXアーキテクチャ]]"
    - "[[12_02_basic_design|MVVM+RX基本設計書]]"
    - "[[12_03_detailed_design/01_core_components/05_common_utilities|共通ユーティリティ実装詳細]]"
    - "[[12_04_system_integration|システム間連携]]"
---

# 共通ユーティリティ概要

> **注意**: このドキュメントは概要と使用例に焦点を当てています。実装の詳細については [[12_03_detailed_design/01_core_components/05_common_utilities|共通ユーティリティ実装詳細]] を参照してください。

## 目次

1. [概要](#1-概要)
2. [ユーティリティ一覧](#2-ユーティリティ一覧)
3. [使用例](#3-使用例)
4. [パフォーマンス最適化](#4-パフォーマンス最適化)
5. [エラー処理](#5-エラー処理)
6. [ベストプラクティス](#6-ベストプラクティス)
7. [テスト](#7-テスト)
8. [制限事項](#8-制限事項)
9. [変更履歴](#9-変更履歴)

## 1. リアクティブユーティリティ

### 1.1 ObservableExtensions

リアクティブプログラミングをサポートする拡張メソッドを提供します。

```csharp
public static class ObservableExtensions
{
    // ストリームのフィルタリング
    public static IObservable<T> WhereNotNull<T>(this IObservable<T> source);

    // 非同期処理の統合
    public static IObservable<T> SelectAsync<T, R>(this IObservable<T> source, Func<T, Task<R>> selector);

    // エラーハンドリング
    public static IObservable<T> RetryWithBackoff<T>(this IObservable<T> source, int maxRetries);
}
```

### 1.2 ReactiveProperty

プロパティの変更を監視し、通知を行うためのヘルパークラスです。

```csharp
public class ReactiveProperty<T>
{
    public T Value { get; set; }
    public IObservable<T> ValueChanged { get; }

    // 値の検証
    public void Validate(Func<T, bool> validator);

    // 値の変換
    public ReactiveProperty<R> Select<R>(Func<T, R> selector);
}
```

## 2. イベントユーティリティ

### 2.1 EventBus

イベントの送受信を管理するユーティリティクラスです。

```csharp
public class EventBus
{
    // イベントの発行
    public void Publish<T>(T event) where T : IEvent;

    // イベントの購読
    public IDisposable Subscribe<T>(Action<T> handler) where T : IEvent;

    // イベントのフィルタリング
    public IDisposable Subscribe<T>(Action<T> handler, Func<T, bool> filter) where T : IEvent;
}
```

### 2.2 EventAggregator

複数のイベントソースを統合するユーティリティです。

```csharp
public class EventAggregator
{
    // イベントの集約
    public IObservable<T> Aggregate<T>(params IObservable<T>[] sources);

    // イベントの変換
    public IObservable<R> Transform<T, R>(IObservable<T> source, Func<T, R> transformer);
}
```

## 3. ロギングユーティリティ

### 3.1 Logger

アプリケーション全体で使用するロギングユーティリティです。

```csharp
public static class Logger
{
    // ログレベルの定義
    public enum LogLevel { Debug, Info, Warning, Error }

    // ログの出力
    public static void Log(LogLevel level, string message, Exception ex = null);

    // パフォーマンス計測
    public static IDisposable MeasureTime(string operationName);
}
```

## 4. 検証ユーティリティ

### 4.1 Validator

データの検証を行うユーティリティクラスです。

```csharp
public static class Validator
{
    // 必須チェック
    public static bool IsRequired<T>(T value);

    // 範囲チェック
    public static bool IsInRange<T>(T value, T min, T max) where T : IComparable<T>;

    // 正規表現チェック
    public static bool MatchesPattern(string value, string pattern);
}
```

## 5. 非同期処理ユーティリティ

### 5.1 AsyncHelper

非同期処理をサポートするユーティリティクラスです。

```csharp
public static class AsyncHelper
{
    // タイムアウト付きの非同期処理
    public static async Task<T> WithTimeout<T>(Task<T> task, TimeSpan timeout);

    // リトライ処理
    public static async Task<T> RetryAsync<T>(Func<Task<T>> action, int maxRetries);

    // 並列処理
    public static async Task<T[]> WhenAll<T>(IEnumerable<Task<T>> tasks);
}
```

## 6. 使用例

### 6.1 リアクティブプロパティの使用

```csharp
public class GameViewModel
{
    private readonly ReactiveProperty<int> _score = new();

    public GameViewModel()
    {
        _score.ValueChanged
            .Where(s => s > 100)
            .Subscribe(s => OnHighScoreAchieved(s));
    }
}
```

### 6.2 イベントバスの使用

```csharp
public class GameManager
{
    private readonly EventBus _eventBus;

    public void StartGame()
    {
        _eventBus.Publish(new GameStartedEvent());
    }

    public void SubscribeToEvents()
    {
        _eventBus.Subscribe<GameOverEvent>(OnGameOver);
    }
}
```

## 7. ベストプラクティス

### 7.1 ユーティリティの選択

-   適切なユーティリティを選択し、一貫性のある使用を心がける
-   必要に応じて新しいユーティリティを追加する

### 7.2 パフォーマンス

-   メモリリークを防ぐため、適切なタイミングで Dispose を呼び出す
-   非同期処理の適切な管理

### 7.3 エラーハンドリング

-   適切なエラーハンドリングの実装
-   ログの適切な出力

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

| バージョン | 更新日     | 変更内容                                                                             |
| ---------- | ---------- | ------------------------------------------------------------------------------------ |
| 0.2.0      | 2024-03-23 | 機能拡張<br>- ユーティリティの追加<br>- 使用例の追加<br>- パフォーマンス最適化の追加 |
| 0.1.0      | 2024-03-21 | 初版作成<br>- 基本実装の追加<br>- エラー処理の定義<br>- ベストプラクティスの追加     |
