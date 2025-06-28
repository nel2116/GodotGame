---
title: Godotテスト実行コマンド
version: 0.1
status: draft
updated: 2025-06-07
tags:
    - Test
    - Reference
    - Workflow
linked_docs:
    - "[[DevWorkflows.md]]"
---

# Godot テスト実行コマンド

プロジェクトで共通して利用する自動テスト実行コマンドをまとめています。

```bash
godot --headless --path . -s addons/gut/gut_cmdln.gd -gconfig=.gutconfig.json
```

-   上記コマンドは GUT テストをヘッドレスモードで実行します。
-   テスト結果はコンソールに出力され、XML レポートは `user://` 以下に保存されます。
-   実行前に `.gutconfig.json` がプロジェクトルートに存在することを確認してください。

詳しい開発ワークフローは [[DevWorkflows.md|共通開発ワークフロー]] を参照してください。

# Godot 依存テストの実行方法（GUT）

## 概要

-   Godot のネイティブ API やシーンライフサイクルに依存するテストは、GUT（Godot Unit Test）で管理・実行します。
-   対象: Input/Movement/Animation/Combat/State/Progression の ViewModel/Model テスト、PlayerSystemIntegrationTests 等

## 実行手順

1. Godot エディタでプロジェクトを開く
2. GUT アドオンを有効化（`addons/gut`）
3. Godot エディタの GUT UI またはコマンドラインでテストを実行
    - コマンド例: `godot --headless --script addons/gut/gut_cmdln.gd`

## 注意事項

-   GUT テストは `Tests/Integration_Godot/` 配下に配置してください
-   .NET テスト（CoreTests）は `dotnet test` で実行し、Godot 依存テストは必ず GUT で実行してください
-   テスト分離により、Godot のクラッシュや不安定な挙動を防止できます
