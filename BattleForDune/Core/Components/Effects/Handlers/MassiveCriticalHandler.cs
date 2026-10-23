using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;
using BattleForDune.Core.Schema;

namespace BattleForDune.Core.Components.Effects.Handlers
{
    public record class MassiveCriticalHandler(Die Die) : IEffectHandler<DamageEvent>
    {
        public void Apply<TSink>(Effect parent, ComponentWorld world, in TSink sink)
            where TSink : IGameEventSink
        { }

        public void Remove<TSink>(Effect parent, ComponentWorld world, in TSink sink)
            where TSink : IGameEventSink
        { }

        public void Intercept(Effect parent, ref DamageEvent @event, ComponentWorld world) { }

        public void Handle<TSink>(Effect parent, in DamageEvent @event, ComponentWorld world, in TSink sink)
            where TSink : IGameEventSink
        {
            if (@event.IsCanceled)
            {
                return;
            }
            if (@event.SubjectId != parent.OwnerId)
            {
                return;
            }
            if (!@event.IsCritical)
            {
                return;
            }
            {
                sink.Queue(
                    DamageEvent.Transitive(
                        @event.SubjectId,
                        @event.AbilityId,
                        @event.TargetId,
                        isPrimary: false,
                        isCritical: false,
                        @event.DamageType,
                        @event.Vector,
                        @event.Stat,
                        Die));
            }
        }
    }
}
