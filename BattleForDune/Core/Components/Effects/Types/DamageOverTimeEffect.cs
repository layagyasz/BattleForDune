using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;
using BattleForDune.Core.Schema;

namespace BattleForDune.Core.Components.Effects.Types
{
    public record class DamageOverTimeEffect(
        RegistryKey EffectKey,
        EntityId Id, 
        EntityId OwnerId, 
        EntityId SourceId,
        EntityId AbilityId,
        DamageType DamageType,
        DamageVector DamageVector)
        : IGenericEffect, IEffect<TickEvent>
    {
        public record class Prototype(RegistryKey Key, DamageType DamageType, DamageVector DamageVector) 
            : IGenericEffect.IEffectPrototype
        {
            public IGenericEffect Materialize(
                EntityId id, EntityId ownerId, EntityId sourceId, EntityId abilityId)
            {
                return new DamageOverTimeEffect(
                    Key, id, ownerId, sourceId, abilityId, DamageType, DamageVector);
            }
        }

        private readonly GameEventType[] _supportedEvents = [GameEventType.Tick];
        public ReadOnlySpan<GameEventType> SupportedEvents => _supportedEvents.AsSpan();
        public bool IsMergeable => true;
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
                    DamageType,
                    DamageVector,
                    StatType.Hitpoints,
                    new(0, 0, (sbyte)Power, 0),
                    0));
        }
    }
}
