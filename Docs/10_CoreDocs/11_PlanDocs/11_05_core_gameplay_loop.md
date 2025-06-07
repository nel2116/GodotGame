---
title: コアゲームループ図
version: 0.1.0
status: draft
updated: 2025-06-01
tags:
    - CoreLoop
    - Document
    - Core
linked_docs:
    - "[[11_1_plan|計画書]]"
    - "[[00_index|計画ドキュメント]]"
---

## コアゲームループ図

```mermaid
flowchart LR
  Start --> Room[戦闘部屋]
  Room --> Reward[報酬取得／スキルルーン選択]
  Reward --> Branch{分岐／探索?}
  Branch -->|戦闘| Room
  Branch -->|探索| Secret[隠しルート発見]
  Secret --> Room
  Room --> Boss[ボス or 次階層]
  Boss --> Meta[拠点／メタ強化]
  Meta --> Start

ループ要素詳細

| フェーズ  | 目的           | プレイヤー入力 | KPI 関連   |
| ----- | ------------ | ------- | -------- |
| 戦闘部屋  | スキル試用／リソース獲得 | 攻撃・回避   | 入力→ヒット遅延 |
| 報酬選択  | ビルド分岐を提示     | 3 択クリック | ビルド多様性   |
| 隠しルート | 発見の驚き        | 壁破壊・鍵使用 | "驚きイベント" |
| メタ強化  | 長期モチベ        | スキルツリー  | 周回数      |
```
