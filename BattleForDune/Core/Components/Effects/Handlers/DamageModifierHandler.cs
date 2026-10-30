using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;
using BattleForDune.Core.Schema;
using BattleForDune.FastLookup;

namespace BattleForDune.Core.Components.Effects.Handlers
{
    public record class DamageModifierHandler(
        EffectDirection Direction,
        EnumSet<DamageType> DamageType,
        AbilityVector Vector,
        EnumSet<StatType> Stat,
        sbyte Modifier)
        : IEffectHandler<DamageEvent>
    {
        public void Apply<TSink>(Effect parent, ComponentWorld world, in TSink sink) 
            where TSink : IGameEventSink { }

        public void Remove<TSink>(Effect parent, ComponentWorld world, in TSink sink)
            where TSink : IGameEventSink { }

        public void Intercept(Effect parent, ref DamageEvent @event, ComponentWorld world)
        {
            if (!IGenericEffectHandler.HasDirection(in @event, Direction, parent.OwnerId))
            {
                return;
            }
            if (DamageType.Contains(@event.DamageType) 
                && (Vector & @event.Vector) != 0
                && Stat.Contains(@event.Stat))
            {
                @event.Roll += Modifier;
            }
        }

        public void Handle<TSink>(Effect parent, in DamageEvent @event, ComponentWorld world, in TSink sink)
            where TSink : IGameEventSink
        { }
    }
}
