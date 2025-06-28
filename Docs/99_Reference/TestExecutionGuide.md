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

## 最新テスト結果（2025-06-29）

### Core テスト実行結果

```bash
# 実行結果サマリー
テスト概要: 合計: 88, 失敗数: 0, 成功数: 88, スキップ済み数: 0, 期間: 2.5 秒
```

### 実行時の注意事項

-   **イベントバッファリング**: GameEventBus の 16ms バッファリングにより、テストで 20ms の遅延が必要
-   **Godot 依存テスト**: `Tests/Integration_Godot/`配下は GUT で実行、Core テストとは分離
-   **警告**: CS8785（Godot 関連）、CS8625/CS8600（null 非許容型）は動作に影響なし

### 推奨実行手順

1. **Core テスト**: `dotnet test Tests/Core/CoreTests.csproj`
2. **GUT テスト**: Godot エディタで GUT パネルから実行
3. **結果確認**: テスト結果レポートを参照

詳細は [[../07_Testing/TestResultsReport|テスト結果レポート]] を参照してください。

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
