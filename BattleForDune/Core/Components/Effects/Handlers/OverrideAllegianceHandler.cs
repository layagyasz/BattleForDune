using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;

namespace BattleForDune.Core.Components.Effects.Handlers
{
    public record class OverrideAllegianceHandler : IGenericEffectHandler
    {
        public void Apply<TSink>(Effect parent, ComponentWorld world, in TSink sink)
            where TSink : IGameEventSink
        {
            ref var allegiance = ref world.Allegiances.TryGet(parent.SourceId, out var isFound);
            if (!isFound)
            {
                return;
            }
            sink.Queue(
                OverrideAllegianceEvent.Transitive(
                    parent.SourceId, parent.AbilityId, parent.OwnerId, allegiance.Faction, allegiance.Team));
        }

        public void Remove<TSink>(Effect parent, ComponentWorld world, in TSink sink)
            where TSink : IGameEventSink
        {
            ref var allegiance = ref world.Allegiances.TryGet(parent.OwnerId, out var isFound);
            if (!isFound)
            {
                return;
            }
            sink.Queue(
                OverrideAllegianceEvent.Transitive(
                    parent.SourceId,
                    parent.AbilityId,
                    parent.OwnerId,
                    allegiance.TrueFaction,
                    allegiance.TrueTeam));
        }
    }
}
