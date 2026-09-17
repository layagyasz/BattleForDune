using BattleForDune.Core.Events;

namespace BattleForDune.Core.Components.Effects
{
    public interface IGenericEffect
    {
        public interface IEffectPrototype
        {
            IGenericEffect Materialize(EntityId id, EntityId ownerId, EntityId sourceId, EntityId abilityId);
        }

        ReadOnlySpan<GameEventType> SupportedEvents { get; }
        RegistryKey EffectKey { get; }
        EntityId Id { get; }
        EntityId OwnerId { get; }
        EntityId SourceId { get; }
        EntityId AbilityId { get; }
        byte Stacks { get; }
        byte Power { get; }

        void Apply(ComponentWorld world);
        void Remove(ComponentWorld world);
        void MergeStacks(byte newStacks);
        void MergePower(byte newPower);
    }
}
