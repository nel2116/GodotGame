---
title: プレイヤー進行システム
version: 0.1.0
status: draft
updated: 2024-03-21
tags:
    - API
    - Player
    - Progression
    - Core
    - Reactive
    - Event
linked_docs:
    - "[[PlayerSystem]]"
    - "[[PlayerStateSystem]]"
    - "[[PlayerMovementSystem]]"
    - "[[PlayerCombatSystem]]"
    - "[[PlayerAnimationSystem]]"
    - "[[PlayerInputSystem]]"
    - "[[ReactiveSystem]]"
    - "[[ViewModelSystem]]"
    - "[[CoreEventSystem]]"
    - "[[CommonEventSystem]]"
---

# プレイヤー進行システム

## 目次

1. [概要](#概要)
2. [進行定義](#進行定義)
3. [主要コンポーネント](#主要コンポーネント)
4. [使用例](#使用例)
5. [制限事項](#制限事項)
6. [変更履歴](#変更履歴)

## 概要

プレイヤー進行システムは、プレイヤーの進行を制御するシステムです。以下の機能を提供します：

-   レベル管理
-   経験値管理
-   スキル管理
-   イベント通知

## 進行定義

### ProgressionDefinition

進行の定義を管理するクラスです。

```csharp
public class ProgressionDefinition
{
    public int Level { get; set; }
    public int Experience { get; set; }
    public int MaxExperience { get; set; }
    public List<Skill> UnlockedSkills { get; set; }
    public Dictionary<string, int> Stats { get; set; }
}

public class Skill
{
    public string Name { get; set; }
    public int RequiredLevel { get; set; }
    public int RequiredExperience { get; set; }
    public Dictionary<string, int> Requirements { get; set; }
}
```

## 主要コンポーネント

### PlayerProgressionController

プレイヤーの進行を制御するコンポーネントです。

```csharp
public class PlayerProgressionController
{
    private readonly ReactiveProperty<int> _level;
    private readonly ReactiveProperty<int> _experience;
    private readonly ReactiveProperty<int> _maxExperience;
    private readonly ReactiveProperty<List<Skill>> _unlockedSkills;
    private readonly ReactiveProperty<Dictionary<string, int>> _stats;
    private readonly ProgressionDefinition _definition;
    private readonly IGameEventBus _eventBus;

    public IReactiveProperty<int> Level => _level;
    public IReactiveProperty<int> Experience => _experience;
    public IReactiveProperty<int> MaxExperience => _maxExperience;
    public IReactiveProperty<List<Skill>> UnlockedSkills => _unlockedSkills;
    public IReactiveProperty<Dictionary<string, int>> Stats => _stats;

    public void AddExperience(int amount);
    public void LevelUp();
    public void UnlockSkill(Skill skill);
    public void UpdateStat(string statName, int value);
}
```

### PlayerProgressionHandler

プレイヤーの進行を処理するコンポーネントです。

```csharp
public class PlayerProgressionHandler : MonoBehaviour
{
    private readonly CompositeDisposable _disposables = new();
    private readonly PlayerProgressionController _progressionController;

    private void OnEnable();
    private void OnDisable();
    private void Update();
    private void OnLevelChanged(int newLevel);
    private void OnExperienceChanged(int newExperience);
    private void OnMaxExperienceChanged(int newMaxExperience);
    private void OnUnlockedSkillsChanged(List<Skill> newSkills);
    private void OnStatsChanged(Dictionary<string, int> newStats);
}
```

## 使用例

### 進行の制御

```csharp
public class PlayerProgressionManager : MonoBehaviour
{
    [SerializeField] private PlayerProgressionController _progressionController;

    private void Start()
    {
        // 初期レベル設定
        _progressionController.Level.Value = 1;
        _progressionController.Experience.Value = 0;
        _progressionController.MaxExperience.Value = 100;

        // 初期スキル設定
        var initialSkills = new List<Skill>
        {
            new Skill
            {
                Name = "Basic Attack",
                RequiredLevel = 1,
                RequiredExperience = 0,
                Requirements = new Dictionary<string, int>()
            }
        };
        _progressionController.UnlockedSkills.Value = initialSkills;

        // 初期ステータス設定
        var initialStats = new Dictionary<string, int>
        {
            { "Strength", 10 },
            { "Dexterity", 10 },
            { "Intelligence", 10 }
        };
        _progressionController.Stats.Value = initialStats;
    }

    public void OnEnemyDefeated(int experienceReward)
    {
        _progressionController.AddExperience(experienceReward);
    }
}
```

### 進行状態の監視

```csharp
public class PlayerProgressionObserver : MonoBehaviour
{
    [SerializeField] private PlayerProgressionController _progressionController;

    private void OnEnable()
    {
        _progressionController.Level
            .Subscribe(OnLevelChanged)
            .AddTo(_disposables);

        _progressionController.Experience
            .Subscribe(OnExperienceChanged)
            .AddTo(_disposables);

        _progressionController.MaxExperience
            .Subscribe(OnMaxExperienceChanged)
            .AddTo(_disposables);

        _progressionController.UnlockedSkills
            .Subscribe(OnUnlockedSkillsChanged)
            .AddTo(_disposables);

        _progressionController.Stats
            .Subscribe(OnStatsChanged)
            .AddTo(_disposables);
    }

    private void OnLevelChanged(int newLevel)
    {
        Debug.Log($"Player level changed to: {newLevel}");
    }

    private void OnExperienceChanged(int newExperience)
    {
        Debug.Log($"Player experience changed to: {newExperience}");
    }

    private void OnMaxExperienceChanged(int newMaxExperience)
    {
        Debug.Log($"Player max experience changed to: {newMaxExperience}");
    }

    private void OnUnlockedSkillsChanged(List<Skill> newSkills)
    {
        Debug.Log($"Player unlocked skills changed to: {string.Join(", ", newSkills.Select(s => s.Name))}");
    }

    private void OnStatsChanged(Dictionary<string, int> newStats)
    {
        Debug.Log($"Player stats changed to: {string.Join(", ", newStats.Select(kvp => $"{kvp.Key}: {kvp.Value}"))}");
    }
}
```

## 制限事項

-   スレッドセーフな実装が必要な箇所では、必ず提供されている同期メカニズムを使用してください
-   リソースの解放は適切なタイミングで行ってください
-   イベントの購読は必要最小限に抑えてください
-   非同期処理の実行時は、必ず`ExecuteAsync`メソッドを使用してください
-   進行定義は、必ず`ProgressionDefinition`を通じて設定してください
-   進行制御は、必ず`PlayerProgressionController`を通じて行ってください
-   進行処理は、必ず`PlayerProgressionHandler`を通じて行ってください

## 変更履歴

| バージョン | 更新日     | 変更内容 |
| ---------- | ---------- | -------- |
| 0.1.0      | 2024-03-21 | 初版作成 |
