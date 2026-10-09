using BattleForDune.Core.Schema;

namespace BattleForDune.Core.Prototypes
{
    public record struct StatBlockPrototype(
        Progression Attack,
        Progression Evasion,
        Progression Armor,
        Progression Shield,
        Progression Initiative,
        Progression Hitpoints,
        Progression Morale,
        Progression Threat)
    {
        public readonly StatBlock Materialize(byte level)
        {
            var block = new StatBlock();
            block[StatType.Attack] = ToStat(Attack, level);
            block[StatType.Evasion] = ToStat(Evasion, level);
            block[StatType.Armor] = ToStat(Armor, level);
            block[StatType.Shield] = ToStat(Shield, level);
            block[StatType.Initiative] = ToStat(Initiative, level);
            block[StatType.Hitpoints] = ToStat(Hitpoints, level);
            block[StatType.Morale] = ToStat(Morale, level);
            block[StatType.Threat] = ToStat(Threat, level);
            return block;
        }

        private static Stat ToStat(Progression progression, byte level)
        {
            var value = (sbyte)progression.Materialize(level);
            return new(value, 0, value);
        }
    }
}
