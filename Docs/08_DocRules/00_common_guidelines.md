# 共通ガイドライン

## 命名規則

### クラス名

-   パスカルケースを使用
    ```gdscript
    class_name PlayerController
    class_name GameManager
    class_name UIManager
    ```
-   インターフェースは`I`プレフィックスを使用
    ```gdscript
    class_name IPlayerController
    class_name IGameManager
    class_name IUIManager
    ```
-   抽象クラスは`Abstract`プレフィックスを使用
    ```gdscript
    class_name AbstractPlayerController
    class_name AbstractGameManager
    class_name AbstractUIManager
    ```

### 変数名

-   プライベート変数は`_`プレフィックスを使用
    ```gdscript
    var _private_variable
    var _internal_state
    ```
-   定数は大文字のスネークケースを使用
    ```gdscript
    const MAX_PLAYERS = 4
    const DEFAULT_TIMEOUT = 30
    ```

## メモリ管理

### リソース管理

-   リソースのキャッシュサイズは適切に制限する
    ```gdscript
    const DEFAULT_MAX_CACHE_SIZE = 1024 * 1024 * 100  # 100MB
    ```
-   未使用リソースは定期的にクリーンアップする
    ```gdscript
    func cleanup_unused_resources():
        var unused = _resource_cache.filter(func(res): return !res.is_in_use)
        for resource in unused:
            unload_resource(resource)
    ```

### オブジェクトプール

-   頻繁に生成・破棄されるオブジェクトはプールを使用

    ```gdscript
    class_name ObjectPool
    extends Node

    func _init():
        setup_pools()
        setup_recycling()

    func setup_pools():
        create_pool("enemies", 50)
        create_pool("projectiles", 100)
        create_pool("effects", 200)
    ```

### メモリ最適化

-   メモリ使用量を監視し、警告閾値を設定
    ```gdscript
    func setup_memory_tracking():
        set_tracking_enabled(true)
        set_warning_threshold(0.8)  # 80%使用時に警告
    ```
-   定期的なメモリ最適化を実施
    ```gdscript
    func setup_optimization():
        set_optimization_interval(60)  # 1分ごと
        set_optimization_targets(["textures", "meshes", "sounds"])
    ```

### リソース解放

-   明示的なリソース解放を実装
    ```gdscript
    func dispose():
        for resource in _resource_cache.values():
            resource.dispose()
        _resource_cache.clear()
    ```
-   未使用リソースの自動解放
    ```gdscript
    func cleanup_oldest_resources():
        var oldest = _resource_cache.values().sort_custom(
            func(a, b): return a.last_access_time < b.last_access_time
        ).slice(0, max_resources_to_evict)
        for resource in oldest:
            unload_resource(resource)
    ```
