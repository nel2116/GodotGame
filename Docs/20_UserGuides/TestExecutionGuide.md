---
title: テスト実行ガイド
version: 0.2.4
status: draft
updated: 2025-06-07
tags:
    - UserGuide
    - Test
linked_docs:
    - "[[10_CoreDocs/11_PlanDocs/11_16_core_system_impl_plan.md]]"
---

# テスト実行ガイド

## 目次
1. [概要](#概要)
2. [環境構築](#環境構築)
3. [テスト実行](#テスト実行)
4. [結果の確認](#結果の確認)
5. [テスト時の注意点](#テスト時の注意点)
6. [変更履歴](#変更履歴)

## 概要

このドキュメントでは、GUT を用いたテスト環境の構築手順と実行方法を説明します。

## 環境構築

1. `sudo apt-get update` を実行してパッケージを最新化します。
2. `setup_godot_cli.sh` を実行すると `.NET SDK` と Godot CLI が自動でインストールされ、ソリューションのビルドも行われます。
3. スクリプト実行後はアセットがインポートされた状態となります。

## テスト実行

下記コマンドでテストを実行できます。

```bash
godot --headless --path . -s addons/gut/gut_cmdln.gd -gconfig=.gutconfig.json
```

## 結果の確認

テスト結果はコンソール出力のほか、`res://Scripts/Tests/test-results_*.xml` に JUnit 形式で保存されます。

## テスト時の注意点

1. C# スクリプトを追加した後は `godot --headless --path . --build-solutions --quit` を実行して DLL を生成してください。
2. テストスクリプトは GDScript で記述できます。C# クラスを参照する場合は `[GlobalClass]` 属性を利用し、メソッド名の大文字・小文字に気を付けます。
3. `.NET SDK` が無い環境では Godot の Mono 版が起動できません。`setup_godot_cli.sh` 実行時に自動インストールされますが、失敗した場合は `apt-get install dotnet-sdk-8.0` を手動で実行してください。
4. テスト実行時に `Nonexistent function` などのエラーが表示された場合は、ソリューションが未ビルドの可能性があります。`godot --headless --path . --build-solutions --quit` を再度実行してからテストを行ってください。

## 変更履歴

| バージョン | 更新日     | 変更内容 |
| ---------- | ---------- | -------- |
| 0.2.4      | 2025-06-07 | setup_godot_cli.sh の自動インストール対応 |
| 0.2.3      | 2025-06-07 | テスト実行時エラー対処法を追加 |
| 0.2.2      | 2025-06-06 | apt 更新と .NET インストール手順を追加 |
| 0.2.1      | 2025-06-06 | setup スクリプトの前提条件を追記 |
| 0.2.0      | 2025-06-06 | テスト時の注意点を追記 |
| 0.1.0      | 2025-06-06 | 初版作成 |

