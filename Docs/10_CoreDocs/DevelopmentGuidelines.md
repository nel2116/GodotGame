---
title: 開発ガイドライン
version: 0.2.0
status: draft
updated: 2025-06-01
tags:
    - Development
    - Guidelines
    - Core
linked_docs:
    - "[[10_CoreDocs/00_index]]"
    - "[[10_CoreDocs/Architecture]]"
    - "[[30_APIReference/CoreSystemAPI]]"
---

# 開発ガイドライン

## 目次

1. [概要](#概要)
2. [コーディング規約](#コーディング規約)
3. [開発フロー](#開発フロー)
4. [テスト方針](#テスト方針)
5. [パフォーマンス最適化](#パフォーマンス最適化)
6. [セキュリティガイドライン](#セキュリティガイドライン)
7. [制限事項](#制限事項)
8. [変更履歴](#変更履歴)

## 概要

このドキュメントでは、プロジェクトの開発に関するガイドラインを説明します。
コーディング規約、開発フロー、テスト方針などを詳細に記述しています。

## コーディング規約

### 命名規則

1. クラス名

    ```gdscript
    # パスカルケースを使用
    class_name PlayerController
    class_name GameManager
    class_name UIManager

    # インターフェースはIプレフィックスを使用
    class_name IPlayerController
    class_name IGameManager
    class_name IUIManager

    # 抽象クラスはAbstractプレフィックスを使用
    class_name AbstractPlayerController
    class_name AbstractGameManager
    class_name AbstractUIManager
    ```

2. 変数名

    ```gdscript
    # スネークケースを使用
    var player_health: int
    var current_scene: String
    var is_game_paused: bool

    # 定数は大文字のスネークケースを使用
    const MAX_HEALTH: int = 100
    const MIN_SPEED: float = 0.0
    const DEFAULT_SCENE: String = "res://scenes/main.tscn"

    # プライベート変数は_プレフィックスを使用
    var _internal_state: Dictionary
    var _cached_data: Array
    var _is_initialized: bool
    ```

3. 関数名

    ```gdscript
    # スネークケースを使用
    func initialize_player() -> void
    func update_game_state() -> void
    func handle_input() -> void

    # プライベート関数は_プレフィックスを使用
    func _internal_update() -> void
    func _validate_data() -> bool
    func _cleanup_resources() -> void

    # ゲッター/セッターはget_/set_プレフィックスを使用
    func get_player_health() -> int
    func set_player_health(value: int) -> void
    ```

### コード構造

1. クラス構造

    ```gdscript
    class_name ExampleClass
    extends Node

    # シグナル定義
    signal health_changed(new_health: int)
    signal player_died()
    signal level_up(new_level: int)

    # 定数
    const MAX_HEALTH = 100
    const MIN_SPEED = 0.0
    const DEFAULT_SCENE = "res://scenes/main.tscn"

    # エクスポート変数（エディタで設定可能）
    @export var health: int = MAX_HEALTH
    @export var speed: float = MIN_SPEED
    @export var scene_path: String = DEFAULT_SCENE

    # プライベート変数
    var _internal_state: Dictionary = {}
    var _cached_data: Array = []
    var _is_initialized: bool = false

    # 初期化
    func _init() -> void:
        _initialize_state()
        _setup_connections()
        _validate_settings()

    # 準備
    func _ready() -> void:
        _load_resources()
        _setup_ui()
        _start_game()

    # 更新処理
    func _process(delta: float) -> void:
        _update_state(delta)
        _handle_input()
        _check_conditions()

    # 物理更新
    func _physics_process(delta: float) -> void:
        _update_physics(delta)
        _check_collisions()
        _apply_forces()

    # 入力処理
    func _input(event: InputEvent) -> void:
        _handle_keyboard(event)
        _handle_mouse(event)
        _handle_gamepad(event)

    # パブリック関数
    func initialize() -> void:
        _internal_state.clear()
        _cached_data.clear()
        _is_initialized = true
        emit_signal("initialized")

    func update_health(value: int) -> void:
        health = clamp(value, 0, MAX_HEALTH)
        emit_signal("health_changed", health)
        if health <= 0:
            emit_signal("player_died")

    # プライベート関数
    func _initialize_state() -> void:
        _internal_state = {
            "health": health,
            "speed": speed,
            "position": Vector2.ZERO
        }

    func _setup_connections() -> void:
        # シグナルの接続
        pass

    func _validate_settings() -> void:
        # 設定の検証
        pass
    ```

2. 関数の構造

    ```gdscript
    # 関数の説明
    # @param param1 パラメータ1の説明
    # @param param2 パラメータ2の説明
    # @return 戻り値の説明
    # @throws 例外の説明
    func example_function(param1: int, param2: String) -> bool:
        # 入力値の検証
        if not _validate_input(param1, param2):
            push_error("Invalid input parameters")
            return false

        # メイン処理
        var result = _process_data(param1, param2)

        # 結果の検証
        if not _validate_result(result):
            push_error("Invalid result")
            return false

        # 後処理
        _cleanup_resources()
        _update_state()

        return true

    # プライベート関数
    func _validate_input(param1: int, param2: String) -> bool:
        if param1 < 0:
            return false
        if param2.is_empty():
            return false
        return true

    func _process_data(param1: int, param2: String) -> Dictionary:
        var result = {}
        # データ処理
        return result

    func _validate_result(result: Dictionary) -> bool:
        if result.is_empty():
            return false
        return true

    func _cleanup_resources() -> void:
        # リソースの解放
        pass

    func _update_state() -> void:
        # 状態の更新
        pass
    ```

## 開発フロー

### 1. 機能開発

1. 要件の確認

    - 機能の目的
        - ユーザーストーリーの作成
        - 機能要件の定義
        - 非機能要件の定義
    - 必要なリソース
        - アセットの確認
        - 外部ライブラリの確認
        - 開発環境の確認
    - 制約条件
        - パフォーマンス要件
        - セキュリティ要件
        - 互換性要件

2. 設計

    - クラス設計
        - クラス図の作成
        - インターフェースの定義
        - 継承関係の決定
    - インターフェース設計
        - API の定義
        - パラメータの設計
        - 戻り値の設計
    - データ構造設計
        - データモデルの作成
        - リレーションの定義
        - インデックスの設計

3. 実装
    - コーディング
        - コーディング規約の遵守
        - エラーハンドリング
        - ログ出力
    - コードレビュー
        - コードの品質確認
        - セキュリティチェック
        - パフォーマンス確認
    - リファクタリング
        - コードの最適化
        - 重複の排除
        - 可読性の向上

### 2. テスト

1. 単体テスト

    ```gdscript
    # テストクラス
    class_name TestPlayer
    extends GutTest

    # テスト対象のインスタンス
    var player: Player

    # テストの準備
    func before_each() -> void:
        player = Player.new()
        add_child(player)

    # テストの後処理
    func after_each() -> void:
        player.queue_free()

    # ヘルス関連のテスト
    func test_player_health() -> void:
        # 初期値の確認
        assert_eq(player.health, 100)

        # ダメージのテスト
        player.take_damage(10)
        assert_eq(player.health, 90)

        # 回復のテスト
        player.heal(20)
        assert_eq(player.health, 100)

        # 死亡のテスト
        player.take_damage(100)
        assert_true(player.is_dead)
    ```

2. 統合テスト

    ```gdscript
    # テストクラス
    class_name TestCombatSystem
    extends GutTest

    # テスト対象のインスタンス
    var combat: CombatSystem
    var player: Player
    var enemy: Enemy

    # テストの準備
    func before_each() -> void:
        combat = CombatSystem.new()
        player = Player.new()
        enemy = Enemy.new()
        add_child(combat)
        add_child(player)
        add_child(enemy)

    # テストの後処理
    func after_each() -> void:
        combat.queue_free()
        player.queue_free()
        enemy.queue_free()

    # 戦闘システムのテスト
    func test_combat_system() -> void:
        # 戦闘開始のテスト
        combat.start_combat(player, enemy)
        assert_true(combat.is_active)

        # 攻撃のテスト
        combat.attack(player, enemy)
        assert_lt(enemy.health, enemy.max_health)

        # 防御のテスト
        combat.defend(player)
        assert_true(player.is_defending)

        # 戦闘終了のテスト
        combat.end_combat()
        assert_false(combat.is_active)
    ```

### 3. デプロイ

1. ビルド

    - リソースの最適化
        - テクスチャの圧縮
        - モデルの最適化
        - サウンドの圧縮
    - エラーチェック
        - コンパイルエラー
        - リンクエラー
        - ランタイムエラー
    - バージョン管理
        - バージョン番号の更新
        - 変更履歴の更新
        - タグの作成

2. リリース
    - 変更履歴の更新
        - 新機能の説明
        - バグ修正の説明
        - 既知の問題
    - ドキュメントの更新
        - API ドキュメント
        - ユーザーガイド
        - 開発者ガイド
    - ユーザーへの通知
        - リリースノート
        - アップデート方法
        - サポート情報

## テスト方針

### テストの種類

1. 単体テスト

    ```gdscript
    # プレイヤーのテスト
    func test_player_health():
        var player = Player.new()
        player.take_damage(10)
        assert(player.health == 90)
    ```

2. 統合テスト
    ```gdscript
    # 戦闘システムのテスト
    func test_combat_system():
        var player = Player.new()
        var enemy = Enemy.new()
        var combat = CombatSystem.new()
        combat.start_combat(player, enemy)
        assert(combat.is_active)
    ```

### テストカバレッジ

-   主要機能: 100%
    -   コアシステム
    -   ゲームプレイシステム
    -   データ管理システム
-   ユーティリティ: 80%以上
    -   ヘルパー関数
    -   ユーティリティクラス
    -   共通処理
-   UI: 70%以上
    -   画面遷移
    -   イベント処理
    -   アニメーション

## パフォーマンス最適化

### メモリ管理

1. リソースの最適化

    - テクスチャの圧縮
    - モデルの最適化
    - サウンドの圧縮

2. メモリ使用量の制御
    - キャッシュサイズの制限
    - リソースの解放
    - メモリリークの防止

### レンダリング最適化

1. 描画の最適化

    - バッチ処理
    - カリング
    - LOD（Level of Detail）

2. シェーダーの最適化
    - シェーダーの簡略化
    - 計算の最適化
    - テクスチャの最適化

## セキュリティガイドライン

### データ保護

1. セーブデータの保護

    - データの暗号化
    - 改ざん検知
    - バックアップ

2. 通信の保護
    - 暗号化通信
    - 認証
    - アクセス制御

### エラーハンドリング

1. 例外処理

    - エラーの検出
    - リカバリー
    - ログ記録

2. デバッグ情報
    - エラー情報
    - スタックトレース
    - デバッグログ

## 制限事項

-   コードの複雑度は循環的複雑度 10 以下を目標とします
-   1 つの関数は 50 行以内を目標とします
-   クラスは 500 行以内を目標とします
-   テストカバレッジは 80%以上を維持します
-   パフォーマンスは実行環境に依存します
-   セキュリティ対策は継続的に更新が必要です

## 変更履歴

| バージョン | 更新日     | 変更内容                                         |
| ---------- | ---------- | ------------------------------------------------ |
| 0.2.0      | 2025-06-01 | パフォーマンスとセキュリティのガイドラインを追加 |
| 0.1.0      | 2025-06-01 | 初版作成                                         |
