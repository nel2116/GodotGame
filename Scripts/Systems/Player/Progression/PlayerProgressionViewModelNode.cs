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

    public int Level
    {
        get
        {
            if (_viewModel?.Level != null)
            {
                var level = _viewModel.Level;
                if (level != null)
                    return level.Value;
            }
            return 1;
        }
    }

    public int Experience
    {
        get
        {
            if (_viewModel?.Experience != null)
            {
                var experience = _viewModel.Experience;
                if (experience != null)
                    return experience.Value;
            }
            return 0;
        }
    }

    public int AvailableSkillPoints
    {
        get
        {
            if (_viewModel?.AvailableSkillPoints != null)
            {
                var skillPoints = _viewModel.AvailableSkillPoints;
                if (skillPoints != null)
                    return skillPoints.Value;
            }
            return 0;
        }
    }

    public void Initialize()
    {
        var bus = GameEventBus.Instance;
        var model = new PlayerProgressionModel();
        _viewModel = new PlayerProgressionViewModel(model, bus);
        _viewModel.Initialize();
    }

    public void UpdateProgression()
    {
        if (!EnsureViewModelInitialized(_viewModel)) return;
        _viewModel!.UpdateProgression();
    }

    public void AddExperience(int experience)
    {
        if (!EnsureViewModelInitialized(_viewModel)) return;
        _viewModel!.AddExperience(experience);
    }

    public bool UnlockSkill(string skillName)
    {
        if (!EnsureViewModelInitialized(_viewModel)) return false;
        return _viewModel!.UnlockSkill(skillName);
    }
}
