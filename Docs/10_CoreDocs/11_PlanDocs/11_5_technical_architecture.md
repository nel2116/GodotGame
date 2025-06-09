---
title: 技術アーキテクチャ設計書
version: 0.2.0
status: draft
updated: 2024-03-23
tags:
    - Architecture
    - Technical
    - Core
    - Project
linked_docs:
    - "[[11_PlanDocs/11_01_project_plan|プロジェクト計画書]]"
    - "[[12_Architecture/12_01_mvvm_rx_architecture|MVVM+RXアーキテクチャ]]"
    - "[[11_PlanDocs/00_index|計画ドキュメント]]"
    - "[[12_Architecture/12_03_detailed_design/01_core_components/01_reactive_property|ReactiveProperty実装詳細]]"
    - "[[12_Architecture/12_03_detailed_design/01_core_components/02_viewmodel_base|ViewModelBase実装詳細]]"
    - "[[12_Architecture/12_03_detailed_design/01_core_components/03_composite_disposable|CompositeDisposable実装詳細]]"
    - "[[12_Architecture/12_03_detailed_design/01_core_components/04_event_bus|イベントバス実装詳細]]"
    - "[[12_Architecture/12_03_detailed_design/02_systems/07_animation_system|アニメーションシステム詳細設計]]"
    - "[[12_Architecture/12_03_detailed_design/02_systems/08_sound_system|サウンドシステム詳細設計]]"
    - "[[12_Architecture/12_03_detailed_design/02_systems/09_ui_system|UIシステム詳細設計]]"
    - "[[12_Architecture/12_03_detailed_design/02_systems/10_network_system|ネットワークシステム詳細設計]]"
---

# 技術アーキテクチャ設計書

## 1. 概要

### 1.1 目的

本ドキュメントは、Shrine of the Lost Ones の技術アーキテクチャを定義し、以下の目的を達成することを目指します：

-   MVVM + リアクティブプログラミングの適用範囲の明確化
-   ゲームシステムの実装方針の確立
-   開発効率と保守性の向上

### 1.2 適用範囲

-   ゲームコアシステム
-   UI/UX システム
-   データ管理システム
-   イベントシステム
-   アニメーションシステム
-   サウンドシステム
-   ネットワークシステム

## 2. アーキテクチャ概要

### 2.1 基本構造

#### 2.1.1 レイヤー構成

```
[View Layer]
    ↓
[ViewModel Layer]
    ↓
[Model Layer]
    ↓
[Service Layer]
```

#### 2.1.2 各レイヤーの責務

| レイヤー  | 責務              | 実装例               |
| --------- | ----------------- | -------------------- |
| View      | UI 表示・入力受付 | Godot Node           |
| ViewModel | 状態管理・変換    | ReactiveProperty     |
| Model     | ビジネスロジック  | ゲームロジッククラス |
| Service   | 共通機能提供      | システムクラス       |

### 2.2 システム構成

#### 2.2.1 コアシステム

-   プレイヤーシステム
-   スキルシステム
-   レベル生成システム
-   敵 AI システム
-   入力システム
-   セーブ/ロードシステム

#### 2.2.2 新規追加システム

-   アニメーションシステム

    -   キャラクターアニメーション
    -   エフェクトアニメーション
    -   UI アニメーション

-   サウンドシステム

    -   BGM 管理
    -   効果音管理
    -   音声管理

-   UI システム

    -   メニュー管理
    -   HUD 管理
    -   ダイアログ管理

-   ネットワークシステム
    -   マルチプレイヤー対応
    -   データ同期
    -   通信管理

## 3. 変更履歴

| バージョン | 更新日     | 変更内容                       |
| ---------- | ---------- | ------------------------------ |
| 0.1.0      | 2024-03-21 | 初版作成                       |
| 0.2.0      | 2024-03-23 | 新規システムの追加と構成の更新 |
