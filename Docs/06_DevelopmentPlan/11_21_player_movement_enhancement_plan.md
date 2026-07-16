---
title: プレイヤー移動システム強化実装計画
version: 0.1.0
status: draft
updated: 2025-06-20
tags:
    - Implementation
    - Plan
    - Player
    - Movement
    - Enhancement
    - DevelopmentPlan
linked_docs:
    - "[[11_09_core_experience|コア体験仕様]]"
    - "[[11_11_player_systems|プレイヤー基盤詳細]]"
    - "[[mvp_definition|MVP定義]]"
    - "[[11_16_core_implementation|コアシステム実装計画]]"
    - "[[02_movement_system|プレイヤー移動システム実装詳細]]"
---

# プレイヤー移動システム強化実装計画

## 目次

1. [概要](#概要)
2. [短期改善（MVP対応）](#短期改善mvp対応)
3. [中期改善（アルファ対応）](#中期改善アルファ対応)
4. [実装スケジュール](#実装スケジュール)
5. [テスト戦略](#テスト戦略)
6. [リスク管理](#リスク管理)
7. [成果物定義](#成果物定義)
8. [制限事項](#制限事項)
9. [変更履歴](#変更履歴)

## 概要

### 1.1 目的

現在のプレイヤー移動システムを企画ドキュメントの詳細仕様に準拠するよう強化し、以下の要素を段階的に実装する：

- フレーム単位の状態管理
- 基本的なキャンセルシステム
- 入力バッファリング基盤
- 詳細なアクション仕様
- 無敵時間システム
- パフォーマンス監視

### 1.2 背景

現在の実装は企画の基盤部分を正確に実装しているが、以下の要素が不足している：

- フレーム単位の詳細なタイミング制御
- アクション間のキャンセルシステム
- 入力バッファリングによる先行入力
- 企画仕様で定義された無敵時間
- パフォーマンスKPIの監視

### 1.3 適用範囲

- プレイヤー移動システム
- プレイヤー戦闘システム
- 入力処理システム
- パフォーマンス監視システム

## 短期改善（MVP対応）

### 2.1 Phase 1: フレーム単位の状態管理実装

#### 2.1.1 フレーム管理システムの基盤構築

**実装ファイル**:
```
Scripts/Systems/Player/State/
├── FrameStateManager.cs
├── ActionFrameData.cs
└── FrameCounter.cs
```

**実装詳細**:

```csharp
// FrameStateManager.cs
public class FrameStateManager
{
    private int _currentFrame = 0;
    private readonly Dictionary<string, ActionFrameData> _actionFrames = new();
    private readonly IGameEventBus _eventBus;

    public void UpdateFrame()
    {
        _currentFrame++;
        UpdateActionStates();
    }

    public void StartAction(string actionName, int totalFrames, int startupFrames, int activeFrames, int recoveryFrames)
    {
        var frameData = new ActionFrameData
        {
            ActionName = actionName,
            StartFrame = _currentFrame,
            TotalFrames = totalFrames,
            StartupFrames = startupFrames,
            ActiveFrames = activeFrames,
            RecoveryFrames = recoveryFrames
        };
        _actionFrames[actionName] = frameData;
    }

    public bool IsActionActive(string actionName)
    {
        return _actionFrames.ContainsKey(actionName) &&
               _actionFrames[actionName].IsActive(_currentFrame);
    }

    public bool CanCancelAction(string actionName)
    {
        return _actionFrames.ContainsKey(actionName) &&
               _actionFrames[actionName].IsInCancelWindow(_currentFrame);
    }
}
```

#### 2.1.2 プレイヤー状態マシンの拡張

**実装ファイル**:
```
Scripts/Systems/Player/State/
├── PlayerStateMachine.cs
├── PlayerActionState.cs
└── StateTransitionRule.cs
```

**完了条件**:
- [ ] フレーム単位の状態管理が正常に動作
- [ ] アクションの開始・終了が正確に制御される
- [ ] 状態遷移が適切に処理される

### 2.2 Phase 2: 基本的なキャンセルシステム実装

#### 2.2.1 キャンセルルール定義

**実装ファイル**:
```
Scripts/Systems/Player/Combat/
├── CancelRuleManager.cs
├── CancelRule.cs
└── CancelPriority.cs
```

**実装詳細**:

```csharp
// CancelRule.cs
public class CancelRule
{
    public string FromAction { get; set; }
    public int CancelStartFrame { get; set; }
    public int CancelEndFrame { get; set; }
    public List<string> AllowedTargetActions { get; set; }
    public CancelPriority Priority { get; set; }
}

// CancelRuleManager.cs
public class CancelRuleManager
{
    private readonly List<CancelRule> _cancelRules = new();
    private readonly FrameStateManager _frameManager;

    public void InitializeRules()
    {
        // 企画ドキュメントに基づくキャンセルルール
        _cancelRules.Add(new CancelRule
        {
            FromAction = "Attack_L1",
            CancelStartFrame = 14,
            CancelEndFrame = 20,
            AllowedTargetActions = new List<string> { "Dash", "Attack_L2" },
            Priority = CancelPriority.High
        });
    }
}
```

#### 2.2.2 アクション実行システムの統合

**実装ファイル**:
```
Scripts/Systems/Player/Combat/
├── ActionExecutionManager.cs
└── ActionQueue.cs
```

**完了条件**:
- [ ] キャンセルルールが正常に動作
- [ ] アクション間の遷移がスムーズに実行される
- [ ] 優先度に基づく処理が正しく動作

### 2.3 Phase 3: 入力バッファリング基盤構築

#### 2.3.1 入力バッファシステム

**実装ファイル**:
```
Scripts/Systems/Player/Input/
├── InputBuffer.cs
├── InputRingBuffer.cs
└── InputProcessor.cs
```

**実装詳細**:

```csharp
// InputRingBuffer.cs
public class InputRingBuffer<T>
{
    private readonly T[] _buffer;
    private int _head = 0;
    private int _tail = 0;
    private int _count = 0;
    private readonly int _capacity;

    public InputRingBuffer(int capacity = 12) // 企画仕様: 12フレーム
    {
        _capacity = capacity;
        _buffer = new T[capacity];
    }

    public void Enqueue(T item)
    {
        if (_count < _capacity)
        {
            _buffer[_tail] = item;
            _tail = (_tail + 1) % _capacity;
            _count++;
        }
        else
        {
            // バッファが満杯の場合、古い入力を上書き
            _buffer[_head] = item;
            _head = (_head + 1) % _capacity;
            _tail = (_tail + 1) % _capacity;
        }
    }
}
```

#### 2.3.2 入力処理システムの統合

**実装ファイル**:
```
Scripts/Systems/Player/Input/
├── EnhancedInputProcessor.cs
└── InputPriorityManager.cs
```

**完了条件**:
- [ ] 入力バッファリングが正常に動作
- [ ] 先行入力が適切に処理される
- [ ] 入力優先度が正しく適用される

## 中期改善（アルファ対応）

### 3.1 Phase 4: 詳細なアクション仕様実装

#### 3.1.1 アクション仕様データシステム

**実装ファイル**:
```
Scripts/Systems/Player/Combat/
├── ActionSpecification.cs
├── ActionSpecificationManager.cs
└── ActionSpecificationData.cs
```

**実装詳細**:

```csharp
// ActionSpecification.cs
public class ActionSpecification
{
    public string ActionName { get; set; }
    public int TotalFrames { get; set; }
    public int StartupFrames { get; set; }
    public int ActiveFrames { get; set; }
    public int RecoveryFrames { get; set; }
    public int InvincibilityStartFrame { get; set; }
    public int InvincibilityEndFrame { get; set; }
    public float MovementDistance { get; set; }
    public float AirControlRate { get; set; }
    public List<string> CancelableTo { get; set; }
    public CancelPriority Priority { get; set; }
}
```

#### 3.1.2 アクション実行エンジンの拡張

**実装ファイル**:
```
Scripts/Systems/Player/Combat/
├── AdvancedActionEngine.cs
├── ActionAnimationController.cs
└── ActionEffectManager.cs
```

**完了条件**:
- [ ] 企画仕様通りのアクションが実行される
- [ ] フレーム単位の制御が正確に動作
- [ ] 移動距離と空中制御が適切に処理される

### 3.2 Phase 5: 無敵時間システム実装

#### 3.2.1 無敵時間管理システム

**実装ファイル**:
```
Scripts/Systems/Player/Combat/
├── InvincibilityManager.cs
├── InvincibilityFrame.cs
└── DamageCollisionSystem.cs
```

**実装詳細**:

```csharp
// InvincibilityManager.cs
public class InvincibilityManager
{
    private readonly List<InvincibilityFrame> _activeInvincibilityFrames = new();
    private readonly FrameStateManager _frameManager;
    private readonly IGameEventBus _eventBus;

    public void AddInvincibilityFrame(string actionName, int startFrame, int endFrame)
    {
        var invincibilityFrame = new InvincibilityFrame
        {
            ActionName = actionName,
            StartFrame = startFrame,
            EndFrame = endFrame
        };
        _activeInvincibilityFrames.Add(invincibilityFrame);
    }

    public bool IsInvincible(int currentFrame)
    {
        return _activeInvincibilityFrames.Any(frame =>
            currentFrame >= frame.StartFrame && currentFrame <= frame.EndFrame);
    }
}
```

**完了条件**:
- [ ] 無敵時間が正確に動作
- [ ] ダメージ判定が適切に処理される
- [ ] 無敵時間の開始・終了が正しく制御される

### 3.3 Phase 6: パフォーマンス監視実装

#### 3.3.1 パフォーマンス監視システム

**実装ファイル**:
```
Scripts/Systems/Performance/
├── PerformanceMonitor.cs
├── FrameTimeTracker.cs
└── InputLatencyMonitor.cs
```

**実装詳細**:

```csharp
// PerformanceMonitor.cs
public class PerformanceMonitor
{
    private readonly FrameTimeTracker _frameTimeTracker;
    private readonly InputLatencyMonitor _inputLatencyMonitor;
    private readonly IGameEventBus _eventBus;

    public void Update()
    {
        var frameTime = _frameTimeTracker.GetCurrentFrameTime();
        var inputLatency = _inputLatencyMonitor.GetCurrentLatency();

        // KPIチェック
        CheckFrameTimeKPI(frameTime);
        CheckInputLatencyKPI(inputLatency);
    }

    private void CheckInputLatencyKPI(float latency)
    {
        var maxLatency = 0.10f; // 企画仕様: ≤ 0.10s
        if (latency > maxLatency)
        {
            _eventBus.Publish(new PerformanceWarningEvent("InputLatency", latency, maxLatency));
        }
    }
}
```

**完了条件**:
- [ ] パフォーマンス監視が正常に動作
- [ ] KPI要件が満たされている
- [ ] 警告システムが適切に機能

## 実装スケジュール

### 4.1 短期改善（MVP対応）: 2週間

| 週 | 実装内容 | 完了条件 |
|---|---------|---------|
| Week 1 | Phase 1-2: フレーム管理・キャンセルシステム | 基本的なアクション遷移が動作 |
| Week 2 | Phase 3: 入力バッファリング | 先行入力が正常に処理される |

### 4.2 中期改善（アルファ対応）: 3週間

| 週 | 実装内容 | 完了条件 |
|---|---------|---------|
| Week 3 | Phase 4: 詳細アクション仕様 | 企画仕様通りのアクションが実行される |
| Week 4 | Phase 5: 無敵時間システム | 無敵時間が正確に動作する |
| Week 5 | Phase 6: パフォーマンス監視 | KPI監視が正常に動作する |

## テスト戦略

### 5.1 単体テスト

**テストファイル**:
```
Tests/Systems/Player/Movement/
├── FrameStateManagerTests.cs
├── CancelRuleManagerTests.cs
├── InputBufferTests.cs
├── ActionSpecificationTests.cs
├── InvincibilityManagerTests.cs
└── PerformanceMonitorTests.cs
```

**テスト内容**:
- フレーム管理システムのテスト
- キャンセルルールのテスト
- 入力バッファリングのテスト
- アクション仕様のテスト
- 無敵時間システムのテスト
- パフォーマンス監視のテスト

### 5.2 統合テスト

**テストファイル**:
```
Tests/Systems/Player/Integration/
├── PlayerMovementIntegrationTests.cs
├── ActionExecutionFlowTests.cs
└── PerformanceIntegrationTests.cs
```

**テスト内容**:
- アクション実行フローのテスト
- パフォーマンス要件のテスト
- 企画仕様との整合性テスト

### 5.3 パフォーマンステスト

**テスト内容**:
- フレームレートの維持（60FPS）
- 入力遅延の測定（≤ 0.10s）
- メモリ使用量の監視
- CPU使用率の監視

## リスク管理

### 6.1 技術的リスク

| リスク | 影響度 | 発生確率 | 対策 |
|--------|--------|----------|------|
| フレーム管理の複雑化 | 高 | 中 | 段階的な実装とテスト |
| パフォーマンス劣化 | 高 | 中 | 継続的な監視と最適化 |
| 入力遅延の増加 | 高 | 低 | バッファサイズの調整 |

### 6.2 スケジュールリスク

| リスク | 影響度 | 発生確率 | 対策 |
|--------|--------|----------|------|
| 実装時間の超過 | 中 | 中 | 優先度の調整とスコープ管理 |
| テスト時間の不足 | 中 | 低 | 自動テストの活用 |

## 成果物定義

### 7.1 必須成果物

#### 7.1.1 ソースコード
- [ ] `Scripts/Systems/Player/State/` ディレクトリ
- [ ] `Scripts/Systems/Player/Combat/` ディレクトリ
- [ ] `Scripts/Systems/Player/Input/` ディレクトリ
- [ ] `Scripts/Systems/Performance/` ディレクトリ

#### 7.1.2 テストコード
- [ ] 単体テストの実装
- [ ] 統合テストの実装
- [ ] パフォーマンステストの実装

#### 7.1.3 ドキュメント
- [ ] API仕様書
- [ ] 使用例
- [ ] トラブルシューティングガイド

### 7.2 品質基準

#### 7.2.1 機能要件
- [ ] 企画仕様との完全な整合性
- [ ] フレーム単位の正確な制御
- [ ] スムーズなアクション遷移
- [ ] 適切なパフォーマンス

#### 7.2.2 非機能要件
- [ ] 60FPSの維持
- [ ] 入力遅延 ≤ 0.10s
- [ ] メモリ使用量の最適化
- [ ] エラーハンドリングの実装

## 制限事項

### 8.1 技術的制限
- 既存のMVVMアーキテクチャとの互換性を維持
- Godot Engine 4.4の制限内での実装
- 既存のテスト環境との統合

### 8.2 スケジュール制限
- MVP対応は2週間以内
- アルファ対応は3週間以内
- 既存機能への影響を最小限に抑制

### 8.3 リソース制限
- 既存の開発環境の活用
- 既存のテストフレームワークの使用
- ドキュメント管理ルールの遵守

## 変更履歴

| バージョン | 更新日     | 変更内容 |
| ---------- | ---------- | -------- |
| 0.1.0      | 2025-06-20 | 初版作成 |
