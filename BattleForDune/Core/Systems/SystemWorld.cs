using BattleForDune.Core.Components;
using BattleForDune.Core.Events;

namespace BattleForDune.Core.Systems
{
    public readonly struct SystemWorld(int delay, Random random) : IGameSystem
    {
        private readonly InitiativeSystem _initiative = new();
        private readonly TurnSystem _turn = new(delay, random);

        public readonly void Run<TSink>(ComponentWorld world, in TSink sink) where TSink : IGameEventSink
        {
            _initiative.Run(world, sink);
            _turn.Run(world, sink);
        }
    }
}
