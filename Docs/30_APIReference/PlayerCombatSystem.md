---
title: プレイヤー戦闘システム
version: 0.1.0
status: draft
updated: 2024-03-21
tags:
    - API
    - Player
    - Combat
    - Core
    - Reactive
    - Event
linked_docs:
    - "[[PlayerSystem]]"
    - "[[PlayerStateSystem]]"
    - "[[ReactiveSystem]]"
    - "[[ViewModelSystem]]"
    - "[[CoreEventSystem]]"
    - "[[CommonEventSystem]]"
---

# プレイヤー戦闘システム

## 目次

1. [概要](#概要)
2. [戦闘パラメータ](#戦闘パラメータ)
3. [主要コンポーネント](#主要コンポーネント)
4. [使用例](#使用例)
5. [制限事項](#制限事項)
6. [変更履歴](#変更履歴)

## 概要

プレイヤー戦闘システムは、プレイヤーの戦闘を制御するシステムです。以下の機能を提供します：

-   攻撃処理
-   ダメージ計算
-   スキル管理
-   イベント通知

## 戦闘パラメータ

### CombatParameters

戦闘に関するパラメータを定義するクラスです。

```csharp
public class CombatParameters
{
    public int MaxHealth { get; set; } = 100;
    public int MaxMana { get; set; } = 100;
    public float AttackPower { get; set; } = 10f;
    public float DefensePower { get; set; } = 5f;
    public float CriticalRate { get; set; } = 0.1f;
    public float CriticalDamage { get; set; } = 1.5f;
}
```

## 主要コンポーネント

### PlayerCombatController

プレイヤーの戦闘を制御するコンポーネントです。

```csharp
public class PlayerCombatController
{
    private readonly ReactiveProperty<int> _health;
    private readonly ReactiveProperty<int> _mana;
    private readonly ReactiveProperty<float> _attackPower;
    private readonly ReactiveProperty<float> _defensePower;
    private readonly CombatParameters _parameters;
    private readonly IGameEventBus _eventBus;

    public IReactiveProperty<int> Health => _health;
    public IReactiveProperty<int> Mana => _mana;
    public IReactiveProperty<float> AttackPower => _attackPower;
    public IReactiveProperty<float> DefensePower => _defensePower;

    public void TakeDamage(int damage);
    public void Heal(int amount);
    public void UseMana(int amount);
    public void RestoreMana(int amount);
    public void Attack(IDamageable target);
    public void UseSkill(Skill skill, IDamageable target);
}
```

### PlayerCombatHandler

プレイヤーの戦闘を処理するコンポーネントです。

```csharp
public class PlayerCombatHandler : MonoBehaviour
{
    private readonly CompositeDisposable _disposables = new();
    private readonly PlayerCombatController _combatController;

    private void OnEnable();
    private void OnDisable();
    private void Update();
    private void OnHealthChanged(int newHealth);
    private void OnManaChanged(int newMana);
    private void OnAttackPowerChanged(float newAttackPower);
    private void OnDefensePowerChanged(float newDefensePower);
}
```

## 使用例

### 戦闘の制御

```csharp
public class PlayerCombatInput : MonoBehaviour
{
    [SerializeField] private PlayerCombatController _combatController;

    private void Update()
    {
        // 通常攻撃
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            var target = GetTarget();
            if (target != null)
            {
                _combatController.Attack(target);
            }
        }

        // スキル使用
        if (Input.GetKeyDown(KeyCode.Q))
        {
            var target = GetTarget();
            if (target != null)
            {
                _combatController.UseSkill(Skill.Fireball, target);
            }
        }
    }

    private IDamageable GetTarget()
    {
        // ターゲット取得の実装
        return null;
    }
}
```

### 戦闘状態の監視

```csharp
public class PlayerCombatObserver : MonoBehaviour
{
    [SerializeField] private PlayerCombatController _combatController;

    private void OnEnable()
    {
        _combatController.Health
            .Subscribe(OnHealthChanged)
            .AddTo(_disposables);

        _combatController.Mana
            .Subscribe(OnManaChanged)
            .AddTo(_disposables);

        _combatController.AttackPower
            .Subscribe(OnAttackPowerChanged)
            .AddTo(_disposables);

        _combatController.DefensePower
            .Subscribe(OnDefensePowerChanged)
            .AddTo(_disposables);
    }

    private void OnHealthChanged(int newHealth)
    {
        Debug.Log($"Player health changed to: {newHealth}");
    }

    private void OnManaChanged(int newMana)
    {
        Debug.Log($"Player mana changed to: {newMana}");
    }

    private void OnAttackPowerChanged(float newAttackPower)
    {
        Debug.Log($"Player attack power changed to: {newAttackPower}");
    }

    private void OnDefensePowerChanged(float newDefensePower)
    {
        Debug.Log($"Player defense power changed to: {newDefensePower}");
    }
}
```

## 制限事項

-   スレッドセーフな実装が必要な箇所では、必ず提供されている同期メカニズムを使用してください
-   リソースの解放は適切なタイミングで行ってください
-   イベントの購読は必要最小限に抑えてください
-   非同期処理の実行時は、必ず`ExecuteAsync`メソッドを使用してください
-   戦闘パラメータは、必ず`CombatParameters`を通じて設定してください
-   戦闘制御は、必ず`PlayerCombatController`を通じて行ってください
-   戦闘処理は、必ず`PlayerCombatHandler`を通じて行ってください

## 変更履歴

| バージョン | 更新日     | 変更内容 |
| ---------- | ---------- | -------- |
| 0.1.0      | 2024-03-21 | 初版作成 |
