using BattleForDune.Core.Schema;
using BattleForDune.Geometry;

namespace BattleForDune.Core.Components.Auras
{
    public record class Aura(
        EntityId Id,
        EntityId SourceId,
        EntityId AbilityId,
        Hex2b Position,
        AbilityVector Vector,
        RegistryKey EffectKey,
        byte Power,
        byte Range);
}
