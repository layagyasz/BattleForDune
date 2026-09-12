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
            return new(
                ToStat(Attack),
                ToStat(Defense),
                ToStat(Armor), 
                ToStat(Shield), 
                ToStat(Initiative),
                ToStat(Hitpoints),
                ToStat(Morale),
                ToStat(Threat));
        }

        private static Stat ToStat(sbyte value)
        {
            return new(value, 0, value);
        }
    }
}
