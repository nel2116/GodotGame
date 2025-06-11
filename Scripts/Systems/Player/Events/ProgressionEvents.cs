using Core.Events;

namespace Systems.Player.Events
{
    /// <summary>
    /// レベルアップイベント
    /// </summary>
    public class LevelUpEvent : GameEvent
    {
        public int Level { get; }
        public LevelUpEvent(int level)
        {
            Level = level;
        }
    }

    /// <summary>
    /// 経験値変更イベント
    /// </summary>
    public class ExperienceChangedEvent : GameEvent
    {
        public int Experience { get; }
        public ExperienceChangedEvent(int exp)
        {
            Experience = exp;
        }
    }

    /// <summary>
    /// スキルポイント変更イベント
    /// </summary>
    public class SkillPointsChangedEvent : GameEvent
    {
        public int SkillPoints { get; }
        public SkillPointsChangedEvent(int points)
        {
            SkillPoints = points;
        }
    }

    /// <summary>
    /// スキル解放イベント
    /// </summary>
    public class SkillUnlockedEvent : GameEvent
    {
        public string SkillName { get; }
        public SkillUnlockedEvent(string skillName)
        {
            SkillName = skillName;
        }
    }
}
