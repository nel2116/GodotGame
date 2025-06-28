---
title: 複合ディスポーザブル
version: 0.1.0
status: draft
updated: 2024-03-21
tags:
    - API
    - Reactive
    - Core
    - Disposable
linked_docs:
    - "[[ReactiveSystem]]"
    - "[[ReactiveProperty]]"
    - "[[CoreEventSystem]]"
    - "[[CommonEventSystem]]"
---

# 複合ディスポーザブル

## 目次

1. [概要](#概要)
2. [クラス定義](#クラス定義)
3. [主要コンポーネント](#主要コンポーネント)
4. [使用例](#使用例)
5. [制限事項](#制限事項)
6. [変更履歴](#変更履歴)

## 概要

複合ディスポーザブルは、複数の`IDisposable`リソースをまとめて管理するためのクラスです。以下の機能を提供します：

-   リソースの追加
-   リソースの削除
-   リソースの一括解放
-   リソースの状態管理

## クラス定義

### CompositeDisposable

複合ディスポーザブルの基本クラスです。

```csharp
public class CompositeDisposable : IDisposable
{
    private readonly List<IDisposable> _disposables;
    private bool _isDisposed;

    public CompositeDisposable()
    {
        _disposables = new List<IDisposable>();
        _isDisposed = false;
    }

    public void Add(IDisposable disposable)
    {
        if (_isDisposed)
        {
            disposable.Dispose();
            return;
        }

        _disposables.Add(disposable);
    }

    public void Remove(IDisposable disposable)
    {
        if (_isDisposed)
            return;

        _disposables.Remove(disposable);
    }

    public void Clear()
    {
        if (_isDisposed)
            return;

        foreach (var disposable in _disposables)
        {
            disposable.Dispose();
        }

        _disposables.Clear();
    }

    public void Dispose()
    {
        if (_isDisposed)
            return;

        Clear();
        _isDisposed = true;
    }
}
```

## 主要コンポーネント

### CompositeDisposableExtensions

複合ディスポーザブルの拡張メソッドを提供するクラスです。

```csharp
public static class CompositeDisposableExtensions
{
    public static T AddTo<T>(this T disposable, CompositeDisposable composite) where T : IDisposable;
    public static void AddRange(this CompositeDisposable composite, IEnumerable<IDisposable> disposables);
    public static void RemoveRange(this CompositeDisposable composite, IEnumerable<IDisposable> disposables);
}
```

## 使用例

### 基本的な使用

```csharp
public class PlayerController : MonoBehaviour
{
    private readonly CompositeDisposable _disposables = new();

    private void Start()
    {
        // イベントの購読
        _eventBus.Subscribe<PlayerDamagedEvent>(OnPlayerDamaged)
            .AddTo(_disposables);

        // プロパティの監視
        _health.Subscribe(OnHealthChanged)
            .AddTo(_disposables);
    }

    private void OnDestroy()
    {
        _disposables.Dispose();
    }
}
```

### リソースの管理

```csharp
public class ResourceManager : MonoBehaviour
{
    private readonly CompositeDisposable _disposables = new();

    public void LoadResources()
    {
        // リソースの読み込み
        var resource1 = LoadResource("resource1");
        var resource2 = LoadResource("resource2");

        // リソースの追加
        _disposables.AddRange(new[] { resource1, resource2 });
    }

    public void UnloadResources()
    {
        _disposables.Clear();
    }

    private void OnDestroy()
    {
        _disposables.Dispose();
    }
}
```

## 制限事項

-   スレッドセーフな実装が必要な箇所では、必ず提供されている同期メカニズムを使用してください
-   リソースの解放は適切なタイミングで行ってください
-   リソースの追加は必要最小限に抑えてください
-   リソースの削除は、必ず`Remove`メソッドを使用してください
-   リソースの一括解放は、必ず`Clear`メソッドを使用してください
-   リソースの状態管理は、必ず`IsDisposed`プロパティを使用してください
-   リソースの追加は、必ず`AddTo`メソッドを使用してください

## 変更履歴

| バージョン | 更新日     | 変更内容 |
| ---------- | ---------- | -------- |
| 0.1.0      | 2024-03-21 | 初版作成 |
