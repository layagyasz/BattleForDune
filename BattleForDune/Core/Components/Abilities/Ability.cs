namespace BattleForDune.Core.Components.Abilities
{
    public record class Ability(EntityId Id, EntityId OwnerId, IGenericHandler[] Handlers);
}
