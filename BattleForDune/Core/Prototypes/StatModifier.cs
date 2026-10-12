using BattleForDune.Core.Schema;

namespace BattleForDune.Core.Prototypes
{
    public record struct StatModifier(StatType Stat, sbyte Modifier, sbyte Cap);
}
