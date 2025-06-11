namespace Systems.Player.Progression
{
    /// <summary>
    /// スキル情報
    /// </summary>
    public class Skill
    {
        public string SkillName { get; }
        public int Level { get; private set; }
        public int MaxLevel { get; }
        public int RequiredLevel { get; }
        public int RequiredSkillPoints { get; }

        public Skill(string name, int maxLevel, int requiredLevel, int requiredPoints)
        {
            SkillName = name;
            Level = 0;
            MaxLevel = maxLevel;
            RequiredLevel = requiredLevel;
            RequiredSkillPoints = requiredPoints;
        }

        public void Upgrade()
        {
            if (Level < MaxLevel)
            {
                Level++;
            }
        }
    }
}
