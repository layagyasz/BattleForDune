using BattleForDune.Core.Events;

namespace BattleForDune.Core.Components.Abilities
{
    public interface IAbility
    {
        public EntityId Id { get; }
        public EntityId OwnerId { get; }
        void Trigger<TSink>(in TSink sink) where TSink : IGameEventSink;
    }
}
