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

            ref var sourceAllegiance = ref world.Allegiances.TryGet(parent.SourceId, out var isFound);
            if (!isFound)
            {
                return;
            }

            ref var targetPosition = ref world.Positions.TryGet(@event.TargetId, out isFound);
            if (!isFound)
            {
                return;
            }

            ref var targetMetadata = ref world.UnitMetadatas.TryGet(@event.TargetId, out isFound);
            if (!isFound)
            {
                return;
            }

            sink.Queue(
                AddUnitEvent.Transitive(
                    parent.SourceId, 
                    parent.AbilityId, 
                    UnitKey, 
                    targetMetadata.Level,
                    sourceAllegiance.TrueFaction,
                    sourceAllegiance.TrueTeam,
                    targetPosition));
        }
    }
}
