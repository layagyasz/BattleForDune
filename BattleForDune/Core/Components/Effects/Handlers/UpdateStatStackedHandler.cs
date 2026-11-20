using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;
using BattleForDune.Core.Schema;

namespace BattleForDune.Core.Components.Effects.Handlers
{
    public record class UpdateStatStackedHandler(StatType Stat, Stat Delta, sbyte PerStack) 
        : IGenericEffectHandler
    {
        public void Apply<TSink>(Effect parent, ComponentWorld world, in TSink sink)
            where TSink : IGameEventSink
        {
            sink.Queue(
                UpdateStatEvent.Transitive(
                    parent.SourceId, 
                    parent.AbilityId, 
                    parent.OwnerId,
                    Stat,
                    Multiply(PerStack * parent.Stacks, Delta)));
        }

        public void Remove<TSink>(Effect parent, ComponentWorld world, in TSink sink)
            where TSink : IGameEventSink
        {
            sink.Queue(
                UpdateStatEvent.TransitiveReversal(
                    parent.SourceId,
                    parent.AbilityId, 
                    parent.OwnerId,
                    Stat,
                    Multiply(PerStack * parent.Stacks, Delta)));
        }

        private static Stat Multiply(int factor, Stat stat)
        {
            return new(
                (sbyte)(factor * stat.Value), (sbyte)(factor * stat.Temporary), (sbyte)(factor * stat.Max));
        }
    }
}
