---
title: AIエージェント向け実装ワークフロー
version: 0.6
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
    - "[[CommitMessageRules.md]]"
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

1. **環境セットアップ**
    - [[14_TechDocs/14.3_GodotEnvironment.md|Godot環境設定]]に従って必要なソフトウェアを準備します。
    - `sudo apt-get update` を実行してから `sudo apt-get install -y dotnet-sdk-8.0` で `.NET SDK` 8.0 以上を導入し、その後 `setup_godot_cli.sh` を実行して C# 対応の Godot CLI をインストールします。インストール後に `godot --headless --path . --build-solutions --quit` を実行してソリューションをビルドします。
    - テスト自動化のために[[14_TechDocs/14.11_TestAutomation.md|テスト自動化システム]]の手順も確認します。
2. **コアシステム実装**
    - [[15_ImplementationSpecs/15.1_InputManagementSpec.md|入力管理]]、[[15_ImplementationSpecs/15.1_ReactiveSystemImpl.md|リアクティブシステム]]、[[15_ImplementationSpecs/15.2_StateManagementImpl.md|状態管理]]を最優先で実装します。
    - インターフェース設計と単体テストを整備し、各コンポーネントが疎結合で連携できるようにします。
3. **MVPスプリント計画の実行**
    - [[11_PlanDocs/11_3_mvp.md|MVP定義]]に記載された4週サイクルに従い、以下を順に実装します。
        1. プレイヤー操作と基本敵1種
        2. 残りの敵種とスキルツリー基幹ノード
        3. ボスと条件解放スキル
        4. UIポリッシュとテスト調整
4. **戦闘・セーブロードシステム**
    - [[15_ImplementationSpecs/15.4_CombatSystemSpec.md|戦闘システム]]と[[15_ImplementationSpecs/15.6_SaveLoadSpec.md|セーブ・ロード]]を実装し、ゲームループを完成させます。
    - 自動テストを用いて戦闘フローとデータ保存の整合性を検証します。
5. **拡張とポリッシュ**
    - プレイヤー成長、フィードバック、パフォーマンス最適化など、[[15_ImplementationSpecs/00_index.md|実装仕様書一覧]]の後半項目を順次反映します。
6. **コミットとプルリクエスト**
    - [[PullRequestProcedure.md|プルリクエスト手順]]に従い、作業ブランチの作成からレビュー依頼までを行います。
    - コード変更は `godot --headless --path . -s addons/gut/gut_cmdln.gd -gconfig=.gutconfig.json` でテスト後にコミットします。
7. **ドキュメント更新**
    - 実装変更があれば[[DocumentManagementRules.md|ドキュメント管理ルール]]に従い、メタデータと変更履歴を記録します。

# 使用方法

- AIエージェントは上記の手順を参考に、該当する仕様書を確認しながら実装を進めます。
- ゲームの処理は C# (Godot .NET) で実装し、GDScript は使用しないでください。
- ただしテストスクリプトは GDScript で記述してもかまいません。C# クラスを参照する際は `[GlobalClass]` 属性を付与し、メソッド名の大文字・小文字に注意してください。
- 仕様書で不明点があれば関連ドキュメントへのリンクから詳細を参照してください。
- コード変更後はテストコマンド `godot --headless --path . -s addons/gut/gut_cmdln.gd -gconfig=.gutconfig.json` を実行して結果を確認します。

# 制限事項

- ワークフローはロードマップやMVP計画を基にした目安であり、開発状況に応じて調整が必要です。
- ドキュメントの更新漏れを防ぐため、実装後は必ず仕様書の該当箇所を確認してください。

# 変更履歴

| バージョン | 更新日     | 変更内容 |
| ---------- | ---------- | -------- |
| 0.6        | 2025-06-06 | apt 更新と .NET インストール手順を追加 |
| 0.5        | 2025-06-06 | setup スクリプトの前提条件を追記 |
| 0.4        | 2025-06-06 | ゲーム処理を C# 実装へ変更 |
| 0.3        | 2025-06-06 | Godot CLI セットアップ手順を追記 |
| 0.2        | 2025-06-06 | 実装ワークフロー詳細を追記 |
| 0.1        | 2025-06-06 | 初版作成 |
