---
title: APIリファレンス
version: 0.1.8
status: draft
updated: 2024-03-21
tags:
    - API
    - Reference
    - Documentation
    - Core
    - Reactive
    - Event
    - Resource
    - Property
    - ViewModel
linked_docs:
    - "[[DocumentManagementRules]]"
    - "[[10_CoreDocs/00_index]]"
    - "[[ReactiveSystem]]"
    - "[[ViewModelSystem]]"
    - "[[ReactiveProperty]]"
    - "[[CoreEventSystem]]"
    - "[[CommonEventSystem]]"
    - "[[CompositeDisposable]]"
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
-   [[ReactiveSystem|リアクティブシステム]]
    -   リアクティブプロパティ
    -   イベントシステム
    -   リソース管理
-   [[ReactiveProperty|リアクティブプロパティ]]
    -   値の変更通知
    -   バリデーション
    -   バッチ更新
-   [[CoreEventSystem|Core Event System]]
    -   イベント発行・購読
    -   型安全なイベント処理
    -   スレッドセーフな実装
-   [[CommonEventSystem|Common Event System]]
    -   イベント発行・購読
    -   型安全なイベント処理
    -   スレッドセーフな実装
-   [[ViewModelSystem|ViewModel システム]]
    -   MVVM パターン
    -   データバインディング
    -   コマンドパターン
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

| バージョン | 更新日     | 変更内容                                                                 |
| ---------- | ---------- | ------------------------------------------------------------------------ |
| 0.1.8      | 2024-03-21 | ViewModel システムのドキュメントを更新                                   |
| 0.1.7      | 2024-03-21 | リアクティブプロパティのドキュメントを更新                               |
| 0.1.6      | 2024-03-21 | 複合リソース管理システムのドキュメントを追加                             |
| 0.1.5      | 2024-03-21 | イベントシステムのドキュメントを追加                                     |
| 0.1.4      | 2024-03-21 | リアクティブプロパティのドキュメントを追加                               |
| 0.1.3      | 2024-03-21 | ViewModel システムのドキュメントを追加                                   |
| 0.1.2      | 2024-03-21 | リアクティブシステムのドキュメントを更新し、ViewModel 機能への参照を追加 |
| 0.1.1      | 2024-03-21 | 目次構造の改善                                                           |
| 0.1.0      | 2024-03-21 | 初版作成                                                                 |
