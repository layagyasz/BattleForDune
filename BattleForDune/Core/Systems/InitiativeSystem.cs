using BattleForDune.Core.Components;
using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;

namespace BattleForDune.Core.Systems
{
    public class InitiativeSystem : IGameSystem
    {
        public void Run<TSink>(ComponentWorld world, in TSink sink) where TSink : IGameEventSink
        {
            // TODO: Systems should only mutate the world through events.
            var component = world.GetInitiative();
            component.Clear();
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
            sink.Flush();
            // TODO: Systems should only mutate the world through events.
            component.Sort();
        }
    }
}
