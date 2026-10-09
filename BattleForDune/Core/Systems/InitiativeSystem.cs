using BattleForDune.Core.Components;
using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;
using BattleForDune.Core.Schema;

namespace BattleForDune.Core.Systems
{
    public readonly struct InitiativeSystem : IGameSystem
    {
        public readonly void Run<TSink>(ComponentWorld world, in TSink sink) where TSink : IGameEventSink
        {
            foreach (var id in world.Units.AsSpan())
            {
                var entityId = new EntityId(id);
                ref var stats = ref world.Stats.TryGet(entityId, out bool isFound);
                if (!isFound)
                {
                    continue;
                }
                sink.Queue(new InitiativeEvent(entityId, stats[StatType.Initiative].Total));
            }
            sink.Queue(new FinalizeInitiativeEvent());
            sink.Flush();
        }
    }
}
