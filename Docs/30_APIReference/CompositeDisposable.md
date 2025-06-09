---
title: 複合リソース管理
version: 0.1.0
status: draft
updated: 2024-03-21
tags:
    - API
    - Resource
    - Core
    - Reactive
linked_docs:
    - "[[ReactiveSystem]]"
    - "[[ViewModelSystem]]"
    - "[[00_index]]"
---

# 複合リソース管理

## 目次

1. [概要](#概要)
2. [インターフェース](#インターフェース)
3. [実装詳細](#実装詳細)
4. [使用方法](#使用方法)
5. [制限事項](#制限事項)
6. [変更履歴](#変更履歴)

## 概要

複合リソース管理は、複数の`IDisposable`リソースをまとめて管理するための基盤を提供します。主に以下の機能を提供します：

-   複数リソースの一括管理
-   スレッドセーフな実装
-   リソースの自動解放
-   循環参照の防止
-   効率的なメモリ管理

## インターフェース

### IDisposable

```csharp
public interface IDisposable
{
    void Dispose();
}
```

## 実装詳細

### CompositeDisposable

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
    -   ロックベースの同期化
    -   アトミックな操作保証
-   効率的なリソース管理
    -   一括操作のサポート
    -   個別リソースの制御
-   メモリリーク防止
    -   循環参照の検出
    -   リソースの確実な解放
-   拡張性
    -   カスタムリソースのサポート
    -   柔軟な管理機能

## 使用方法

### 基本的な使用例

```csharp
// 複合リソースの作成
var disposables = new CompositeDisposable();

// リソースの追加
var subscription1 = observable1.Subscribe(x => { });
var subscription2 = observable2.Subscribe(x => { });

disposables.Add(subscription1);
disposables.Add(subscription2);

// 複数リソースの一括追加
var subscriptions = new[]
{
    observable3.Subscribe(x => { }),
    observable4.Subscribe(x => { })
};
disposables.AddRange(subscriptions);

// リソースの解放
disposables.Dispose();
```

### ViewModel での使用例

```csharp
public class PlayerViewModel : ViewModelBase
{
    private readonly ReactiveProperty<int> _health;
    public IReactiveProperty<int> Health => _health;

    public PlayerViewModel(IGameEventBus eventBus) : base(eventBus)
    {
        _health = new ReactiveProperty<int>(100)
            .AddTo(Disposables); // 自動的にDisposablesに追加

        // イベントの購読も自動的に管理
        SubscribeToEvent<PlayerDamagedEvent>(OnPlayerDamaged);
    }

    private void OnPlayerDamaged(PlayerDamagedEvent evt)
    {
        _health.Value -= evt.Damage;
    }
}
```

### 拡張メソッドの使用例

```csharp
public static class DisposableExtensions
{
    public static T AddTo<T>(this T disposable, CompositeDisposable disposables)
        where T : IDisposable
    {
        disposables.Add(disposable);
        return disposable;
    }
}
```

## 制限事項

1. **スレッドセーフ**

    - リソースの追加と削除はスレッドセーフです
    - リソースの解放中は新しいリソースを追加できません
    - 複数のスレッドからの同時操作は避けてください

2. **メモリ管理**

    - リソースは明示的に追加する必要があります
    - 不要なリソースは早期に解放してください
    - 循環参照に注意してください

3. **パフォーマンス**

    - 大量のリソース管理時は注意が必要です
    - 不要なリソースは早期に解放してください
    - リソースの追加/削除は最小限に抑えてください

4. **例外処理**

    - リソース解放時の例外は適切に処理してください
    - 解放済みリソースへの操作は`ObjectDisposedException`をスロー
    - リソース追加時の例外は呼び出し元で処理してください

## 変更履歴

| バージョン | 更新日     | 変更内容 |
| ---------- | ---------- | -------- |
| 0.1.0      | 2024-03-21 | 初版作成 |
