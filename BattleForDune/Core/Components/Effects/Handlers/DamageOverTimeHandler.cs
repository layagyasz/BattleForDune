using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;
using BattleForDune.Core.Schema;

namespace BattleForDune.Core.Components.Effects.Handlers
{
    public record class DamageOverTimeHandler(
        DamageType DamageType,
        AbilityVector Vector)
        : IEffectHandler<TickEvent>
    {
        public void Apply<TSink>(Effect parent, ComponentWorld world, in TSink sink) 
            where TSink : IGameEventSink { }

        public void Remove<TSink>(Effect parent, ComponentWorld world, in TSink sink) 
            where TSink : IGameEventSink { }

        public void Intercept(Effect parent, ref TickEvent @event, ComponentWorld world) { }

        public void Handle<TSink>(Effect parent, in TickEvent @event, ComponentWorld world, in TSink sink)
            where TSink : IGameEventSink
        {
            sink.Queue(
                new DamageEvent(
                    parent.SourceId,
                    parent.AbilityId,
                    parent.OwnerId,
                    isPrimary: false,
                    isCritical: false,
                    DamageType,
                    Vector,
                    StatType.Hitpoints,
                    new(0, 0, (sbyte)parent.Power)));
        }
    }
}
