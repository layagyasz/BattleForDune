namespace BattleForDune.Core.Components.Abilities
{
    public record class BasicAttackAbility(EntityId Id, EntityId OwnerId, IGenericHandler[] Handlers) 
        : IAbility { }
}
