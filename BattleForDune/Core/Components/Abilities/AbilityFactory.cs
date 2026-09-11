using Cardamom.Utils.Suppliers.Ids;

namespace BattleForDune.Core.Components.Abilities
{
    public static class AbilityFactory
    {
        public static IAbility Create(
            AbilityType abilityType, EntityId id, EntityId ownerId) => abilityType switch
        {
            AbilityType.Attack => new AttackAbility(id, ownerId),
            _ => throw new ArgumentException($"Unsupported AbilityType {abilityType}"),
        };
    }
}
