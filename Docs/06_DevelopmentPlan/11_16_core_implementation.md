---
title: コアシステム実装計画
version: 0.1.0
status: draft
updated: 2025-06-06
tags:
    - Plan
    - Implementation
    - Core
linked_docs:
    - "[[11_5_development_roadmap|開発ロードマップ]]"
    - "[[15_ImplementationSpecs/15.1_InputManagementSpec.md|入力管理実装仕様]]"
    - "[[15_ImplementationSpecs/15.1_ReactiveSystemImpl.md|リアクティブシステム実装仕様]]"
    - "[[15_ImplementationSpecs/15.2_StateManagementImpl.md|状態管理実装仕様]]"
    - "[[99_Reference/AI_Agent_ImplementationWorkflow.md|AIエージェント実装ワークフロー]]"
    - "[[12_Architecture/12_03_detailed_design/01_core_components/02_viewmodel_base|ViewModelBase実装詳細]]"
    - "[[12_Architecture/12_03_detailed_design/01_core_components/01_reactive_property|ReactiveProperty実装詳細]]"
    - "[[12_Architecture/12_03_detailed_design/01_core_components/03_composite_disposable|CompositeDisposable実装詳細]]"
    - "[[12_Architecture/12_03_detailed_design/01_core_components/04_event_bus|イベントバス実装詳細]]"
---

# コアシステム実装計画

## 目次

1. [概要](#概要)
2. [詳細](#詳細)
3. [使用方法](#使用方法)
4. [制限事項](#制限事項)
5. [変更履歴](#変更履歴)

## 概要

AI エージェントが最初に取り組むべきコアシステム実装の方針をまとめる。入力管理、リアクティブシステム、状態管理の 3 要素を連携させ、拡張可能な基盤を構築することを目的とする。

## 詳細

### 1. 入力管理

-   `InputBuffer` と `PlayerStateMachine` で非同期ストリーム入力を制御する。
-   キーボードとゲームパッドを統合した入力マッピングを設定する。
-   入力履歴の保持と先行入力の予約を可能とする。

### 2. リアクティブシステム

-   イベントバスとリアクティブストリームを用いてゲーム内イベントを処理する。
-   状態管理と連携し、各システムの通知をリアクティブに伝播させる。
-   テスト容易性を考慮し、イベント履歴の取得とモック化を行う。

### 3. 状態管理

-   `StateManager` によりゲーム・プレイヤー・敵の状態を一元管理する。
-   状態遷移ルールを定義し、無効遷移やタイムアウトの検出を行う。
-   状態履歴の記録と永続化処理を実装し、デバッグ時の追跡を容易にする。

### 4. インターフェースとテスト

-   各モジュールのインターフェースを明確化し、疎結合を維持する。
-   単体テストを整備し、`godot --headless` で自動実行できる環境を構築する。

## 使用方法

-   上記 3 要素の実装を段階的に進め、完了後に他システムの実装へ移行する。
-   実装時は関連する仕様書を参照し、インターフェース変更があればドキュメントを更新する。

## 制限事項

-   本計画は初期実装向けの指針であり、開発状況に応じて内容を調整する可能性がある。
-   仕様書やワークフローの更新を反映しない場合、計画と実装が乖離する恐れがある。

## 変更履歴

| バージョン | 更新日     | 変更内容 |
| ---------- | ---------- | -------- |
| 0.1.0      | 2025-06-06 | 初版作成 |
