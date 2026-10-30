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
                    case GameTimerType.Ability:
                        sink.Queue(CooldownEvent.Intransitive(timer.OwnerId));
                        break;
                    case GameTimerType.Aura:
                        sink.Queue(RemoveAuraEvent.Intransitive(timer.OwnerId));
                        break;
                    case GameTimerType.Effect:
                        sink.Queue(RemoveEffectEvent.Intransitive(timer.OwnerId));
                        break;
                }
                sink.Flush();
            }
        }
    }
}
