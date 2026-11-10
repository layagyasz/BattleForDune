using BattleForDune.Core.Schema;

namespace BattleForDune.Core.Components.Abilities.Targeting
{
    public readonly struct UnitStatPredicate(ComparisonType Comparison, StatType Stat, sbyte Value)
    {
        public readonly bool Matches(EntityId unitId, ComponentWorld world)
        {
            if (Comparison == ComparisonType.None)
            {
                return true;
            }
            ref var stats = ref world.Stats.TryGet(unitId, out var isFound);
            if (!isFound)
            {
                return false;
            }
            var comp = stats[Stat].Value.CompareTo(Value);
            if (comp < 0)
            {
                return Comparison.HasFlag(ComparisonType.LT);
            }
            if (comp > 0)
            {
                return Comparison.HasFlag(ComparisonType.GT);
            }
            return Comparison.HasFlag(ComparisonType.EQ);
        }
    }
}
