---
title: リアクティブシステム
version: 0.3.0
status: draft
updated: 2024-03-21
tags:
    - API
    - Reactive
    - Events
    - Core
    - Tests
linked_docs:
    - "[[01_reactive_property]]"
    - "[[02_composite_disposable]]"
    - "[[03_event_bus]]"
    - "[[ReactiveSystemTestResults]]"
---

# リアクティブシステム

## 目次

1. [概要](#概要)
2. [リアクティブプロパティ](#リアクティブプロパティ)
3. [イベントシステム](#イベントシステム)
4. [リソース管理](#リソース管理)
5. [使用例](#使用例)
6. [制限事項](#制限事項)
7. [テスト](#テスト)
8. [変更履歴](#変更履歴)

## 概要

リアクティブシステムは、値の変更通知とイベント処理を提供するコアシステムです。主に以下の機能を提供します：

-   リアクティブプロパティによる値変更通知
-   イベントバスによるイベント発行・購読
-   リソースの自動解放管理

## リアクティブプロパティ

### IReactiveProperty<T>

値の変更を通知するリアクティブプロパティのインターフェースです。

```csharp
public interface IReactiveProperty<T> : IDisposable
{
    T Value { get; set; }
    IDisposable Subscribe(Action<T> onNext);
}
```

### ReactiveProperty<T>

`IReactiveProperty<T>`の実装クラスです。値が変更された時に購読者に通知します。

```csharp
public class ReactiveProperty<T> : IReactiveProperty<T>
{
    private T _value;
    private readonly Subject<T> _raw_subject = new();
    private readonly ISubject<T> _sync_subject;
    private readonly object _sync_lock = new();
    private bool _is_disposed;

    public T Value { get; set; }
    public IDisposable Subscribe(Action<T> onNext);
}
```

主な特徴：

-   スレッドセーフな実装（`Subject.Synchronize`を使用）
-   同一値設定時の通知制御
-   リソース解放時の適切な処理

## イベントシステム

### IGameEvent

ゲームイベントの基底インターフェースです。

```csharp
public interface IGameEvent
{
    DateTime Timestamp { get; }
}
```

### GameEvent

`IGameEvent`の実装クラスです。イベントの発生時刻を管理します。

```csharp
public abstract class GameEvent : IGameEvent
{
    public DateTime Timestamp { get; } = DateTime.UtcNow;
}
```

### IGameEventBus

イベントバスのインターフェースです。

```csharp
public interface IGameEventBus
{
    void Publish<T>(T evt) where T : GameEvent;
    IObservable<T> GetEventStream<T>() where T : GameEvent;
}
```

### GameEventBus

`IGameEventBus`の実装クラスです。イベントの発行と購読を管理します。

```csharp
public class GameEventBus : IGameEventBus
{
    private readonly ConcurrentDictionary<Type, ISubject<GameEvent>> _subjects = new();

    public void Publish<T>(T evt) where T : GameEvent;
    public IObservable<T> GetEventStream<T>() where T : GameEvent;
}
```

主な特徴：

-   スレッドセーフな実装（`ConcurrentDictionary`と`Subject.Synchronize`を使用）
-   型ごとのイベントストリーム管理
-   効率的なメモリ使用

## リソース管理

### CompositeDisposable

複数の`IDisposable`リソースをまとめて管理するクラスです。

```csharp
public class CompositeDisposable : IDisposable
{
    private readonly List<IDisposable> _disposables = new();
    private bool _is_disposed;
    private readonly object _sync_lock = new();

    public int DisposableCount { get; }
    public void Add(IDisposable disposable);
    public void AddRange(IEnumerable<IDisposable> disposables);
    public bool Remove(IDisposable disposable);
    public void Clear();
    public void Dispose();
}
```

主な特徴：

-   スレッドセーフな実装
-   循環参照の防止
-   効率的なリソース管理
-   一括操作のサポート

## 使用例

### リアクティブプロパティの使用例

```csharp
// プロパティの作成
var health = new ReactiveProperty<int>(100);

// 値の変更を購読
var subscription = health.Subscribe(newValue =>
{
    Debug.Log($"Health changed to: {newValue}");
});

// 値の変更
health.Value = 80; // 購読者に通知される

// リソースの解放
subscription.Dispose();
```

### イベントシステムの使用例

```csharp
// イベントの定義
public class PlayerDamagedEvent : GameEvent
{
    public int Damage { get; }
    public PlayerDamagedEvent(int damage) => Damage = damage;
}

// イベントバスの使用
var eventBus = new GameEventBus();

// イベントの購読
var subscription = eventBus.GetEventStream<PlayerDamagedEvent>()
    .Subscribe(evt => Debug.Log($"Player took {evt.Damage} damage"));

// イベントの発行
eventBus.Publish(new PlayerDamagedEvent(10));

// リソースの解放
subscription.Dispose();
```

## 制限事項

1. スレッドセーフ

    - `GameEventBus`は`ConcurrentDictionary`と`Subject.Synchronize`でスレッドセーフに実装
    - `ReactiveProperty<T>`は`Subject.Synchronize`でスレッドセーフに実装
    - `CompositeDisposable`はロックベースでスレッドセーフに実装

2. メモリ管理

    - 購読は明示的に解除する必要があります
    - `CompositeDisposable`を使用して複数の購読を管理することを推奨
    - イベントバスは型ごとにストリームを管理

3. パフォーマンス
    - 大量のイベント発行時は注意が必要
    - 不要な購読は早期に解除
    - 同一値設定時の通知制御で不要な通知を防止

## テスト

### テストカバレッジ

各コンポーネントに対して、以下のテストケースが実装されています：

#### ReactiveProperty<T>

1. 値の変更通知

    - 初期値の設定と検証
    - 値変更時の購読者への通知
    - 複数回の値変更時の通知順序
    - 同一値設定時の通知制御
    - リソース解放後の動作

2. スレッドセーフ性
    - 複数スレッドからの値変更
    - 複数購読者への同時通知
    - アトミックな値更新

#### GameEventBus

1. イベント発行・購読

    - イベント発行時の購読者への通知
    - 複数タイプのイベント処理
    - 未購読タイプのイベント処理
    - イベントストリームの取得と購読

2. パフォーマンス
    - 大量イベント発行時の処理
    - 購読者への通知速度

#### CompositeDisposable

1. リソース管理

    - 複数リソースの追加と解放
    - 一括追加（AddRange）の動作
    - 個別リソースの削除
    - 全リソースの一括解放（Clear）

2. スレッドセーフ性

    - 複数スレッドからのリソース追加
    - 並行処理時の整合性

3. エッジケース
    - 大量リソースの処理
    - 循環参照の安全な処理
    - 削除されたリソースの解放状態

### テスト実行方法

```bash
# テストの実行
dotnet test Tests/Core/CoreTests.csproj
```

### テストの追加

新しいテストケースを追加する場合は、以下の点に注意してください：

1. テストクラスは`Tests.Core`名前空間に配置
2. テストメソッドには`[Test]`属性を付与
3. テスト名は`対象_条件_期待結果`の形式で命名
4. アサーションは NUnit の`Assert`クラスを使用
5. スレッドセーフ性のテストには`Parallel.For`を使用
6. パフォーマンステストには`[MaxTime]`属性を付与

## 変更履歴

| バージョン | 更新日     | 変更内容                                                                                                                             |
| ---------- | ---------- | ------------------------------------------------------------------------------------------------------------------------------------ |
| 0.3.0      | 2024-03-21 | コアシステムの実装詳細を追加<br>- スレッドセーフ性の実装詳細を更新<br>- メモリ管理の説明を改善<br>- パフォーマンス最適化の説明を追加 |
| 0.2.0      | 2024-03-21 | テストセクションの更新<br>- テストカバレッジの詳細化<br>- スレッドセーフ性のテスト追加<br>- パフォーマンステストの追加               |
| 0.1.0      | 2024-03-21 | 初版作成                                                                                                                             |
