using Godot;
using Systems.Player.Progression;
using Core.Events;

public partial class PlayerProgressionViewModelNode : Node
{
    private PlayerProgressionViewModel? _viewModel;
    
    /// <summary>
    /// ViewModelが初期化されているかどうかを確認
    /// </summary>
    public bool IsInitialized => _viewModel != null;

    public int Level => _viewModel?.Level?.Value ?? 1;
    public int Experience => _viewModel?.Experience?.Value ?? 0;
    public int AvailableSkillPoints => _viewModel?.AvailableSkillPoints?.Value ?? 0;

    public void Initialize()
    {
        var bus = GameEventBus.Instance;
        var model = new PlayerProgressionModel();
        _viewModel = new PlayerProgressionViewModel(model, bus);
        _viewModel.Initialize();
    }

    /// <summary>
    /// ViewModelが初期化されていることを確認し、未初期化の場合はエラーメッセージを出力してfalseを返す
    /// </summary>
    private bool EnsureViewModelInitialized()
    {
        if (_viewModel == null)
        {
            GD.PrintErr("PlayerProgressionViewModelNode: ViewModel is not initialized. Call Initialize() first.");
            return false;
        }
        return true;
    }

    public void UpdateProgression()
    {
        if (!EnsureViewModelInitialized()) return;
        
        _viewModel.UpdateProgression();
    }

    public void AddExperience(int exp)
    {
        if (!EnsureViewModelInitialized()) return;
        
        _viewModel.AddExperience(exp);
    }

    public bool UnlockSkill(string skillName)
    {
        if (!EnsureViewModelInitialized()) return false;
        
        return _viewModel.UnlockSkill(skillName);
    }
} 