---
title: PlayerStateMachine.cs API
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

# PlayerStateMachine.cs API

## 目次

1. [概要](#概要)
2. [詳細](#詳細)
3. [使用方法](#使用方法)
4. [制限事項](#制限事項)
5. [変更履歴](#変更履歴)

## 概要

`PlayerStateMachine` はプレイヤーの状態遷移を管理します。状態変更時には `EventBus` への通知と `StateManager` への記録を行います。

## 詳細

- `ChangeState(PlayerState)` : 状態を変更してイベントを通知します。
- `CancelState()` : 現在の状態を `Idle` に戻します。
- `CanTransition(PlayerState)` : 指定状態へ遷移可能か判定します。
- `CurrentState` : 現在の状態を取得します。

## 使用方法

1. `PlayerStateMachine` ノードをシーンに追加します。
2. `EventBus` と `StateManager` をプロパティ経由で設定してください。

## 制限事項

- タイムアウト設定が無い状態では自動キャンセルが行われません。

## 変更履歴

| バージョン | 更新日     | 変更内容 |
| ---------- | ---------- | -------- |
| 0.1.0      | 2025-06-07 | 初版作成 |

