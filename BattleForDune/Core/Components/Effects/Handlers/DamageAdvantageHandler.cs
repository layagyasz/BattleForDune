using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;
using BattleForDune.Core.Schema;
using BattleForDune.FastLookup;

namespace BattleForDune.Core.Components.Effects.Handlers
{
    public record class DamageAdvantageHandler(
        EnumSet<DamageType> DamageType,
        DamageVector DamageVector,
        sbyte Advantage)
        : IEffectHandler<DamageEvent>
    {
        public record class Prototype(
            EnumSet<DamageType> DamageType, DamageVector DamageVector, sbyte Advantage)
            : IGenericEffectHandler.IHandlerPrototype
        {
            public IGenericEffectHandler Materialize()
            {
                return new DamageAdvantageHandler(DamageType, DamageVector, Advantage);
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
                && DamageVector.HasFlag(@event.DamageVector))
            {
                @event.Die.Advantage += Advantage;
            }
        }

        public void Handle<TSink>(Effect parent, in DamageEvent @event, ComponentWorld world, in TSink sink)
            where TSink : IGameEventSink
        { }
    }
}
