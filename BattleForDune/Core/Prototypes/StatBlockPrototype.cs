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
            block[StatType.Attack] = ToStat(Attack);
            block[StatType.Defense] = ToStat(Defense);
            block[StatType.Armor] = ToStat(Armor);
            block[StatType.Shield] = ToStat(Shield);
            block[StatType.Initiative] = ToStat(Initiative);
            block[StatType.Hitpoints] = ToStat(Hitpoints);
            block[StatType.Morale] = ToStat(Morale);
            block[StatType.Threat] = ToStat(Threat);
            return block;
        }

        private static Stat ToStat(sbyte value)
        {
            return new(value, 0, value, 0);
        }
    }
}
