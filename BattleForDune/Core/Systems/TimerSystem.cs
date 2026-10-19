using BattleForDune.Core.Components;
using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;

namespace BattleForDune.Core.Systems
{
    public readonly struct TimerSystem : IGameSystem
    {
        public readonly void Run<TSink>(ComponentWorld world, in TSink sink) where TSink : IGameEventSink
        {
            sink.Queue(new TickEvent());
            sink.Flush();
            while (true)
            {
                if (world.Timers.Peek(out var timer) && timer.Turn <= world.Turn)
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
