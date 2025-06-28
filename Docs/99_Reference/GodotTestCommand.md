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

- 上記コマンドは GUT テストをヘッドレスモードで実行します。
- テスト結果はコンソールに出力され、XML レポートは `user://` 以下に保存されます。
- 実行前に `.gutconfig.json` がプロジェクトルートに存在することを確認してください。

詳しい開発ワークフローは [[DevWorkflows.md|共通開発ワークフロー]] を参照してください。

