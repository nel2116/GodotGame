---
title: レベル生成システム実装詳細
version: 0.2.0
status: draft
updated: 2025-06-01
tags:
    - Architecture
    - MVVM
    - Reactive
    - System
    - Implementation
linked_docs:
    - "[[12_03_detailed_design|MVVM+RX詳細設計書]]"
    - "[[12_01_mvvm_rx_architecture|MVVM+RXアーキテクチャ]]"
    - "[[12_03_detailed_design/01_core_components/02_viewmodel_base|ViewModelBase実装詳細]]"
    - "[[12_03_detailed_design/01_core_components/01_reactive_property|ReactiveProperty実装詳細]]"
    - "[[12_03_detailed_design/01_core_components/03_composite_disposable|CompositeDisposable実装詳細]]"
    - "[[12_03_detailed_design/01_core_components/04_event_bus|イベントバス実装詳細]]"
    - "[[11_10_prototype_guidelines|プロトタイプ制作要素まとめ]]"
---

# レベル生成システム実装詳細

## 1. 概要

### 1.1 目的

本ドキュメントは、MVVM + リアクティブプログラミングにおけるレベル生成システムの実装詳細を定義し、以下の目的を達成することを目指します：

-   16×16 タイル部屋のランダム生成
-   8 つの部屋の接続ロジック
-   隠し通路と鍵扉のギミック実装
-   ナビゲーションメッシュの自動生成

### 1.2 適用範囲

-   部屋生成
-   通路生成
-   ギミック配置
-   ナビゲーション管理

## 2. クラス図

```mermaid
classDiagram
    class RoomGenerator {
        -Dictionary~Vector2I, Room~ _rooms
        -Random _random
        +GenerateLevel() void
        +ConnectRooms() void
        +PlaceGimmicks() void
    }

    class Room {
        -Vector2I _position
        -Vector2I _size
        -List~Door~ _doors
        -List~Gimmick~ _gimmicks
        +GenerateLayout() void
        +AddDoor(Door) void
        +AddGimmick(Gimmick) void
    }

    class Door {
        -Vector2I _position
        -DoorType _type
        -bool _isLocked
        +Unlock() void
    }

    class Gimmick {
        -Vector2I _position
        -GimmickType _type
        -bool _isActive
        +Activate() void
    }

    class NavigationManager {
        -NavigationServer _server
        -Dictionary~Vector2I, NavigationMesh~ _meshes
        +GenerateNavigation() void
        +UpdateNavigation(Vector2I) void
    }

    class TileMapManager {
        -TileMap _tileMap
        -Dictionary~Vector2I, TileSet~ _tileSets
        +PlaceTiles(Room) void
        +UpdateTiles(Vector2I) void
    }

    RoomGenerator --> Room : generates
    Room --> Door : contains
    Room --> Gimmick : contains
    RoomGenerator --> NavigationManager : uses
    RoomGenerator --> TileMapManager : uses
```

## 3. シーケンス図

```mermaid
sequenceDiagram
    participant RG as RoomGenerator
    participant R as Room
    participant TM as TileMapManager
    participant NM as NavigationManager
    participant G as Gimmick

    RG->>RG: GenerateLevel()
    RG->>R: GenerateLayout()
    R->>TM: PlaceTiles()
    RG->>RG: ConnectRooms()
    RG->>RG: PlaceGimmicks()
    RG->>NM: GenerateNavigation()
    G->>TM: UpdateTiles()
    G->>NM: UpdateNavigation()
```

## 4. 実装詳細

### 4.1 部屋生成

```csharp
public class RoomGenerator
{
    private readonly Dictionary<Vector2I, Room> _rooms = new();
    private readonly Random _random = new();
    private const int ROOM_SIZE = 16;
    private const int ROOM_COUNT = 8;

    public void GenerateLevel()
    {
        // 部屋の生成
        for (int i = 0; i < ROOM_COUNT; i++)
        {
            var position = GenerateRoomPosition();
            var room = new Room(position, new Vector2I(ROOM_SIZE, ROOM_SIZE));
            _rooms[position] = room;
            room.GenerateLayout();
        }

        // 部屋の接続
        ConnectRooms();

        // ギミックの配置
        PlaceGimmicks();

        // ナビゲーションメッシュの生成
        NavigationManager.GenerateNavigation();
    }

    private Vector2I GenerateRoomPosition()
    {
        // 部屋の位置を生成（重複を避ける）
        Vector2I position;
        do
        {
            position = new Vector2I(
                _random.Next(-3, 4) * ROOM_SIZE,
                _random.Next(-3, 4) * ROOM_SIZE
            );
        } while (_rooms.ContainsKey(position));

        return position;
    }
}
```

### 4.2 部屋クラス

```csharp
public class Room
{
    private readonly Vector2I _position;
    private readonly Vector2I _size;
    private readonly List<Door> _doors = new();
    private readonly List<Gimmick> _gimmicks = new();

    public Room(Vector2I position, Vector2I size)
    {
        _position = position;
        _size = size;
    }

    public void GenerateLayout()
    {
        // 部屋の基本レイアウトを生成
        GenerateWalls();
        GenerateFloor();
        GenerateObstacles();
    }

    public void AddDoor(Door door)
    {
        _doors.Add(door);
    }

    public void AddGimmick(Gimmick gimmick)
    {
        _gimmicks.Add(gimmick);
    }
}
```

### 4.3 ナビゲーション管理

```csharp
public class NavigationManager
{
    private readonly NavigationServer _server;
    private readonly Dictionary<Vector2I, NavigationMesh> _meshes = new();

    public void GenerateNavigation()
    {
        foreach (var room in _rooms.Values)
        {
            var mesh = new NavigationMesh();
            mesh.GenerateFromTileMap(room.TileMap);
            _meshes[room.Position] = mesh;
        }
    }

    public void UpdateNavigation(Vector2I roomPosition)
    {
        if (_meshes.TryGetValue(roomPosition, out var mesh))
        {
            mesh.Update();
        }
    }
}
```

### 4.4 タイルマップ管理

```csharp
public class TileMapManager
{
    private readonly TileMap _tileMap;
    private readonly Dictionary<Vector2I, TileSet> _tileSets = new();

    public void PlaceTiles(Room room)
    {
        var position = room.Position;
        var tiles = room.GenerateTiles();

        foreach (var tile in tiles)
        {
            _tileMap.SetCell(
                position + tile.Position,
                tile.TileSet,
                tile.SourceId,
                tile.AtlasCoords
            );
        }
    }

    public void UpdateTiles(Vector2I position)
    {
        // タイルの更新（ギミックの状態変更時など）
        var room = GetRoomAtPosition(position);
        if (room != null)
        {
            PlaceTiles(room);
        }
    }
}
```

## 5. パフォーマンス最適化

### 5.1 メモリ管理

-   部屋の遅延生成
-   不要な部屋のアンロード
-   タイルセットのキャッシュ

### 5.2 更新最適化

-   ナビゲーションメッシュの部分更新
-   タイルの差分更新
-   視界外の部屋の更新スキップ

## 6. テスト戦略

### 6.1 単体テスト

```csharp
[Test]
public void RoomGenerator_GenerateLevel_CreatesCorrectNumberOfRooms()
{
    var generator = new RoomGenerator();
    generator.GenerateLevel();

    Assert.AreEqual(8, generator.RoomCount);
}

[Test]
public void Room_GenerateLayout_CreatesValidLayout()
{
    var room = new Room(Vector2I.Zero, new Vector2I(16, 16));
    room.GenerateLayout();

    Assert.IsTrue(room.HasValidLayout());
}
```

### 6.2 統合テスト

```csharp
[Test]
public void LevelGeneration_WithGimmicks_PlacesCorrectly()
{
    var generator = new RoomGenerator();
    generator.GenerateLevel();

    var gimmickCount = generator.GetGimmickCount();
    Assert.AreEqual(2, gimmickCount); // 隠し通路と鍵扉
}
```

## 7. 変更履歴

| バージョン | 更新日     | 変更内容                                                                                                     |
| ---------- | ---------- | ------------------------------------------------------------------------------------------------------------ |
| 0.2.0      | 2025-06-01 | ドキュメント管理ルールに準拠した更新<br>- メタデータの更新<br>- バージョン管理の改善<br>- 変更履歴の形式統一 |
| 0.1.0      | 2024-03-21 | 初版作成                                                                                                     |
