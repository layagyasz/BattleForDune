using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;
using BattleForDune.Core.Schema;
using BattleForDune.FastLookup;

namespace BattleForDune.Core.Components.Effects.Types
{
    public record class DamageImmunityEffect(
        RegistryKey EffectKey,
        EntityId Id,
        EntityId OwnerId,
        EntityId SourceId,
        byte AbilityId,
        EnumSet<DamageType> DamageType,
        DamageVector DamageVector,
        EnumSet<StatType> Stat)
        : IGenericEffect, IEffect<DamageEvent>
    {
        public record class Prototype(
            RegistryKey Key, EnumSet<DamageType> DamageType, DamageVector DamageVector, EnumSet<StatType> Stat)
            : IGenericEffect.IEffectPrototype
        {
            public IGenericEffect Materialize(
                EntityId id, EntityId ownerId, EntityId sourceId, byte abilityId)
            {
                return new DamageImmunityEffect(
                    Key, id, ownerId, sourceId, abilityId, DamageType, DamageVector, Stat);
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
            if (@event.TargetId == OwnerId
                && DamageType.Contains(@event.DamageType)
                && DamageVector.HasFlag(@event.DamageVector) 
                && Stat.Contains(@event.Stat))
            {
                @event.IsCanceled = true;
            }
        }

        public void Handle<TSink>(in DamageEvent @event, ComponentWorld world, in TSink sink)
            where TSink : IGameEventSink
        { }
    }
}
