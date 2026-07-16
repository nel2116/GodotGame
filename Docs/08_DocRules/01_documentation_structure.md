---
title: ドキュメント構造ルール
version: 1.0.0
status: approved
updated: 2026-07-16
tags:
    - DocRules
    - Structure
    - Guidelines
linked_docs:
    - "[[00_common_guidelines|共通ガイドライン]]"
    - "[[DocumentManagementRules|ドキュメント管理ルール詳細]]"
    - "[[00_Index/moc|ドキュメント全体マップ]]"
---

# ドキュメント構造ルール

## 概要

本ドキュメントは、2026-07-16 のドキュメント再構成で採用した構造ルールを定義します。
Diátaxis フレームワークと Obsidian vault 運用のベストプラクティスに基づきます。

## 1. ドキュメントの4分類（Diátaxis）

各ドキュメントは以下のいずれか **1つ** の役割に属するように書きます。
1ページに複数の役割を混在させないでください。

| 分類 | 目的 | 本プロジェクトでの置き場所 |
|------|------|--------------------------|
| チュートリアル（学習） | 初学者を手を動かして導く | 99_Reference（GettingStarted, BasicFeatures 等） |
| ハウツーガイド（目的達成） | 特定の作業手順を示す | 99_Reference（ワークフロー・手順書）、07_Testing（テスト実行） |
| リファレンス（情報） | 仕様・データを正確に記述 | 02_GameSystem（データ仕様）、03_Architecture（設計仕様） |
| 解説（理解） | 背景・設計判断を説明 | 01_WorldConcept、06_DevelopmentPlan（計画・方針） |

## 2. フォルダ構造の原則

- トップレベルは番号付きカテゴリ（00〜08, 99）+ `__templates` のみ。番号なしフォルダを直下に作らない
- ネストは Docs/ から **最大3〜4階層** まで（例: `03_Architecture/Systems/Common/xxx.md`）
- フォルダは「どこに属するか」、タグは「何についてか」を表す
- 深いフォルダ階層よりも MOC とリンクでナビゲーションを構成する

## 3. ナビゲーション（MOC）の原則

- 各カテゴリのハブは `moc.md` に統一する（`index.md` は使用しない）
- MOC は **実在するドキュメントのみ** をリンクする
- 未作成のドキュメントは MOC 内の「🚧 作成予定ドキュメント」セクションに列挙する（リンクにしない）
- 全体ナビゲーションは [[00_Index/moc|トップMOC]] → 各カテゴリ MOC の2段構成

## 4. ファイル命名の原則

- スネークケース、拡張子 `.md`
- ファイル名（basename）は vault 全体で一意にする（Obsidian のリンク解決を安定させるため）
- リンクは原則 basename 形式（`[[file_name|表示名]]`）。同名の恐れがある場合のみパス付きで書く

## 5. 禁止事項

- **空スタブの作成禁止**: frontmatter だけの中身のないファイルを作らない（未作成リンクのクリック時は注意）
- **重複ドキュメントの禁止**: 同一内容のファイルを複数箇所に置かない。1ファイル1事実（Single Source of Truth）
- **生成物のコミット注意**: `SkilData.*.translation` 等の Godot 生成物はドキュメントとして扱わない

## 6. frontmatter

[[__templates/frontmatter|frontmatter テンプレート]] に従い、`title` / `version` / `status` / `updated` / `tags` / `linked_docs` を必ず記入する。
`updated` は実際の更新日（システム日付）を使用する。

## 変更履歴

| バージョン | 更新日 | 変更内容 |
| ---------- | ---------- | -------- |
| 1.0.0 | 2026-07-16 | 初版作成（Diátaxis / Obsidian ベストプラクティスに基づく再構成ルール） |
