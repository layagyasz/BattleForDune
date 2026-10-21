using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;

namespace BattleForDune.Core.Components.Effects.Handlers
{
    public record class SpawnOutAddEffectHandler(RegistryKey EffectKey, byte Stacks, byte Power, sbyte Duration) 
        : IEffectHandler<AddUnitEvent>
    {
        public void Apply<TSink>(Effect parent, ComponentWorld world, in TSink sink)
            where TSink : IGameEventSink
        { }

        public void Remove<TSink>(Effect parent, ComponentWorld world, in TSink sink)
            where TSink : IGameEventSink
        { }

        public void Intercept(Effect parent, ref AddUnitEvent @event, ComponentWorld world) { }

        public void Handle<TSink>(Effect parent, in AddUnitEvent @event, ComponentWorld world, in TSink sink)
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

            sink.Queue(
                new AddEffectEvent(
                    @event.SubjectId, 
                    @event.AbilityId,
                    @event.UnitId, 
                    Stacks, 
                    Power,
                    Duration,
                    EffectKey,
                    0, 
                    0));
        }
    }
}
