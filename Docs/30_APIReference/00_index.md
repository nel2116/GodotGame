---
title: APIリファレンス
version: 0.1.1
status: draft
updated: 2025-06-07
tags:
    - API
    - Reference
    - Documentation
linked_docs:
    - "[[DocumentManagementRules]]"
    - "[[10_CoreDocs/00_index]]"
---

# API リファレンス

## 目次

1. [概要](#概要)
2. [API 一覧](#api一覧)
3. [使用方法](#使用方法)
4. [制限事項](#制限事項)
5. [変更履歴](#変更履歴)

## 概要

このドキュメントは、プロジェクトで使用される API の詳細な仕様を提供します。

## API 一覧

### コアシステム

-   [[CoreSystemAPI|コアシステムAPI]]
    -   ゲームエンジン関連
    -   システム管理
    -   ユーティリティ機能
-   [[MainScriptAPI|Main.cs API]]
    -   ゲーム開始処理
-   [[EventBusAPI|EventBus.cs API]]
    -   イベント通知
-   [[InputBufferAPI|InputBuffer.cs API]]
    -   入力バッファ
-   [[InputObserverAPI|InputObserver.cs API]]
    -   入力監視
-   [[PlayerStateMachineAPI|PlayerStateMachine.cs API]]
    -   プレイヤー状態管理
-   [[StateManagerAPI|StateManager.cs API]]
    -   ゲーム状態管理

### ゲームプレイ

-   [[GameplayAPI|ゲームプレイAPI]]
    -   プレイヤー管理
    -   スキルシステム
    -   戦闘システム

### データ管理

-   [[DataManagementAPI|データ管理API]]
    -   セーブ/ロード
    -   設定管理
    -   リソース管理

## 使用方法

各 API の詳細な仕様は、対応するドキュメントを参照してください。
API の使用にあたっては、以下の点に注意してください：

1. バージョン互換性の確認
2. エラーハンドリングの実装
3. パフォーマンスへの影響の考慮

## 制限事項

-   API の仕様は予告なく変更される可能性があります
-   非推奨の API は将来のバージョンで削除される可能性があります
-   パフォーマンスに影響を与える可能性のある API の使用は慎重に行ってください

## 変更履歴

| バージョン | 更新日     | 変更内容 |
| ---------- | ---------- | -------- |
| 0.1.1      | 2025-06-07 | スクリプトAPIへのリンクを追加 |
| 0.1.0      | 2025-06-01 | 初版作成 |
