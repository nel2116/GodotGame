# 🎮 ゲームシステム MOC（02_GameSystem）

> **🎯 目的**: ゲームシステム関連のドキュメントを体系的にナビゲート

## ⚔️ 戦闘・スキルシステム

### 🎯 スキルシステム
- [[Skills/moc|スキルシステム]]
  - スキルデータ仕様、スキルメカニクス、スキルツリー
- [[Skills/skill_mechanics|スキルメカニクス詳細]]
  - スキルの効果、クールダウン、リソース消費
- [[Skills/skill_data|スキルデータ仕様書]]
  - スキルデータの構造とCSVファイル仕様

### ⚔️ 戦闘システム
- [[Combat/combat_system|戦闘システム]]
  - ダメージ計算、ヒットボックス、戦闘フロー
- [[Combat/player_combat|プレイヤー戦闘システム]]
  - プレイヤー固有の戦闘機能

## 🎮 プレイヤーシステム

### 🏃 移動システム
- [[Player/movement_system|プレイヤー移動システム]]
  - 移動制御、物理演算、アニメーション連携
- [[Player/input_system|プレイヤー入力システム]]
  - 入力処理、バッファリング、キーバインド

### 🎭 アニメーション・状態
- [[Player/animation_system|プレイヤーアニメーションシステム]]
  - アニメーション制御、状態遷移
- [[Player/state_system|プレイヤー状態システム]]
  - 状態管理、状態遷移ロジック

### 📈 進行・成長
- [[Player/progression_system|プレイヤー進行システム]]
  - レベルアップ、スキルポイント、成長要素

## 💾 データ管理

### 💾 セーブ・ロード
- [[SaveLoad/save_load_system|セーブ・ロードシステム]]
  - データ永続化、チェックポイント、自動セーブ

### 📦 リソース管理
- [[Resource/resource_system|リソース管理システム]]
  - アセット管理、メモリ最適化、ローディング

## 🎯 ゲーム進行

### 🎲 メタゲーム要素
- [[Meta/meta_elements|メタゲーム要素]]
  - 進行度管理、アンロック要素、リプレイ性

### 🎨 UI・UX連携
- [[UI/ui_integration|UI・UX連携]]
  - ゲームシステムとUIの連携

---

## 🔗 関連リンク

### 🧱 技術実装
- [[../03_Architecture/Systems/moc|システム設計]]
- [[../03_Architecture/Components/moc|コアコンポーネント]]

### 📚 リファレンス
- [[../99_Reference/PlayerSystem|プレイヤーシステム詳細]]
- [[../99_Reference/PlayerCombatSystem|プレイヤー戦闘システム]]
- [[../99_Reference/PlayerMovementSystem|プレイヤー移動システム]]
- [[../99_Reference/PlayerInputSystem|プレイヤー入力システム]]
- [[../99_Reference/PlayerAnimationSystem|プレイヤーアニメーションシステム]]
- [[../99_Reference/PlayerStateSystem|プレイヤー状態システム]]
- [[../99_Reference/PlayerProgressionSystem|プレイヤー進行システム]]

### 📅 開発計画
- [[../06_DevelopmentPlan/11_11_player_systems|プレイヤーシステム開発計画]]
- [[../06_DevelopmentPlan/11_13_skill_mechanics|スキルメカニクス開発計画]]

---

## 📊 ゲームシステム統計
- **スキルシステム**: 5個
- **戦闘システム**: 3個
- **プレイヤーシステム**: 8個
- **データ管理**: 4個
- **メタ要素**: 2個

---

## 🔄 更新履歴
| 日付 | 更新内容 |
|------|----------|
| 2025-07-04 | ゲームシステムMOC作成 |
| 2025-07-04 | システム分類整理 |

---

> **💡 ヒント**: 各システムのMOCファイルをクリックして、詳細な仕様にアクセスできます。
