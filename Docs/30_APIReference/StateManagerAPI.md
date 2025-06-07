---
title: StateManager.cs API
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

# StateManager.cs API

## 目次

1. [概要](#概要)
2. [詳細](#詳細)
3. [使用方法](#使用方法)
4. [制限事項](#制限事項)
5. [変更履歴](#変更履歴)

## 概要

`StateManager` は任意の状態をキーと値で保持する汎用管理クラスです。履歴管理や遷移ルール登録、ファイルへの保存と読み込みをサポートします。

## 詳細

- `SetState(string, Variant)` : 状態を設定し履歴に追加します。
- `GetState(string)` : 現在の状態を取得します。
- `RegisterTransition(string, Variant, Variant)` : 状態遷移ルールを登録します。
- `Observe(string, Callable)` : 状態変更を監視するコールバックを追加します。
- `SaveAll(string)` / `LoadAll(string)` : 状態をファイルに保存・読み込みします。

## 使用方法

1. ゲーム全体の状態管理に `StateManager` ノードを利用します。
2. 遷移ルールを登録したい場合は `RegisterTransition` を呼び出します。

## 制限事項

- 大量の状態履歴を保持するとメモリ消費が増加します。

## 変更履歴

| バージョン | 更新日     | 変更内容 |
| ---------- | ---------- | -------- |
| 0.1.0      | 2025-06-07 | 初版作成 |

