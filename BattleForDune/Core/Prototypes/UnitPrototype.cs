using BattleForDune.Core.Components.Abilities;
using BattleForDune.Core.Schema;

namespace BattleForDune.Core.Prototypes
{
    public record class UnitPrototype(StatBlockPrototype Stats, AbilityType[] Abilities) { }
}
