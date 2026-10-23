using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;
using BattleForDune.Core.Schema;

namespace BattleForDune.Core.Components.Effects.Handlers
{
    public record class UpdateStatHandler(StatType Stat, Stat Delta)
        : IGenericEffectHandler
    {
        public void Apply<TSink>(Effect parent, ComponentWorld world, in TSink sink) 
            where TSink : IGameEventSink
        {
            sink.Queue(
                UpdateStatEvent.Transitive(
                    parent.SourceId, parent.AbilityId, parent.OwnerId, Stat, Delta));
        }

        public void Remove<TSink>(Effect parent, ComponentWorld world, in TSink sink) 
            where TSink : IGameEventSink
        {
            sink.Queue(
                UpdateStatEvent.TransitiveReversal(
                    parent.SourceId, parent.AbilityId, parent.OwnerId, Stat, Delta));
        }
    }
}
