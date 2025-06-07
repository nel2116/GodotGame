---
title: EventBus.cs API
version: 0.1.0
status: draft
updated: 2025-06-07
tags:
    - API
    - Script
linked_docs:
    - "[[30_APIReference/00_index]]"
    - "[[10_CoreDocs/15_ImplementationSpecs/15.1_InputManagementSpec.md]]"
---

# EventBus.cs API

## 目次

1. [概要](#概要)
2. [詳細](#詳細)
3. [使用方法](#使用方法)
4. [制限事項](#制限事項)
5. [変更履歴](#変更履歴)

## 概要

`EventBus` は優先度付きキューを用いたイベント通知システムです。履歴を保持し、フィルター機能付きの購読が可能です。

## 詳細

- `EmitEvent(string, Dictionary, int)` : 優先度を指定してイベントをキューに追加します。
- `EmitEvent(string, Dictionary)` : 優先度なしでイベントを追加します。
- `Subscribe(string, Callable, Callable)` : フィルター付きでイベントを購読します。
- `Subscribe(string, Callable)` : フィルターなしでイベントを購読します。
- `Unsubscribe(string, Callable)` : 登録済みハンドラーを解除します。
- `GetEventHistory(string)` : 指定イベントの履歴を取得します。

## 使用方法

1. `EventBus` ノードをシーンに配置するか、`Main` から取得して利用します。
2. `EmitEvent` でイベントを発行し、`Subscribe` でハンドラーを登録します。

## 制限事項

- `EmitEvent` の多用はパフォーマンスに影響する場合があります。

## 変更履歴

| バージョン | 更新日     | 変更内容 |
| ---------- | ---------- | -------- |
| 0.1.0      | 2025-06-07 | 初版作成 |

