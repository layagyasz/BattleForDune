using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;
using BattleForDune.Core.Schema;

namespace BattleForDune.Core.Components.Effects.Types
{
    public record class FastAttackEffect(
        RegistryKey EffectKey,
        EntityId Id,
        EntityId OwnerId,
        EntityId SourceId,
        EntityId AbilityId)
        : IGenericEffect, IEffect<InitiativeEvent>
    {
        public record class Prototype(RegistryKey Key) : IGenericEffect.IEffectPrototype
        {
            public IGenericEffect Materialize(
                EntityId id, EntityId ownerId, EntityId sourceId, EntityId abilityId)
            {
                return new FastAttackEffect(Key, id, ownerId, sourceId, abilityId);
            }
        }

        private readonly GameEventType[] _supportedEvents = [GameEventType.Initiative];
        public ReadOnlySpan<GameEventType> SupportedEvents => _supportedEvents.AsSpan();
        public byte Stacks { get; internal set; }
        public byte Power { get; internal set; }

        public void Apply<TSink>(ComponentWorld world, in TSink sink) where TSink : IGameEventSink { }

        public void Remove<TSink>(ComponentWorld world, in TSink sink) where TSink : IGameEventSink { }

        public void MergeStacks(byte newStacks)
        {
            Stacks += newStacks;
        }

        public void MergePower(byte newPower)
        {
            Power = Math.Max(Power, newPower);
        }

        public void Intercept(ref InitiativeEvent @event, ComponentWorld world)
        {
            if (@event.SubjectId != OwnerId)
            {
                return;
            }
            @event.Advantage += 1;
        }

        public void Handle<TSink>(in InitiativeEvent @event, ComponentWorld world, in TSink sink)
            where TSink : IGameEventSink
        { }
    }
}
