# 🗂 技術設計 MOC（03_Architecture）

> **🎯 目的**: 技術設計・アーキテクチャ関連のドキュメントを体系的にナビゲート

## 📐 設計関連

### 🏗️ アーキテクチャ設計
- [[Design/mvvm_rx_architecture|MVVM/Rxベースのアーキテクチャ設計]]
  - MVVMパターンとリアクティブプログラミングの統合
- [[Design/basic_design|設計の基本方針]]
  - プロジェクト全体の設計原則とガイドライン
- [[Design/detailed_design|詳細設計のまとめ]]
  - システム全体の詳細設計仕様

### 🔗 システム統合
- [[Integration/system_integration|システム間の統合戦略]]
  - 各システム間の連携と統合方法

## 🔌 共通・補助機能

### 🛠️ 共通ユーティリティ
- [[Utilities/common_utilities|共通ユーティリティ機能の設計]]
  - プロジェクト全体で使用する共通機能

### 🎮 入力管理
- [[Implementation/input_management|入力管理システムの実装仕様]]
  - プレイヤー入力の処理と管理

### ⚡ リアクティブシステム
- [[Implementation/reactive_system|リアクティブシステムの仕様と設計]]
  - リアクティブプログラミングパターンの実装

### 🔄 ステート管理
- [[Implementation/state_management|ステート管理のアーキテクチャ]]
  - ゲーム状態とプレイヤー状態の管理

## ⚙️ コンポーネント／システム構造

### 🔧 コアコンポーネント
- [[Components/moc|コアコンポーネントの構造と責務]]
  - ReactiveProperty、ViewModelBase、CompositeDisposable、EventBus

### 🎯 システム群
- [[Systems/moc|システム群の設計構成]]
  - 移動、アニメーション、戦闘、リソース管理システム

## 🌐 環境・開発フロー

### 🎮 Godot環境
- [[Environment/godot_environment|Godotエンジン設定と構築]]
  - 開発環境のセットアップと設定

### 🤖 テスト自動化
- [[../07_Testing/Automation/test_automation|テスト自動化システム]]
  - 自動テストの実行と管理

---

## 🔗 関連リンク

### 📚 リファレンス
- [[../99_Reference/ReactiveSystem|リアクティブシステム詳細]]
- [[../99_Reference/ViewModelSystem|ビューモデルシステム]]
- [[../99_Reference/CompositeDisposable|コンポジットディスポーザブル]]

### 🧪 テスト結果
- [[../99_Reference/ReactiveSystemTestResults|リアクティブシステムテスト結果]]
- [[../99_Reference/CoreSystemTestResults|コアシステムテスト結果]]

### 📖 ガイド
- [[../99_Reference/GettingStarted|はじめに]]
- [[../99_Reference/BasicFeatures|基本機能]]
- [[../99_Reference/AdvancedFeatures|応用機能]]

---

## 📊 技術設計統計
- **設計ドキュメント**: 15個
- **実装仕様書**: 8個
- **コンポーネント**: 6個
- **システム**: 12個

---

## 🔄 更新履歴
| 日付 | 更新内容 |
|------|----------|
| 2025-07-04 | 技術設計MOC作成 |
| 2025-07-04 | アーキテクチャ構造整理 |

---

> **💡 ヒント**: 各セクションのMOCファイルをクリックして、詳細な技術仕様にアクセスできます。
