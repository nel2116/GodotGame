---
title: Core System テスト結果
version: 0.1.0
status: draft
updated: 2024-03-21
tags:
    - API
    - Core
    - Tests
    - TestResults
linked_docs:
    - "[[ReactiveSystemTestResults]]"
    - "[[DocumentManagementRules]]"
---

# Core System テスト結果

## 目次

1. [概要](#概要)
2. [テスト環境](#テスト環境)
3. [テスト結果](#テスト結果)
4. [パフォーマンス測定](#パフォーマンス測定)
5. [変更履歴](#変更履歴)

## 概要

このドキュメントは、Core System のテスト実行結果を記録します。

## テスト環境

-   実行環境: Windows 10
-   .NET バージョン: .NET 8.0
-   テストフレームワーク: NUnit 3.13.3
-   テスト実行時間: 0.9 秒

## テスト結果概要

-   総テスト数: 35
-   成功: 35
-   失敗: 0
-   スキップ: 0

## 詳細なテスト結果

### CommandTests

| テスト名                                 | 結果 | 実行時間 |
| ---------------------------------------- | ---- | -------- |
| ReactiveCommand_Execute_Notifies         | 成功 | <1ms     |
| ReactiveCommandT_Execute_PassesValue     | 成功 | <1ms     |
| AsyncCommand_Execute_UpdatesState        | 成功 | <1ms     |
| ReactiveCommand_CanExecuteChanged_Raises | 成功 | <1ms     |

### ViewModelBaseTests

| テスト名                           | 結果 | 実行時間 |
| ---------------------------------- | ---- | -------- |
| SubscribeToEvent_AddsToDisposables | 成功 | <1ms     |
| Dispose_UnsubscribesEvents         | 成功 | <1ms     |
| GetValue_ReturnsPropertyValue      | 成功 | <1ms     |
| SetValue_UpdatesPropertyValue      | 成功 | <1ms     |
| Activate_ChangesState              | 成功 | <1ms     |

### GameEventBusTests

| テスト名                                   | 結果 | 実行時間 |
| ------------------------------------------ | ---- | -------- |
| Publish_NotifiesSubscribers                | 成功 | <1ms     |
| Subscribe_MultipleTypes_NotifyOnlyMatching | 成功 | <1ms     |
| Publish_UnsubscribedType_DoesNotNotify     | 成功 | <1ms     |

### ReactivePropertyTests

| テスト名                        | 結果 | 実行時間 |
| ------------------------------- | ---- | -------- |
| ValueChange_NotifiesSubscribers | 成功 | <1ms     |
| Constructor_SetsInitialValue    | 成功 | <1ms     |
| MultipleChanges_NotifyInOrder   | 成功 | <1ms     |
| Dispose_StopNotifications       | 成功 | <1ms     |
| SetSameValue_DoesNotNotify      | 成功 | <1ms     |

### ReactivePropertyAdvancedTests

| テスト名                            | 結果 | 実行時間 |
| ----------------------------------- | ---- | -------- |
| ValueChanged_Observable_Notifies    | 成功 | <1ms     |
| SetValidator_PreventsInvalidValue   | 成功 | <1ms     |
| BeginUpdate_SuppressesNotifications | 成功 | <1ms     |

### CompositeDisposableTests

| テスト名                            | 結果 | 実行時間 |
| ----------------------------------- | ---- | -------- |
| AddAndDispose_DisposesAllResources  | 成功 | <1ms     |
| AddRange_AddsAllItems               | 成功 | <1ms     |
| Remove_ReturnsTrueAndDoesNotDispose | 成功 | <1ms     |
| Clear_DisposesAllAndEmpties         | 成功 | <1ms     |

### WeakEventManagerTests

| テスト名                  | 結果 | 実行時間 |
| ------------------------- | ---- | -------- |
| AddRaiseRemove_Works      | 成功 | <1ms     |
| DeadHandlers_AreCleanedUp | 成功 | <1ms     |

### ReactiveCollectionTests

| テスト名                                  | 結果 | 実行時間 |
| ----------------------------------------- | ---- | -------- |
| Add_RaisesChangeEvent                     | 成功 | <1ms     |
| Remove_RaisesChangeEvent                  | 成功 | <1ms     |
| Indexer_Set_ReplacesItemWithNotifications | 成功 | <1ms     |

## パフォーマンス測定結果

-   テスト実行の総時間: 0.9 秒
-   平均テスト実行時間: <1ms
-   最大テスト実行時間: <1ms

## 変更履歴

| バージョン | 日付       | 変更内容 |
| ---------- | ---------- | -------- |
| 0.1.0      | 2024-03-21 | 初版作成 |
