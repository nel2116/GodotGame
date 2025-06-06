---
title: プロトタイプ制作要素まとめ
version: 0.1.0
status: draft
updated: 2025-06-06
tags:
    - Prototype
    - Plan
    - Development
linked_docs:
    - "[[11_1_plan|プロジェクト計画書]]"
    - "[[11_3_mvp|MVP定義]]"
    - "[[11_5_development_roadmap|開発ロードマップ]]"
    - "[[11_5_core_loop|コアループ]]"
    - "[[11_6_feature_list|機能一覧]]"
---

# プロトタイプ制作要素まとめ

## 目次

1. [概要](#概要)
2. [必要要素](#必要要素)
3. [開発環境](#開発環境)
4. [進行手順](#進行手順)
5. [変更履歴](#変更履歴)

## 概要

Docs/10_CoreDocs/11_PlanDocs の各ドキュメントを参照し、プロトタイプ段階で実装すべき機能と準備事項を整理する。

## 必要要素

- **ジョブ & スキル**：
    - 影使い 1 種のみ実装
    - スキル 3 種（通常攻撃、回避ロール、チャージ攻撃）
- **レベル構造**：
    - 16×16 タイル部屋を 8 個接続するランダム生成
    - 隠し通路と鍵扉のギミックを 1 種ずつ
- **敵 & ボス**：
    - 通常敵 4 タイプ（近接、遠隔、突進、召喚）
    - ミニボス 1 体 / ボス 1 体
- **メタ進行要素**：
    - 影の欠片による恒久ステータス強化
    - 簡易スキルツリー UI
- **UX**：
    - キーボード / ゲームパッド両対応
    - 死亡後 10 秒以内の再挑戦ロード

## 開発環境

- **Godot 4.x (.NET/C#)** を使用
- `TileMap` + `NavigationServer` で部屋生成と AI パス管理
- `Resource` ファイルでスキルや聖遺物をデータ駆動化
- `StateMachine`(`AnimationTree`) によるアクション遷移
- JSON 形式でメタ進行データを保存

## 進行手順

1. **プレイヤーキャラクター基盤**：
    - 移動・ジャンプ・ダッシュ、ステートマシンと入力システム
    - 詳細: [[11_11_player_foundation|プレイヤー基盤詳細]]
2. **レベル生成**：
    - 部屋形状定義と接続ロジック、ナビゲーションメッシュ
    - 詳細: [[11_12_level_generation|レベル生成詳細]]
3. **スキルシステム**：
    - スキル定義と効果実行、クールダウン管理、スキルツリー UI
    - 詳細: [[11_13_skill_system|スキルシステム詳細]]
4. **UI とメタ要素**：
    - ヘルスバー、スキルアイコン、インベントリの簡易実装
    - 詳細: [[11_14_ui_meta|UIとメタ要素詳細]]
5. **テストと調整**：
    - コアループ通りに遊べるかチェックし、KPI 指標を計測
    - 詳細: [[11_15_testing_adjustment|テストと調整詳細]]

## 変更履歴

| バージョン | 更新日     | 変更内容       |
| ---------- | ---------- | -------------- |
| 0.1.0      | 2025-06-06 | 初版作成       |

