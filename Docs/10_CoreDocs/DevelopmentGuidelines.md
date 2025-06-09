---
title: 開発ガイドライン
version: 0.4.0
status: draft
updated: 2024-03-23
tags:
    - Development
    - Guidelines
    - Core
    - MVVM
    - Reactive
linked_docs:
    - "[[10_CoreDocs/00_index|コアドキュメントインデックス]]"
    - "[[12_Architecture/12_01_mvvm_rx_architecture|MVVM+RXアーキテクチャ]]"
    - "[[30_APIReference/CoreSystemAPI|コアシステムAPI]]"
    - "[[12_Architecture/12_03_detailed_design/01_core_components/02_viewmodel_base|ViewModelBase実装詳細]]"
    - "[[12_Architecture/12_03_detailed_design/01_core_components/01_reactive_property|ReactiveProperty実装詳細]]"
    - "[[12_Architecture/12_03_detailed_design/01_core_components/03_composite_disposable|CompositeDisposable実装詳細]]"
    - "[[12_Architecture/12_03_detailed_design/01_core_components/04_event_bus|イベントバス実装詳細]]"
    - "[[12_Architecture/12_03_detailed_design/02_systems/07_animation_system|アニメーションシステム詳細設計]]"
    - "[[12_Architecture/12_03_detailed_design/02_systems/08_sound_system|サウンドシステム詳細設計]]"
    - "[[12_Architecture/12_03_detailed_design/02_systems/09_ui_system|UIシステム詳細設計]]"
    - "[[12_Architecture/12_03_detailed_design/02_systems/10_network_system|ネットワークシステム詳細設計]]"
---

# 開発ガイドライン

## 目次

1. [概要](#概要)
2. [開発方針](#開発方針)
3. [コーディング規約](#コーディング規約)
4. [開発フロー](#開発フロー)
5. [テスト方針](#テスト方針)
6. [パフォーマンス最適化](#パフォーマンス最適化)
7. [セキュリティガイドライン](#セキュリティガイドライン)
8. [制限事項](#制限事項)
9. [変更履歴](#変更履歴)

## 概要

このドキュメントでは、プロジェクトの開発に関するガイドラインを説明します。
コーディング規約、開発フロー、テスト方針などを詳細に記述しています。

## 開発方針

本プロジェクトは「MVVM + リアクティブプログラミング」に基づいて実装されます。

### アーキテクチャ原則

-   **Model**：状態とドメインロジックを保持。副作用なし。
-   **ViewModel**：Model の状態を購読し、View へ変換・通知。
-   **View**：Godot Node。ViewModel を購読し、描画・演出を行う。
-   状態は`ReactiveProperty<T>`で通知可能に。
-   入力やイベントは`Subject<T>`や`SignalBus`で非同期処理。

### 実装例

```gdscript
# Model
class_name PlayerModel
extends Object

var health: ReactiveProperty[int] = ReactiveProperty.new(100)
var position: ReactiveProperty[Vector2] = ReactiveProperty.new(Vector2.ZERO)

func take_damage(amount: int) -> void:
    health.value = max(0, health.value - amount)

# ViewModel
class_name PlayerViewModel
extends Object

var model: PlayerModel
var display_health: ReactiveProperty[String] = ReactiveProperty.new("")
var is_dead: ReactiveProperty[bool] = ReactiveProperty.new(false)

func _init(p_model: PlayerModel) -> void:
    model = p_model
    model.health.subscribe(_on_health_changed).add_to(self)

func _on_health_changed(new_health: int) -> void:
    display_health.value = str(new_health)
    is_dead.value = new_health <= 0

# View
class_name PlayerView
extends Node2D

@onready var health_label: Label = $HealthLabel
@onready var view_model: PlayerViewModel

func _ready() -> void:
    view_model = PlayerViewModel.new(PlayerModel.new())
    view_model.display_health.subscribe(_on_display_health_changed).add_to(self)
    view_model.is_dead.subscribe(_on_death_state_changed).add_to(self)

func _on_display_health_changed(new_text: String) -> void:
    health_label.text = new_text

func _on_death_state_changed(is_dead: bool) -> void:
    modulate.a = 0.5 if is_dead else 1.0
```

### システム開発ガイドライン

#### アニメーションシステム

-   アニメーションの状態管理は Model で行う
-   アニメーションの再生制御は ViewModel で行う
-   アニメーションの実際の再生は View で行う
-   アニメーションの切り替えはイベントバスを使用

#### サウンドシステム

-   サウンドの状態管理は Model で行う
-   サウンドの再生制御は ViewModel で行う
-   サウンドの実際の再生は View で行う
-   音量やミキシングの設定は ViewModel で管理

#### UI システム

-   UI の状態管理は Model で行う
-   UI の表示制御は ViewModel で行う
-   UI の実際の描画は View で行う
-   UI のアニメーションは ViewModel で制御

#### ネットワークシステム

-   ネットワークの状態管理は Model で行う
-   ネットワークの通信制御は ViewModel で行う
-   ネットワークの実際の通信は View で行う
-   データの同期は ViewModel で管理

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
    class_name ExampleViewModel
    extends Object

    # シグナル定義
    signal health_changed(new_health: int)
    signal player_died()
    signal level_up(new_level: int)

    # リアクティブプロパティ
    var health: ReactiveProperty[int] = ReactiveProperty.new(100)
    var position: ReactiveProperty[Vector2] = ReactiveProperty.new(Vector2.ZERO)
    var is_active: ReactiveProperty[bool] = ReactiveProperty.new(false)

    # プライベート変数
    var _model: ExampleModel
    var _disposables: Array[Disposable] = []

    # 初期化
    func _init(model: ExampleModel) -> void:
        _model = model
        _setup_subscriptions()

    # 購読の設定
    func _setup_subscriptions() -> void:
        _model.health.subscribe(_on_health_changed).add_to(self)
        _model.position.subscribe(_on_position_changed).add_to(self)
        _model.is_active.subscribe(_on_active_changed).add_to(self)

    # 購読ハンドラ
    func _on_health_changed(new_health: int) -> void:
        health.value = new_health
        emit_signal("health_changed", new_health)

    func _on_position_changed(new_position: Vector2) -> void:
        position.value = new_position

    func _on_active_changed(new_active: bool) -> void:
        is_active.value = new_active
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
        - Model / ViewModel / View の分離を考慮する（MVVM）
        - リアクティブな状態管理の設計
        - イベントフローの設計
    - インターフェース設計
        - リアクティブプロパティの定義
        - イベントストリームの定義
        - エラーハンドリングの設計
    - データ構造設計
        - 状態モデルの設計
        - イベントモデルの設計
        - キャッシュ戦略の設計

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
    # Modelのテスト
    func test_player_model():
        var model = PlayerModel.new()
        model.take_damage(10)
        assert_eq(model.health.value, 90)
    ```

2. ViewModel の単体テスト

    ```gdscript
    # ViewModelのテスト
    func test_player_view_model():
        var model = PlayerModel.new()
        var vm = PlayerViewModel.new(model)
        vm.take_damage(20)
        assert_eq(model.health.value, 80)
        assert_eq(vm.display_health.value, "80")
        assert_false(vm.is_dead.value)
    ```

3. 統合テスト

    ```gdscript
    # MVVM統合テスト
    func test_mvvm_integration():
        var model = PlayerModel.new()
        var vm = PlayerViewModel.new(model)
        var view = PlayerView.new()
        view.view_model = vm

        vm.take_damage(50)
        assert_eq(view.health_label.text, "50")
        assert_eq(view.modulate.a, 1.0)
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
    # Modelのテスト
    func test_player_model():
        var model = PlayerModel.new()
        model.take_damage(10)
        assert_eq(model.health.value, 90)
    ```

2. ViewModel の単体テスト

    ```gdscript
    # ViewModelのテスト
    func test_player_view_model():
        var model = PlayerModel.new()
        var vm = PlayerViewModel.new(model)
        vm.take_damage(20)
        assert_eq(model.health.value, 80)
        assert_eq(vm.display_health.value, "80")
        assert_false(vm.is_dead.value)
    ```

3. 統合テスト

    ```gdscript
    # MVVM統合テスト
    func test_mvvm_integration():
        var model = PlayerModel.new()
        var vm = PlayerViewModel.new(model)
        var view = PlayerView.new()
        view.view_model = vm

        vm.take_damage(50)
        assert_eq(view.health_label.text, "50")
        assert_eq(view.modulate.a, 1.0)
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

| バージョン | 更新日     | 変更内容                           |
| ---------- | ---------- | ---------------------------------- |
| 0.3.0      | 2025-06-01 | 初版作成                           |
| 0.4.0      | 2024-03-23 | 新規システムの開発ガイドライン追加 |
