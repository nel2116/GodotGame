using System;
using System.Collections.Generic;
using System.Linq;
using Core.Reactive;

namespace Systems.Player.Progression
{
    /// <summary>
    /// プレイヤー進行モデル
    /// </summary>
    public class PlayerProgressionModel : IDisposable
    {
        private readonly Dictionary<string, Skill> _unlocked_skills = new();
        private readonly SkillTree _skill_tree = new();
        private readonly CompositeDisposable _disposables = new();

        public int CurrentLevel { get; private set; }
        public int CurrentExp { get; private set; }
        public int SkillPoints { get; private set; }
        public Dictionary<string, Skill> UnlockedSkills => _unlocked_skills;

        public void Initialize()
        {
            CurrentLevel = 1;
            CurrentExp = 0;
            SkillPoints = 0;
            _skill_tree.Initialize();
        }

        public void AddExperience(int exp)
        {
            CurrentExp += exp;
            CheckLevelUp();
        }

        public bool UnlockSkill(string skillName)
        {
            if (!_skill_tree.CanUnlock(skillName, CurrentLevel, SkillPoints))
            {
                return false;
            }

            var skill = _skill_tree.GetSkill(skillName);
            _unlocked_skills[skillName] = skill;
            SkillPoints -= skill.RequiredSkillPoints;
            return true;
        }

        private void CheckLevelUp()
        {
            var needed = CalculateExpNeeded(CurrentLevel);
            if (CurrentExp >= needed)
            {
                LevelUp();
            }
        }

        private void LevelUp()
        {
            CurrentLevel++;
            SkillPoints += CalculateSkillPoints(CurrentLevel);
            CurrentExp -= CalculateExpNeeded(CurrentLevel - 1);
        }

        private int CalculateExpNeeded(int level) => level * 100;

        private int CalculateSkillPoints(int level) => level;

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}
