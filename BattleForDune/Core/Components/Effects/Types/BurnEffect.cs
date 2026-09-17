using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;
using BattleForDune.Core.Schema;

namespace BattleForDune.Core.Components.Effects.Types
{
    public record class BurnEffect(
        RegistryKey EffectKey,
        EntityId Id, 
        EntityId OwnerId, 
        EntityId SourceId,
        EntityId AbilityId)
        : IGenericEffect, IEffect<TickEvent>
    {
        public record class Prototype(RegistryKey Key) : IGenericEffect.IEffectPrototype
        {
            public IGenericEffect Materialize(
                EntityId id, EntityId ownerId, EntityId sourceId, EntityId abilityId)
            {
                return new BurnEffect(Key, id, ownerId, sourceId, abilityId);
            }
        }

        private readonly GameEventType[] _supportedEvents = [GameEventType.Tick];
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

        public void Intercept(ref TickEvent @event, ComponentWorld world) { }

        public void Handle<TSink>(in TickEvent @event, ComponentWorld world, in TSink sink)
            where TSink : IGameEventSink
        {
            sink.Queue(
                new DamageEvent(
                    SourceId,
                    AbilityId,
                    OwnerId,
                    isPrimary: false,
                    isCritical: false,
                    DamageType.Energy,
                    new(0, 0, (sbyte)Power),
                    0,
                    0,
                    0,
                    0));
        }
    }
}
