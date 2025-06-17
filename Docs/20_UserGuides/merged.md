00_index.md

---
title: ユーザーガイド
version: 0.3.0
status: draft
updated: 2025-06-13
tags:
    - UserGuide
    - Documentation
    - Guide
linked_docs:
    - "[[DocumentManagementRules]]"
    - "[[10_CoreDocs/00_index]]"
    - "[[30_APIReference/00_index]]"
    - "[[40_Tutorials/00_index]]"
---

# ユーザーガイド

## 目次

1. [概要](#概要)
2. [ガイド一覧](#ガイド一覧)
3. [使用方法](#使用方法)
4. [制限事項](#制限事項)
5. [変更履歴](#変更履歴)

## 概要

このディレクトリには、プロジェクトのユーザー向けガイドが含まれています。
インストールから設定、基本的な使用方法まで、段階的に説明しています。

## ガイド一覧

### 基本ガイド

-   [[Installation|インストールガイド]]

    -   システム要件
    -   インストール手順
    -   初期設定

-   [[Configuration|設定ガイド]]
    -   基本設定
    -   詳細設定
    -   カスタマイズ

### 機能ガイド

-   [[TestExecutionGuide|テスト実行ガイド]]

-   [[TestGuidelines|テストガイドライン]]
-   [[PerformanceOptimization|パフォーマンス最適化ガイド]]
-   [[Testing|テストガイド]]
-   [[Troubleshooting|トラブルシューティングガイド]]

-   [[BasicFeatures|基本機能]]

    -   キャラクター操作
    -   スキル使用
    -   アイテム管理

-   [[AdvancedFeatures|応用機能]]
    -   カスタマイズ
    -   拡張機能
    -   トラブルシューティング

## 使用方法

各ガイドは独立した Markdown ファイルとして提供されています。
必要に応じて該当するガイドを参照してください。

## 制限事項

-   ガイドは随時更新される可能性があります
-   最新の情報は必ず最新バージョンを参照してください
-   一部の機能は、特定の条件を満たす必要があります

## 変更履歴

| バージョン | 更新日     | 変更内容                 |
| ---------- | ---------- | ------------------------ |
| 0.3.0      | 2025-06-13 | ガイドを追加 |
| 0.2.0      | 2025-06-01 | ガイド一覧の更新と構造化 |
| 0.1.0      | 2025-06-01 | 初版作成                 |

---
PerformanceOptimization.md

---
title: パフォーマンス最適化ガイド
version: 0.2.0
status: draft
updated: 2025-06-13
tags:
    - Guide
    - Performance
    - Optimization
    - System
linked_docs:
    - "[[PlayerSystem]]"
    - "[[PlayerInputSystem]]"
    - "[[PlayerStateSystem]]"
    - "[[PlayerMovementSystem]]"
    - "[[PlayerCombatSystem]]"
    - "[[PlayerAnimationSystem]]"
---

# パフォーマンス最適化ガイド

## 目次

1. [概要](#概要)
2. [詳細](#詳細)
3. [使用方法](#使用方法)
4. [制限事項](#制限事項)
5. [メモリ最適化](#メモリ最適化)
6. [CPU 最適化](#CPU最適化)
7. [GPU 最適化](#GPU最適化)
8. [ネットワーク最適化](#ネットワーク最適化)
9. [アセット最適化](#アセット最適化)
10. [コード最適化](#コード最適化)
11. [プロファイリング](#プロファイリング)
12. [変更履歴](#変更履歴)

## 概要

このドキュメントでは、ゲームのパフォーマンスを向上させるための
各種最適化手法を解説します。

## 詳細

メモリ、CPU、GPU などのリソースを効率的に活用する方法を具体例と
ともに示します。

## 使用方法

必要なセクションを参照し、プロジェクトに合わせて最適化を実施して
ください。

## 制限事項

- ハードウェアやプラットフォームによって効果が異なる場合があります

## メモリ最適化

### オブジェクトプール

**目的**: 頻繁に生成・破棄されるオブジェクトのメモリ割り当てを削減

**実装例**:

```csharp
public class ObjectPool<T> where T : class, new()
{
    private readonly Stack<T> _pool = new Stack<T>();
    private readonly int _maxSize;

    public ObjectPool(int maxSize = 100)
    {
        _maxSize = maxSize;
    }

    public T Get()
    {
        return _pool.Count > 0 ? _pool.Pop() : new T();
    }

    public void Return(T item)
    {
        if (_pool.Count < _maxSize)
        {
            _pool.Push(item);
        }
    }
}

// 使用例
private readonly ObjectPool<GameObject> _gameObjectPool = new ObjectPool<GameObject>();

public GameObject CreateGameObject()
{
    return _gameObjectPool.Get();
}

public void DestroyGameObject(GameObject obj)
{
    _gameObjectPool.Return(obj);
}
```

### リソース管理

**目的**: リソースの効率的な管理とメモリリークの防止

**実装例**:

```csharp
public class ResourceManager : IDisposable
{
    private readonly Dictionary<string, object> _resources = new Dictionary<string, object>();
    private readonly List<IDisposable> _disposables = new List<IDisposable>();

    public T LoadResource<T>(string path) where T : class
    {
        if (_resources.TryGetValue(path, out var resource))
        {
            return resource as T;
        }

        var newResource = LoadResourceFromPath<T>(path);
        _resources[path] = newResource;
        return newResource;
    }

    public void Dispose()
    {
        foreach (var disposable in _disposables)
        {
            disposable.Dispose();
        }
        _disposables.Clear();
        _resources.Clear();
    }
}
```

### ガベージコレクション

**目的**: ガベージコレクションの発生を最小限に抑える

**実装例**:

```csharp
public class GarbageCollectionOptimizer
{
    private readonly List<object> _reusableObjects = new List<object>();
    private readonly int _maxReusableObjects = 1000;

    public void Optimize()
    {
        // 再利用可能なオブジェクトを保持
        if (_reusableObjects.Count < _maxReusableObjects)
        {
            _reusableObjects.Add(new object());
        }

        // 不要なオブジェクトを明示的に解放
        _reusableObjects.Clear();
        GC.Collect();
    }
}
```

## CPU 最適化

### 更新処理の最適化

**目的**: 不要な更新処理を削減し、CPU 負荷を軽減

**実装例**:

```csharp
public class UpdateOptimizer
{
    private bool _needsUpdate = false;
    private float _updateInterval = 0.1f;
    private float _lastUpdateTime = 0f;

    public void Update(float deltaTime)
    {
        if (!_needsUpdate) return;

        _lastUpdateTime += deltaTime;
        if (_lastUpdateTime < _updateInterval) return;

        PerformUpdate();
        _lastUpdateTime = 0f;
        _needsUpdate = false;
    }

    public void RequestUpdate()
    {
        _needsUpdate = true;
    }
}
```

### 計算処理の最適化

**目的**: 計算処理の効率化とキャッシュの活用

**実装例**:

```csharp
public class CalculationOptimizer
{
    private Dictionary<string, object> _cache = new Dictionary<string, object>();
    private float _cacheTimeout = 1.0f;
    private float _lastCacheClear = 0f;

    public T Calculate<T>(string key, Func<T> calculation)
    {
        if (_cache.TryGetValue(key, out var cached))
        {
            return (T)cached;
        }

        var result = calculation();
        _cache[key] = result;
        return result;
    }

    public void UpdateCache(float deltaTime)
    {
        _lastCacheClear += deltaTime;
        if (_lastCacheClear >= _cacheTimeout)
        {
            _cache.Clear();
            _lastCacheClear = 0f;
        }
    }
}
```

### スレッド処理

**目的**: 重い処理を別スレッドで実行し、メインスレッドの負荷を軽減

**実装例**:

```csharp
public class ThreadManager
{
    private readonly ThreadPool _threadPool = new ThreadPool();
    private readonly ConcurrentQueue<Action> _tasks = new ConcurrentQueue<Action>();

    public async Task ExecuteTask(Action task)
    {
        await Task.Run(() =>
        {
            try
            {
                task();
            }
            catch (Exception ex)
            {
                Debug.LogError($"タスク実行中にエラーが発生: {ex.Message}");
            }
        });
    }

    public void AddTask(Action task)
    {
        _tasks.Enqueue(task);
    }

    public void ProcessTasks()
    {
        while (_tasks.TryDequeue(out var task))
        {
            ExecuteTask(task);
        }
    }
}
```

## GPU 最適化

### 描画最適化

**目的**: 描画処理の効率化と GPU 負荷の軽減

**実装例**:

```csharp
public class RenderingOptimizer
{
    private readonly List<Renderer> _visibleRenderers = new List<Renderer>();
    private readonly float _cullingDistance = 100f;
    private readonly Camera _mainCamera;

    public void OptimizeRendering()
    {
        // 視界外のオブジェクトを非表示
        foreach (var renderer in _visibleRenderers)
        {
            var distance = Vector3.Distance(_mainCamera.transform.position, renderer.transform.position);
            renderer.enabled = distance <= _cullingDistance;
        }
    }

    public void UpdateVisibleRenderers()
    {
        _visibleRenderers.Clear();
        var renderers = FindObjectsOfType<Renderer>();
        foreach (var renderer in renderers)
        {
            if (IsVisible(renderer))
            {
                _visibleRenderers.Add(renderer);
            }
        }
    }
}
```

### シェーダー最適化

**目的**: シェーダーの効率化と GPU 負荷の軽減

**実装例**:

```csharp
public class ShaderOptimizer
{
    private readonly Dictionary<string, Material> _materialCache = new Dictionary<string, Material>();
    private readonly int _maxLights = 4;

    public void OptimizeShader(Material material)
    {
        // ライト数の制限
        material.SetInt("_MaxLights", _maxLights);

        // 不要なパスの無効化
        material.DisableKeyword("_NORMALMAP");
        material.DisableKeyword("_METALLICGLOSSMAP");
    }

    public Material GetOptimizedMaterial(string shaderName)
    {
        if (_materialCache.TryGetValue(shaderName, out var material))
        {
            return material;
        }

        var newMaterial = new Material(Shader.Find(shaderName));
        OptimizeShader(newMaterial);
        _materialCache[shaderName] = newMaterial;
        return newMaterial;
    }
}
```

### テクスチャ最適化

**目的**: テクスチャのメモリ使用量と GPU 負荷の軽減

**実装例**:

```csharp
public class TextureOptimizer
{
    private readonly Dictionary<string, Texture2D> _textureCache = new Dictionary<string, Texture2D>();
    private readonly int _maxTextureSize = 1024;

    public Texture2D OptimizeTexture(Texture2D texture)
    {
        // テクスチャサイズの最適化
        if (texture.width > _maxTextureSize || texture.height > _maxTextureSize)
        {
            var resizedTexture = new Texture2D(_maxTextureSize, _maxTextureSize);
            Graphics.ConvertTexture(texture, resizedTexture);
            return resizedTexture;
        }

        return texture;
    }

    public Texture2D GetOptimizedTexture(string path)
    {
        if (_textureCache.TryGetValue(path, out var texture))
        {
            return texture;
        }

        var newTexture = LoadTexture(path);
        var optimizedTexture = OptimizeTexture(newTexture);
        _textureCache[path] = optimizedTexture;
        return optimizedTexture;
    }
}
```

## ネットワーク最適化

### データ圧縮

**目的**: ネットワークトラフィックの削減

**実装例**:

```csharp
public class NetworkOptimizer
{
    private readonly int _compressionLevel = 6;

    public byte[] CompressData(byte[] data)
    {
        using (var output = new MemoryStream())
        {
            using (var gzip = new GZipStream(output, CompressionLevel.Optimal))
            {
                gzip.Write(data, 0, data.Length);
            }
            return output.ToArray();
        }
    }

    public byte[] DecompressData(byte[] compressedData)
    {
        using (var input = new MemoryStream(compressedData))
        using (var output = new MemoryStream())
        {
            using (var gzip = new GZipStream(input, CompressionMode.Decompress))
            {
                gzip.CopyTo(output);
            }
            return output.ToArray();
        }
    }
}
```

### パケット最適化

**目的**: ネットワークパケットの効率化

**実装例**:

```csharp
public class PacketOptimizer
{
    private readonly int _maxPacketSize = 1024;
    private readonly Queue<byte[]> _packetQueue = new Queue<byte[]>();

    public void OptimizePacket(byte[] data)
    {
        if (data.Length > _maxPacketSize)
        {
            // パケットの分割
            var chunks = SplitData(data, _maxPacketSize);
            foreach (var chunk in chunks)
            {
                _packetQueue.Enqueue(chunk);
            }
        }
        else
        {
            _packetQueue.Enqueue(data);
        }
    }

    private IEnumerable<byte[]> SplitData(byte[] data, int chunkSize)
    {
        for (int i = 0; i < data.Length; i += chunkSize)
        {
            var length = Math.Min(chunkSize, data.Length - i);
            var chunk = new byte[length];
            Array.Copy(data, i, chunk, 0, length);
            yield return chunk;
        }
    }
}
```

### 同期最適化

**目的**: ネットワーク同期の効率化

**実装例**:

```csharp
public class SynchronizationOptimizer
{
    private readonly float _syncInterval = 0.1f;
    private readonly Dictionary<int, float> _lastSyncTimes = new Dictionary<int, float>();

    public bool ShouldSync(int objectId, float currentTime)
    {
        if (!_lastSyncTimes.TryGetValue(objectId, out var lastSync))
        {
            _lastSyncTimes[objectId] = currentTime;
            return true;
        }

        if (currentTime - lastSync >= _syncInterval)
        {
            _lastSyncTimes[objectId] = currentTime;
            return true;
        }

        return false;
    }

    public void UpdateSyncTimes(float currentTime)
    {
        var expiredIds = _lastSyncTimes
            .Where(kvp => currentTime - kvp.Value > _syncInterval * 2)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var id in expiredIds)
        {
            _lastSyncTimes.Remove(id);
        }
    }
}
```

## アセット最適化

### モデル最適化

**目的**: 3D モデルの最適化

**実装例**:

```csharp
public class ModelOptimizer
{
    private readonly int _maxVertices = 10000;
    private readonly float _simplificationRatio = 0.5f;

    public void OptimizeModel(GameObject model)
    {
        var meshFilters = model.GetComponentsInChildren<MeshFilter>();
        foreach (var meshFilter in meshFilters)
        {
            var mesh = meshFilter.sharedMesh;
            if (mesh.vertexCount > _maxVertices)
            {
                var simplifiedMesh = SimplifyMesh(mesh);
                meshFilter.sharedMesh = simplifiedMesh;
            }
        }
    }

    private Mesh SimplifyMesh(Mesh originalMesh)
    {
        // メッシュの簡略化処理
        var simplifiedMesh = new Mesh();
        // 簡略化ロジックの実装
        return simplifiedMesh;
    }
}
```

### テクスチャ最適化

**目的**: テクスチャの最適化

**実装例**:

```csharp
public class TextureOptimizer
{
    private readonly int _maxTextureSize = 1024;
    private readonly TextureFormat _preferredFormat = TextureFormat.RGBA32;

    public void OptimizeTexture(Texture2D texture)
    {
        // テクスチャサイズの最適化
        if (texture.width > _maxTextureSize || texture.height > _maxTextureSize)
        {
            ResizeTexture(texture);
        }

        // テクスチャフォーマットの最適化
        if (texture.format != _preferredFormat)
        {
            ConvertTextureFormat(texture);
        }
    }

    private void ResizeTexture(Texture2D texture)
    {
        var resizedTexture = new Texture2D(_maxTextureSize, _maxTextureSize);
        Graphics.ConvertTexture(texture, resizedTexture);
        // リサイズ処理の実装
    }

    private void ConvertTextureFormat(Texture2D texture)
    {
        // フォーマット変換処理の実装
    }
}
```

### サウンド最適化

**目的**: サウンドファイルの最適化

**実装例**:

```csharp
public class SoundOptimizer
{
    private readonly int _maxSampleRate = 44100;
    private readonly int _maxBitDepth = 16;

    public void OptimizeSound(AudioClip audioClip)
    {
        // サンプルレートの最適化
        if (audioClip.frequency > _maxSampleRate)
        {
            ResampleAudio(audioClip);
        }

        // ビット深度の最適化
        if (audioClip.bitsPerSample > _maxBitDepth)
        {
            ConvertBitDepth(audioClip);
        }
    }

    private void ResampleAudio(AudioClip audioClip)
    {
        // リサンプリング処理の実装
    }

    private void ConvertBitDepth(AudioClip audioClip)
    {
        // ビット深度変換処理の実装
    }
}
```

## コード最適化

### アルゴリズム最適化

**目的**: アルゴリズムの効率化

**実装例**:

```csharp
public class AlgorithmOptimizer
{
    // 二分探索の最適化
    public int BinarySearch(int[] array, int target)
    {
        int left = 0;
        int right = array.Length - 1;

        while (left <= right)
        {
            int mid = left + (right - left) / 2;

            if (array[mid] == target)
                return mid;

            if (array[mid] < target)
                left = mid + 1;
            else
                right = mid - 1;
        }

        return -1;
    }

    // ソートの最適化
    public void QuickSort(int[] array, int left, int right)
    {
        if (left < right)
        {
            int pivot = Partition(array, left, right);
            QuickSort(array, left, pivot - 1);
            QuickSort(array, pivot + 1, right);
        }
    }

    private int Partition(int[] array, int left, int right)
    {
        int pivot = array[right];
        int i = left - 1;

        for (int j = left; j < right; j++)
        {
            if (array[j] <= pivot)
            {
                i++;
                Swap(array, i, j);
            }
        }

        Swap(array, i + 1, right);
        return i + 1;
    }

    private void Swap(int[] array, int i, int j)
    {
        int temp = array[i];
        array[i] = array[j];
        array[j] = temp;
    }
}
```

### データ構造最適化

**目的**: データ構造の効率化

**実装例**:

```csharp
public class DataStructureOptimizer
{
    // キャッシュ付きハッシュマップ
    public class CachedHashMap<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> _cache = new Dictionary<TKey, TValue>();
        private readonly int _maxCacheSize;
        private readonly Queue<TKey> _keyQueue = new Queue<TKey>();

        public CachedHashMap(int maxCacheSize = 1000)
        {
            _maxCacheSize = maxCacheSize;
        }

        public TValue Get(TKey key, Func<TKey, TValue> valueFactory)
        {
            if (_cache.TryGetValue(key, out var value))
            {
                return value;
            }

            value = valueFactory(key);
            AddToCache(key, value);
            return value;
        }

        private void AddToCache(TKey key, TValue value)
        {
            if (_cache.Count >= _maxCacheSize)
            {
                var oldestKey = _keyQueue.Dequeue();
                _cache.Remove(oldestKey);
            }

            _cache[key] = value;
            _keyQueue.Enqueue(key);
        }
    }

    // 最適化されたリスト
    public class OptimizedList<T>
    {
        private T[] _items;
        private int _count;
        private int _capacity;

        public OptimizedList(int initialCapacity = 4)
        {
            _capacity = initialCapacity;
            _items = new T[_capacity];
            _count = 0;
        }

        public void Add(T item)
        {
            if (_count == _capacity)
            {
                Resize();
            }

            _items[_count++] = item;
        }

        private void Resize()
        {
            _capacity *= 2;
            var newItems = new T[_capacity];
            Array.Copy(_items, newItems, _count);
            _items = newItems;
        }
    }
}
```

### メモリ管理最適化

**目的**: メモリ使用の効率化

**実装例**:

```csharp
public class MemoryOptimizer
{
    // メモリプール
    public class MemoryPool<T> where T : class, new()
    {
        private readonly Stack<T> _pool = new Stack<T>();
        private readonly int _maxSize;

        public MemoryPool(int maxSize = 1000)
        {
            _maxSize = maxSize;
        }

        public T Get()
        {
            return _pool.Count > 0 ? _pool.Pop() : new T();
        }

        public void Return(T item)
        {
            if (_pool.Count < _maxSize)
            {
                _pool.Push(item);
            }
        }
    }

    // メモリキャッシュ
    public class MemoryCache<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> _cache = new Dictionary<TKey, TValue>();
        private readonly int _maxSize;
        private readonly Queue<TKey> _keyQueue = new Queue<TKey>();

        public MemoryCache(int maxSize = 1000)
        {
            _maxSize = maxSize;
        }

        public TValue Get(TKey key, Func<TKey, TValue> valueFactory)
        {
            if (_cache.TryGetValue(key, out var value))
            {
                return value;
            }

            value = valueFactory(key);
            AddToCache(key, value);
            return value;
        }

        private void AddToCache(TKey key, TValue value)
        {
            if (_cache.Count >= _maxSize)
            {
                var oldestKey = _keyQueue.Dequeue();
                _cache.Remove(oldestKey);
            }

            _cache[key] = value;
            _keyQueue.Enqueue(key);
        }
    }
}
```

## プロファイリング

### パフォーマンス測定

-   フレームレートの監視
-   メモリ使用量の監視
-   CPU 使用率の監視

### メモリプロファイリング

-   メモリリークの検出
-   オブジェクトプールの使用状況
-   リソースの解放状況

### CPU プロファイリング

-   処理時間の計測
-   ボトルネックの特定
-   最適化の効果測定

## 変更履歴

| バージョン | 更新日     | 変更内容                                                                                               |
| ---------- | ---------- | ------------------------------------------------------------------------------------------------------ |
| 0.2.0      | 2025-06-13 | 概要、詳細、使用方法、制限事項セクションを追加 |
| 0.1.0      | 2024-03-24 | 初版作成<br>- メモリ最適化の手法を追加<br>- CPU/GPU 最適化の手法を追加<br>- プロファイリング手法を追加 |

---
TestExecutionGuide.md

---
title: テスト実行ガイド
version: 1.0.0
status: active
updated: 2025-06-07
tags:
    - UserGuide
    - Test
linked_docs:
    - "[[20_UserGuides/TestGuidelines|テストガイドライン]]"
    - "[[99_Reference/AI_Agent_TestWorkflow|AIエージェント向けテスト実行ワークフロー]]"
---

# テスト実行ガイド

## 目次

1. [概要](#概要)
2. [テスト実行の基本手順](#テスト実行の基本手順)
3. [C#テストの実行](#cテストの実行)
4. [GUT テストの実行](#gutテストの実行)
5. [テスト結果の確認](#テスト結果の確認)
6. [トラブルシューティング](#トラブルシューティング)
7. [変更履歴](#変更履歴)

## 概要

このドキュメントでは、プロジェクトのテスト実行に関する具体的な手順を説明します。
テストの書き方やガイドラインについては、[テストガイドライン](TestGuidelines.md)を参照してください。

## テスト実行の基本手順

1. 環境構築

    ```bash
    ./setup_godot_cli.sh
    ```

    - `.NET SDK`と Godot CLI がインストールされます
    - プロジェクトのビルドが自動で実行されます

2. テストの実行
    - C#テスト: `dotnet test`コマンドを使用
    - GUT テスト: Godot CLI を使用

## C#テストの実行

```bash
dotnet test Tests/Core/CoreTests.csproj -v minimal
```

### オプション

-   `-v minimal`: 最小限の出力
-   `-v normal`: 通常の出力
-   `-v detailed`: 詳細な出力

## GUT テストの実行

```bash
godot --headless --path . -s addons/gut/gut_cmdln.gd -gconfig=.gutconfig.json
```

### 設定

-   テスト設定は`.gutconfig.json`で管理
-   テスト結果は`res://Scripts/Tests/test-results_*.xml`に出力

## テスト結果の確認

1. C#テスト

    - コンソール出力で結果を確認
    - 詳細なレポートは`TestResults`ディレクトリに生成

2. GUT テスト
    - コンソール出力で結果を確認
    - JUnit 形式の XML ファイルで結果を保存

## トラブルシューティング

1. C#スクリプトを追加/変更した場合

    ```bash
    godot --headless --path . --build-solutions --quit
    ```

    を実行して DLL を再生成

2. テストが実行されない場合

    - ソリューションが正しくビルドされているか確認
    - テストファイルの場所が正しいか確認
    - テストクラス/メソッドに正しい属性が付いているか確認

3. エラーメッセージ
    - `Nonexistent function`: ソリューションの再ビルドが必要
    - `No tests found`: テストファイルの場所や命名規則を確認

## 変更履歴

| バージョン | 更新日     | 変更内容 |
| ---------- | ---------- | -------- |
| 1.0.0      | 2025-06-07 | 初版作成 |

---
TestGuidelines.md

---
title: テストガイドライン
version: 1.1.0
status: active
updated: 2025-06-09
tags:
    - UserGuide
    - Test
    - Guideline
linked_docs:
    - "[[20_UserGuides/TestExecutionGuide|テスト実行ガイド]]"
    - "[[99_Reference/AI_Agent_TestWorkflow|AIエージェント向けテスト実行ワークフロー]]"
---

# テストガイドライン

## 目次

1. [概要](#概要)
2. [テスト戦略](#テスト戦略)
3. [テストの種類と使い分け](#テストの種類と使い分け)
4. [テストの書き方](#テストの書き方)
5. [推奨アプローチ](#推奨アプローチ)
6. [変更履歴](#変更履歴)

## 概要

このドキュメントでは、プロジェクトのテスト戦略と実装ガイドラインを説明します。
具体的な実行手順については、[テスト実行ガイド](TestExecutionGuide.md)を参照してください。

## テスト戦略

1. **基本方針**

    - すべての変更はコミット前にテストを実行
    - テストカバレッジを維持・向上
    - テストの自動化を推進

2. **テストの優先順位**
    - 重要なビジネスロジック
    - 頻繁に変更されるコード
    - 複雑なロジック
    - エッジケース

## テストの種類と使い分け

1. **C#テスト（NUnit）**

    - ビジネスロジックのテスト
    - ユーティリティクラスのテスト
    - データ構造のテスト
    - 非同期処理のテスト

2. **GUT テスト**
    - Godot エンジン機能のテスト
    - シーンとノードのテスト
    - ゲーム特有の機能テスト
    - 物理演算やアニメーションのテスト

## テストの書き方

### C#テストの例

```csharp
[Test]
public void ValueChange_NotifiesSubscribers()
{
    var property = new ReactiveProperty<int>(0);
    int notifiedValue = -1;
    using (property.Subscribe(v => notifiedValue = v))
    {
        property.Value = 42;
    }
    Assert.AreEqual(42, notifiedValue);
}
```

### GUT テストの例

```gdscript
extends GutTest

var bus
var received

func _on_event(data: Dictionary) -> void:
    received = data

func before_each() -> void:
    bus = EventBus.new()
    add_child(bus)
    received = null

func after_each() -> void:
    bus.free()

func test_emit_and_subscribe() -> void:
    bus.Subscribe("TestEvent", Callable(self, "_on_event"))
    var data := {"value": 42}
    bus.EmitEvent("TestEvent", data)
    assert_eq(received, data)
```

## 推奨アプローチ

1. **ハイブリッドアプローチ**

    - ビジネスロジック → C#テスト
    - ゲームエンジン機能 → GUT テスト
    - 統合テスト → 必要に応じて両方を使用

2. **テストの粒度**

    - ユニットテスト: 個々の機能をテスト
    - インテグレーションテスト: 複数の機能の連携をテスト
    - エンドツーエンドテスト: 実際の使用シナリオをテスト

3. **テストの命名規則**

    - C#テスト: `[Test]`属性を使用
    - GUT テスト: `test_`プレフィックスを使用

4. **テストの構造**
    - 準備（Arrange）
    - 実行（Act）
    - 検証（Assert）

## テストケースの説明

-   各テストには目的と期待結果をコメントで明確に記述します
-   異常系や境界値のテストも網羅し、理由を併記します

## テストデータ生成方法

-   テストデータは `SetUp` メソッドやヘルパークラスで生成します
-   ランダムデータが必要な場合は `System.Random` を使用し、再現性のためシード値を固定します
-   大量データが必要な場合は `Enumerable.Range` を活用し、メモリ消費に注意します

## 変更履歴

| バージョン | 更新日     | 変更内容 |
| ---------- | ---------- | -------- |
| 1.0.0      | 2025-06-07 | 初版作成 |
| 1.1.0      | 2025-06-09 | テストケース説明とデータ生成方法を追加 |

---
Testing.md

---
title: テストガイド
version: 0.1.0
status: draft
updated: 2025-06-13
tags:
    - UserGuide
    - Testing
linked_docs:
    - "[[20_UserGuides/TestExecutionGuide|テスト実行ガイド]]"
    - "[[20_UserGuides/TestGuidelines|テストガイドライン]]"
---

# テストガイド

## 目次

1. [概要](#概要)
2. [詳細](#詳細)
3. [使用方法](#使用方法)
4. [制限事項](#制限事項)
5. [単体テスト](#単体テスト)
6. [統合テスト](#統合テスト)
7. [パフォーマンステスト](#パフォーマンステスト)
8. [UI テスト](#UIテスト)
9. [テスト自動化](#テスト自動化)
10. [テスト環境](#テスト環境)
11. [テストレポート](#テストレポート)
12. [変更履歴](#変更履歴)
## 概要

テストの種類や実行方法をまとめた総合的なガイドです。

## 詳細

各テスト手法の目的と実装例を示します。

## 使用方法

必要に応じてセクションを参照してください。

## 制限事項

- プロジェクトに合わせてサンプルコードを調整してください。


## 単体テスト

### テストの基本構造

**目的**: 個々のコンポーネントの動作を検証

**実装例**:

```csharp
[TestFixture]
public class PlayerSystemTests
{
    private PlayerSystem _playerSystem;
    private IEventBus _eventBus;

    [SetUp]
    public void Setup()
    {
        _eventBus = new EventBus();
        _playerSystem = new PlayerSystem(_eventBus);
    }

    [Test]
    public void Initialize_ShouldCreateSubsystems()
    {
        // Arrange
        var expectedSubsystems = new[]
        {
            typeof(PlayerInputSystem),
            typeof(PlayerStateSystem),
            typeof(PlayerMovementSystem),
            typeof(PlayerCombatSystem),
            typeof(PlayerAnimationSystem)
        };

        // Act
        _playerSystem.Initialize();

        // Assert
        foreach (var subsystemType in expectedSubsystems)
        {
            Assert.That(_playerSystem.GetSubsystem(subsystemType), Is.Not.Null);
        }
    }

    [Test]
    public void HandleError_ShouldPublishErrorEvent()
    {
        // Arrange
        var errorReceived = false;
        _eventBus.Subscribe<PlayerErrorEvent>(e => errorReceived = true);

        // Act
        _playerSystem.HandleError(new Exception("Test error"));

        // Assert
        Assert.That(errorReceived, Is.True);
    }
}
```

### モックとスタブ

**目的**: 依存関係を分離し、テストを独立させる

**実装例**:

```csharp
public class PlayerInputSystemTests
{
    private PlayerInputSystem _inputSystem;
    private Mock<IEventBus> _eventBusMock;
    private Mock<IPlayerInputModel> _inputModelMock;

    [SetUp]
    public void Setup()
    {
        _eventBusMock = new Mock<IEventBus>();
        _inputModelMock = new Mock<IPlayerInputModel>();
        _inputSystem = new PlayerInputSystem(_eventBusMock.Object, _inputModelMock.Object);
    }

    [Test]
    public void ProcessInput_ShouldPublishInputEvent()
    {
        // Arrange
        var inputEvent = new PlayerInputEvent { Action = "Move", Value = 1.0f };
        _inputModelMock.Setup(m => m.ProcessInput(It.IsAny<PlayerInputEvent>()))
            .Returns(true);

        // Act
        _inputSystem.ProcessInput(inputEvent);

        // Assert
        _eventBusMock.Verify(b => b.Publish(It.IsAny<PlayerInputEvent>()), Times.Once);
    }
}
```

### テストカバレッジ

**目的**: コードのテストカバレッジを確保

**実装例**:

```csharp
[TestFixture]
public class PlayerStateSystemTests
{
    private PlayerStateSystem _stateSystem;
    private IEventBus _eventBus;

    [SetUp]
    public void Setup()
    {
        _eventBus = new EventBus();
        _stateSystem = new PlayerStateSystem(_eventBus);
    }

    [Test]
    [TestCase(PlayerState.Idle, PlayerState.Walking)]
    [TestCase(PlayerState.Walking, PlayerState.Running)]
    [TestCase(PlayerState.Running, PlayerState.Jumping)]
    public void ChangeState_ShouldTransitionCorrectly(PlayerState fromState, PlayerState toState)
    {
        // Arrange
        _stateSystem.SetState(fromState);

        // Act
        var result = _stateSystem.ChangeState(toState);

        // Assert
        Assert.That(result, Is.True);
        Assert.That(_stateSystem.CurrentState, Is.EqualTo(toState));
    }

    [Test]
    public void ChangeState_ShouldRejectInvalidTransition()
    {
        // Arrange
        _stateSystem.SetState(PlayerState.Idle);

        // Act
        var result = _stateSystem.ChangeState(PlayerState.Attacking);

        // Assert
        Assert.That(result, Is.False);
        Assert.That(_stateSystem.CurrentState, Is.EqualTo(PlayerState.Idle));
    }
}
```

## 統合テスト

### システム間の連携テスト

**目的**: 複数のシステムが正しく連携することを検証

**実装例**:

```csharp
[TestFixture]
public class PlayerSystemsIntegrationTests
{
    private PlayerSystem _playerSystem;
    private IEventBus _eventBus;

    [SetUp]
    public void Setup()
    {
        _eventBus = new EventBus();
        _playerSystem = new PlayerSystem(_eventBus);
        _playerSystem.Initialize();
    }

    [Test]
    public void InputToMovement_ShouldWorkCorrectly()
    {
        // Arrange
        var inputSystem = _playerSystem.GetSubsystem<PlayerInputSystem>();
        var movementSystem = _playerSystem.GetSubsystem<PlayerMovementSystem>();
        var stateSystem = _playerSystem.GetSubsystem<PlayerStateSystem>();

        // Act
        inputSystem.ProcessInput(new PlayerInputEvent { Action = "Move", Value = 1.0f });

        // Assert
        Assert.That(stateSystem.CurrentState, Is.EqualTo(PlayerState.Walking));
        Assert.That(movementSystem.IsMoving, Is.True);
    }

    [Test]
    public void CombatToAnimation_ShouldWorkCorrectly()
    {
        // Arrange
        var combatSystem = _playerSystem.GetSubsystem<PlayerCombatSystem>();
        var animationSystem = _playerSystem.GetSubsystem<PlayerAnimationSystem>();

        // Act
        combatSystem.StartAttack();

        // Assert
        Assert.That(animationSystem.CurrentAnimation, Is.EqualTo("Attack"));
    }
}
```

### イベントフロー検証

**目的**: イベントの伝播と処理を検証

**実装例**:

```csharp
[TestFixture]
public class EventFlowTests
{
    private PlayerSystem _playerSystem;
    private IEventBus _eventBus;
    private List<IPlayerEvent> _receivedEvents;

    [SetUp]
    public void Setup()
    {
        _eventBus = new EventBus();
        _playerSystem = new PlayerSystem(_eventBus);
        _playerSystem.Initialize();
        _receivedEvents = new List<IPlayerEvent>();

        _eventBus.Subscribe<IPlayerEvent>(e => _receivedEvents.Add(e));
    }

    [Test]
    public void InputEvent_ShouldTriggerCorrectEventFlow()
    {
        // Arrange
        var inputSystem = _playerSystem.GetSubsystem<PlayerInputSystem>();

        // Act
        inputSystem.ProcessInput(new PlayerInputEvent { Action = "Jump" });

        // Assert
        Assert.That(_receivedEvents.Count, Is.GreaterThan(0));
        Assert.That(_receivedEvents.Any(e => e is PlayerStateChangeEvent), Is.True);
        Assert.That(_receivedEvents.Any(e => e is PlayerAnimationEvent), Is.True);
    }
}
```

### エラー処理検証

**目的**: エラー発生時のシステムの挙動を検証

**実装例**:

```csharp
[TestFixture]
public class ErrorHandlingTests
{
    private PlayerSystem _playerSystem;
    private IEventBus _eventBus;
    private List<PlayerErrorEvent> _errorEvents;

    [SetUp]
    public void Setup()
    {
        _eventBus = new EventBus();
        _playerSystem = new PlayerSystem(_eventBus);
        _playerSystem.Initialize();
        _errorEvents = new List<PlayerErrorEvent>();

        _eventBus.Subscribe<PlayerErrorEvent>(e => _errorEvents.Add(e));
    }

    [Test]
    public void SubsystemError_ShouldBeHandledCorrectly()
    {
        // Arrange
        var inputSystem = _playerSystem.GetSubsystem<PlayerInputSystem>();

        // Act
        inputSystem.ProcessInput(null);

        // Assert
        Assert.That(_errorEvents.Count, Is.EqualTo(1));
        Assert.That(_errorEvents[0].Message, Does.Contain("Input processing failed"));
    }
}
```

## パフォーマンステスト

### 負荷テスト

**目的**: システムの負荷耐性を検証

**実装例**:

```csharp
[TestFixture]
public class PerformanceTests
{
    private PlayerSystem _playerSystem;
    private IEventBus _eventBus;
    private PerformanceProfiler _profiler;

    [SetUp]
    public void Setup()
    {
        _eventBus = new EventBus();
        _playerSystem = new PlayerSystem(_eventBus);
        _playerSystem.Initialize();
        _profiler = new PerformanceProfiler();
    }

    [Test]
    public void HighFrequencyInput_ShouldProcessCorrectly()
    {
        // Arrange
        var inputSystem = _playerSystem.GetSubsystem<PlayerInputSystem>();
        var iterations = 1000;

        // Act
        _profiler.StartMeasurement("InputProcessing");
        for (int i = 0; i < iterations; i++)
        {
            inputSystem.ProcessInput(new PlayerInputEvent { Action = "Move", Value = 1.0f });
        }
        _profiler.StopMeasurement("InputProcessing");

        // Assert
        var results = _profiler.GetResults("InputProcessing");
        Assert.That(results.Average, Is.LessThan(1.0)); // 1ms未満
    }
}
```

### メモリ使用量テスト

**目的**: メモリ使用量を検証

**実装例**:

```csharp
[TestFixture]
public class MemoryUsageTests
{
    private PlayerSystem _playerSystem;
    private IEventBus _eventBus;
    private MemoryProfiler _profiler;

    [SetUp]
    public void Setup()
    {
        _eventBus = new EventBus();
        _playerSystem = new PlayerSystem(_eventBus);
        _profiler = new MemoryProfiler();
    }

    [Test]
    public void SystemInitialization_ShouldNotLeakMemory()
    {
        // Arrange
        _profiler.TakeSnapshot("BeforeInit");

        // Act
        _playerSystem.Initialize();
        _profiler.TakeSnapshot("AfterInit");

        // Assert
        var memoryDiff = _profiler.GetMemoryDifference("BeforeInit", "AfterInit");
        Assert.That(memoryDiff, Is.LessThan(10 * 1024 * 1024)); // 10MB未満
    }
}
```

### CPU 使用率テスト

**目的**: CPU 使用率を検証

**実装例**:

```csharp
[TestFixture]
public class CPUUsageTests
{
    private PlayerSystem _playerSystem;
    private IEventBus _eventBus;
    private CPUProfiler _profiler;

    [SetUp]
    public void Setup()
    {
        _eventBus = new EventBus();
        _playerSystem = new PlayerSystem(_eventBus);
        _playerSystem.Initialize();
        _profiler = new CPUProfiler();
    }

    [Test]
    public void UpdateLoop_ShouldNotExceedCPULimit()
    {
        // Arrange
        _profiler.StartProfiling("UpdateLoop");

        // Act
        for (int i = 0; i < 1000; i++)
        {
            _playerSystem.Update(0.016f); // 60FPS相当
            _profiler.RecordCPUUsage("UpdateLoop");
        }

        // Assert
        var results = _profiler.GetResults("UpdateLoop");
        Assert.That(results.Average, Is.LessThan(5.0)); // 5%未満
    }
}
```

## UI テスト

### 入力検証

**目的**: UI 入力の正確性を検証

**実装例**:

```csharp
[TestFixture]
public class UIInputTests
{
    private PlayerUI _playerUI;
    private Mock<IEventBus> _eventBusMock;

    [SetUp]
    public void Setup()
    {
        _eventBusMock = new Mock<IEventBus>();
        _playerUI = new PlayerUI(_eventBusMock.Object);
    }

    [Test]
    public void ButtonClick_ShouldTriggerCorrectAction()
    {
        // Arrange
        var button = _playerUI.GetButton("AttackButton");

        // Act
        button.Click();

        // Assert
        _eventBusMock.Verify(b => b.Publish(It.Is<PlayerInputEvent>(
            e => e.Action == "Attack")), Times.Once);
    }
}
```

### 表示検証

**目的**: UI 表示の正確性を検証

**実装例**:

```csharp
[TestFixture]
public class UIDisplayTests
{
    private PlayerUI _playerUI;
    private IEventBus _eventBus;

    [SetUp]
    public void Setup()
    {
        _eventBus = new EventBus();
        _playerUI = new PlayerUI(_eventBus);
    }

    [Test]
    public void HealthDisplay_ShouldUpdateCorrectly()
    {
        // Arrange
        var healthBar = _playerUI.GetHealthBar();

        // Act
        _eventBus.Publish(new PlayerHealthEvent { CurrentHealth = 75, MaxHealth = 100 });

        // Assert
        Assert.That(healthBar.Value, Is.EqualTo(75));
        Assert.That(healthBar.MaxValue, Is.EqualTo(100));
    }
}
```

### アニメーション検証

**目的**: UI アニメーションの正確性を検証

**実装例**:

```csharp
[TestFixture]
public class UIAnimationTests
{
    private PlayerUI _playerUI;
    private IEventBus _eventBus;

    [SetUp]
    public void Setup()
    {
        _eventBus = new EventBus();
        _playerUI = new PlayerUI(_eventBus);
    }

    [Test]
    public void DamageAnimation_ShouldPlayCorrectly()
    {
        // Arrange
        var damageEffect = _playerUI.GetDamageEffect();

        // Act
        _eventBus.Publish(new PlayerDamageEvent { Amount = 10 });

        // Assert
        Assert.That(damageEffect.IsPlaying, Is.True);
        Assert.That(damageEffect.Duration, Is.EqualTo(0.5f));
    }
}
```

## テスト自動化

### CI/CD 統合

**目的**: 継続的インテグレーション/デリバリーの自動化

**実装例**:

```yaml
# .github/workflows/test.yml
name: Test

on:
    push:
        branches: [main]
    pull_request:
        branches: [main]

jobs:
    test:
        runs-on: ubuntu-latest
        steps:
            - uses: actions/checkout@v2
            - name: Setup .NET
              uses: actions/setup-dotnet@v1
              with:
                  dotnet-version: "6.0.x"
            - name: Restore dependencies
              run: dotnet restore
            - name: Build
              run: dotnet build --no-restore
            - name: Test
              run: dotnet test --no-build --verbosity normal
```

### テストレポート生成

**目的**: テスト結果の自動レポート生成

**実装例**:

```csharp
public class TestReporter
{
    private readonly string _reportPath;
    private readonly List<TestResult> _results;

    public TestReporter(string reportPath)
    {
        _reportPath = reportPath;
        _results = new List<TestResult>();
    }

    public void AddResult(TestResult result)
    {
        _results.Add(result);
    }

    public void GenerateReport()
    {
        var report = new StringBuilder();
        report.AppendLine("# テストレポート");
        report.AppendLine($"生成日時: {DateTime.Now}");
        report.AppendLine();

        report.AppendLine("## サマリー");
        report.AppendLine($"総テスト数: {_results.Count}");
        report.AppendLine($"成功: {_results.Count(r => r.Passed)}");
        report.AppendLine($"失敗: {_results.Count(r => !r.Passed)}");
        report.AppendLine();

        report.AppendLine("## 詳細");
        foreach (var result in _results)
        {
            report.AppendLine($"### {result.Name}");
            report.AppendLine($"結果: {(result.Passed ? "成功" : "失敗")}");
            if (!result.Passed)
            {
                report.AppendLine($"エラー: {result.ErrorMessage}");
            }
            report.AppendLine();
        }

        File.WriteAllText(_reportPath, report.ToString());
    }
}
```

### テストスケジュール

**目的**: テストの自動実行スケジュール管理

**実装例**:

```csharp
public class TestScheduler
{
    private readonly List<TestSchedule> _schedules;
    private readonly TestRunner _testRunner;
    private readonly TestReporter _reporter;

    public TestScheduler(TestRunner testRunner, TestReporter reporter)
    {
        _schedules = new List<TestSchedule>();
        _testRunner = testRunner;
        _reporter = reporter;
    }

    public void AddSchedule(TestSchedule schedule)
    {
        _schedules.Add(schedule);
    }

    public async Task RunScheduledTests()
    {
        foreach (var schedule in _schedules)
        {
            if (schedule.ShouldRun())
            {
                var results = await _testRunner.RunTests(schedule.TestCategories);
                _reporter.AddResults(results);
            }
        }
    }
}
```

## テスト環境

### テストデータ

**目的**: テスト用データの管理

**実装例**:

```csharp
public class TestDataManager
{
    private readonly Dictionary<string, object> _testData;

    public TestDataManager()
    {
        _testData = new Dictionary<string, object>();
        LoadTestData();
    }

    private void LoadTestData()
    {
        // テストデータの読み込み
        _testData["PlayerStats"] = new PlayerStats
        {
            Health = 100,
            Speed = 5.0f,
            JumpForce = 10.0f
        };

        _testData["EnemyStats"] = new EnemyStats
        {
            Health = 50,
            Damage = 10,
            AttackRange = 2.0f
        };
    }

    public T GetTestData<T>(string key)
    {
        if (_testData.TryGetValue(key, out var data))
        {
            return (T)data;
        }
        throw new KeyNotFoundException($"Test data not found: {key}");
    }
}
```

### テスト設定

**目的**: テスト環境の設定管理

**実装例**:

```csharp
public class TestConfiguration
{
    public bool EnableLogging { get; set; }
    public string LogLevel { get; set; }
    public bool EnablePerformanceProfiling { get; set; }
    public int MaxTestIterations { get; set; }
    public float TestTimeout { get; set; }

    public static TestConfiguration Load()
    {
        var config = new TestConfiguration
        {
            EnableLogging = true,
            LogLevel = "Debug",
            EnablePerformanceProfiling = true,
            MaxTestIterations = 1000,
            TestTimeout = 30.0f
        };

        return config;
    }
}
```

### テストフィクスチャ

**目的**: テスト環境のセットアップとクリーンアップ

**実装例**:

```csharp
[TestFixture]
public class PlayerSystemTestFixture
{
    protected PlayerSystem PlayerSystem { get; private set; }
    protected IEventBus EventBus { get; private set; }
    protected TestDataManager TestData { get; private set; }
    protected TestConfiguration Config { get; private set; }

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        Config = TestConfiguration.Load();
        TestData = new TestDataManager();
    }

    [SetUp]
    public void Setup()
    {
        EventBus = new EventBus();
        PlayerSystem = new PlayerSystem(EventBus);
        PlayerSystem.Initialize();
    }

    [TearDown]
    public void TearDown()
    {
        PlayerSystem.Dispose();
        EventBus.Dispose();
    }

    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
        TestData.Dispose();
    }
}
```

## テストレポート

### レポート生成

**目的**: テスト結果のレポート生成

**実装例**:

```csharp
public class TestReportGenerator
{
    private readonly string _outputPath;
    private readonly List<TestResult> _results;

    public TestReportGenerator(string outputPath)
    {
        _outputPath = outputPath;
        _results = new List<TestResult>();
    }

    public void AddResults(IEnumerable<TestResult> results)
    {
        _results.AddRange(results);
    }

    public void GenerateReport()
    {
        var report = new StringBuilder();
        report.AppendLine("# テストレポート");
        report.AppendLine($"生成日時: {DateTime.Now}");
        report.AppendLine();

        // サマリー
        report.AppendLine("## サマリー");
        var summary = GenerateSummary();
        report.AppendLine(summary);

        // 詳細
        report.AppendLine("## 詳細");
        var details = GenerateDetails();
        report.AppendLine(details);

        // パフォーマンス
        report.AppendLine("## パフォーマンス");
        var performance = GeneratePerformanceReport();
        report.AppendLine(performance);

        File.WriteAllText(_outputPath, report.ToString());
    }

    private string GenerateSummary()
    {
        var total = _results.Count;
        var passed = _results.Count(r => r.Passed);
        var failed = total - passed;

        return $@"総テスト数: {total}
成功: {passed}
失敗: {failed}
成功率: {(float)passed / total * 100:F2}%";
    }

    private string GenerateDetails()
    {
        var details = new StringBuilder();
        foreach (var result in _results)
        {
            details.AppendLine($"### {result.Name}");
            details.AppendLine($"結果: {(result.Passed ? "成功" : "失敗")}");
            if (!result.Passed)
            {
                details.AppendLine($"エラー: {result.ErrorMessage}");
                details.AppendLine($"スタックトレース: {result.StackTrace}");
            }
            details.AppendLine();
        }
        return details.ToString();
    }

    private string GeneratePerformanceReport()
    {
        var performance = new StringBuilder();
        var performanceResults = _results
            .Where(r => r.PerformanceMetrics != null)
            .Select(r => r.PerformanceMetrics);

        if (performanceResults.Any())
        {
            performance.AppendLine("### 実行時間");
            performance.AppendLine($"平均: {performanceResults.Average(p => p.ExecutionTime):F2}ms");
            performance.AppendLine($"最小: {performanceResults.Min(p => p.ExecutionTime):F2}ms");
            performance.AppendLine($"最大: {performanceResults.Max(p => p.ExecutionTime):F2}ms");

            performance.AppendLine("\n### メモリ使用量");
            performance.AppendLine($"平均: {performanceResults.Average(p => p.MemoryUsage) / 1024 / 1024:F2}MB");
            performance.AppendLine($"最小: {performanceResults.Min(p => p.MemoryUsage) / 1024 / 1024:F2}MB");
            performance.AppendLine($"最大: {performanceResults.Max(p => p.MemoryUsage) / 1024 / 1024:F2}MB");
        }

        return performance.ToString();
    }
}
```

### レポート分析

**目的**: テストレポートの分析と改善提案

**実装例**:

```csharp
public class TestReportAnalyzer
{
    private readonly List<TestResult> _results;
    private readonly TestConfiguration _config;

    public TestReportAnalyzer(IEnumerable<TestResult> results, TestConfiguration config)
    {
        _results = results.ToList();
        _config = config;
    }

    public AnalysisResult Analyze()
    {
        var result = new AnalysisResult();

        // テスト成功率の分析
        result.SuccessRate = (float)_results.Count(r => r.Passed) / _results.Count;
        result.IsSuccessRateAcceptable = result.SuccessRate >= 0.95f;

        // パフォーマンスの分析
        var performanceResults = _results
            .Where(r => r.PerformanceMetrics != null)
            .Select(r => r.PerformanceMetrics);

        if (performanceResults.Any())
        {
            result.AverageExecutionTime = performanceResults.Average(p => p.ExecutionTime);
            result.IsPerformanceAcceptable = result.AverageExecutionTime < 100.0f;

            result.AverageMemoryUsage = performanceResults.Average(p => p.MemoryUsage);
            result.IsMemoryUsageAcceptable = result.AverageMemoryUsage < 100 * 1024 * 1024;
        }

        // 改善提案の生成
        result.ImprovementSuggestions = GenerateImprovementSuggestions(result);

        return result;
    }

    private List<string> GenerateImprovementSuggestions(AnalysisResult result)
    {
        var suggestions = new List<string>();

        if (!result.IsSuccessRateAcceptable)
        {
            suggestions.Add("テストの成功率が95%未満です。失敗したテストの修正を優先してください。");
        }

        if (!result.IsPerformanceAcceptable)
        {
            suggestions.Add($"テストの実行時間が平均{result.AverageExecutionTime:F2}msと長いです。パフォーマンスの最適化を検討してください。");
        }

        if (!result.IsMemoryUsageAcceptable)
        {
            suggestions.Add($"メモリ使用量が平均{result.AverageMemoryUsage / 1024 / 1024:F2}MBと多いです。メモリリークの可能性を調査してください。");
        }

        return suggestions;
    }
}
```

### レポート通知

**目的**: テスト結果の通知

**実装例**:

```csharp
public class TestReportNotifier
{
    private readonly string _slackWebhookUrl;
    private readonly string _emailRecipients;

    public TestReportNotifier(string slackWebhookUrl, string emailRecipients)
    {
        _slackWebhookUrl = slackWebhookUrl;
        _emailRecipients = emailRecipients;
    }

    public async Task NotifyTestResults(TestReport report)
    {
        // Slack通知
        await NotifySlack(report);

        // メール通知
        await NotifyEmail(report);
    }

    private async Task NotifySlack(TestReport report)
    {
        var message = new
        {
            text = $"テスト結果: {report.Summary}",
            attachments = new[]
            {
                new
                {
                    color = report.IsSuccess ? "good" : "danger",
                    fields = new[]
                    {
                        new { title = "総テスト数", value = report.TotalTests.ToString(), @short = true },
                        new { title = "成功", value = report.PassedTests.ToString(), @short = true },
                        new { title = "失敗", value = report.FailedTests.ToString(), @short = true },
                        new { title = "成功率", value = $"{report.SuccessRate:F2}%", @short = true }
                    }
                }
            }
        };

        using (var client = new HttpClient())
        {
            var content = new StringContent(
                JsonConvert.SerializeObject(message),
                Encoding.UTF8,
                "application/json");
            await client.PostAsync(_slackWebhookUrl, content);
        }
    }

    private async Task NotifyEmail(TestReport report)
    {
        var message = new MailMessage
        {
            Subject = $"テスト結果: {report.Summary}",
            Body = GenerateEmailBody(report)
        };

        foreach (var recipient in _emailRecipients.Split(','))
        {
            message.To.Add(recipient.Trim());
        }

        using (var client = new SmtpClient())
        {
            await client.SendMailAsync(message);
        }
    }

    private string GenerateEmailBody(TestReport report)
    {
        var body = new StringBuilder();
        body.AppendLine($"テスト実行日時: {report.ExecutionTime}");
        body.AppendLine();
        body.AppendLine("サマリー:");
        body.AppendLine($"総テスト数: {report.TotalTests}");
        body.AppendLine($"成功: {report.PassedTests}");
        body.AppendLine($"失敗: {report.FailedTests}");
        body.AppendLine($"成功率: {report.SuccessRate:F2}%");
        body.AppendLine();
        body.AppendLine("失敗したテスト:");
        foreach (var failure in report.Failures)
        {
            body.AppendLine($"- {failure.Name}: {failure.ErrorMessage}");
        }

        return body.ToString();
    }
}
```

## 変更履歴

| バージョン | 更新日     | 変更内容                     |
| ---------- | ---------- | ---------------------------- |
| 0.1.0      | 2025-06-13 | メタデータを追加し構成を整理 |

---
Troubleshooting.md

---
title: トラブルシューティングガイド
version: 0.2.0
status: draft
updated: 2025-06-13
tags:
    - Guide
    - Troubleshooting
    - Player
    - System
linked_docs:
    - "[[PlayerSystem]]"
    - "[[PlayerInputSystem]]"
    - "[[PlayerStateSystem]]"
    - "[[PlayerMovementSystem]]"
    - "[[PlayerCombatSystem]]"
    - "[[PlayerAnimationSystem]]"
---

# トラブルシューティングガイド

## 目次

1. [概要](#概要)
2. [詳細](#詳細)
3. [使用方法](#使用方法)
4. [制限事項](#制限事項)
5. [一般的な問題](#一般的な問題)
6. [入力システムの問題](#入力システムの問題)
7. [状態システムの問題](#状態システムの問題)
8. [移動システムの問題](#移動システムの問題)
9. [戦闘システムの問題](#戦闘システムの問題)
10. [アニメーションシステムの問題](#アニメーションシステムの問題)
11. [パフォーマンスの問題](#パフォーマンスの問題)
12. [エラー処理の問題](#エラー処理の問題)
13. [変更履歴](#変更履歴)

## 概要

プレイヤーから報告される一般的な問題とその解決策をまとめた
ガイドです。

## 詳細

各システムで想定される不具合の原因と対処方法を示します。

## 使用方法

問題の発生箇所に応じたセクションを参照してください。

## 制限事項

- プロジェクト固有の要因には対応していない場合があります

## 一般的な問題

### システムの初期化に失敗する

**症状**:

-   システムの初期化時に例外が発生する
-   サブシステムが正しく動作しない

**考えられる原因**:

1. イベントバスが正しく初期化されていない
2. 必要なリソースが読み込まれていない
3. 依存関係の順序が正しくない

**解決方法**:

```csharp
// 1. イベントバスの初期化を確認
var eventBus = new GameEventBus();
if (eventBus == null)
{
    Debug.LogError("イベントバスの初期化に失敗しました");
    return;
}

// 2. リソースの読み込みを確認
if (!ResourceLoader.Exists("res://Resources/Player/player_config.tres"))
{
    Debug.LogError("必要なリソースが見つかりません");
    return;
}

// 3. 初期化順序の確認
// 正しい順序: イベントバス → 入力 → 状態 → 移動 → 戦闘 → アニメーション
var inputSystem = new PlayerInputSystem(eventBus);
var stateSystem = new PlayerStateSystem(eventBus);
var movementSystem = new PlayerMovementSystem(eventBus);
var combatSystem = new PlayerCombatSystem(eventBus);
var animationSystem = new PlayerAnimationSystem(eventBus);
```

### イベントが発火しない

**症状**:

-   イベントハンドラーが呼び出されない
-   システム間の連携が機能しない

**考えられる原因**:

1. イベントの購読が正しく設定されていない
2. イベントの発火タイミングが適切でない
3. イベントの型が一致していない

**解決方法**:

```csharp
// 1. イベントの購読を確認
eventBus.GetEventStream<PlayerStateChangedEvent>()
    .Subscribe(evt => {
        Debug.Log($"状態が変更されました: {evt.PreviousState} → {evt.NewState}");
    })
    .AddTo(_disposables);

// 2. イベントの発火を確認
public void ChangeState(PlayerState newState)
{
    var previousState = _currentState;
    _currentState = newState;

    // イベントの発火を確認
    _eventBus.Publish(new PlayerStateChangedEvent(previousState, newState));
    Debug.Log($"状態変更イベントを発火: {previousState} → {newState}");
}

// 3. イベントの型を確認
public class PlayerStateChangedEvent
{
    public PlayerState PreviousState { get; }
    public PlayerState NewState { get; }

    public PlayerStateChangedEvent(PlayerState previousState, PlayerState newState)
    {
        PreviousState = previousState;
        NewState = newState;
    }
}
```

## 入力システムの問題

### 入力が検出されない

**症状**:

-   キー入力が反応しない
-   入力イベントが発火しない

**考えられる原因**:

1. 入力アクションが正しく登録されていない
2. 入力の有効化/無効化が適切でない
3. 入力の優先順位が正しく設定されていない

**解決方法**:

```csharp
// 1. 入力アクションの登録を確認
public void RegisterInputActions()
{
    // 移動入力の登録
    RegisterInputAction(new InputAction("Move", Key.W, Key.S, Key.A, Key.D));

    // 攻撃入力の登録
    RegisterInputAction(new InputAction("Attack", Key.Space));

    // 防御入力の登録
    RegisterInputAction(new InputAction("Block", Key.LeftShift));
}

// 2. 入力の有効化/無効化を確認
public void EnableInput()
{
    _isInputEnabled = true;
    Debug.Log("入力システムを有効化しました");
}

public void DisableInput()
{
    _isInputEnabled = false;
    Debug.Log("入力システムを無効化しました");
}

// 3. 入力の優先順位を確認
public void SetInputPriority(string actionName, int priority)
{
    if (_inputActions.TryGetValue(actionName, out var action))
    {
        action.Priority = priority;
        Debug.Log($"入力アクション '{actionName}' の優先順位を {priority} に設定しました");
    }
}
```

### 入力の遅延

**症状**:

-   入力の反応が遅い
-   入力の処理に時間がかかる

**考えられる原因**:

1. 入力処理の最適化が不十分
2. 不要な入力チェックが行われている
3. 入力のバッファリングが適切でない

**解決方法**:

```csharp
// 1. 入力処理の最適化
public void ProcessInput()
{
    if (!_isInputEnabled) return;

    // 必要な入力のみをチェック
    var currentInput = GetCurrentInput();
    if (currentInput == null) return;

    // 入力の処理
    ProcessInputAction(currentInput);
}

// 2. 入力チェックの最適化
private InputAction GetCurrentInput()
{
    // 優先順位の高い入力からチェック
    return _inputActions.Values
        .OrderByDescending(a => a.Priority)
        .FirstOrDefault(a => a.IsTriggered());
}

// 3. 入力のバッファリング
private readonly Queue<InputAction> _inputBuffer = new Queue<InputAction>();
private const int MAX_BUFFER_SIZE = 5;

public void BufferInput(InputAction input)
{
    if (_inputBuffer.Count >= MAX_BUFFER_SIZE)
    {
        _inputBuffer.Dequeue();
    }
    _inputBuffer.Enqueue(input);
}
```

## 状態システムの問題

### 状態遷移が機能しない

**症状**:

-   状態が変更されない
-   状態遷移の条件が満たされても遷移しない

**考えられる原因**:

1. 状態遷移の条件が正しく設定されていない
2. 状態のロックが適切でない
3. 状態遷移のイベントが正しく発火していない

**解決方法**:

```csharp
// 1. 状態遷移の条件を確認
public bool CanTransitionTo(PlayerState newState)
{
    // 現在の状態から遷移可能かチェック
    if (!_stateTransitions.ContainsKey(_currentState))
    {
        Debug.LogError($"現在の状態 '{_currentState}' からの遷移が定義されていません");
        return false;
    }

    var allowedTransitions = _stateTransitions[_currentState];
    if (!allowedTransitions.Contains(newState))
    {
        Debug.LogError($"状態 '{_currentState}' から '{newState}' への遷移は許可されていません");
        return false;
    }

    return true;
}

// 2. 状態のロックを確認
public void LockState(PlayerState state)
{
    _lockedStates.Add(state);
    Debug.Log($"状態 '{state}' をロックしました");
}

public void UnlockState(PlayerState state)
{
    _lockedStates.Remove(state);
    Debug.Log($"状態 '{state}' のロックを解除しました");
}

// 3. 状態遷移のイベントを確認
public void ChangeState(PlayerState newState)
{
    if (!CanTransitionTo(newState))
    {
        Debug.LogError($"状態 '{_currentState}' から '{newState}' への遷移が失敗しました");
        return;
    }

    var previousState = _currentState;
    _currentState = newState;

    // 状態遷移イベントの発火
    _eventBus.Publish(new PlayerStateChangedEvent(previousState, newState));
    Debug.Log($"状態を '{previousState}' から '{newState}' に変更しました");
}
```

### 状態の競合

**症状**:

-   複数の状態が同時に有効になる
-   状態の優先順位が正しく機能しない

**考えられる原因**:

1. 状態の優先順位が正しく設定されていない
2. 状態の排他制御が不十分
3. 状態の更新タイミングが適切でない

**解決方法**:

```csharp
// 1. 状態の優先順位を設定
public void SetStatePriority(PlayerState state, int priority)
{
    _statePriorities[state] = priority;
    Debug.Log($"状態 '{state}' の優先順位を {priority} に設定しました");
}

// 2. 状態の排他制御
public bool IsStateExclusive(PlayerState state)
{
    return _exclusiveStates.Contains(state);
}

public void SetStateExclusive(PlayerState state, bool isExclusive)
{
    if (isExclusive)
    {
        _exclusiveStates.Add(state);
    }
    else
    {
        _exclusiveStates.Remove(state);
    }
}

// 3. 状態の更新タイミング
public void UpdateState()
{
    if (_isUpdating) return;
    _isUpdating = true;

    try
    {
        // 状態の更新処理
        UpdateCurrentState();
    }
    finally
    {
        _isUpdating = false;
    }
}
```

## 移動システムの問題

### 移動が機能しない

**症状**:

-   キャラクターが移動しない
-   移動速度が正しく適用されない

**考えられる原因**:

1. 移動の有効化/無効化が適切でない
2. 移動速度の計算が正しくない
3. 移動の制限が適切でない

**解決方法**:

```csharp
// 1. 移動の有効化/無効化を確認
public void EnableMovement()
{
    _isMovementEnabled = true;
    Debug.Log("移動システムを有効化しました");
}

public void DisableMovement()
{
    _isMovementEnabled = false;
    Debug.Log("移動システムを無効化しました");
}

// 2. 移動速度の計算を確認
public Vector2 CalculateMovementSpeed(Vector2 input)
{
    var speed = _baseSpeed;

    // 状態に応じた速度調整
    if (_currentState == PlayerState.Running)
    {
        speed *= _runMultiplier;
    }
    else if (_currentState == PlayerState.Walking)
    {
        speed *= _walkMultiplier;
    }

    // 入力方向の正規化
    if (input.magnitude > 0)
    {
        input.Normalize();
    }

    return input * speed;
}

// 3. 移動の制限を確認
public bool CanMove()
{
    // 移動可能な状態かチェック
    if (!_movableStates.Contains(_currentState))
    {
        Debug.Log($"現在の状態 '{_currentState}' では移動できません");
        return false;
    }

    // 移動の制限をチェック
    if (_movementRestrictions.Any(r => r.IsRestricted()))
    {
        Debug.Log("移動が制限されています");
        return false;
    }

    return true;
}
```

### 移動の滑らかさ

**症状**:

-   移動が滑らかでない
-   移動の加速/減速が不自然

**考えられる原因**:

1. 移動の補間が適切でない
2. 加速度の設定が不適切
3. 移動の更新頻度が低い

**解決方法**:

```csharp
// 1. 移動の補間
public Vector2 InterpolateMovement(Vector2 current, Vector2 target, float deltaTime)
{
    return Vector2.Lerp(current, target, _movementSmoothness * deltaTime);
}

// 2. 加速度の設定
public void SetAcceleration(float acceleration)
{
    _acceleration = acceleration;
    Debug.Log($"加速度を {acceleration} に設定しました");
}

public void SetDeceleration(float deceleration)
{
    _deceleration = deceleration;
    Debug.Log($"減速度を {deceleration} に設定しました");
}

// 3. 移動の更新頻度
public void UpdateMovement(float deltaTime)
{
    if (!_isMovementEnabled) return;

    // 移動の更新
    var currentVelocity = _currentVelocity;
    var targetVelocity = CalculateMovementSpeed(_inputDirection);

    // 加速度/減速度の適用
    if (targetVelocity.magnitude > 0)
    {
        _currentVelocity = Vector2.MoveTowards(
            currentVelocity,
            targetVelocity,
            _acceleration * deltaTime
        );
    }
    else
    {
        _currentVelocity = Vector2.MoveTowards(
            currentVelocity,
            Vector2.zero,
            _deceleration * deltaTime
        );
    }

    // 位置の更新
    _position += _currentVelocity * deltaTime;
}
```

## 戦闘システムの問題

### 攻撃が機能しない

**症状**:

-   攻撃が発動しない
-   ダメージが適用されない

**考えられる原因**:

1. 攻撃の有効化/無効化が適切でない
2. 攻撃の判定が正しくない
3. ダメージ計算が正しくない

**解決方法**:

```csharp
// 1. 攻撃の有効化/無効化を確認
public void EnableCombat()
{
    _isCombatEnabled = true;
    Debug.Log("戦闘システムを有効化しました");
}

public void DisableCombat()
{
    _isCombatEnabled = false;
    Debug.Log("戦闘システムを無効化しました");
}

// 2. 攻撃の判定を確認
public bool CanAttack()
{
    // 戦闘可能な状態かチェック
    if (!_combatEnabledStates.Contains(_currentState))
    {
        Debug.Log($"現在の状態 '{_currentState}' では攻撃できません");
        return false;
    }

    // 攻撃の制限をチェック
    if (_attackRestrictions.Any(r => r.IsRestricted()))
    {
        Debug.Log("攻撃が制限されています");
        return false;
    }

    return true;
}

// 3. ダメージ計算を確認
public float CalculateDamage(float baseDamage)
{
    var damage = baseDamage;

    // 攻撃力の補正
    damage *= _attackPower;

    // クリティカル判定
    if (IsCriticalHit())
    {
        damage *= _criticalMultiplier;
    }

    // 防御力の計算
    damage = Mathf.Max(0, damage - _defense);

    return damage;
}
```

### 戦闘のバランス

**症状**:

-   戦闘が簡単すぎる/難しすぎる
-   ダメージのバランスが取れていない

**考えられる原因**:

1. パラメータの設定が不適切
2. 戦闘の難易度調整が不十分
3. バランス調整の仕組みが不十分

**解決方法**:

```csharp
// 1. パラメータの設定
public void SetCombatParameters(CombatParameters parameters)
{
    _baseAttackDamage = parameters.BaseAttackDamage;
    _baseDefense = parameters.BaseDefense;
    _criticalRate = parameters.CriticalRate;
    _criticalMultiplier = parameters.CriticalMultiplier;

    Debug.Log("戦闘パラメータを更新しました");
}

// 2. 難易度調整
public void SetDifficultyLevel(DifficultyLevel level)
{
    switch (level)
    {
        case DifficultyLevel.Easy:
            _damageMultiplier = 0.8f;
            _defenseMultiplier = 1.2f;
            break;
        case DifficultyLevel.Normal:
            _damageMultiplier = 1.0f;
            _defenseMultiplier = 1.0f;
            break;
        case DifficultyLevel.Hard:
            _damageMultiplier = 1.2f;
            _defenseMultiplier = 0.8f;
            break;
    }

    Debug.Log($"難易度を {level} に設定しました");
}

// 3. バランス調整
public void AdjustBalance(float damageMultiplier, float defenseMultiplier)
{
    _damageMultiplier = damageMultiplier;
    _defenseMultiplier = defenseMultiplier;

    Debug.Log($"戦闘バランスを調整: 攻撃力 {damageMultiplier}, 防御力 {defenseMultiplier}");
}
```

## アニメーションシステムの問題

### アニメーションが再生されない

**症状**:

-   アニメーションが開始しない
-   アニメーションの遷移が機能しない

**考えられる原因**:

1. アニメーションの初期化が不適切
2. アニメーションの再生条件が正しくない
3. アニメーションの遷移条件が正しくない

**解決方法**:

```csharp
// 1. アニメーションの初期化を確認
public void InitializeAnimation()
{
    // アニメーションクリップの読み込み
    foreach (var clip in _animationClips)
    {
        if (!LoadAnimationClip(clip))
        {
            Debug.LogError($"アニメーションクリップ '{clip.Name}' の読み込みに失敗しました");
            continue;
        }
    }

    // アニメーションの初期状態を設定
    SetInitialAnimation();
}

// 2. アニメーションの再生条件を確認
public bool CanPlayAnimation(string animationName)
{
    // アニメーションが存在するかチェック
    if (!_animationClips.ContainsKey(animationName))
    {
        Debug.LogError($"アニメーション '{animationName}' が見つかりません");
        return false;
    }

    // 再生条件をチェック
    var clip = _animationClips[animationName];
    if (!clip.CanPlay())
    {
        Debug.Log($"アニメーション '{animationName}' の再生条件が満たされていません");
        return false;
    }

    return true;
}

// 3. アニメーションの遷移条件を確認
public bool CanTransitionTo(string targetAnimation)
{
    // 現在のアニメーションから遷移可能かチェック
    if (!_animationTransitions.ContainsKey(_currentAnimation))
    {
        Debug.LogError($"現在のアニメーション '{_currentAnimation}' からの遷移が定義されていません");
        return false;
    }

    var allowedTransitions = _animationTransitions[_currentAnimation];
    if (!allowedTransitions.Contains(targetAnimation))
    {
        Debug.LogError($"アニメーション '{_currentAnimation}' から '{targetAnimation}' への遷移は許可されていません");
        return false;
    }

    return true;
}
```

### アニメーションの同期

**症状**:

-   アニメーションと他のシステムの同期が取れない
-   アニメーションのタイミングがずれる

**考えられる原因**:

1. アニメーションの更新タイミングが適切でない
2. イベントの同期が不十分
3. アニメーションの補間が不適切

**解決方法**:

```csharp
// 1. アニメーションの更新タイミング
public void UpdateAnimation(float deltaTime)
{
    if (!_isAnimationEnabled) return;

    // アニメーションの更新
    _currentAnimationTime += deltaTime;

    // アニメーションの更新処理
    UpdateCurrentAnimation();

    // アニメーションの完了チェック
    CheckAnimationCompletion();
}

// 2. イベントの同期
public void SynchronizeWithEvent(GameEvent evt)
{
    switch (evt)
    {
        case PlayerStateChangedEvent stateEvent:
            OnStateChanged(stateEvent);
            break;
        case PlayerMovementEvent movementEvent:
            OnMovementChanged(movementEvent);
            break;
        case PlayerCombatEvent combatEvent:
            OnCombatChanged(combatEvent);
            break;
    }
}

// 3. アニメーションの補間
public void InterpolateAnimation(float targetTime, float deltaTime)
{
    var currentTime = _currentAnimationTime;
    var interpolatedTime = Mathf.Lerp(currentTime, targetTime, _animationSmoothness * deltaTime);

    SetAnimationTime(interpolatedTime);
}
```

## パフォーマンスの問題

### メモリ使用量の増加

**症状**:

-   メモリ使用量が徐々に増加する
-   パフォーマンスが低下する

**考えられる原因**:

1. リソースの解放が不適切
2. イベントの購読解除が漏れている
3. オブジェクトプールの使用が不適切

**解決方法**:

```csharp
// 1. リソースの解放
public void Dispose()
{
    // リソースの解放
    foreach (var resource in _resources)
    {
        resource.Dispose();
    }
    _resources.Clear();

    // イベントの購読解除
    _disposables.Dispose();

    // オブジェクトプールのクリア
    _objectPool.Clear();
}

// 2. イベントの購読解除
public void UnsubscribeFromEvents()
{
    _disposables.Clear();
    Debug.Log("すべてのイベント購読を解除しました");
}

// 3. オブジェクトプールの使用
public T GetFromPool<T>() where T : class, new()
{
    if (_objectPool.TryGetValue(typeof(T), out var pool))
    {
        return pool.Get() as T;
    }
    return new T();
}

public void ReturnToPool<T>(T obj) where T : class
{
    if (_objectPool.TryGetValue(typeof(T), out var pool))
    {
        pool.Return(obj);
    }
}
```

### CPU 使用率の増加

**症状**:

-   CPU 使用率が高い
-   フレームレートが低下する

**考えられる原因**:

1. 不要な更新処理が行われている
2. 計算処理が最適化されていない
3. スレッドの使用が不適切

**解決方法**:

```csharp
// 1. 更新処理の最適化
public void Update(float deltaTime)
{
    if (!_isEnabled) return;

    // 必要な更新のみを実行
    if (_needsUpdate)
    {
        UpdateSystems(deltaTime);
        _needsUpdate = false;
    }
}

// 2. 計算処理の最適化
public Vector2 CalculateMovement(Vector2 input)
{
    // キャッシュを使用
    if (_cachedInput == input)
    {
        return _cachedResult;
    }

    _cachedInput = input;
    _cachedResult = OptimizedMovementCalculation(input);
    return _cachedResult;
}

// 3. スレッドの使用
public async Task ProcessHeavyComputation()
{
    await Task.Run(() =>
    {
        // 重い計算処理
        PerformHeavyComputation();
    });
}
```

## エラー処理の問題

### 症状

-   エラーが適切に処理されない
-   エラー状態からの回復ができない
-   エラーログが不十分

### 考えられる原因

-   エラーハンドリングの実装が不適切
-   エラー状態の検出が不十分
-   エラーログの出力が不適切

### 解決方法

1. エラーハンドリングの実装を確認
2. エラー状態の検出を強化
3. エラーログの出力を改善

## 変更履歴

| バージョン | 更新日     | 変更内容                                                                         |
| ---------- | ---------- | -------------------------------------------------------------------------------- |
| 0.2.0      | 2025-06-13 | 概要、詳細、使用方法、制限事項セクションを追加 |
| 0.1.0      | 2024-03-24 | 初版作成<br>- 一般的な問題の解決方法を追加<br>- 各システムの問題と解決方法を追加 |
