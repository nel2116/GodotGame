---
title: Week 2: ダンジョン生成システム機能拡張実装計画
version: 0.2.0
status: in-progress
updated: 2026-07-17
tags:
    - Implementation
    - Plan
    - Week2
    - Dungeon
    - Extension
    - DevelopmentPlan
linked_docs:
    - "[[11_17_week1_dungeon_foundation_plan|Week 1: 基盤実装計画]]"
    - "[[mvp_definition|MVP定義]]"
    - "[[11_12_level_design|レベル生成詳細]]"
    - "[[03_level_generation|レベル生成システム実装詳細]]"
---

# Week 2: ダンジョン生成システム機能拡張実装計画

## 目次

1. [概要](#1-概要)
2. [実装範囲](#2-実装範囲)
3. [日別実装計画](#3-日別実装計画)
4. [技術仕様](#4-技術仕様)
5. [成果物定義](#5-成果物定義)
6. [テスト戦略](#6-テスト戦略)
7. [リスク管理](#7-リスク管理)
8. [依存関係](#8-依存関係)
9. [制限事項](#9-制限事項)
10. [変更履歴](#10-変更履歴)

## 1. 概要

### 1.1 目的
Week 1で構築したダンジョン生成基盤を拡張し、ゲームプレイに必要なギミック・ナビゲーション・タイルマップ・ViewModel・イベント統合を実装する。

### 1.2 期間
**Week 2**: 2025年2月3日 - 2025年2月9日（7日間）

### 1.3 前提条件
- Week 1の基盤システムが完成していること
- データ構造・レベル生成モデルが利用可能であること

## 2. 実装範囲

### 2.1 対象システム
- ギミック配置システム（隠し通路・鍵扉）
- ナビゲーションシステム（経路探索・移動管理）
- タイルマップシステム（視覚的表現）
- ViewModel層（MVVM統合）
- イベントシステム統合（システム間通信）

### 2.2 非対象システム
- 敵AI・バトル・UI（Week 3以降）

## 3. 日別実装計画

### 3.1 Day 1-2: ギミック配置システム
#### Day 1: ギミック配置ロジック
- GimmickPlacementModel: 隠し通路・鍵扉の配置
- 配置アルゴリズムの最適化・重複防止

#### Day 2: ギミックアクティベーション
- GimmickActivator: ギミックの状態管理・発動条件
- 隠し通路開通・鍵扉解除の実装
- イベント発行・視覚的フィードバック

### 3.2 Day 3-4: ナビゲーションシステム
#### Day 3: ナビゲーションマネージャー
- NavigationManager: 経路管理・ナビメッシュ生成
- NavigationMesh: 通行可能エリア・障害物除外

#### Day 4: パスファインディング
- PathFinder: A*アルゴリズムによる最短経路探索
- NavigationNode: ノード定義・コスト計算

### 3.3 Day 5-6: タイルマップシステム
#### Day 5: タイルマップマネージャー
- TileMapManager: タイル配置・動的更新
- TileSetManager: タイルセット管理・自動配置

#### Day 6: 部屋タイル生成
- RoomTileGenerator: 部屋タイプ別タイル生成
- TileRenderer: 描画・レイヤー管理・最適化

### 3.4 Day 7: ViewModel層とイベント統合
#### Day 7: システム統合
- DungeonViewModel: ダンジョン状態管理・イベント購読
- DungeonEvents: イベント定義・システム間通信

## 4. 技術仕様

### 4.1 パフォーマンス最適化
- 遅延生成・不要データ解放
- 視界外描画スキップ
- 経路計算のキャッシュ

### 4.2 拡張性
- プラグイン式ギミック追加
- 外部設定ファイル対応
- モジュラー設計

### 4.3 エラーハンドリング
- 例外処理・ログ出力
- フォールバック処理

## 5. 成果物定義

### 5.1 必須成果物
- Scripts/Systems/Dungeon/Gimmicks/
  - GimmickPlacementModel.cs
  - GimmickActivator.cs
  - HiddenPassageGimmick.cs
  - LockedDoorGimmick.cs
- Scripts/Systems/Dungeon/Navigation/
  - NavigationManager.cs
  - NavigationMesh.cs
  - PathFinder.cs
  - NavigationNode.cs
- Scripts/Systems/Dungeon/TileMap/
  - TileMapManager.cs
  - TileSetManager.cs
  - RoomTileGenerator.cs
  - TileTemplate.cs
  - TileRenderer.cs
- Scripts/Systems/Dungeon/ViewModels/
  - DungeonViewModel.cs
  - RoomViewModel.cs
- Scripts/Systems/Dungeon/Events/
  - DungeonEvents.cs

### 5.2 テストコード
- Tests/Systems/Dungeon/Gimmicks/
- Tests/Systems/Dungeon/Navigation/
- Tests/Systems/Dungeon/TileMap/
- Tests/Systems/Dungeon/ViewModels/
- Tests/Systems/Dungeon/Integration/

### 5.3 ドキュメント
- API仕様書・使用例・トラブルシューティング

## 6. テスト戦略

### 6.1 単体テスト
- 各ギミック・ナビゲーション・タイルマップ・ViewModelの個別テスト
- カバレッジ80%以上

### 6.2 統合テスト
- システム連携・イベント伝播の検証

### 6.3 パフォーマンステスト
- 経路探索・描画・ギミック発動の速度測定

## 7. リスク管理

### 7.1 技術的リスク
- 経路探索の計算量増大
- ギミック配置のバグ
- タイル描画のパフォーマンス

### 7.2 対策
- 早期プロファイリング・段階的実装
- テストファースト・コードレビュー
- キャッシュ・遅延生成の活用

## 8. 依存関係
- Week 1基盤システム
- Godot Engine 4.4 / .NET 8.0
- Core/Reactive, Core/Events, Core/ViewModels
- NUnit, MSBuild, Git

## 9. 制限事項

### 9.1 技術的制限
- ギミックは隠し通路と鍵扉のみ実装
- ナビゲーションはA*アルゴリズムのみ使用
- タイルマップは2D表示のみ対応

### 9.2 スコープ制限
- 敵AIシステムは対象外
- バトルシステムは対象外
- UIシステムは対象外（Week 3以降）

### 9.3 パフォーマンス制限
- 経路探索は100ms以内に完了
- タイル描画は60FPSを維持
- メモリ使用量は50MB以内

## 10. 変更履歴

| バージョン | 更新日     | 変更内容 |
| ---------- | ---------- | -------- |
| 0.2.0      | 2026-07-17 | Phase 1-4 実装完了を反映<br>- Gimmicks（GimmickPlacementModel/GimmickActivator/HiddenPassageGimmick/LockedDoorGimmick）、Navigation（NavigationMesh/NavigationNode/PathFinder/NavigationManager）、TileMap（TileType/TileTemplate/TileSetManager/RoomTileGenerator/TileMapManager/TileRenderer）、ViewModels+Events（DungeonViewModel/RoomViewModel/DungeonEvents）を実装<br>- `dotnet test` 196件全pass<br>- テストは計画書記載の `Tests/Systems/Dungeon/` ではなく `Tests/Core/Dungeon/{Gimmicks,Navigation,TileMap,ViewModels}` に配置（Week 1からの既存規約に統一）<br>- 未実施: GUTテスト（TileMapManager/TileRendererはGodotノード依存のため薄いラッパーに留め、ロジックはNUnitでカバーする方針としGDScript側のGUTテストは見送り）、実際の`.tres`タイルセット資産の作成（プレースホルダーのアトラス座標マッピングのみ）、テストカバレッジ計測、パフォーマンス/メモリ計測、API仕様書等の別ドキュメント作成 |
| 0.1.0      | 2025-06-20 | 初版作成<br>- Week 2機能拡張実装計画<br>- ギミック・ナビゲーション・タイルマップ・ViewModel統合<br>- 7日間の日別実装スケジュール<br>- テスト戦略とリスク管理 |

