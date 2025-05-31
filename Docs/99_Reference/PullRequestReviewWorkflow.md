---
title: プルリクエストレビューAIワークフロー
tags:
    - Reference
    - Workflow
    - PullRequest
    - AI
version: 1.0.0
status: draft
updated: 2025-06-01
---

# プルリクエストレビュー AI ワークフロー

## 目的

AI エージェントがプロジェクトのルール・ガイドラインに従い、効率的かつ正確にプルリクエスト（PR）レビューを実施できるよう、標準化された手順を定義します。

## 前提

-   プロジェクトのコーディング規約・開発ガイドライン・PR 手順書（Docs/10_CoreDocs/DevelopmentGuidelines.md, Docs/99_Reference/PullRequestProcedure.md など）を参照可能であること
-   GitHub CLI（gh）が利用可能であること
-   レビュー対象 PR の番号または URL が分かっていること

## ワークフロー手順

### 1. レビュー準備

1.1. リモートリポジトリの設定を確認し、必要に応じて `git remote -v` で URL を確認する。
1.2. `git fetch origin` でリモートの最新情報を取得する。
1.3. レビュー対象 PR のブランチがローカルにない場合は `git checkout` で取得する。

### 2. PR 情報の取得

2.1. `gh pr view <PR番号>` で PR の概要・変更点・コメントを確認する。
2.2. 変更ファイルや主な修正点を把握する。

### 3. ルール・ガイドラインとの照合

3.1. コーディング規約や開発ガイドライン（例: Docs/10_CoreDocs/DevelopmentGuidelines.md）と照合し、命名規則・構造・テスト方針等の遵守状況を確認する。
3.2. PR 手順書（Docs/99_Reference/PullRequestProcedure.md）やプロジェクトルール（Docs/99_Reference/ProjectRules.md）も参照し、手続き・記載内容の不備がないか確認する。

### 4. レビューコメントの作成

4.1. 指摘事項・改善提案・推奨アクションを日本語で明確にまとめる。
4.2. コメント内容は一時ファイル（例: review_comment.txt）に保存する。

### 5. コメント投稿

5.1. `gh pr review <PR番号> --body-file review_comment.txt --comment` でコメントを投稿する。
5.2. 必要に応じて `--approve` や `--request-changes` オプションを利用する（自分の PR の場合は承認不可）。

### 6. 後処理

6.1. 一時ファイルを削除する。
6.2. レビュー内容を記録・共有する場合は、適切な場所に転記する。

## 注意事項

-   プロジェクトのルールやガイドラインが更新された場合は、本ワークフローも随時見直すこと。
-   AI エージェントは必ず最新のルール・手順書を参照すること。
-   コメントは明確・具体的・建設的に記載する。

## 参考ドキュメント

-   Docs/10_CoreDocs/DevelopmentGuidelines.md
-   Docs/99_Reference/PullRequestProcedure.md
-   Docs/99_Reference/ProjectRules.md
-   Docs/99_Reference/DocumentManagementRules.md

---
