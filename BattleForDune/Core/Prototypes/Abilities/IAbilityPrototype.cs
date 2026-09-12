using BattleForDune.Core.Components.Abilities;

namespace BattleForDune.Core.Prototypes.Abilities
{
    public interface IAbilityPrototype
    {
        IAbility Materialize(EntityId Id, EntityId OwnerId);
    }
}
