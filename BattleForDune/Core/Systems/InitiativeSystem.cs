using BattleForDune.Core.Components;
using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;

namespace BattleForDune.Core.Systems
{
    public class InitiativeSystem : IGameSystem
    {
        public void Run<TSink>(ComponentWorld world, in TSink sink) where TSink : IGameEventSink
        {
            sink.Queue(new TickEvent());
            foreach (var id in world.GetUnitIds())
            {
                var entityId = new EntityId(id);
                ref var stats = ref world.TryGetStats(entityId, out bool isFound);
                if (!isFound)
                {
                    continue;
                }
                sink.Queue(new InitiativeEvent(entityId, stats.Initiative.Value));
            }
            sink.Queue(new FinalizeInitiativeEvent());
            sink.Flush();
        }
    }
}
