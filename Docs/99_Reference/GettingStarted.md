---
title: はじめに
version: 0.2.0
status: draft
updated: 2025-06-01
tags:
    - Tutorial
    - GettingStarted
    - Guide
linked_docs:
    - "[[40_Tutorials/00_index]]"
    - "[[40_Tutorials/BasicFeatures]]"
    - "[[30_APIReference/CoreSystemAPI]]"
---

# はじめに

## 目次

1. [概要](#概要)
2. [環境構築](#環境構築)
3. [基本操作](#基本操作)
4. [プロジェクト構造](#プロジェクト構造)
5. [開発フロー](#開発フロー)
6. [制限事項](#制限事項)
7. [変更履歴](#変更履歴)

## 概要

このドキュメントでは、プロジェクトの始め方について説明します。
環境構築から基本操作、プロジェクト構造まで、開発を始めるために必要な情報を提供します。

## 環境構築

### 必要なソフトウェア

1. Godot Engine

    - バージョン: 4.2.0 以上
    - ダウンロード: [Godot Engine](https://godotengine.org/download)
    - インストール手順:
        1. ダウンロードしたファイルを実行
        2. インストール先を選択
        3. インストールの完了を待つ

2. Visual Studio Code

    - バージョン: 1.80.0 以上
    - ダウンロード: [Visual Studio Code](https://code.visualstudio.com/)
    - インストール手順:
        1. ダウンロードしたファイルを実行
        2. インストール先を選択
        3. インストールの完了を待つ

3. Git
    - バージョン: 2.40.0 以上
    - ダウンロード: [Git](https://git-scm.com/downloads)
    - インストール手順:
        1. ダウンロードしたファイルを実行
        2. インストール先を選択
        3. インストールの完了を待つ

### プロジェクトのセットアップ

1. リポジトリのクローン

    ```bash
    git clone https://github.com/your-username/your-project.git
    cd your-project
    ```

2. 依存関係のインストール

    ```bash
    # プロジェクトの依存関係をインストール
    godot --headless --export-release "Windows Desktop" ./build/game.exe
    ```

3. 開発環境の設定
    - Visual Studio Code の設定
        1. Godot Tools 拡張機能のインストール
        2. C# 拡張機能のインストール
        3. 設定ファイルの編集

## 基本操作

### エディタの操作

1. シーンの作成

    - シーンツリーの使用
    - ノードの追加
    - プロパティの設定

2. スクリプトの作成

    - スクリプトの新規作成
    - クラスの定義
    - 関数の実装

3. リソースの管理
    - アセットのインポート
    - リソースの整理
    - 参照の設定

### デバッグ

1. デバッグツール

    - ブレークポイントの設定
    - 変数の監視
    - コールスタックの確認

2. ログ出力

    ```gdscript
    # デバッグログ
    print("Debug message")

    # エラーログ
    push_error("Error message")

    # 警告ログ
    push_warning("Warning message")
    ```

3. プロファイリング
    - パフォーマンスの計測
    - メモリ使用量の確認
    - ボトルネックの特定

## プロジェクト構造

### ディレクトリ構成

```
project/
├── assets/          # アセットファイル
│   ├── images/     # 画像ファイル
│   ├── models/     # 3Dモデル
│   └── sounds/     # サウンドファイル
├── scenes/         # シーンファイル
│   ├── levels/     # レベルシーン
│   ├── ui/         # UIシーン
│   └── common/     # 共通シーン
├── scripts/        # スクリプトファイル
│   ├── core/       # コアスクリプト
│   ├── gameplay/   # ゲームプレイスクリプト
│   └── utils/      # ユーティリティスクリプト
├── docs/           # ドキュメント
│   ├── api/        # APIリファレンス
│   ├── guides/     # ガイド
│   └── tutorials/  # チュートリアル
└── tests/          # テストファイル
    ├── unit/       # 単体テスト
    └── integration/# 統合テスト
```

### ファイル命名規則

1. シーンファイル

    - プレフィックス: `scene_`
    - 例: `scene_main_menu.tscn`

2. スクリプトファイル

    - プレフィックス: `script_`
    - 例: `script_player.gd`

3. リソースファイル
    - プレフィックス: `res_`
    - 例: `res_player_sprite.png`

## 開発フロー

### 1. 機能開発

1. ブランチの作成

    ```bash
    git checkout -b feature/new-feature
    ```

2. 開発

    - コードの実装
    - テストの作成
    - ドキュメントの更新

3. コミット
    ```bash
    git add .
    git commit -m "Add new feature"
    ```

### 2. テスト

1. 単体テスト

    ```gdscript
    # テストクラス
    class_name TestPlayer
    extends GutTest

    # テストの準備
    func before_each() -> void:
        player = Player.new()
        add_child(player)

    # テストの実行
    func test_player_health() -> void:
        assert_eq(player.health, 100)
    ```

2. 統合テスト

    ```gdscript
    # テストクラス
    class_name TestGame
    extends GutTest

    # テストの準備
    func before_each() -> void:
        game = Game.new()
        add_child(game)

    # テストの実行
    func test_game_flow() -> void:
        assert_true(game.is_running)
    ```

### 3. レビュー

1. プルリクエストの作成

    - 変更内容の説明
    - レビュアーの指定
    - テスト結果の添付

2. コードレビュー

    - コードの品質確認
    - セキュリティチェック
    - パフォーマンス確認

3. マージ
    - レビューコメントの反映
    - コンフリクトの解決
    - マージの実行

## 制限事項

-   プロジェクトは Godot 4.2.0 以上が必要です
-   一部の機能は特定の環境でのみ動作します
-   パフォーマンスは実行環境に依存します
-   セキュリティ対策は継続的に更新が必要です

## 変更履歴

| バージョン | 更新日     | 変更内容                                 |
| ---------- | ---------- | ---------------------------------------- |
| 0.2.0      | 2025-06-01 | プロジェクト構造と開発フローの詳細を追加 |
| 0.1.0      | 2025-06-01 | 初版作成                                 |
