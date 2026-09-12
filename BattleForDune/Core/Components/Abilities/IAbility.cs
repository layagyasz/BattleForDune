using BattleForDune.Core.Events;

namespace BattleForDune.Core.Components.Abilities
{
    public interface IAbility
    {
        public EntityId Id { get; }
        public EntityId OwnerId { get; }
        public IGenericHandler[] Handlers { get; }
    }
}
