using BattleForDune.Core.Events;

namespace BattleForDune.Core.Components.Effects
{
    public interface IGenericEffect
    {
        public interface IEffectPrototype
        {
            IGenericEffect Materialize(EntityId id, EntityId ownerId, EntityId sourceId, byte abilityId);
        }

        ReadOnlySpan<GameEventType> SupportedEvents { get; }
        bool IsMergeable { get; }
        RegistryKey EffectKey { get; }
        EntityId Id { get; }
        EntityId OwnerId { get; }
        EntityId SourceId { get; }
        byte AbilityId { get; }
        byte Stacks { get; }
        byte Power { get; }

        void Apply<TSink>(ComponentWorld world, in TSink sink) where TSink : IGameEventSink;
        void Remove<TSink>(ComponentWorld world, in TSink sink) where TSink : IGameEventSink;
        void MergeStacks(byte newStacks);
        void MergePower(byte newPower);
    }
}
