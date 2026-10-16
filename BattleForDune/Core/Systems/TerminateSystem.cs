using BattleForDune.Core.Components;
using BattleForDune.Core.Components.Behaviors;
using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;

namespace BattleForDune.Core.Systems
{
    public readonly struct TerminateSystem : IGameSystem
    {
        private const byte NULL_TEAM = 255;

        public readonly void Run<TSink>(ComponentWorld world, in TSink sink) where TSink : IGameEventSink
        {
            // TODO: also consider round limit

            byte firstTeam = NULL_TEAM;
            foreach (var unitId in world.Units.AsSpan())
            {
                ref var team = ref world.Teams.TryGet(new(unitId), out var isFound);
                if (!isFound)
                {
                    continue;
                }

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
