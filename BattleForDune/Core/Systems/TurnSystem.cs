using BattleForDune.Core.Components;
using BattleForDune.Core.Components.Abilities;
using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;
using BattleForDune.Core.Schema;

namespace BattleForDune.Core.Systems
{
    public readonly struct TurnSystem(int delay, Random random) : IGameSystem
    {
        private readonly int _delay = delay;
        private readonly Random _random = random;

        public readonly void Run<TSink>(ComponentWorld world, in TSink sink) where TSink : IGameEventSink
        {
            foreach (var unitId in world.GetInitiative().AsSpan())
            {
                DoTurn(unitId, world, sink);
            }
        }

        private void DoTurn<TSink>(EntityId unitId, ComponentWorld world, TSink sink) 
            where TSink : IGameEventSink
        {
            var team = world.GetTeams().TryGet(unitId, out bool isFound);
            if (!isFound)
            {
                return;
            }

            if (world.GetAbilities().TryGet(unitId, out var abilities))
            {
                for (int i = 0; i < abilities.Length; ++i)
                {
                    var target = FindTarget(unitId, team, abilities[i], world);
                    if (target != EntityId.None)
                    {
                        sink.Queue(new UseAbilityEvent(unitId, 0, target));
                        break;
                    }
                }
            }
            sink.Flush();
            Thread.Sleep(_delay);
        }

        private EntityId FindTarget(
            EntityId subjectId, byte subjectTeam, Ability ability, ComponentWorld world)
        {
            if (ability.TargetType == TargetType.Self)
            {
                return subjectId;
            }
            EntityId target = EntityId.None;
            int considered = 0;
            // TODO: Restrict targets to ability range.
            foreach (var optionId in world.GetUnits().AsSpan())
            {
                var option = new EntityId(optionId);
                var targetType = GetTargetType(subjectId, subjectTeam, option, world);
                if ((targetType & ability.TargetType) != 0)
                {
                    if (target == EntityId.None)
                    {
                        target = option;
                        considered = 0;
                        continue;
                    }
                    var comparison = ability.TargetComparator.Compare(world, subjectId, target, option);
                    if (comparison < 0)
                    {
                        target = option;
                        considered = 0;
                    }
                    else if (comparison == 0 && _random.Next(++considered + 1) == 1)
                    {
                        target = option;
                    }
                }
            }
            return target;
        }

        private static TargetType GetTargetType(
            EntityId subjectId, byte subjectTeam, EntityId targetId, ComponentWorld world)
        {
            if (subjectId == targetId)
            {
                return TargetType.Self;
            }
            var targetTeam = world.GetTeams().TryGet(targetId, out bool isFound);
            if (!isFound)
            {
                return TargetType.None;
            }
            return subjectTeam == targetTeam ? TargetType.Friend : TargetType.Enemy;
        }
    }
}
