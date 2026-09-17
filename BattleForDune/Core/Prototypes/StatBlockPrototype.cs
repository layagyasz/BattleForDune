using BattleForDune.Core.Schema;

namespace BattleForDune.Core.Prototypes
{
    public record struct StatBlockPrototype(
        sbyte Attack,
        sbyte Defense,
        sbyte Armor,
        sbyte Shield,
        sbyte Initiative,
        sbyte Hitpoints,
        sbyte Morale,
        sbyte Threat)
    {
        public readonly StatBlock Materialize()
        {
            var block = new StatBlock();
            block.Set(StatType.Attack, ToStat(Attack));
            block.Set(StatType.Defense, ToStat(Defense));
            block.Set(StatType.Armor, ToStat(Armor));
            block.Set(StatType.Shield, ToStat(Shield));
            block.Set(StatType.Initiative, ToStat(Initiative));
            block.Set(StatType.Hitpoints, ToStat(Hitpoints));
            block.Set(StatType.Morale, ToStat(Morale));
            block.Set(StatType.Threat, ToStat(Threat));
            return block;
        }

        private static Stat ToStat(sbyte value)
        {
            return new(value, 0, value);
        }
    }
}
