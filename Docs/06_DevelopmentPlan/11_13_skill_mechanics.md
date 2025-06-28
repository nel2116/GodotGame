---
title: スキルシステム詳細
version: 0.1.0
status: draft
updated: 2025-06-06
tags:
    - Prototype
    - Plan
    - Development
linked_docs:
    - "[[11_10_prototype_guidelines|プロトタイプ制作要素まとめ]]"
    - "[[00_index|計画ドキュメント]]"
    - "[[12_Architecture/12_03_detailed_design/01_core_components/02_viewmodel_base|ViewModelBase実装詳細]]"
    - "[[12_Architecture/12_03_detailed_design/01_core_components/01_reactive_property|ReactiveProperty実装詳細]]"
    - "[[12_Architecture/12_03_detailed_design/01_core_components/03_composite_disposable|CompositeDisposable実装詳細]]"
    - "[[12_Architecture/12_03_detailed_design/01_core_components/04_event_bus|イベントバス実装詳細]]"
---

# スキルシステム詳細

## 概要

プレイヤーが使用するスキルの枠組みを決定し、プロトタイプで試すべき基本的な要素を洗い出す。データ定義とクールダウン管理の仕組みを検証することが目的。

## 詳細

-   スキルごとの効果内容とパラメータを一覧化
-   スキル発動から効果適用までの流れを整理
-   クールダウン時間とリソース消費量の仮設定
-   スキルツリー UI 上でのアンロック条件をまとめる
-   テスト用にスキル切り替え操作を準備し、挙動確認を行う

## 目的

-   各スキルの役割とバランスを把握する
-   データ駆動化の方針を固め、拡張しやすい構造を検討する

## 完了条件

-   最低 3 種のスキルが問題なく発動できる
-   スキルツリー UI 上でアンロック状態の変化を確認できる

## 変更履歴

| バージョン | 更新日     | 変更内容 |
| ---------- | ---------- | -------- |
| 0.1.0      | 2025-06-06 | 初版作成 |
