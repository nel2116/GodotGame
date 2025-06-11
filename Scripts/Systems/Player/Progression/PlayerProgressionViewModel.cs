using System.Linq;
using Core.Events;
using Core.Reactive;
using Core.ViewModels;
using Systems.Player.Events;

namespace Systems.Player.Progression
{
    /// <summary>
    /// プレイヤー進行ビューモデル
    /// </summary>
    public class PlayerProgressionViewModel : ViewModelBase
    {
        private readonly PlayerProgressionModel _model;
        private readonly ReactiveProperty<int> _level;
        private readonly ReactiveProperty<int> _experience;
        private readonly ReactiveProperty<int> _skill_points;
        private readonly ReactiveProperty<System.Collections.Generic.List<Skill>> _unlocked;

        public ReactiveProperty<int> Level => _level;
        public ReactiveProperty<int> Experience => _experience;
        public ReactiveProperty<int> AvailableSkillPoints => _skill_points;
        public ReactiveProperty<System.Collections.Generic.List<Skill>> UnlockedSkills => _unlocked;

        public PlayerProgressionViewModel(PlayerProgressionModel model, IGameEventBus bus)
            : base(bus)
        {
            _model = model;
            _level = new ReactiveProperty<int>().AddTo(Disposables);
            _experience = new ReactiveProperty<int>().AddTo(Disposables);
            _skill_points = new ReactiveProperty<int>().AddTo(Disposables);
            _unlocked = new ReactiveProperty<System.Collections.Generic.List<Skill>>().AddTo(Disposables);

            _level.Subscribe(OnLevelChanged).AddTo(Disposables);
            _experience.Subscribe(OnExperienceChanged).AddTo(Disposables);
            _skill_points.Subscribe(OnSkillPointsChanged).AddTo(Disposables);
        }

        public void Initialize()
        {
            _model.Initialize();
            UpdateProgressionState();
        }

        public void AddExperience(int exp)
        {
            _model.AddExperience(exp);
            UpdateProgressionState();
        }

        public bool UnlockSkill(string name)
        {
            var result = _model.UnlockSkill(name);
            if (result)
            {
                UpdateProgressionState();
                EventBus.Publish(new SkillUnlockedEvent(name));
            }
            return result;
        }

        /// <summary>
        /// 毎フレームの更新処理
        /// </summary>
        public void UpdateProgression()
        {
            UpdateProgressionState();
        }

        /// <summary>
        /// 毎フレーム更新処理
        /// </summary>
        public void Update()
        {
            UpdateProgression();
        }

        private void UpdateProgressionState()
        {
            _level.Value = _model.CurrentLevel;
            _experience.Value = _model.CurrentExp;
            _skill_points.Value = _model.SkillPoints;
            _unlocked.Value = _model.UnlockedSkills.Values.ToList();
        }

        private void OnLevelChanged(int level)
        {
            EventBus.Publish(new LevelUpEvent(level));
        }

        private void OnExperienceChanged(int exp)
        {
            EventBus.Publish(new ExperienceChangedEvent(exp));
        }

        private void OnSkillPointsChanged(int points)
        {
            EventBus.Publish(new SkillPointsChangedEvent(points));
        }
    }
}
