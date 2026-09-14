using BattleForDune.Core.Prototypes.Abilities;

namespace BattleForDune.Core.Prototypes
{
    public record class UnitPrototype(EntityId Id, StatBlockPrototype Stats, IAbilityPrototype[] Abilities) { }
}
