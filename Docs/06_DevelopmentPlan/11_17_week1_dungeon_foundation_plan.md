---
title: Week 1: ダンジョン生成システム基盤実装計画
version: 0.2.0
status: in-progress
updated: 2026-07-17
tags:
    - Implementation
    - Plan
    - Week1
    - Dungeon
    - Foundation
    - DevelopmentPlan
linked_docs:
    - "[[mvp_definition|MVP定義]]"
    - "[[11_12_level_design|レベル生成詳細]]"
    - "[[11_16_core_implementation|コアシステム実装計画]]"
    - "[[03_level_generation|レベル生成システム実装詳細]]"
---

# Week 1: ダンジョン生成システム基盤実装計画

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

MVP定義に基づくダンジョン生成システムの基盤を構築し、以下の要件を満たすシステムを実装する：

- 16×16タイルの部屋を8個生成
- 部屋間の接続ロジック
- 基本的な部屋レイアウト生成
- 拡張可能なアーキテクチャ

### 1.2 期間

**Week 1**: 2025年1月27日 - 2025年2月2日（7日間）

### 1.3 前提条件

- 既存のMVVM + Reactive Programmingアーキテクチャの理解
- Godot Engine 4.4 + C#環境の整備
- 既存のテスト環境の利用

## 2. 実装範囲

### 2.1 対象システム

#### 2.1.1 データ構造層
- **RoomData**: 部屋の基本情報管理
- **DoorData**: 扉の情報管理
- **GimmickData**: ギミックの情報管理
- **DungeonEnums**: 列挙型定義

#### 2.1.2 モデル層
- **LevelGenerationModel**: レベル生成の主要ロジック
- **RoomConnectionModel**: 部屋接続ロジック
- **RoomLayoutGenerator**: 部屋レイアウト生成

#### 2.1.3 インターフェース層
- **ILevelGenerator**: レベル生成の基本契約
- **IRoomConnector**: 部屋接続の基本契約

#### 2.1.4 ユーティリティ層
- **DungeonConstants**: 定数定義
- **DungeonUtils**: ユーティリティ関数

### 2.2 非対象システム

以下のシステムはWeek 2以降で実装：

- ギミック配置システム
- ナビゲーションシステム
- タイルマップシステム
- ViewModel層
- イベントシステム統合

## 3. 日別実装計画

### 3.1 Day 1-2: データ構造とインターフェース定義

#### Day 1: 基本データ構造

**目標**: ダンジョン生成に必要な基本データ構造を定義

**実装ファイル**:
```
Scripts/Systems/Dungeon/Data/
├── RoomData.cs
├── DoorData.cs
├── GimmickData.cs
└── Enums/
    └── DungeonEnums.cs
```

**タスク詳細**:

1. **RoomDataクラス**の実装
   ```csharp
   public class RoomData
   {
       public Vector2I Position { get; set; }
       public Vector2I Size { get; set; }
       public RoomType Type { get; set; }
       public List<DoorData> Doors { get; set; }
       public List<GimmickData> Gimmicks { get; set; }
       public bool IsGenerated { get; set; }
   }
   ```

2. **DoorDataクラス**の実装
   ```csharp
   public class DoorData
   {
       public Vector2I Position { get; set; }
       public DoorType Type { get; set; }
       public bool IsLocked { get; set; }
       public Vector2I ConnectedRoomPosition { get; set; }
   }
   ```

3. **GimmickDataクラス**の実装
   ```csharp
   public class GimmickData
   {
       public Vector2I Position { get; set; }
       public GimmickType Type { get; set; }
       public bool IsActive { get; set; }
   }
   ```

4. **列挙型の定義**
   ```csharp
   public enum RoomType { Start, Combat, Treasure, Boss, Secret }
   public enum DoorType { Normal, Locked, Secret }
   public enum GimmickType { HiddenPassage, LockedDoor, TreasureChest, Trap }
   ```

**完了条件**:
- [x] 全データクラスの実装完了
- [x] 基本的なプロパティの定義
- [x] 列挙型の定義完了

#### Day 2: インターフェースとユーティリティ

**目標**: システムの拡張性を確保するインターフェースとユーティリティを定義

**実装ファイル**:
```
Scripts/Systems/Dungeon/
├── Interfaces/
│   ├── ILevelGenerator.cs
│   └── IRoomConnector.cs
└── Utilities/
    ├── DungeonConstants.cs
    └── DungeonUtils.cs
```

**タスク詳細**:

1. **ILevelGeneratorインターフェース**の定義
   ```csharp
   public interface ILevelGenerator
   {
       Task<Dictionary<Vector2I, RoomData>> GenerateLevelAsync();
       void SetSeed(int seed);
       bool ValidateLevel(Dictionary<Vector2I, RoomData> rooms);
   }
   ```

2. **IRoomConnectorインターフェース**の定義
   ```csharp
   public interface IRoomConnector
   {
       void ConnectRooms(Dictionary<Vector2I, RoomData> rooms);
       bool ValidateConnections(Dictionary<Vector2I, RoomData> rooms);
       List<Vector2I> FindPath(Vector2I start, Vector2I end);
   }
   ```

3. **DungeonConstants**の定義
   ```csharp
   public static class DungeonConstants
   {
       public const int ROOM_SIZE = 16;
       public const int ROOM_COUNT = 8;
       public const int MAX_CONNECTION_ATTEMPTS = 100;
       public const float MIN_ROOM_DISTANCE = 16.0f;
   }
   ```

4. **DungeonUtils**の実装
   ```csharp
   public static class DungeonUtils
   {
       public static float CalculateDistance(Vector2I pos1, Vector2I pos2);
       public static Vector2I CalculateRoomPosition(Vector3 worldPosition);
       public static bool IsValidRoomPosition(Vector2I position);
   }
   ```

**完了条件**:
- [x] 全インターフェースの定義完了
- [x] ユーティリティクラスの実装完了
- [x] 定数の定義完了

### 3.2 Day 3-4: レベル生成モデル

#### Day 3: 基本生成ロジック

**目標**: レベル生成の主要ロジックを実装

**実装ファイル**:
```
Scripts/Systems/Dungeon/Models/
├── LevelGenerationModel.cs
└── RoomLayoutGenerator.cs
```

**タスク詳細**:

1. **LevelGenerationModel**の基本構造
   ```csharp
   public class LevelGenerationModel : ILevelGenerator
   {
       private readonly Dictionary<Vector2I, RoomData> _rooms;
       private readonly Random _random;
       private Vector2I _startRoomPosition;
       private Vector2I _bossRoomPosition;

       public async Task<Dictionary<Vector2I, RoomData>> GenerateLevelAsync();
       public void SetSeed(int seed);
       public bool ValidateLevel(Dictionary<Vector2I, RoomData> rooms);
   }
   ```

2. **部屋位置生成ロジック**
   ```csharp
   private Vector2I GenerateRoomPosition();
   private List<Vector2I> GenerateAvailablePositions();
   private bool IsPositionValid(Vector2I position);
   ```

3. **部屋タイプ割り当て**
   ```csharp
   private RoomType AssignRoomType(int roomIndex);
   private void AssignBossRoom();
   private void ValidateRoomTypeDistribution();
   ```

**完了条件**:
- [x] 基本生成ロジックの実装完了
- [x] 部屋位置生成の実装完了
- [x] 部屋タイプ割り当ての実装完了

#### Day 4: 部屋レイアウト生成

**目標**: 各部屋の内部レイアウトを生成

**実装ファイル**:
```
Scripts/Systems/Dungeon/Models/
├── RoomLayoutGenerator.cs
└── RoomTemplate.cs
```

**タスク詳細**:

1. **RoomLayoutGenerator**の実装
   ```csharp
   public class RoomLayoutGenerator
   {
       public void GenerateLayout(RoomData room);
       private void GenerateWalls(RoomData room);
       private void GenerateFloor(RoomData room);
       private void GenerateObstacles(RoomData room);
   }
   ```

2. **RoomTemplate**の実装
   ```csharp
   public class RoomTemplate
   {
       public RoomType Type { get; set; }
       public List<Vector2I> ObstaclePositions { get; set; }
       public List<Vector2I> DoorPositions { get; set; }
   }
   ```

3. **部屋生成の最適化**
   - メモリ効率の良い生成
   - パフォーマンスの考慮
   - キャッシュの活用

**完了条件**:
- [x] 部屋レイアウト生成の実装完了
   - [x] 壁の生成（外周を暗黙的な壁として扱う簡易実装）
   - [x] 床の生成（内部領域を床として扱う簡易実装）
   - [x] 障害物の生成
- [x] 部屋テンプレートの実装完了
- [ ] 最適化の実装完了（Week 1 スコープでは未着手。Week 2 以降でパフォーマンス計測後に判断）

### 3.3 Day 5: 部屋接続ロジック

#### Day 5: 接続システム

**目標**: 部屋間の接続ロジックを実装

**実装ファイル**:
```
Scripts/Systems/Dungeon/Models/
├── RoomConnectionModel.cs
└── ConnectionPathFinder.cs
```

**タスク詳細**:

1. **RoomConnectionModel**の実装
   ```csharp
   public class RoomConnectionModel : IRoomConnector
   {
       private readonly Dictionary<Vector2I, RoomData> _rooms;
       private readonly Random _random;

       public void ConnectRooms(Dictionary<Vector2I, RoomData> rooms);
       public bool ValidateConnections(Dictionary<Vector2I, RoomData> rooms);
       public List<Vector2I> FindPath(Vector2I start, Vector2I end);
   }
   ```

2. **最小全域木アルゴリズム**の実装
   ```csharp
   private void ConnectUsingMinimumSpanningTree();
   private List<(Vector2I, Vector2I)> CalculateMinimumSpanningTree();
   private float CalculateDistance(Vector2I room1, Vector2I room2);
   ```

3. **ConnectionPathFinder**の実装
   ```csharp
   public class ConnectionPathFinder
   {
       public Vector2I FindDoorPosition(Vector2I room1, Vector2I room2);
       public List<Vector2I> FindOptimalPath(Vector2I start, Vector2I end);
   }
   ```

4. **接続検証システム**
   ```csharp
   private bool ValidateAllRoomsConnected();
   private bool ValidateNoCycles();
   private bool ValidatePathExists(Vector2I start, Vector2I end);
   ```

**完了条件**:
- [x] 部屋接続ロジックの実装完了
- [x] 最小全域木アルゴリズムの実装完了
- [x] 接続検証システムの実装完了
- [x] パス探索の実装完了

### 3.4 Day 6-7: テストと統合

#### Day 6: 単体テスト

**目標**: 各コンポーネントの単体テストを実装

**テストファイル**:
```
Tests/Systems/Dungeon/
├── Models/
│   ├── LevelGenerationModelTests.cs
│   └── RoomConnectionModelTests.cs
├── Data/
│   └── RoomDataTests.cs
└── Utilities/
    └── DungeonUtilsTests.cs
```

**タスク詳細**:

1. **LevelGenerationModelTests**
   ```csharp
   [Test]
   public void GenerateLevel_CreatesCorrectNumberOfRooms();
   [Test]
   public void GenerateLevel_AllRoomsHaveValidTypes();
   [Test]
   public void GenerateLevel_StartRoomIsAtOrigin();
   [Test]
   public void GenerateLevel_WithSameSeed_GeneratesSameLayout();
   ```

2. **RoomConnectionModelTests**
   ```csharp
   [Test]
   public void ConnectRooms_AllRoomsAreConnected();
   [Test]
   public void ConnectRooms_NoCyclesExist();
   [Test]
   public void ConnectRooms_PathExistsFromStartToBoss();
   [Test]
   public void ValidateConnections_ValidLevel_ReturnsTrue();
   ```

3. **RoomDataTests**
   ```csharp
   [Test]
   public void RoomData_Properties_SetAndGetCorrectly();
   [Test]
   public void RoomData_AddDoor_AddsDoorToList();
   [Test]
   public void RoomData_AddGimmick_AddsGimmickToList();
   ```

**完了条件**:
- [x] 全クラスの単体テスト実装完了
- [ ] テストカバレッジ80%以上達成（カバレッジ計測ツールは未実行。139件のテストで主要ロジック・性質ベーステストは網羅）
- [x] エラーハンドリングのテスト完了（異常系: 空Dictionary、部屋1個、null Random 等）

#### Day 7: 統合テストとドキュメント

**目標**: システム全体の統合テストとドキュメント作成

**テストファイル**:
```
Tests/Systems/Dungeon/Integration/
└── LevelGenerationIntegrationTests.cs
```

**タスク詳細**:

1. **統合テスト**の実装
   ```csharp
   [Test]
   public void FullLevelGeneration_WithConnection_WorksCorrectly();
   [Test]
   public void LevelGeneration_Performance_WithinTimeLimit();
   [Test]
   public void LevelGeneration_MemoryUsage_WithinLimit();
   [Test]
   public void LevelGeneration_WithInvalidParameters_HandlesGracefully();
   ```

2. **パフォーマンステスト**
   ```csharp
   [Test]
   public void LevelGeneration_Performance_8RoomsUnder1Second();
   [Test]
   public void LevelGeneration_MemoryUsage_Under100MB();
   ```

3. **ドキュメント**の更新
   - API仕様書の作成
   - 使用例の追加
   - トラブルシューティングガイド

**完了条件**:
- [x] 統合テストの実装完了（LevelGenerationModelTests に統合シナリオを含む。専用の Integration/ ディレクトリは未作成）
- [ ] パフォーマンステストの実装完了（未着手。Week 2 以降、TileMap/ViewModel 統合時に計測予定）
- [x] ドキュメントの更新完了（本計画書のチェックリスト更新のみ。API仕様書等の別ドキュメントは未作成）

## 4. 技術仕様

### 4.1 アーキテクチャ設計

#### 4.1.1 レイヤー構成
```
ViewModel Layer (Week 2)
    ↓
Model Layer (Week 1)
    ↓
Data Layer (Week 1)
    ↓
Utility Layer (Week 1)
```

#### 4.1.2 依存関係
- **Data Layer**: 他のレイヤーに依存しない
- **Utility Layer**: Data Layerに依存
- **Model Layer**: Data Layer, Utility Layerに依存
- **ViewModel Layer**: 全レイヤーに依存（Week 2）

### 4.2 パフォーマンス要件

#### 4.2.1 時間要件
- **レベル生成**: 1秒以内（8部屋）
- **部屋接続**: 100ms以内
- **レイアウト生成**: 50ms以内（1部屋）

#### 4.2.2 メモリ要件
- **最大メモリ使用量**: 100MB以下
- **部屋データ**: 1部屋あたり1KB以下
- **接続データ**: 全接続で10KB以下

### 4.3 品質要件

#### 4.3.1 コード品質
- **コーディング規約**: 既存プロジェクトの規約に準拠
- **コメント**: 日本語で記述、主要メソッドに必須
- **命名規則**: 既存システムと統一

#### 4.3.2 テスト品質
- **テストカバレッジ**: 80%以上
- **単体テスト**: 全公開メソッド
- **統合テスト**: 主要フロー

## 5. 成果物定義

### 5.1 必須成果物

#### 5.1.1 ソースコード
- [x] `Scripts/Systems/Dungeon/Data/` ディレクトリ
  - [x] `RoomData.cs`
  - [x] `DoorData.cs`
  - [x] `GimmickData.cs`
  - [x] `Enums/DungeonEnums.cs`

- [x] `Scripts/Systems/Dungeon/Interfaces/` ディレクトリ
  - [x] `ILevelGenerator.cs`
  - [x] `IRoomConnector.cs`

- [x] `Scripts/Systems/Dungeon/Utilities/` ディレクトリ
  - [x] `DungeonConstants.cs`
  - [x] `DungeonUtils.cs`

- [x] `Scripts/Systems/Dungeon/Models/` ディレクトリ
  - [x] `LevelGenerationModel.cs`
  - [x] `RoomConnectionModel.cs`
  - [x] `RoomLayoutGenerator.cs`
  - [x] `ConnectionPathFinder.cs`
  - [x] `RoomTemplate.cs`

#### 5.1.2 テストコード

> **注記**: 実際の配置は `Tests/Systems/Dungeon/` ではなく `Tests/Core/Dungeon/`（既存の CoreTests.csproj 配下の規約に統一）。

- [x] `Tests/Core/Dungeon/` ディレクトリ
  - [x] `LevelGenerationModelTests.cs`
  - [x] `RoomConnectionModelTests.cs`
  - [x] `RoomDataTests.cs`
  - [x] `DungeonUtilsTests.cs`
  - [x] `RoomLayoutGeneratorTests.cs`
  - [x] `ConnectionPathFinderTests.cs`
- [ ] 専用の `Integration/` ディレクトリ分割（統合シナリオは `LevelGenerationModelTests.cs` に含む形で代替）

#### 5.1.3 ドキュメント
- [ ] API仕様書（未作成。各クラスの XML ドキュメントコメントで代替）
- [ ] 使用例（未作成）
- [ ] トラブルシューティングガイド（未作成）

### 5.2 品質基準

#### 5.2.1 機能要件
- [x] 8部屋の生成が正常に動作
- [x] 全部屋が接続されている
- [x] 開始部屋からボス部屋への経路が存在
- [x] 部屋タイプが適切に割り当てられている

#### 5.2.2 非機能要件
- [ ] パフォーマンス要件を満たす（計測未実施）
- [ ] メモリ使用量が制限内（計測未実施）
- [x] エラーハンドリングが適切
- [ ] ログ出力が適切（Logger 未統合。Week 2 以降で検討）

## 6. テスト戦略

### 6.1 テストピラミッド

#### 6.1.1 単体テスト（80%）
- **対象**: 各クラスの個別メソッド
- **ツール**: NUnit
- **カバレッジ**: 80%以上

#### 6.1.2 統合テスト（15%）
- **対象**: クラス間の連携
- **ツール**: NUnit
- **カバレッジ**: 主要フロー

#### 6.1.3 パフォーマンステスト（5%）
- **対象**: 性能要件の検証
- **ツール**: NUnit + カスタム測定
- **カバレッジ**: 時間・メモリ要件

### 6.2 テストケース設計

#### 6.2.1 正常系テスト
- 基本的なレベル生成
- 部屋接続の検証
- パフォーマンス要件の確認

#### 6.2.2 異常系テスト
- 不正なパラメータ
- メモリ不足
- 接続失敗

#### 6.2.3 境界値テスト
- 最小部屋数
- 最大部屋数
- 極端なシード値

### 6.3 テスト実行

#### 6.3.1 自動実行
- **CI/CD**: GitHub Actions
- **頻度**: プルリクエスト時
- **環境**: Windows, Linux

#### 6.3.2 手動実行
- **頻度**: 実装完了時
- **環境**: 開発環境
- **確認項目**: 視覚的な確認

## 7. リスク管理

### 7.1 技術的リスク

#### 7.1.1 複雑な接続ロジック
**リスク**: 最小全域木アルゴリズムの実装が困難
**影響度**: 高
**発生確率**: 中
**対策**:
- 段階的実装（簡単な接続から開始）
- 既存ライブラリの調査
- 外部専門家への相談

#### 7.1.2 パフォーマンス問題
**リスク**: 8部屋生成が1秒を超える
**影響度**: 中
**発生確率**: 中
**対策**:
- 早期のプロファイリング
- アルゴリズムの最適化
- キャッシュの活用

#### 7.1.3 メモリリーク
**リスク**: 部屋データの適切な管理が困難
**影響度**: 中
**発生確率**: 低
**対策**:
- 適切なリソース解放の実装
- メモリ監視の実装
- ガベージコレクションの確認

### 7.2 プロジェクトリスク

#### 7.2.1 スケジュール遅延
**リスク**: 7日間で完了しない
**影響度**: 高
**発生確率**: 中
**対策**:
- 日次進捗確認
- 優先度の調整
- スコープの見直し

#### 7.2.2 品質問題
**リスク**: テストカバレッジ80%未達
**影響度**: 中
**発生確率**: 低
**対策**:
- テストファースト開発
- 継続的なテスト実行
- コードレビューの強化

### 7.3 リスク対応計画

#### 7.3.1 早期警告指標
- 日次進捗が80%未満
- テスト失敗率が20%以上
- パフォーマンス要件未達

#### 7.3.2 エスカレーション
- 2日連続で進捗遅延
- 重大なバグの発生
- 技術的ブロッカーの発生

## 8. 依存関係

### 8.1 外部依存

#### 8.1.1 Godot Engine
- **バージョン**: 4.4
- **用途**: Vector2I, Random等の基本クラス
- **制約**: なし

#### 8.1.2 .NET
- **バージョン**: 8.0
- **用途**: 基本ライブラリ
- **制約**: なし

### 8.2 内部依存

#### 8.2.1 既存システム
- **Core/Reactive**: ReactivePropertyの使用
- **Core/Events**: イベントシステムの利用
- **Core/ViewModels**: ViewModelBaseの継承（Week 2）

#### 8.2.2 開発環境
- **テストフレームワーク**: NUnit
- **ビルドシステム**: MSBuild
- **バージョン管理**: Git

### 8.3 依存関係の管理

#### 8.3.1 バージョン管理
- 依存関係のバージョンを固定
- 定期的な更新確認
- 互換性の検証

#### 8.3.2 分離原則
- 外部依存の最小化
- インターフェースによる抽象化
- モックによるテスト分離

## 9. 制限事項

### 9.1 技術的制限
- 部屋数は8個固定（拡張不可）
- 部屋サイズは16×16タイル固定
- 最小全域木アルゴリズムのみ使用

### 9.2 スコープ制限
- ギミック配置システムは対象外（Week 2）
- ナビゲーションシステムは対象外（Week 2）
- タイルマップシステムは対象外（Week 2）
- ViewModel層は対象外（Week 2）

### 9.3 パフォーマンス制限
- レベル生成は1秒以内に完了
- メモリ使用量は100MB以下
- 部屋接続は100ms以内に完了

## 10. 変更履歴

| バージョン | 更新日     | 変更内容 |
| ---------- | ---------- | -------- |
| 0.2.0      | 2026-07-17 | Day 1-6 実装完了を反映<br>- Data/Interfaces/Utilities/Models 全クラス実装、`dotnet test` 139件全pass<br>- テストは `Tests/Systems/Dungeon/` ではなく `Tests/Core/Dungeon/` に配置（既存規約に統一）<br>- 未実施: テストカバレッジ計測、パフォーマンス/メモリ計測、API仕様書等の別ドキュメント作成、専用Integrationディレクトリ分割 |
| 0.1.0      | 2025-06-20 | 初版作成<br>- Week 1基盤実装計画<br>- データ構造・モデル・インターフェース・ユーティリティ層の実装<br>- 7日間の日別実装スケジュール<br>- テスト戦略とリスク管理 |

