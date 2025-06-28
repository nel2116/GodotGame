---
title: テスト実行環境ガイド
version: 1.1.0
status: active
updated: 2025-06-29
tags:
    - Testing
    - CI/CD
    - Environment
    - CoreTests
    - GUT
linked_docs:
    - "[[index|テスト戦略]]"
    - "[[TestResultsReport|テスト結果レポート]]"
    - "[[../99_Reference/TestExecutionGuide|テスト実行ガイド]]"
---

# テスト実行環境ガイド

## 目次

1. [概要](#概要)
2. [テスト環境のセットアップ](#テスト環境のセットアップ)
3. [テストの種類と実行方法](#テストの種類と実行方法)
4. [CI/CD 連携](#cicd連携)
5. [最新テスト結果](#最新テスト結果)
6. [トラブルシューティング](#トラブルシューティング)
7. [ベストプラクティス](#ベストプラクティス)

## 概要

このドキュメントでは、テスト実行環境のセットアップと CI/CD との連携方法について説明します。
Core テスト（.NET/NUnit）と GUT テスト（Godot 依存）の分離実行環境についても詳しく説明します。

## テスト環境のセットアップ

### 1. 必要なツール

-   **Godot Engine 4.2**（GUT テスト用）
-   **.NET SDK 8.0**（Core テスト用）
-   **Git**
-   **Visual Studio 2022**（推奨）
-   **GUT アドオン**（Godot Unit Test）

### 2. 環境変数の設定

```powershell
# 環境変数の設定
$env:GODOT_PATH = "C:\Program Files\Godot\Godot_v4.2.2-stable_win64.exe"
$env:DOTNET_PATH = "C:\Program Files\dotnet\dotnet.exe"
```

### 3. テストプロジェクトの構成

```
/Tests
  /Core                    # .NET/NUnitテスト（Godot非依存）
    /Events
    /ViewModels
    /Reactive
    /Utilities
    /State
    /Resource
  /Integration_Godot       # GUTテスト（Godot依存）
    /Player
    /Common
  /TestData
```

## テストの種類と実行方法

### 1. Core テスト（.NET/NUnit）

```powershell
# Coreテストの実行
cd Tests/Core
dotnet test

# 詳細出力
dotnet test -v detailed

# 特定のテストクラスのみ実行
dotnet test --filter "FullyQualifiedName~GameEventBusTests"
```

### 2. GUT テスト（Godot 依存）

```powershell
# GUTテストの実行（CLI）
godot --headless --path . -s addons/gut/gut_cmdln.gd -gconfig=.gutconfig.json

# Godotエディタでの実行
# 1. Godotエディタを開く
# 2. GUTパネルを開く
# 3. テストを選択して実行
```

### 3. テスト実行の分離

```powershell
# Coreテストのみ実行（推奨）
dotnet test Tests/Core/CoreTests.csproj

# GUTテストのみ実行
godot --headless --script addons/gut/gut_cmdln.gd --test-script Tests/Integration_Godot/Player/PlayerMovementViewModelTests.gd
```

## CI/CD 連携

### 1. GitHub Actions 設定（更新版）

```yaml
name: Test

on:
    push:
        branches: [main]
    pull_request:
        branches: [main]

jobs:
    core-test:
        runs-on: windows-latest
        steps:
            - uses: actions/checkout@v3
            - name: Setup .NET
              uses: actions/setup-dotnet@v3
              with:
                  dotnet-version: 8.0.x
            - name: Restore dependencies
              run: dotnet restore
            - name: Build
              run: dotnet build --no-restore
            - name: Core Tests
              run: dotnet test Tests/Core/CoreTests.csproj --no-build --verbosity normal
            - name: Upload test results
              uses: actions/upload-artifact@v3
              with:
                  name: core-test-results
                  path: Tests/Core/TestResults/

    gut-test:
        runs-on: windows-latest
        needs: core-test
        steps:
            - uses: actions/checkout@v3
            - name: Setup Godot
              uses: actions/setup-dotnet@v3
              with:
                  dotnet-version: 8.0.x
            - name: Download Godot
              run: |
                  curl -L -o godot.zip https://github.com/godotengine/godot/releases/download/4.2.2-stable/Godot_v4.2.2-stable_win64.exe.zip
                  Expand-Archive godot.zip -DestinationPath godot
            - name: GUT Tests
              run: ./godot/Godot_v4.2.2-stable_win64.exe --headless --script addons/gut/gut_cmdln.gd
```

### 2. テストレポート

-   **Core テスト結果**: `Tests/Core/TestResults/`ディレクトリに出力
-   **GUT テスト結果**: `res://Scripts/Tests/test-results_*.xml`に出力
-   **カバレッジレポート**: `coverage/`ディレクトリに出力

### 3. 自動デプロイ

-   テスト成功時のみデプロイを実行
-   Core テストと GUT テストの両方が成功した場合のみデプロイ
-   デプロイ先は環境変数で指定

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

詳細は [[TestResultsReport|テスト結果レポート]] を参照してください。

## トラブルシューティング

### 1. Core テストの問題

-   **テストが失敗する場合**

    -   イベントバッファリング遅延の確認（Thread.Sleep(20)が必要）
    -   EventBus・ViewModel の明示的インスタンス生成確認
    -   型名・using ディレクティブの確認

-   **コンパイルエラー**
    -   .NET SDK 8.0 の確認
    -   依存関係の復元確認
    -   プロジェクトファイルの確認

### 2. GUT テストの問題

-   **Godot 依存テストがクラッシュ**

    -   Godot エディタでの実行を確認
    -   GUT アドオンの有効化確認
    -   テストファイルの配置確認

-   **テストが見つからない**
    -   テストファイルの命名規則確認
    -   GUT 設定ファイルの確認

### 3. CI/CD 特有の問題

-   **ビルドが失敗する場合**

    -   キャッシュのクリア
    -   依存関係の再インストール
    -   ログの確認

-   **テスト実行が遅い場合**
    -   並列実行の設定確認
    -   テストの最適化
    -   リソース使用量の確認

## ベストプラクティス

### 1. テスト設計

-   **テストは独立して実行可能であること**
-   **テストデータは適切に管理すること**
-   **モックとスタブを適切に使用すること**
-   **イベント駆動システムのテストは適切な遅延を設定すること**

### 2. メンテナンス

-   **定期的なテストの見直し**
-   **不要なテストの削除**
-   **テストカバレッジの監視**
-   **テスト結果レポートの定期更新**

### 3. セキュリティ

-   **機密情報は環境変数で管理**
-   **テストデータに機密情報を含めない**
-   **アクセス権限の適切な設定**

### 4. パフォーマンス

-   **イベントバッファリングを考慮したテスト設計**
-   **大量データ処理時のメモリ監視**
-   **テスト実行時間の最適化**

## 変更履歴

| バージョン | 更新日     | 変更内容                                             |
| ---------- | ---------- | ---------------------------------------------------- |
| 1.1.0      | 2025-06-29 | Core テストと GUT テストの分離、最新テスト結果を追記 |
| 1.0.0      | 2024-03-21 | 初版作成                                             |
