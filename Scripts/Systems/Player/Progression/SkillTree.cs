using System.Collections.Generic;

namespace Systems.Player.Progression
{
    /// <summary>
    /// スキルツリー
    /// </summary>
    public class SkillTree
    {
        private readonly Dictionary<string, Skill> _skills = new();

        public Dictionary<string, Skill> Skills => _skills;

        public void Initialize()
        {
            _skills["Fireball"] = new Skill("Fireball", 5, 1, 1);
            _skills["Icebolt"] = new Skill("Icebolt", 5, 2, 2);
        }

        public bool CanUnlock(string name, int level, int points)
        {
            return _skills.ContainsKey(name) && level >= _skills[name].RequiredLevel && points >= _skills[name].RequiredSkillPoints;
        }

        public Skill GetSkill(string name) => _skills[name];
    }
}
