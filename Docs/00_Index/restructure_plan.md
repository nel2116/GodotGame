# 🔄 GodotGame ドキュメント再構成計画書

> **🎯 目的**: MOCシステム構築後のドキュメント統合・移動・命名修正の実行計画

## 📊 現在の状況

### ✅ 完了済み
- **MOCシステム構築**: 8カテゴリすべてのMOCファイル作成完了
- **トップレベルナビゲーション**: 00_Index/moc.md作成
- **概要ファイル**: overview.md作成
- **README整理**: クイックアクセス対応

### 📈 統計情報
- **総ファイル数**: 135個（+12個）
- **内部リンク数**: 465個（+261個）
- **タグ数**: 45個（変更なし）

## 🔄 実行計画

### 🎯 フェーズ1: 重複ファイル・フォルダの統合

#### 1.1 スキルシステム関連の統合
| 現在のパス | 新しいパス | アクション | 理由 |
|------------|------------|------------|------|
| `10_CoreDocs/13_Skils/` | `02_GameSystem/Skills/` | フォルダ移動 + 名前修正 | typo修正（Skils→Skills） |
| `10_CoreDocs/13_Skills/` | `02_GameSystem/Skills/` | フォルダ統合 | 重複フォルダの統合 |
| `02_GameSystem/01_SkillSystem/` | `02_GameSystem/Skills/` | フォルダ統合 | スキル関連の一元化 |

#### 1.2 MVP定義の統合
| 現在のパス | 新しいパス | アクション | 理由 |
|------------|------------|------------|------|
| `06_DevelopmentPlan/11_03_mvp_definition.md` | `06_DevelopmentPlan/MVP/mvp_definition.md` | ファイル移動 | MVP専用フォルダに整理 |
| `11_PlanDocs/11_3_mvp.md` | `06_DevelopmentPlan/MVP/mvp_definition.md` | ファイル統合 | 重複ファイルの統合 |

#### 1.3 技術設計の統合
| 現在のパス | 新しいパス | アクション | 理由 |
|------------|------------|------------|------|
| `03_Technical/12_01_mvvm_rx_architecture.md` | `03_Architecture/Design/mvvm_rx_architecture.md` | ファイル移動 | 技術設計カテゴリに統合 |
| `12_Architecture/12_01_mvvm_rx_architecture.md` | `03_Architecture/Design/mvvm_rx_architecture.md` | ファイル統合 | 重複ファイルの統合 |

### 🎯 フェーズ2: カテゴリ構造の最適化

#### 2.1 技術設計カテゴリの再編成
```
03_Architecture/
├── Design/                    # 設計関連
│   ├── mvvm_rx_architecture.md
│   ├── basic_design.md
│   └── detailed_design.md
├── Components/                # コアコンポーネント
│   ├── reactive_property.md
│   ├── viewmodel_base.md
│   └── composite_disposable.md
├── Systems/                   # システム設計
│   ├── movement_system.md
│   ├── animation_system.md
│   └── combat_system.md
├── Implementation/            # 実装仕様
│   ├── input_management.md
│   ├── reactive_system.md
│   └── state_management.md
├── Environment/               # 環境設定
│   └── godot_environment.md
└── Utilities/                 # 共通ユーティリティ
    └── common_utilities.md
```

#### 2.2 ゲームシステムカテゴリの再編成
```
02_GameSystem/
├── Skills/                    # スキルシステム
│   ├── skill_mechanics.md
│   ├── skill_data.md
│   └── data/                  # スキルデータファイル
├── Combat/                    # 戦闘システム
│   ├── combat_system.md
│   └── player_combat.md
├── Player/                    # プレイヤーシステム
│   ├── movement_system.md
│   ├── input_system.md
│   ├── animation_system.md
│   ├── state_system.md
│   └── progression_system.md
├── SaveLoad/                  # セーブ・ロード
│   └── save_load_system.md
├── Resource/                  # リソース管理
│   └── resource_system.md
└── Meta/                      # メタ要素
    └── meta_elements.md
```

#### 2.3 開発計画カテゴリの再編成
```
06_DevelopmentPlan/
├── Project/                   # プロジェクト計画
│   ├── project_plan.md
│   ├── design_pillars.md
│   └── core_experience.md
├── MVP/                       # MVP定義
│   ├── mvp_definition.md
│   ├── feature_specifications.md
│   └── core_gameplay_loop.md
├── Metrics/                   # 指標・分析
│   ├── kpi_metrics.md
│   └── risk_analysis.md
├── Implementation/            # 実装計画
│   ├── player_systems.md
│   ├── level_design.md
│   ├── skill_mechanics.md
│   └── ui_metagame.md
├── Weekly/                    # 週次計画
│   ├── week1_dungeon_foundation.md
│   ├── week2_dungeon_extension.md
│   └── week3_dungeon_integration_test.md
├── Optimization/              # 最適化計画
│   ├── optimization_implementation.md
│   └── player_movement_enhancement.md
└── Architecture/              # 技術アーキテクチャ
    ├── development_roadmap.md
    └── technical_architecture.md
```

### 🎯 フェーズ3: リンク更新・検証

#### 3.1 内部リンクの更新
- 既存の`index.md`ファイルを`moc.md`に置き換え
- 移動・統合されたファイルへのリンク更新
- 新しいカテゴリ構造へのリンク更新

#### 3.2 リンク切れの検出・修正
- `vault_audit.py`によるリンク切れ検出
- 手動でのリンク更新
- 最終検証

## 📋 実行チェックリスト

### ✅ フェーズ1: 重複統合
- [ ] `13_Skils/` → `13_Skills/` フォルダ名修正
- [ ] スキル関連フォルダの統合
- [ ] MVP定義ファイルの統合
- [ ] 技術設計ファイルの統合

### ✅ フェーズ2: 構造最適化
- [ ] `03_Architecture/` フォルダ構造作成
- [ ] `02_GameSystem/` フォルダ構造作成
- [ ] `06_DevelopmentPlan/` フォルダ構造作成
- [ ] ファイルの移動・再配置

### ✅ フェーズ3: リンク更新
- [ ] 既存`index.md`を`moc.md`に置き換え
- [ ] 内部リンクの更新
- [ ] リンク切れの検出・修正
- [ ] 最終検証

## 🎯 期待される効果

### 📈 ナビゲーション改善
- **検索性向上**: 体系的構造による素早い情報アクセス
- **関連性の可視化**: カテゴリ間の連携が明確化
- **重複排除**: 同一内容の重複ファイルを統合

### 🛠️ 開発効率化
- **クイックアクセス**: 目的別のナビゲーション
- **一貫性向上**: 統一された命名規則
- **保守性向上**: モジュラー構造による管理しやすさ

### 📊 品質向上
- **リンク整合性**: リンク切れの解消
- **構造最適化**: 論理的なカテゴリ分け
- **拡張性**: 将来の追加に対応しやすい構造

---

## 🔄 更新履歴
| 日付 | 更新内容 |
|------|----------|
| 2025-07-04 | 再構成計画書作成 |
| 2025-07-04 | MOCシステム構築完了 |

---

> **💡 ヒント**: この計画に従って段階的に実行することで、ドキュメント構造の最適化を実現できます。
