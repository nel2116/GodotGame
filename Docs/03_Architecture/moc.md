# 🗂 技術設計 MOC（03_Architecture）

> **🎯 目的**: 技術設計・アーキテクチャ関連のドキュメントを体系的にナビゲート

## 📐 設計（Design/）

- [[Design/mvvm_rx_architecture|MVVM/Rxベースのアーキテクチャ設計]]
  - MVVMパターンとリアクティブプログラミングの統合
- [[Design/basic_design|基本設計]]
  - プロジェクト全体の設計原則とガイドライン
- [[Design/detailed_design|詳細設計のまとめ]]
  - システム全体の詳細設計仕様
- [[Design/architecture_overview|詳細設計アーキテクチャ概要]]
  - 詳細設計ドキュメント群の全体像
- [[SystemResponsibilities|システム責任範囲]]
  - 各システムの責務定義

## 🔧 コアコンポーネント（Components/）

- [[Components/moc|コアコンポーネント MOC]]
  - ReactiveProperty、ViewModelBase、CompositeDisposable、EventBus など

## 🎯 システム設計（Systems/）

- [[Systems/moc|システム設計 MOC]]
  - 共通システム、プレイヤーシステム、スキル・レベル生成・敵AI など

## 🔗 システム統合（Integration/）

- [[Integration/system_integration|システム間の統合戦略]]
  - 各システム間の連携と統合方法

## ⚡ 最適化（Optimization/）

- [[01_performance_optimization|パフォーマンス最適化]]
  - 最適化の設計方針と実装詳細

## 🛠️ 共通ユーティリティ（Utilities/）

- [[Utilities/common_utilities|共通ユーティリティ機能の設計]]
  - プロジェクト全体で使用する共通機能

---

## 🔗 関連リンク

### 📚 リファレンス
- [[ReactiveSystem|リアクティブシステム詳細]]
- [[ViewModelSystem|ビューモデルシステム]]
- [[CompositeDisposable|コンポジットディスポーザブル]]

### 🧪 テスト
- [[testing_strategy|テスト戦略]]
- [[07_Testing/moc|テスト・品質保証 MOC]]

### 📖 ガイド
- [[GettingStarted|はじめに]]
- [[BasicFeatures|基本機能]]
- [[AdvancedFeatures|応用機能]]

---

## 🔄 更新履歴
| 日付 | 更新内容 |
|------|----------|
| 2025-07-04 | 技術設計MOC作成 |
| 2026-07-16 | 03_Technical を統合し新フォルダ構造（Design/Components/Systems/Integration/Optimization/Utilities）に更新 |
