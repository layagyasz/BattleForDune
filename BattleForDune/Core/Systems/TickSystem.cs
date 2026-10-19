using BattleForDune.Core.Components;
using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;

namespace BattleForDune.Core.Systems
{
    public readonly struct TickSystem : IGameSystem
    {
        public readonly void Run<TSink>(ComponentWorld world, in TSink sink) where TSink : IGameEventSink
        {
            sink.Queue(new TickEvent());
            sink.Flush();
        }
    }
}
