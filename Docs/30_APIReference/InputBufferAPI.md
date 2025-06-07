---
title: InputBuffer.cs API
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

# InputBuffer.cs API

## 目次

1. [概要](#概要)
2. [詳細](#詳細)
3. [使用方法](#使用方法)
4. [制限事項](#制限事項)
5. [変更履歴](#変更履歴)

## 概要

`InputBuffer` は一定時間入力イベントを保持するキューです。`RetentionTime` 秒を過ぎた入力は自動的に削除されます。

## 詳細

- `Enqueue(InputEvent)` : 入力をバッファに追加します。
- `Dequeue()` : バッファから入力を取り出します。無い場合は `null` を返します。
- `Clear()` : 全ての入力履歴を消去します。

## 使用方法

1. 入力受付ノードの `_Input` または `_UnhandledInput` から `Enqueue` を呼び出します。
2. 必要に応じて `Dequeue` で直近の入力を取得します。

## 制限事項

- `RetentionTime` を長く設定するとメモリ使用量が増えます。

## 変更履歴

| バージョン | 更新日     | 変更内容 |
| ---------- | ---------- | -------- |
| 0.1.0      | 2025-06-07 | 初版作成 |

