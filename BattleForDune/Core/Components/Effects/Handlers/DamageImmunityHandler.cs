using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;
using BattleForDune.Core.Schema;
using BattleForDune.FastLookup;

namespace BattleForDune.Core.Components.Effects.Handlers
{
    public record class DamageImmunityHandler(
        EnumSet<DamageType> DamageType,
        DamageVector DamageVector,
        EnumSet<StatType> Stat)
        : IEffectHandler<DamageEvent>
    {
        public record class Prototype(
            RegistryKey Key, EnumSet<DamageType> DamageType, DamageVector DamageVector, EnumSet<StatType> Stat)
            : IGenericEffectHandler.IHandlerPrototype
        {
            public IGenericEffectHandler Materialize()
            {
                return new DamageImmunityHandler(DamageType, DamageVector, Stat);
            }
        }

        public void Apply<TSink>(Effect parent, ComponentWorld world, in TSink sink) 
            where TSink : IGameEventSink { }

        public void Remove<TSink>(Effect parent, ComponentWorld world, in TSink sink)
            where TSink : IGameEventSink { }

        public void Intercept(Effect parent, ref DamageEvent @event, ComponentWorld world)
        {
            if (@event.TargetId == parent.OwnerId
                && DamageType.Contains(@event.DamageType)
                && DamageVector.HasFlag(@event.DamageVector) 
                && Stat.Contains(@event.Stat))
            {
                @event.IsCanceled = true;
            }
        }

        public void Handle<TSink>(Effect parent, in DamageEvent @event, ComponentWorld world, in TSink sink)
            where TSink : IGameEventSink
        { }
    }
}
