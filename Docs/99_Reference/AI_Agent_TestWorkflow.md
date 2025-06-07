---
title: AIエージェント向けテスト実行ワークフロー
version: 0.1
status: draft
updated: 2025-06-07
tags:
    - AI
    - Test
    - Workflow
    - Reference
linked_docs:
    - "[[TestExecutionGuide.md]]"
    - "[[AI_Agent_ImplementationWorkflow.md]]"
---

# 目次
1. [概要](#概要)
2. [手順](#手順)
3. [注意点](#注意点)
4. [変更履歴](#変更履歴)

## 概要

このドキュメントは、AI エージェントがリポジトリに含まれるテストを実行するための
標準的な手順を示します。`setup_godot_cli.sh` による環境構築後、C# の単体テストと
GUT テストを順に実行し、結果を確認します。

## 手順

1. **環境構築**
    ```bash
    ./setup_godot_cli.sh
    ```
    - `.NET SDK` と Godot CLI がインストールされ、プロジェクトがビルドされます。
2. **C# テスト実行**
    ```bash
    dotnet test Tests/Core/CoreTests.csproj -v minimal
    ```
    - 成功すると各テストの結果が表示されます。
3. **GUT テスト実行**
    ```bash
    godot --headless --path . -s addons/gut/gut_cmdln.gd -gconfig=.gutconfig.json
    ```
    - テストが無い場合でもエラーが出力される場合がありますが、終了コード 0 で完了します。
4. **作業ツリー確認**
    ```bash
    git status --short
    ```
    - 余分なファイルが生成されていないか確認し、必要に応じて削除します。

## 注意点

- テスト実行前に必ず `setup_godot_cli.sh` を実行してください。
- `.NET` のビルド成果物 (`bin/`, `obj/`) はコミットしません。`.gitignore` にパターンを追加してあります。
- テストが存在しない場合、GUT は何も実行せず終了します。

## 変更履歴

| バージョン | 更新日     | 変更内容 |
| ---------- | ---------- | -------- |
| 0.1        | 2025-06-07 | 初版作成 |

