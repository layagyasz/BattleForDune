using BattleForDune.Core.Components.Abilities.Procs;
using BattleForDune.Core.Schema;

namespace BattleForDune.Core.Components.Abilities
{
    public static class AbilityFactory
    {
        public static IAbility Create(
            AbilityType abilityType, EntityId id, EntityId ownerId) => abilityType switch
        {
            AbilityType.Attack => 
                new AttackAbility(id, ownerId, 0, new(1, 6, 0), DamageType.Physical, 20, new Critical(2)),
            _ => throw new ArgumentException($"Unsupported AbilityType {abilityType}"),
        };
    }
}
