---
title: InputObserver.cs API
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

# InputObserver.cs API

## 目次

1. [概要](#概要)
2. [詳細](#詳細)
3. [使用方法](#使用方法)
4. [制限事項](#制限事項)
5. [変更履歴](#変更履歴)

## 概要

`InputObserver` は `_UnhandledInput` で受け取った入力を指定の `InputBuffer` に登録する補助ノードです。

## 詳細

- `Buffer` プロパティに `InputBuffer` を設定して使用します。
- `_UnhandledInput(InputEvent)` : 受け取った入力を `Buffer` へ送ります。

## 使用方法

1. 監視対象のノードに `InputObserver` をアタッチします。
2. `Buffer` プロパティに共有の `InputBuffer` を割り当てます。

## 制限事項

- `Buffer` が未設定の場合、入力は無視されます。

## 変更履歴

| バージョン | 更新日     | 変更内容 |
| ---------- | ---------- | -------- |
| 0.1.0      | 2025-06-07 | 初版作成 |

