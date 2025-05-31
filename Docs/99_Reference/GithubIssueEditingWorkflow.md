---
title: GitHub Issue編集ワークフロー
version: 0.1.0
status: draft
updated: 2025-06-01
tags:
    - GitHub
    - Issue
    - Workflow
    - Reference
linked_docs:
    - "[[ProjectRules#プロジェクトルール|プロジェクトルール]]"
    - "[[PullRequestProcedure#プルリクエスト作成手順|プルリクエスト作成手順]]"
---

# GitHub Issue 編集ワークフロー (GitHub CLI 使用)

## 目次

1. [目的](#目的)
2. [使用ツール](#使用ツール)
3. [基本手順](#基本手順)
4. [本文編集の詳細手順](#本文編集の詳細手順-推奨)
5. [全体的な注意点](#全体的な注意点)

## 目的

本ドキュメントは、エージェントが GitHub CLI (`gh`) を使用して既存の GitHub Issue の情報を編集する際の標準的な手順と注意点を示すことを目的とします。

## 使用ツール

-   GitHub CLI (`gh`)
-   エージェントのファイル編集ツール (`edit_file`)
-   エージェントのターミナル実行ツール (`run_terminal_cmd`)

## 基本手順

Issue のタイトルなど、比較的短いテキストフィールドを変更する場合、`gh issue edit` コマンドを直接使用できます。

```bash
gh issue edit <issue_number> --title "新しいタイトル"
gh issue edit <issue_number> --body "新しい本文の短い内容"
```

**注意点:** `--body` に長い本文やマークダウン記法を直接渡すと、シェルのエスケープ処理や CLI の挙動により、意図しない表示になることがあります（例: 改行が `\n` のまま表示される）。

## 本文編集の詳細手順 (推奨)

長い本文やマークダウン記法を含む本文を正確に更新するには、一度本文の内容をファイルに書き出し、`--body-file` オプションを使用することを推奨します。

1.  **編集したい本文の準備:**
    更新したい Issue の本文（マークダウン形式）を用意します。

2.  **一時ファイルへの本文書き出し:**
    エージェントの `edit_file` ツールを使用して、準備した本文の内容を一時ファイルに書き出します。一時ファイルは `.github/` ディレクトリなど、プロジェクトのルートからの相対パスで指定し、分かりやすい名前にします（例: `.github/temp_issue_body_<issue_number>.md`）。

    ```tool_code
    print(default_api.edit_file(target_file = ".github/temp_issue_body_<issue_number>.md", code_edit = "## 編集したいIssueの本文をここに記述します\n- リストアイテム\n", instructions = "Issue本文のための一時ファイルを書き出します。"))
    ```

    \*実際のツール呼び出し時には `<issue_number>` や `code_edit` の内容は適切に置き換えてください。

3.  **GitHub CLI での本文更新:**
    エージェントの `run_terminal_cmd` ツールを使用して、一時ファイルを指定して `gh issue edit` コマンドを実行します。

            ```bash

        gh issue edit <issue*number> --body-file .github/temp_issue_body*<issue*number>.md
        `

    ` tool_code
print(default_api.run_terminal_cmd(command = "gh issue edit <issue_number> --body-file .github/temp_issue_body*<issue_number>.md", explanation = "一時ファイルの内容で Issue 本文を更新します。", is_background = False))
```    *実際のツール呼び出し時には `<issue_number>` やファイルパスは適切に置き換えてください。

4.  **(任意) 変更内容の確認:**
    必要に応じて、再度 `gh issue view` コマンドを実行して、本文が正しく更新されたか確認します。

            ```bash

        gh issue view <issue_number> --json body
        `

    `tool_code
    print(default_api.run_terminal_cmd(command = "gh issue view <issue_number> --json body", explanation = "Issue 本文が正しく更新されたか確認します。", is_background = False))

        ```

        ```

5.  **一時ファイルの削除:**
    Issue の更新が完了したら、使用した一時ファイルを削除します。エージェントの `run_terminal_cmd` ツールを使用します。

            ```bash

        rm .github/temp*issue_body*<issue*number>.md # または Windows の場合:
        del .github/temp_issue_body*<issue*number>.md
        `

    `tool_code
    print(default_api.run_terminal_cmd(command = "rm .github/temp_issue_body\*<issue_number>.md", explanation = "使用済みの一時ファイルを削除します。", is_background = False))

        ```*実際のツール呼び出し時にはファイルパスは適切に置き換えてください。OS に応じたコマンドを使用してください。

        ```

## 全体的な注意点

-   GitHub CLI がローカル環境にインストールされ、適切に認証されている必要があります。
-   `run_terminal_cmd` ツールを使用する際は、カレントワーキングディレクトリが GitHub リポジトリのルートになっていることを確認してください。
-   コマンド実行の成否はツールからの出力で判断し、エラーが発生した場合は原因を調査してください。
