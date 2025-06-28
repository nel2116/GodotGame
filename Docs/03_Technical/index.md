---
title: 技術実装・アーキテクチャ
version: 0.1
status: draft
updated: 2024-03-21
tags:
    - Technical
    - Architecture
    - Documentation
    - Index
linked_docs:
    - "[[00_index]]"
    - "[[12_01_mvvm_rx_architecture|MVVM/Rxアーキテクチャ]]"
    - "[[12_02_basic_design|基本設計]]"
    - "[[12_03_detailed_design|詳細設計]]"
    - "[[12_04_system_integration|システム統合]]"
    - "[[12_05_common_utilities|共通ユーティリティ]]"
---

# 技術実装・アーキテクチャ

## 目次

1. [概要](#概要)
2. [関連ドキュメント](#関連ドキュメント)
3. [最適化](#最適化)
4. [注意事項](#注意事項)
5. [変更履歴](#変更履歴)

## 概要

このセクションでは、ゲームの技術的な実装とアーキテクチャに関するドキュメントを管理します。

## 関連ドキュメント

### アーキテクチャ設計

-   [[12_01_mvvm_rx_architecture|MVVM/Rx アーキテクチャ]]
-   [[12_02_basic_design|基本設計]]
-   [[12_03_detailed_design|詳細設計]]
-   [[12_04_system_integration|システム統合]]
-   [[12_05_common_utilities|共通ユーティリティ]]

### コアコンポーネント

-   [[12_03_detailed_design/01_core_components/01_reactive_property|ReactiveProperty]]
-   [[12_03_detailed_design/01_core_components/02_viewmodel_base|ViewModelBase]]
-   [[12_03_detailed_design/01_core_components/03_composite_disposable|CompositeDisposable]]
-   [[12_03_detailed_design/01_core_components/04_event_bus|EventBus]]

### システム実装

-   [[12_03_detailed_design/02_systems/02_skill_system|スキルシステム]]
-   [[12_03_detailed_design/02_systems/03_level_generation|レベル生成]]
-   [[12_03_detailed_design/02_systems/04_enemy_ai|敵 AI]]
-   [[12_03_detailed_design/02_systems/05_input_system|入力システム]]

### 共通システム

-   [[12_03_detailed_design/02_systems/00_common_systems/01_movement_system|移動システム]]
-   [[12_03_detailed_design/02_systems/00_common_systems/02_animation_system|アニメーションシステム]]
-   [[12_03_detailed_design/02_systems/00_common_systems/03_state_system|状態システム]]
-   [[12_03_detailed_design/02_systems/00_common_systems/04_combat_system|戦闘システム]]

## 最適化

-   [[12_03_detailed_design/03_optimization/01_performance_optimization|パフォーマンス最適化]]

## 注意事項

-   アーキテクチャの変更は必ず関連するドキュメントを更新してください
-   パフォーマンス最適化の際は、このセクションのドキュメントを参照してください
-   新しいシステムの実装は、既存のアーキテクチャに準拠してください

## 変更履歴

| バージョン | 更新日     | 変更内容 |
| ---------- | ---------- | -------- |
| 0.1.0      | 2024-03-21 | 初版作成 |
