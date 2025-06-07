---
title: 技術ドキュメント
version: 0.4.2
status: draft
updated: 2025-06-07
tags:
- Technical
- Documentation
- Core
linked_docs:
- "[[14.1_Requirement.md]]"
- "[[14.2_PrototypeTechnicalDesign.md]]"
- "[[14.3_GodotEnvironment.md]]"
- "[[14.4_InputStateMachine.md]]"
- "[[14.5_DungeonGeneration.md]]"
- "[[14.6_EnemyAIFoundation.md]]"
- "[[14.7_CombatSystem.md]]"
- "[[14.8_SkillCooldown.md]]"
- "[[14.9_SaveDataManagement.md]]"
- "[[14.10_UILayout.md]]"
- "[[14.11_TestAutomation.md]]"
- "[[14.12_PerformanceProfiling.md]]"
- "[[14.14_GameManager.md]]"
- "[[14.15_UIManager.md]]"
- "[[14.16_SoundManager.md]]"
- "[[14.17_DebugManager.md]]"
- "[[14.18_SystemArchitecture.md]]"
- "[[14.19_MVVMReactiveDesign.md]]"
- "[[15.1_ReactiveSystemImpl.md]]"
- "[[15.2_StateManagementImpl.md]]"
- "[[15.3_EnemyAISpec.md]]"
- "[[15.4_CombatSystemSpec.md]]"
- "[[15.5_SkillSystemSpec.md]]"
- "[[15.6_SaveLoadSpec.md]]"
- "[[15.7_UIUXSpec.md]]"
- "[[15.8_TestPerformanceSpec.md]]"
- "[[15.12_PerformanceOptimizationSpec.md]]"
---

# 技術ドキュメント

## 目次

1. [概要](#概要)
2. [カテゴリ別ドキュメント](#カテゴリ別ドキュメント)
3. [使用方法](#使用方法)
4. [制限事項](#制限事項)
5. [変更履歴](#変更履歴)

## 概要

このディレクトリには、プロジェクトの技術的な詳細と仕様に関するドキュメントが含まれています。
各ドキュメントは、開発チームが参照すべき技術的な基準と実装ガイドラインを提供します。

## カテゴリ別ドキュメント

### 1. 基盤システム
- [14.1 要件定義](14.1_Requirement.md) - プロジェクトの基本要件と技術要件
- [14.2 プロトタイプ技術設計](14.2_PrototypeTechnicalDesign.md) - プロトタイプ開発の技術設計
- [14.3 Godot環境設定](14.3_GodotEnvironment.md) - 開発環境の設定と構成
- [14.13 技術設計仕様](14.13_TechnicalDesignSpec.md) - 全体の技術設計仕様

### 2. コアシステム
- [14.4 入力状態管理](14.4_InputStateMachine.md) - 入力システムの状態管理
- [14.5 ダンジョン生成](14.5_DungeonGeneration.md) - ダンジョン生成システム
- [14.6 敵AI基盤](14.6_EnemyAIFoundation.md) - 敵AIの基本設計
- [14.7 戦闘システム](14.7_CombatSystem.md) - 戦闘システムの基本設計

### 3. マネージャーシステム
- [14.14 ゲーム管理](14.14_GameManager.md) - ゲーム全体の管理システム
- [14.15 UI管理](14.15_UIManager.md) - UIシステムの管理
- [14.16 サウンド管理](14.16_SoundManager.md) - サウンドシステムの管理
- [14.17 デバッグ管理](14.17_DebugManager.md) - デバッグシステムの管理

### 4. ユーティリティ
- [14.8 スキルクールダウン](14.8_SkillCooldown.md) - スキルクールダウンシステム
- [14.9 セーブデータ管理](14.9_SaveDataManagement.md) - セーブデータの管理システム
- [14.10 UIレイアウト](14.10_UILayout.md) - UIレイアウトシステム
- [14.11 テスト自動化](14.11_TestAutomation.md) - テスト自動化システム
- [14.12 パフォーマンス分析](14.12_PerformanceProfiling.md) - パフォーマンス分析ツール

### 5. アーキテクチャ
- [14.18 システムアーキテクチャ](14.18_SystemArchitecture.md) - 全体システムアーキテクチャ
- [14.19 MVVM + リアクティブ設計ガイド](14.19_MVVMReactiveDesign.md) - MVVMとリアクティブプログラミングの方針

## 使用方法

1. 各ドキュメントは独立したMarkdownファイルとして提供されています
2. 技術的な実装や最適化時には、必ず最新のドキュメントを参照してください
3. 関連する実装仕様書（15_ImplementationSpecs）と併せて参照することを推奨します

## 制限事項

- 技術仕様は随時更新される可能性があります
- 最新の情報は必ず最新バージョンを参照してください
- 技術的な変更は必ずドキュメントに反映されます
- 実装時は必ず関連する実装仕様書も確認してください

## 変更履歴

| バージョン | 更新日     | 変更内容 |
| ---------- | ---------- | -------- |
| 0.4.1 | 2025-06-07 | リンク整合性修正         |
| 0.4.0      | 2025-06-06 | ドキュメント構造の再編成とカテゴリ分類の追加 |
| 0.3.0      | 2025-06-06 | 各技術要素の設計書を追加 |
| 0.2.0      | 2025-06-06 | プロトタイプ技術設計書を追加 |
| 0.1.0      | 2025-06-01 | 初版作成 |
