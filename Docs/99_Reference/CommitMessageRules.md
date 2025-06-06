---
title: コミットメッセージルール
version: 0.1
status: draft
updated: 2025-06-06
tags:
    - Git
    - Commit
    - Reference
linked_docs:
    - "[[PullRequestProcedure|プルリクエスト手順]]"
    - "[[ProjectRules|プロジェクトルール]]"
---

# 目次

1. [概要](#概要)
2. [基本規則](#基本規則)
3. [書き方の例](#書き方の例)
4. [変更履歴](#変更履歴)

# 概要

このドキュメントでは、AIエージェントが使用するコミットメッセージの書き方をまとめます。Angular.js のガイドラインを参考にした Prefix 形式を基本とし、変更理由を明確に記述することでレビューの効率を高めます。

# 基本規則

1. **Prefix を付与する**
   - `feat`: 新機能の追加
   - `fix` : バグ修正
   - `docs`: ドキュメントのみの変更
   - `style`: フォーマット修正など機能に影響しない変更
   - `refactor`: 振る舞いを変えないリファクタリング
   - `perf`: パフォーマンス改善
   - `test`: テスト追加・修正
   - `chore`: ビルドや補助ツールの更新
2. **理由を一緒に書く**
   - 変更の目的や背景を一行で説明します。
   - 例: `feat: ゲーム開始時に設定を読み込むため処理を追加`
3. **言語はプロジェクトに合わせる**
   - OSS では英語、内部向けでは日本語を優先します。
4. **1行目に要約を記述する**
   - 必要であれば空行を挟んで詳細を続けます。
   - 関連 Issue がある場合は `Closes #番号` を記述します。

# 書き方の例

```text
feat: 入力管理を改善するためキー割り当て処理を追加

詳細な説明が必要な場合は2行目以降に記載します。
```

# 変更履歴

| バージョン | 更新日     | 変更内容 |
| ---------- | ---------- | -------- |
| 0.1        | 2025-06-06 | 初版作成 |

