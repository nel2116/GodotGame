---
title: AIエージェント向け実装ワークフロー
version: 0.1
status: draft
updated: 2025-06-06
tags:
    - AI
    - Implementation
    - Workflow
    - Reference
linked_docs:
    - "[[11_PlanDocs/11_5_development_roadmap.md]]"
    - "[[11_PlanDocs/11_3_mvp.md]]"
    - "[[15_ImplementationSpecs/00_index.md]]"
    - "[[14_TechDocs/00_index.md]]"
    - "[[DocumentManagementRules.md]]"
---

# 目次

1. [概要](#概要)
2. [詳細](#詳細)
3. [使用方法](#使用方法)
4. [制限事項](#制限事項)
5. [変更履歴](#変更履歴)

# 概要

本ドキュメントは、実装仕様書と技術ドキュメントに基づき、AIエージェントがコード実装を進める際の標準的なワークフローを示します。コアシステムからゲームプレイ、セーブ・ロードまでの優先順位を整理し、各フェーズで参照すべき資料を明確にします。

# 詳細

1. **コアシステム実装**
    - [[15_ImplementationSpecs/15.1_InputManagementSpec.md|入力管理]]、[[15_ImplementationSpecs/15.1_ReactiveSystemImpl.md|リアクティブシステム]]、[[15_ImplementationSpecs/15.2_StateManagementImpl.md|状態管理]]を最優先で実装します。
    - 入力監視、イベント処理、状態遷移の基盤を固め、各コンポーネントが疎結合で連携できるようにします。
2. **MVPスプリント計画の実行**
    - [[11_PlanDocs/11_3_mvp.md|MVP定義]]に記載された4週サイクルに従い、以下を順に実装します。
        1. プレイヤー操作と基本敵1種
        2. 残りの敵種とスキルツリー基幹ノード
        3. ボスと条件解放スキル
        4. UIポリッシュとテスト調整
3. **戦闘・セーブロードシステム**
    - [[15_ImplementationSpecs/15.4_CombatSystemSpec.md|戦闘システム]]と[[15_ImplementationSpecs/15.6_SaveLoadSpec.md|セーブ・ロード]]を実装し、ゲームループを完成させます。
4. **拡張とポリッシュ**
    - プレイヤー成長、フィードバック、パフォーマンス最適化など、[[15_ImplementationSpecs/00_index.md|実装仕様書一覧]]の後半項目を順次反映します。
5. **ドキュメント更新**
    - 実装変更があれば[[DocumentManagementRules.md|ドキュメント管理ルール]]に従い、メタデータと変更履歴を記録します。

# 使用方法

- AIエージェントは上記の手順を参考に、該当する仕様書を確認しながら実装を進めます。
- 仕様書で不明点があれば関連ドキュメントへのリンクから詳細を参照してください。
- コード変更後はテストコマンド `godot --headless --path . -s addons/gut/gut_cmdln.gd -gconfig=.gutconfig.json` を実行して結果を確認します。

# 制限事項

- ワークフローはロードマップやMVP計画を基にした目安であり、開発状況に応じて調整が必要です。
- ドキュメントの更新漏れを防ぐため、実装後は必ず仕様書の該当箇所を確認してください。

# 変更履歴

| バージョン | 更新日     | 変更内容 |
| ---------- | ---------- | -------- |
| 0.1        | 2025-06-06 | 初版作成 |
