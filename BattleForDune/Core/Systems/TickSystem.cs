using BattleForDune.Core.Components;
using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;

namespace BattleForDune.Core.Systems
{
    public struct TickSystem : IGameSystem
    {
        public readonly void Run<TSink>(ComponentWorld world, in TSink sink) where TSink : IGameEventSink
        {
            sink.Queue(new TickEvent());
            sink.Flush();
            while (true)
            {
                if (world.GetTimers().Peek(out var timer) && timer.Turn <= world.GetTurn())
                {
                    sink.Queue(new RemoveEffectEvent(EntityId.None, 0, timer.OwnerId));
                    sink.Flush();
                }
                else
                {
                    break;
                }
            }
        }
    }
}
