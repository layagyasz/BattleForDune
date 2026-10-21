using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;

namespace BattleForDune.Core.Components.Effects.Handlers
{
    public record class KillInSpawnHandler(RegistryKey UnitKey)
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
            if (@event.IsCanceled)
            {
                return;
            }
            if (@event.TargetId != parent.OwnerId)
            {
                return;
            }

            ref var sourceTeam = ref world.Teams.TryGet(parent.SourceId, out var isFound);
            if (!isFound)
            {
                return;
            }

            ref var sourceFaction = ref world.Factions.TryGet(parent.SourceId, out isFound);
            if (!isFound)
            {
                return;
            }

            ref var targetPosition = ref world.Positions.TryGet(@event.TargetId, out isFound);
            if (!isFound)
            {
                return;
            }

            // TODO: copy level from target?
            sink.Queue(
                new AddUnitEvent(
                    parent.SourceId, 
                    parent.AbilityId, 
                    UnitKey, 
                    0,
                    sourceFaction, 
                    sourceTeam,
                    targetPosition));
        }
    }
}
