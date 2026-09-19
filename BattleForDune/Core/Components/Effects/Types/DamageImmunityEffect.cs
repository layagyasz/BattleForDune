using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;
using BattleForDune.Core.Schema;

namespace BattleForDune.Core.Components.Effects.Types
{
    public record class DamageImmunityEffect(
        RegistryKey EffectKey,
        EntityId Id,
        EntityId OwnerId,
        EntityId SourceId,
        EntityId AbilityId,
        DamageType DamageType,
        DamageVector DamageVector,
        byte StatMask)
        : IGenericEffect, IEffect<DamageEvent>
    {
        public record class Prototype(
            RegistryKey Key, DamageType DamageType, DamageVector DamageVector, byte StatMask)
            : IGenericEffect.IEffectPrototype
        {
            public IGenericEffect Materialize(
                EntityId id, EntityId ownerId, EntityId sourceId, EntityId abilityId)
            {
                return new DamageImmunityEffect(
                    Key, id, ownerId, sourceId, abilityId, DamageType, DamageVector, StatMask);
            }
        }

        private readonly GameEventType[] _supportedEvents = [GameEventType.Initiative];
        public ReadOnlySpan<GameEventType> SupportedEvents => _supportedEvents.AsSpan();
        public bool IsMergeable => false;
        public byte Stacks => 1;
        public byte Power => 1;

        public void Apply<TSink>(ComponentWorld world, in TSink sink) where TSink : IGameEventSink { }

        public void Remove<TSink>(ComponentWorld world, in TSink sink) where TSink : IGameEventSink { }

        public void MergeStacks(byte newStacks) { }

        public void MergePower(byte newPower) { }

        public void Intercept(ref DamageEvent @event, ComponentWorld world)
        {
            if (@event.DamageType == DamageType 
                && (@event.DamageVector & DamageVector) != 0 
                && ((1 << (byte)@event.Stat) & StatMask) != 0)
            {
                @event.IsCanceled = true;
            }
        }

        public void Handle<TSink>(in DamageEvent @event, ComponentWorld world, in TSink sink)
            where TSink : IGameEventSink
        { }
    }
}
