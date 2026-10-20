using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;
using BattleForDune.Core.Schema;

namespace BattleForDune.Core.Components.Effects.Handlers
{
    public record class KillOutActionPointsHandler(
        BooleanFlag IsPrimaryDamage, sbyte ActionPoints, sbyte MovePoints) 
        : IEffectHandler<KillEvent>
    {
        public void Apply<TSink>(Effect parent, ComponentWorld world, in TSink sink)
            where TSink : IGameEventSink
        { }

        public void Remove<TSink>(Effect parent, ComponentWorld world, in TSink sink)
            where TSink : IGameEventSink
        { }

        public void Intercept(Effect parent, ref KillEvent @event, ComponentWorld world) { }

        public void Handle<TSink>(Effect parent, in KillEvent @event, ComponentWorld world, in TSink sink)
            where TSink : IGameEventSink
        {
            if (@event.SubjectId == parent.OwnerId && IsPrimaryDamage.HasFlag(@event.IsPrimaryDamage))
            {
                sink.Queue(
                    new UpdateActionsEvent(
                        @event.SubjectId, @event.AbilityId, @event.SubjectId, ActionPoints, MovePoints));
            }
        }
    }
}
