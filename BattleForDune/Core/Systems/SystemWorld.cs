using BattleForDune.Core.Components;
using BattleForDune.Core.Events;

namespace BattleForDune.Core.Systems
{
    public readonly struct SystemWorld(byte turnLimit, int delay, Random random) : IGameSystem
    {
        private readonly TickSystem _tick = new();
        private readonly InitiativeSystem _initiative = new();
        private readonly TurnSystem _turn = new(delay, random);
        private readonly TerminateSystem _terminate = new(turnLimit);

        public readonly void Run<TSink>(ComponentWorld world, in TSink sink) where TSink : IGameEventSink
        {
            _tick.Run(world, sink);
            _initiative.Run(world, sink);
            _turn.Run(world, sink);
            _terminate.Run(world, sink);
        }
    }
}
