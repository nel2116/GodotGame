---
title: Main.cs API
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

# Main.cs API

## 目次

1. [概要](#概要)
2. [詳細](#詳細)
3. [使用方法](#使用方法)
4. [制限事項](#制限事項)
5. [変更履歴](#変更履歴)

## 概要

`Main` ノードはゲーム開始時にコアシステムを初期化します。`EventBus`、`StateManager`、`PlayerStateMachine`、`InputBuffer` を自動でシーンツリーに登録し、入力を `InputBuffer` へ転送します。

## 詳細

```csharp
public override void _Ready()
```
: 初期化時に各システムノードを `AddChild` します。

```csharp
public override void _Input(InputEvent @event)
```
: 受け取った入力イベントを `InputBuffer` に送ります。

## 使用方法

1. シーンのルートに `Main` ノードを配置します。
2. ゲーム起動時に自動で各種システムが追加されます。

## 制限事項

- コアシステムの依存関係を変更する場合は `_Ready` 内の処理を修正してください。

## 変更履歴

| バージョン | 更新日     | 変更内容 |
| ---------- | ---------- | -------- |
| 0.1.0      | 2025-06-07 | 初版作成 |

