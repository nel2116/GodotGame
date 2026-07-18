# 🎮 ゲームシステム MOC（02_GameSystem）

> **🎯 目的**: ゲームシステム関連のドキュメントを体系的にナビゲート

## ⚔️ スキルシステム（Skills/）

- [[02_GameSystem/Skills/moc|スキルシステム MOC]]
- [[13_1_skill_data|スキルデータ仕様書]]
- [[11_13_skill_mechanics|スキルシステム詳細（開発計画）]]

## 🎮 プレイヤーシステム

設計詳細は 03_Architecture、実装リファレンスは 99_Reference を参照。

### 🧱 詳細設計（03_Architecture/Systems/Player/）
- [[player_system_overview|プレイヤーシステム実装詳細（概要）]]
- [[Player/01_input_system|入力システム]] / [[Player/02_movement_system|移動システム]] / [[Player/03_combat_system|戦闘システム]]
- [[Player/04_animation_system|アニメーションシステム]] / [[Player/05_state_system|状態システム]] / [[Player/06_progression_system|進行システム]]

### 📚 実装リファレンス（99_Reference/）
- [[PlayerSystem|プレイヤーシステム詳細]]
- [[PlayerCombatSystem|戦闘]] / [[PlayerMovementSystem|移動]] / [[PlayerInputSystem|入力]]
- [[PlayerAnimationSystem|アニメーション]] / [[PlayerStateSystem|状態]] / [[PlayerProgressionSystem|進行]]

## ⚔️ 戦闘システム

- [[Common/04_combat_system|共通戦闘システム設計]]
- [[PlayerCombatSystem|プレイヤー戦闘システム リファレンス]]

## 💾 セーブ・ロード / リソース管理

- [[06_save_load_system|セーブ・ロードシステム設計]]
- [[Common/08_resource_system|リソースシステム設計]]
- [[ResourceSystem|リソースシステム リファレンス]]

## 🔗 関連リンク

### 🧱 技術設計
- [[03_Architecture/moc|技術設計 MOC]]
- [[03_Architecture/Systems/moc|システム設計 MOC]]
- [[03_Architecture/Components/moc|コアコンポーネント MOC]]

### 📅 開発計画
- [[11_11_player_systems|プレイヤーシステム開発計画]]
- [[11_13_skill_mechanics|スキルメカニクス開発計画]]

### 🔄 メタ要素
- [[05_MetaElements/moc|メタ要素・リプレイ性 MOC]]

## 🚧 作成予定ドキュメント
- 戦闘システム統合仕様（Combat/）
- メタゲーム要素詳細（Meta/）
- UI・UX連携仕様（UI/）

---

## 🔄 更新履歴
| 日付 | 更新内容 |
|------|----------|
| 2025-07-04 | ゲームシステムMOC作成 |
| 2026-07-16 | 実在ドキュメントに合わせてリンク修正、空スタブ削除に伴い作成予定セクションへ移動 |
