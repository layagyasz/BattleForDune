using BattleForDune.Core.Prototypes.Abilities;

namespace BattleForDune.Core.Prototypes
{
    public record class UnitPrototype(StatBlockPrototype Stats, IAbilityPrototype[] Abilities) { }
}
