using BattleForDune.Core.Schema;

namespace BattleForDune.Core.Components.Abilities.Targeting
{
    public record struct AbilityTargeting(
        TargetType TargetType, 
        sbyte Range,
        ITargetComparator Comparator, 
        TileAuraPredicate AuraCheck, 
        UnitStatPredicate StatCheck);
}
