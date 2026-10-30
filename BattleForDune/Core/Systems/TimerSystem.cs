using BattleForDune.Core.Components;
using BattleForDune.Core.Components.Timers;
using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;

namespace BattleForDune.Core.Systems
{
    public readonly struct TimerSystem : IGameSystem
    {
        public readonly void Run<TSink>(ComponentWorld world, in TSink sink) where TSink : IGameEventSink
        {
            while (world.Timers.Peek(out var timer) && timer.Turn <= world.Turn)
            {
                switch (timer.Type)
                {
                    case GameTimerType.Aura:
                        sink.Queue(RemoveAuraEvent.Intransitive(timer.OwnerId));
                        sink.Flush();
                        break;
                    case GameTimerType.Effect:
                        sink.Queue(RemoveEffectEvent.Intransitive(timer.OwnerId));
                        sink.Flush();
                        break;
                }
            }
        }
    }
}
