---
title: Reactive System テスト結果
version: 0.2.0
status: draft
updated: 2024-03-21
tags:
    - API
    - Reactive
    - Events
    - Core
    - Tests
    - TestResults
linked_docs:
    - "[[ReactiveSystem]]"
    - "[[01_reactive_property]]"
    - "[[02_composite_disposable]]"
    - "[[03_event_bus]]"
---

# Reactive System テスト結果

## 目次

1. [概要](#概要)
2. [テスト環境](#テスト環境)
3. [テスト結果](#テスト結果)
4. [パフォーマンス測定](#パフォーマンス測定)
5. [変更履歴](#変更履歴)

## 概要

このドキュメントは、Reactive System のテスト実行結果を記録します。

## テスト環境

-   実行環境: Windows 10
-   .NET バージョン: .NET 8.0
-   テストフレームワーク: NUnit 3.13.3
-   テスト実行時間: 2.5 秒

## テスト結果概要

-   総テスト数: 18
-   成功: 18
-   失敗: 0
-   スキップ: 0

## 詳細なテスト結果

### CompositeDisposableTests

| テスト名                            | 結果 | 実行時間 |
| ----------------------------------- | ---- | -------- |
| AddAndDispose_DisposesAllResources  | 成功 | 2ms      |
| AddRange_AddsAllItems               | 成功 | <1ms     |
| CircularReference_DisposeSafely     | 成功 | 4ms      |
| Clear_DisposesAllAndEmpties         | 成功 | <1ms     |
| Dispose_LargeNumberOfResources      | 成功 | <1ms     |
| Remove_ReturnsTrueAndDoesNotDispose | 成功 | <1ms     |
| ThreadSafety_AddFromMultipleThreads | 成功 | 5ms      |

### GameEventBusTests

| テスト名                                   | 結果 | 実行時間 |
| ------------------------------------------ | ---- | -------- |
| Publish_NotifiesSubscribers                | 成功 | 12ms     |
| Publish_Performance                        | 成功 | 2ms      |
| Publish_UnsubscribedType_DoesNotNotify     | 成功 | <1ms     |
| Subscribe_MultipleTypes_NotifyOnlyMatching | 成功 | <1ms     |

### ReactivePropertyTests

| テスト名                            | 結果 | 実行時間 |
| ----------------------------------- | ---- | -------- |
| Constructor_SetsInitialValue        | 成功 | <1ms     |
| Dispose_StopNotifications           | 成功 | 4ms      |
| ManySubscribers_AllReceiveUpdates   | 成功 | <1ms     |
| MultipleChanges_NotifyInOrder       | 成功 | 1ms      |
| SetSameValue_DoesNotNotify          | 成功 | <1ms     |
| ThreadSafety_SetFromMultipleThreads | 成功 | <1ms     |
| ValueChange_NotifiesSubscribers     | 成功 | <1ms     |

## パフォーマンス測定結果

-   イベント発行の平均時間: 12ms
-   複数スレッドからの同時操作: 5ms
-   大量のリソース処理: <1ms

## 変更履歴

| バージョン | 日付       | 変更内容                                   |
| ---------- | ---------- | ------------------------------------------ |
| 0.2.0      | 2024-03-21 | テスト結果の更新、パフォーマンス測定の追加 |
| 0.1.0      | 2024-03-20 | 初版作成                                   |
