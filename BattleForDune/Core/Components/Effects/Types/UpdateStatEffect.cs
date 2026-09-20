using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;
using BattleForDune.Core.Schema;

namespace BattleForDune.Core.Components.Effects.Types
{
    public record class UpdateStatEffect(
        RegistryKey EffectKey,
        EntityId Id,
        EntityId OwnerId,
        EntityId SourceId,
        byte AbilityId,
        StatType Stat,
        Stat Delta)
        : IGenericEffect
    {
        public record class Prototype(RegistryKey Key, StatType Stat, Stat Delta) 
            : IGenericEffect.IEffectPrototype
        {
            public IGenericEffect Materialize(
                EntityId id, EntityId ownerId, EntityId sourceId, byte abilityId)
            {
                return new UpdateStatEffect(Key, id, ownerId, sourceId, abilityId, Stat, Delta);
            }
        }

        public ReadOnlySpan<GameEventType> SupportedEvents => [];
        public bool IsMergeable => false;
        public byte Stacks => 1;
        public byte Power => 1;

        public void Apply<TSink>(ComponentWorld world, in TSink sink) where TSink : IGameEventSink
        {
            sink.Queue(new UpdateStatEvent(SourceId, AbilityId, OwnerId, Stat, Delta, isReversal: false));
        }

        public void Remove<TSink>(ComponentWorld world, in TSink sink) where TSink : IGameEventSink
        {
            sink.Queue(new UpdateStatEvent(SourceId, AbilityId, OwnerId, Stat, Delta, isReversal: true));
        }

        public void MergeStacks(byte newStacks) { }

        public void MergePower(byte newPower) { }
    }
}
