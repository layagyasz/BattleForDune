using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;
using BattleForDune.Core.Schema;

namespace BattleForDune.Core.Components.Effects.Handlers
{
    public record class EffectImmunityHandler(AbilityVector Vector) : IEffectHandler<AddEffectEvent>
    {
        public void Apply<TSink>(Effect parent, ComponentWorld world, in TSink sink)
            where TSink : IGameEventSink
        { }

        public void Remove<TSink>(Effect parent, ComponentWorld world, in TSink sink)
            where TSink : IGameEventSink
        { }

        public void Intercept(Effect parent, ref AddEffectEvent @event, ComponentWorld world)
        {
            if (@event.TargetId != parent.OwnerId)
            {
                return;
            }
            if ((Vector & @event.Vector) == 0)
            {
                return;
            }
            @event.IsCanceled = true;
        }

        public void Handle<TSink>(Effect parent, in AddEffectEvent @event, ComponentWorld world, in TSink sink)
            where TSink : IGameEventSink
        { }
    }
}
