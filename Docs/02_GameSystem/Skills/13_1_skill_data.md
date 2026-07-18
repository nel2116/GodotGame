---
title: スキルデータ仕様書
version: 0.1.0
status: draft
updated: 2024-03-21
tags:
    - Skills
    - Data
    - Specification
    - Core
linked_docs:
    - "[[02_GameSystem/Skills/moc|スキルシステム]]"
---

# スキルデータ仕様書

## 概要

このドキュメントでは、ゲーム内のスキルデータの構造と仕様について説明します。

## データ構造

スキルデータは以下の CSV ファイルで管理されています：

```dataview
TABLE WITHOUT ID skill_id,branch,tier,skill_name,skill_type,active_slot,description,base_value,scaling_stat,scaling_ratio,cooldown_sec,cost_type,cost,hitbox,range_m,duration_sec,unlock_condition,tags,vfx_id,sfx_id
FROM csv("SkilData.csv")
```

## フィールド説明

### 基本情報

-   `skill_id`: スキルの一意識別子
-   `branch`: スキルの分岐（例：攻撃、防御、サポート）
-   `tier`: スキルの段階（1-5）
-   `skill_name`: スキル名
-   `skill_type`: スキルの種類（アクティブ/パッシブ）
-   `active_slot`: アクティブスキルの場合の装備スロット

### 効果

-   `description`: スキルの説明文
-   `base_value`: 基本効果値
-   `scaling_stat`: スケーリング対象のステータス
-   `scaling_ratio`: スケーリング比率

### 制限

-   `cooldown_sec`: クールダウン時間（秒）
-   `cost_type`: コストの種類（MP/HP/その他）
-   `cost`: コスト量
-   `hitbox`: ヒットボックスの種類
-   `range_m`: 効果範囲（メートル）
-   `duration_sec`: 効果持続時間（秒）

### その他

-   `unlock_condition`: アンロック条件
-   `tags`: スキルのタグ（カンマ区切り）
-   `vfx_id`: 視覚効果の ID
-   `sfx_id`: 音響効果の ID

## 注意事項

-   スキルデータの変更は必ずバージョン管理システムを使用してください
-   新しいスキルを追加する際は、既存のスキルとのバランスを考慮してください
-   スキルの効果は、ゲームの基本設計に準拠してください
