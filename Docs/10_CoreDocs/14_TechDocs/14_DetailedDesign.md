---
title: 14_DetailedDesign
version: 0.1
status: draft
updated: 2025-05-29
tags:
  - DetailedDesign
  - Document
linked_docs:
  - 11.1_Plan
  - 14.1_Requirement
---

# 詳細設計書 – Shrine of the Lost Ones (仮)

## 目次
1. [はじめに](#はじめに)
2. [システムアーキテクチャ概要](#システムアーキテクチャ概要)
3. [クラス図](#クラス図)
4. [アクティビティ図 ― ゲームプレイループ](#アクティビティ図-ゲームプレイループ)
5. [シーケンス図 ― プレイヤー攻撃フロー](#シーケンス図-プレイヤー攻撃フロー)
6. [モジュール構成図](#モジュール構成図)
7. [設計原則とガイドライン](#設計原則とガイドライン)
8. [変更履歴](#変更履歴)

## はじめに
本書は、個人開発タイトル **“Shrine of the Lost Ones”** の詳細設計をまとめたものである。要件定義書 (14.1_Requirement) で定義された機能要求・非機能要求を満たすため、Godot 4.x 上に構築する各サブシステム（入力、アクション、AI、メタ進行など）の責務と依存関係を明示する。

## システムアーキテクチャ概要
- **GameSystems (AutoLoad)** を起点に `EventBus` / `EffectBus` / `Telemetry` を横断的に共有  
- プレイヤー・敵ともに **StateMachine + StatBlock** 構成を持ち、`EventBus` 経由で緩く結合  
- KPI 計測は **Telemetry** が一元記録し、CI( GitHub Actions ) による可視化を想定  

## クラス図

```mermaid
classDiagram
    %% ==== Singleton Systems (AutoLoad) ====
    %% AutoLoad Singleton
    class GameSystems
    GameSystems : +EventBus
    GameSystems : +EffectBus
    GameSystems : +Telemetry

    %% Signal Hubs
    class EventBus
    class EffectBus
    class Telemetry

    %% ==== Core Gameplay ====
    class Player
    class InputRingBuffer
    class ActionStateMachine
    class StatBlock
    class Modifier
    class AbilityResource
    class AbilityRunner
    class Enemy
    class AIController
    class Blackboard
    class BTNode

    %% ==== Data / Persistence ====
    class SaveDataManager
    %% Godot JSON Resource
    class JSONData
    %% Godot .tres Resource
    class TRESResource

    %% ==== Relationships ====
    Player "1" -- "1" InputRingBuffer : captures
    InputRingBuffer --> ActionStateMachine : feed(bufferedInput)
    ActionStateMachine --> AbilityRunner : trigger(abilityId)
    AbilityRunner --> EffectBus : notify(hitEvent)
    EffectBus --> Player : subscribe(playVFX/SE)

    AbilityRunner --> EventBus : notify(damageEvent)
    EventBus --> StatBlock : subscribe(applyDamage)

    StatBlock --> Modifier : uses

    Enemy -- AIController
    AIController --> Blackboard
    AIController --> BTNode
    AIController --> EventBus : notify(aiEvent)

    SaveDataManager --> JSONData
    SaveDataManager --> TRESResource
    SaveDataManager --> Telemetry : recordMetrics()
    Telemetry --> EventBus : subscribe(metricEvent)

    %% ==== Dependencies on GameSystems ====
    Player --> GameSystems : depends
    Enemy --> GameSystems : depends
    InputRingBuffer --> GameSystems : readSettings()
    AbilityRunner --> GameSystems : depends
```
### 変更概要
| 修正項目                      | 内容                                                                                               |
| ----------------------------- | -------------------------------------------------------------------------------------------------- |
| **GameSystems 追加**          | AutoLoad シングルトンに主要ハブ (EventBus・EffectBus・Telemetry) を集約                         |
| **EventBus / EffectBus 分離** | 演出系通知とロジック系通知を分離し、責務境界を明示                                               |
| **命名規約統一**              | PascalCase に統一 (例: `BTNode`)                                                                  |
| **矢印ラベル追加**            | `notify` / `subscribe` など動詞で関係を明確化                                                     |
| **データリソース表記**        | `.json` や `.tres` をステレオタイプ表示                                                           |

## アクティビティ図 ― ゲームプレイループ
```mermaid
flowchart TD
    Start([Game Start])
    Init[Load Engine and SaveData]
    Title[Title Menu]
    Start --> Init --> Title
    Title -->|Start Run| Gen["Generate Dungeon"]
    Gen --> Room[Enter Room]

    subgraph Runtime_Loop
        direction TB
        Room --> Input[Player Input]
        Input --> Action[Handle Action State]
        Action --> Hit[Resolve Combat]
        Hit --> Stat[Update Stats]
        Hit --> Effects[Trigger Effects]
        Effects --> Telemetry[Log KPI Metrics]
        Stat --> DeadCheck{Is Player Dead?}
        DeadCheck -->|Yes| Retry[Respawn / Retry]
        Retry --> Room
        DeadCheck -->|No| AI[Process Enemy AI]
        AI --> Hit
        Hit --> ClearCheck{Room Cleared?}
        ClearCheck -->|No| Input
        ClearCheck -->|Yes| NextRoom[Move to Next Room]
        NextRoom --> Room
    end

    NextRoom --> ClearBoss{Is Boss Defeated?}
    ClearBoss -->|No| Gen
    ClearBoss -->|Yes| Meta["Save Progress and Upgrade"]
    Meta --> Title
```

## シーケンス図 ― プレイヤー攻撃フロー
攻撃入力からダメージ適用・演出・KPI 記録までのフロー。

```mermaid
sequenceDiagram
    participant Player
    participant InputBuffer
    participant ActionSM as ActionStateMachine
    participant Enemy
    participant StatBlock
    participant EffectBus
    participant Telemetry
    participant UIHUD

    Player->>InputBuffer: press Attack
    InputBuffer->>ActionSM: buffered input
    ActionSM->>Enemy: call dealDamage
    Enemy->>StatBlock: applyDamage
    StatBlock-->>Enemy: HP updated
    ActionSM->>EffectBus: emit Hit Light
    EffectBus->>Player: hitStop & SE
    EffectBus->>UIHUD: cameraShake
    ActionSM->>Telemetry: log attack metrics
    Telemetry->>FileSystem: append CSV
```

## モジュール構成図
Godot プロジェクトをモジュール単位で俯瞰する。

```mermaid
graph TD
    %% Player Control
    Input["Input System"]
    Action["Action State Machine"]
    Stats["StatBlock / Modifier"]
    Ability["Ability Runner"]
    EffectBus["Effect Bus"]
    Telemetry["Telemetry Logger"]
    EventBus["Event Bus"]

    %% Enemy & AI
    AI["Enemy AI System"]
    BT["Behavior Tree"]
    Blackboard["AI Blackboard"]

    %% Gameplay Systems
    Dungeon["Room Generator"]
    Save["SaveData Manager"]
    Meta["Meta Progress System"]
    UI["UI Widgets / Theme"]
    HUD["HUD / KPI Display"]

    %% Dependencies
    Input --> Action
    Action --> Stats
    Action --> Ability
    Ability --> Stats
    Ability --> EffectBus
    Action --> EffectBus
    EffectBus --> Telemetry
    EffectBus --> HUD
    Stats --> HUD

    AI --> BT
    BT --> Blackboard
    AI --> EffectBus
    AI --> Stats

    Dungeon --> Save
    Meta --> Save
    Meta --> Stats
    Meta --> UI

    HUD --> UI
    Telemetry --> Save
    Action --> EventBus
    AI --> EventBus
    Telemetry --> EventBus
```

## 設計原則とガイドライン
- **疎結合 / 高凝集**：Signal ベースの通知と `Resource` データ駆動化で依存関係を最小化  
- **データ & 振る舞い分離**：`StatBlock` + `Modifier` で計算ロジックを統一  
- **テスト容易性**：`Bot` 操作によるインテグレーションテストを前提に、すべてのサブシステムが Headless 実行をサポート  
- **パフォーマンス最適化**：`NavigationServer` / Multi-threaded **PhysicsServer** を有効活用し 60 fps を保証  

## 変更履歴
| バージョン | 更新日       | 変更内容                 |
| --------- | ----------- | ------------------------ |
| 0.1       | 2025-05-29 | 初版作成（設計書の統合） |
