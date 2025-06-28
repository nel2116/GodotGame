using Godot;
using Systems.Player.Progression;
using Core.Events;
using Core.ViewModels;

public partial class PlayerProgressionViewModelNode : BaseViewModelNode
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

    public void UpdateProgression()
    {
        if (!EnsureViewModelInitialized(_viewModel, "PlayerProgressionViewModelNode")) return;
        
        _viewModel.UpdateProgression();
    }

    public void AddExperience(int exp)
    {
        if (!EnsureViewModelInitialized(_viewModel, "PlayerProgressionViewModelNode")) return;
        
        _viewModel.AddExperience(exp);
    }

    public bool UnlockSkill(string skillName)
    {
        if (!EnsureViewModelInitialized(_viewModel, "PlayerProgressionViewModelNode")) return false;
        
        return _viewModel.UnlockSkill(skillName);
    }
} 