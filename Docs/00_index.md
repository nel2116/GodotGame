---
title: プロジェクト概要
version: 0.1.0
status: draft
updated: 2024-03-21
tags:
    - Overview
    - Documentation
    - Project
linked_docs:
    - "[[DocumentManagementRules]]"
    - "[[10_CoreDocs/00_index]]"
    - "[[20_UserGuides/00_index]]"
    - "[[30_APIReference/00_index]]"
    - "[[40_Tutorials/00_index]]"
---

# NewGameProject

## 目次

1. [概要](#概要)
2. [使用技術](#使用技術)
3. [プロジェクトルール](#プロジェクトルール)
4. [フォルダ構成](#フォルダ構成)
5. [ドキュメント構成](#ドキュメント構成)
6. [ライセンス](#ライセンス)

## 概要

このプロジェクトは、Godot Engine 4.2 を使用したゲーム開発プロジェクトです。

## 使用技術

-   Godot Engine 4.2
-   GDScript
-   Git + GitHub
-   Cursor（AI 補完）

## プロジェクトルール

-   読みやすく保守性の高いコードを目指す
-   コメントは日本語で記述
-   大きな要件は分割し、理由も明記する
-   `.cursor.json` にルールを記述し、AI 支援に反映

## フォルダ構成

```
/Scenes/    - シーンファイル
/Scripts/   - スクリプト
/Assets/    - アセット素材
/Docs/      - 設計資料
```

## ドキュメント構成

### コアドキュメント

-   [[10_CoreDocs/00_index|コアドキュメントインデックス]]
-   [[10_CoreDocs/Architecture|アーキテクチャ設計]]
-   [[10_CoreDocs/DevelopmentGuidelines|開発ガイドライン]]

### ユーザーガイド

-   [[20_UserGuides/00_index|ユーザーガイドインデックス]]
-   [[20_UserGuides/Installation|インストールガイド]]
-   [[20_UserGuides/Configuration|設定ガイド]]

### API リファレンス

-   [[30_APIReference/00_index|APIリファレンスインデックス]]
-   [[30_APIReference/CoreSystemAPI|コアシステムAPI]]
-   [[30_APIReference/GameplayAPI|ゲームプレイAPI]]
-   [[30_APIReference/DataManagementAPI|データ管理API]]

### チュートリアル

-   [[40_Tutorials/00_index|チュートリアルインデックス]]
-   [[40_Tutorials/GettingStarted|はじめに]]
-   [[40_Tutorials/BasicFeatures|基本機能]]
-   [[40_Tutorials/AdvancedFeatures|応用機能]]

## ライセンス

MIT

## 変更履歴

| バージョン | 更新日     | 変更内容 |
| ---------- | ---------- | -------- |
| 0.1.0      | 2025-06-01 | 初版作成 |
