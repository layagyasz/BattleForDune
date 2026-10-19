using BattleForDune.Core.Components;
using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;

namespace BattleForDune.Core.Systems
{
    public readonly struct ActionSystem : IGameSystem
    {
        public readonly void Run<TSink>(ComponentWorld world, in TSink sink) where TSink : IGameEventSink
        {
            foreach (var id in world.Units.AsSpan())
            {
                var entityId = new EntityId(id);
                ref var actions = ref world.Actions.TryGet(entityId, out bool isFound);
                if (!isFound)
                {
                    continue;
                }
                sink.Queue(new ResetActionsEvent(entityId, actions.AttacksLimit, actions.MovesLimit));
            }
            sink.Flush();
        }
    }
}
