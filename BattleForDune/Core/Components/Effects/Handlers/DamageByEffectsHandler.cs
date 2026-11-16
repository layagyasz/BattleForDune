using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;
using BattleForDune.Core.Schema;
using BattleForDune.FastLookup;

namespace BattleForDune.Core.Components.Effects.Handlers
{
    public record class DamageByEffectsHandler(
        AbilityDirection Direction,
        EnumSet<DamageType> DamageType,
        AbilityVector Vector,
        EnumSet<StatType> Stat,
        AbilityVector EffectVector,
        sbyte PerInstance, 
        sbyte PerStack, 
        sbyte PerPower) 
        : IEffectHandler<DamageEvent>
    {
        public void Apply<TSink>(Effect parent, ComponentWorld world, in TSink sink)
            where TSink : IGameEventSink
        { }

        public void Remove<TSink>(Effect parent, ComponentWorld world, in TSink sink)
            where TSink : IGameEventSink
        { }

        public void Intercept(Effect parent, ref DamageEvent @event, ComponentWorld world)
        {
            if (!IGenericEffectHandler.HasDirection(in @event, Direction, parent.OwnerId))
            {
                return;
            }
            if (!@event.IsPrimary)
            {
                return;
            }
            if ((Vector & @event.Vector) == 0)
            {
                return;
            }
            if (!Stat.Contains(@event.Stat))
            {
                return;
            }
            if (!DamageType.Contains(@event.DamageType))
            {
                return;
            }

            int modifier = 0;
            foreach (var effect in world.Effects.ByOwner(@event.TargetId))
            {
                if ((EffectVector & effect.Vector) == 0)
                {
                    continue;
                }
                modifier += PerInstance;
                modifier += PerStack * effect.Stacks;
                modifier += PerPower * effect.Power;
            }

            @event.Roll += (sbyte)modifier;
        }

        public void Handle<TSink>(Effect parent, in DamageEvent @event, ComponentWorld world, in TSink sink)
            where TSink : IGameEventSink
        { }
    }
}
