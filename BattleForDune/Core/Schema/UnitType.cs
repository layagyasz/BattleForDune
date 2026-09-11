using BattleForDune.Core.Components.Abilities;

namespace BattleForDune.Core.Schema
{
    public record class UnitType(StatBlock Stats, AbilityType[] Abilities) { }
}
