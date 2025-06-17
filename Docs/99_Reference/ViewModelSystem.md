---
title: ビューモデルシステム
version: 0.1.0
status: draft
updated: 2024-03-21
tags:
    - API
    - ViewModel
    - Core
    - Reactive
    - Event
linked_docs:
    - "[[ReactiveSystem]]"
    - "[[ReactiveProperty]]"
    - "[[CompositeDisposable]]"
    - "[[CoreEventSystem]]"
    - "[[CommonEventSystem]]"
---

# ビューモデルシステム

## 目次

1. [概要](#概要)
2. [ビューモデル定義](#ビューモデル定義)
3. [主要コンポーネント](#主要コンポーネント)
4. [使用例](#使用例)
5. [制限事項](#制限事項)
6. [変更履歴](#変更履歴)

## 概要

ビューモデルシステムは、UI とビジネスロジックの間のデータバインディングを管理するシステムです。以下の機能を提供します：

-   データバインディング
-   コマンド実行
-   イベント通知
-   状態管理

## ビューモデル定義

### ViewModelBase

ビューモデルの基本クラスです。

```csharp
public abstract class ViewModelBase : IDisposable
{
    protected readonly CompositeDisposable _disposables = new();
    protected readonly IGameEventBus _eventBus;

    public void Dispose()
    {
        _disposables.Dispose();
    }

    protected void AddDisposable(IDisposable disposable)
    {
        _disposables.Add(disposable);
    }
}
```

### ICommand

コマンドのインターフェースです。

```csharp
public interface ICommand
{
    bool CanExecute { get; }
    void Execute();
    event EventHandler CanExecuteChanged;
}
```

## 主要コンポーネント

### ViewModelController

ビューモデルを制御するコンポーネントです。

```csharp
public class ViewModelController<T> where T : ViewModelBase
{
    private readonly ReactiveProperty<T> _currentViewModel;
    private readonly IGameEventBus _eventBus;

    public IReactiveProperty<T> CurrentViewModel => _currentViewModel;

    public void SetViewModel(T viewModel);
    public void ClearViewModel();
    public void UpdateViewModel(Action<T> updateAction);
}
```

### ViewModelHandler

ビューモデルを処理するコンポーネントです。

```csharp
public class ViewModelHandler<T> : MonoBehaviour where T : ViewModelBase
{
    private readonly CompositeDisposable _disposables = new();
    private readonly ViewModelController<T> _viewModelController;

    private void OnEnable();
    private void OnDisable();
    private void Update();
    private void OnViewModelChanged(T newViewModel);
}
```

## 使用例

### ビューモデルの実装

```csharp
public class PlayerViewModel : ViewModelBase
{
    private readonly ReactiveProperty<string> _name;
    private readonly ReactiveProperty<int> _health;
    private readonly ReactiveProperty<int> _level;
    private readonly ReactiveCommand _attackCommand;

    public IReactiveProperty<string> Name => _name;
    public IReactiveProperty<int> Health => _health;
    public IReactiveProperty<int> Level => _level;
    public ICommand AttackCommand => _attackCommand;

    public PlayerViewModel(IGameEventBus eventBus) : base(eventBus)
    {
        _name = new ReactiveProperty<string>();
        _health = new ReactiveProperty<int>();
        _level = new ReactiveProperty<int>();
        _attackCommand = new ReactiveCommand();

        _attackCommand.Subscribe(ExecuteAttack)
            .AddTo(_disposables);
    }

    private void ExecuteAttack()
    {
        _eventBus.Publish(new PlayerAttackEvent());
    }
}
```

### ビューモデルの使用

```csharp
public class PlayerView : MonoBehaviour
{
    [SerializeField] private ViewModelController<PlayerViewModel> _viewModelController;
    [SerializeField] private Text _nameText;
    [SerializeField] private Text _healthText;
    [SerializeField] private Text _levelText;
    [SerializeField] private Button _attackButton;

    private readonly CompositeDisposable _disposables = new();

    private void Start()
    {
        var viewModel = new PlayerViewModel(GameEventBus.Instance);
        _viewModelController.SetViewModel(viewModel);

        viewModel.Name
            .Subscribe(name => _nameText.text = name)
            .AddTo(_disposables);

        viewModel.Health
            .Subscribe(health => _healthText.text = $"HP: {health}")
            .AddTo(_disposables);

        viewModel.Level
            .Subscribe(level => _levelText.text = $"Lv: {level}")
            .AddTo(_disposables);

        _attackButton.onClick.AddListener(() => viewModel.AttackCommand.Execute());
    }

    private void OnDestroy()
    {
        _disposables.Dispose();
    }
}
```

## 制限事項

-   スレッドセーフな実装が必要な箇所では、必ず提供されている同期メカニズムを使用してください
-   リソースの解放は適切なタイミングで行ってください
-   イベントの購読は必要最小限に抑えてください
-   非同期処理の実行時は、必ず`ExecuteAsync`メソッドを使用してください
-   ビューモデルは、必ず`ViewModelBase`を継承してください
-   コマンドは、必ず`ICommand`インターフェースを実装してください
-   ビューモデルの制御は、必ず`ViewModelController`を通じて行ってください
-   ビューモデルの処理は、必ず`ViewModelHandler`を通じて行ってください

## 変更履歴

| バージョン | 更新日     | 変更内容 |
| ---------- | ---------- | -------- |
| 0.1.0      | 2024-03-21 | 初版作成 |
