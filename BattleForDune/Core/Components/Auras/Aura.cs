using BattleForDune.Core.Schema;
using BattleForDune.Geometry;

namespace BattleForDune.Core.Components.Auras
{
    public record class Aura(
        RegistryKey AuraKey,
        EntityId Id,
        EntityId SourceId,
        EntityId AbilityId,
        RegistryKey Effect,
        AbilityVector Vector,
        Hex2b Position,
        byte Power,
        byte Range)
    {
        public record class Prototype(
            RegistryKey AuraKey, RegistryKey Effect, AbilityVector Vector) : IComparable<Prototype>
        {
            public int CompareTo(Prototype? other)
            {
                if (other == null)
                {
                    return 1;
                }
                return AuraKey.CompareTo(other.AuraKey);
            }

            public Aura Materialize(
                EntityId id, 
                EntityId sourceId,
                EntityId abilityId,
                Hex2b position,
                byte power,
                byte range)
            {
                return new(AuraKey, id, sourceId, abilityId, Effect, Vector, position, power, range);
            }
        }
    }
}
