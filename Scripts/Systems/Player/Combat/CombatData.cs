using System;

namespace Systems.Player.Combat
{
    /// <summary>
    /// 戦闘アクションデータ
    /// </summary>
    public class CombatData
    {
        public string Name { get; }
        public float DamageMultiplier { get; }
        public Action<float> OnExecute { get; }

        public CombatData(string name, float multiplier, Action<float> onExecute)
        {
            Name = name;
            DamageMultiplier = multiplier;
            OnExecute = onExecute;
        }
    }
}
