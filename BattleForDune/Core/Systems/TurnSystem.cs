using BattleForDune.Core.Components;
using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;

namespace BattleForDune.Core.Systems
{
    public readonly struct TurnSystem(Random random) : IGameSystem
    {
        private readonly Random _random = random;

        public readonly void Run<TSink>(ComponentWorld world, in TSink sink) where TSink : IGameEventSink
        {
            foreach (var unitId in world.GetUnits().AsSpan())
            {
                DoTurn(new(unitId), world, sink);
            }
        }

        private void DoTurn<TSink>(EntityId unitId, ComponentWorld world, TSink sink) 
            where TSink : IGameEventSink
        {
            // TODO: Implement searching for best target by
            // 1) looking at friend/enemy classification of target and ability
            // 2) ability target comparator.
            if (world.GetAbilities().TryGet(unitId, out var _))
            {
                var target = world.GetUnits().AsSpan()[_random.Next(world.GetUnits().Count)];
                sink.Queue(new UseAbilityEvent(unitId, 0, new(target)));
            }
            sink.Flush();
        }
    }
}
