using BattleForDune.Core.Prototypes.Abilities;

namespace BattleForDune.Core.Prototypes
{
    public record class UnitPrototype(
        EntityId Id, LocalizationKey Name, StatBlockPrototype Stats, AbilityPrototype[] Abilities) { }
}
