using BattleForDune.Core.Components;
using BattleForDune.Core.Components.Behaviors;
using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;

namespace BattleForDune.Core.Systems
{
    public readonly struct TerminateSystem(byte turnLimit) : IGameSystem
    {
        private const byte NULL_TEAM = 255;

        private readonly byte _turnLimit = turnLimit;

        public readonly void Run<TSink>(ComponentWorld world, in TSink sink) where TSink : IGameEventSink
        {
            if (world.Turn >= _turnLimit)
            {
                sink.Queue(new TerminateEvent(NULL_TEAM));
                sink.Flush();
            }

            byte firstTeam = NULL_TEAM;
            foreach (var unitId in world.Units.AsSpan())
            {
                ref var allegiance = ref world.Allegiances.TryGet(new(unitId), out var isFound);
                if (!isFound)
                {
                    continue;
                }
                var team = allegiance.TrueTeam;

                ref var behavior = ref world.Behaviors.TryGet(new(unitId), out isFound);
                if (!isFound || behavior.Mode == BehaviorMode.Broken)
                {
                    continue;
                }

                if (firstTeam == NULL_TEAM)
                {
                    firstTeam = team;
                }
                else if (team != firstTeam)
                {
                    // Found a second active team.
                    return;
                }
            }
            // At most one active team found.
            sink.Queue(new TerminateEvent(firstTeam));
            sink.Flush();
        }
    }
}
