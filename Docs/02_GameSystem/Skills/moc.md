# ⚔️ スキルシステム MOC（02_GameSystem/Skills）

> **🎯 目的**: スキルシステム関連のドキュメントを体系的にナビゲート

## 📋 スキルシステム概要

### 🎯 基本概念
- [[skill_mechanics|スキルメカニクス]]
  - スキルの基本動作、効果、仕組み
- [[skill_data|スキルデータ仕様]]
  - スキルデータの構造と定義

### 📊 スキルデータ

#### 🗂️ データファイル
- [[data/SkilData.csv|スキルデータCSV]]
  - スキル情報のCSVファイル
- [[data/SkilData.csv.import|スキルデータインポート設定]]
  - Godotインポート設定

#### 🏷️ 翻訳ファイル
- [[data/SkilData.skill.translation|スキル名翻訳]]
- [[data/SkilData.description.translation|スキル説明翻訳]]
- [[data/SkilData.tags.translation|スキルタグ翻訳]]
- [[data/SkilData.tier.translation|スキル階層翻訳]]
- [[data/SkilData.branch.translation|スキルブランチ翻訳]]

#### ⚙️ スキルパラメータ
- [[data/SkilData.base.translation|基本値翻訳]]
- [[data/SkilData.scaling.translation|スケーリング翻訳]]
- [[data/SkilData.cost.translation|コスト翻訳]]
- [[data/SkilData.cooldown.translation|クールダウン翻訳]]
- [[data/SkilData.duration.translation|持続時間翻訳]]
- [[data/SkilData.range.translation|射程翻訳]]
- [[data/SkilData.hitbox.translation|ヒットボックス翻訳]]

#### 🎨 エフェクト・アニメーション
- [[data/SkilData.active.translation|アクティブ状態翻訳]]
- [[data/SkilData.sfx.translation|音響効果翻訳]]
- [[data/SkilData.vfx.translation|視覚効果翻訳]]
- [[data/SkilData.unlock.translation|アンロック条件翻訳]]

## 🔗 関連リンク

### 🎮 ゲームシステム
- [[../moc|ゲームシステム]]
- [[../Combat/combat_system|戦闘システム]]
- [[../Player/progression_system|プレイヤー進行システム]]

### 🧱 技術実装
- [[../../03_Architecture/moc|技術設計・アーキテクチャ]]
- [[../../03_Architecture/Systems/combat_system|戦闘システム設計]]

### 📅 開発計画
- [[../../06_DevelopmentPlan/moc|開発計画・ロードマップ]]
- [[../../06_DevelopmentPlan/Implementation/skill_mechanics|スキルメカニクス実装計画]]

---

## 📊 スキルシステム統計
- **スキルデータファイル**: 1個
- **翻訳ファイル**: 16個
- **ドキュメント**: 2個

---

## 🔄 更新履歴
| 日付 | 更新内容 |
|------|----------|
| 2025-07-04 | スキルシステムMOC作成 |
| 2025-07-04 | スキル関連フォルダ統合 |

---

## 📋 既存ドキュメント

### ✅ 作成済みドキュメント
- [[13_1_skill_data|スキルデータ仕様]]
- [[index|スキルシステム概要]]

### 🚧 作成予定ドキュメント
- スキルメカニクス詳細
- スキルツリー設計
- スキルバランス調整

> **💡 ヒント**: 各セクションのドキュメントを作成して、詳細なスキルシステム設計を展開できます。
