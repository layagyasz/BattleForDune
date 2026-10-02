using BattleForDune.Core.Components;
using BattleForDune.Core.Components.Abilities;
using BattleForDune.Core.Components.Behaviors;
using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;
using BattleForDune.Core.Schema;
using BattleForDune.FastLookup;
using BattleForDune.Geometry;

namespace BattleForDune.Core.Systems
{
    public readonly struct TurnSystem(int delay, Random random) : IGameSystem
    {
        private ref struct TargetAccumulator(
            EntityId subjectId, 
            byte subjectTeam, 
            ComponentWorld world, 
            TargetType targetType, 
            ITargetComparator targetComparator, 
            Random random) : ICallback<int>
        {
            public EntityId SubjectId = subjectId;
            public byte SubjectTeam = subjectTeam;
            public ComponentWorld World = world;
            public TargetType TargetType = targetType;
            public ITargetComparator TargetComparator = targetComparator;
            public Random Random = random;

            private EntityId _target;
            private int _considered;

            public readonly EntityId GetTarget()
            {
                return _target;
            }

            public void Notify(int handle, ref int value)
            {
                var option = new EntityId(value);
                var targetType = GetTargetType(SubjectId, SubjectTeam, option, World);
                if ((targetType & TargetType) != 0)
                {
                    if (_target == EntityId.None)
                    {
                        _target = option;
                        _considered = 0;
                        return;
                    }
                    var comparison = TargetComparator.Compare(World, SubjectId, _target, option);
                    if (comparison < 0)
                    {
                        _target = option;
                        _considered = 0;
                    }
                    else if (comparison == 0 && Random.Next(++_considered + 1) == 1)
                    {
                        _target = option;
                    }
                }
            }
        };

        private readonly int _delay = delay;
        private readonly Random _random = random;

        public readonly void Run<TSink>(ComponentWorld world, in TSink sink) where TSink : IGameEventSink
        {
            foreach (var unitId in world.Inititiative.AsSpan())
            {
                DoTurn(unitId, world, sink);
                sink.Flush();
                Thread.Sleep(_delay);
            }
        }

        private void DoTurn<TSink>(EntityId unitId, ComponentWorld world, TSink sink) 
            where TSink : IGameEventSink
        {
            var team = world.Teams.TryGet(unitId, out bool isFound);
            if (!isFound)
            {
                return;
            }

            var behavior = world.Behaviors.TryGet(unitId, out isFound);
            if (!isFound)
            {
                return;
            }

            if (behavior.Mode == BehaviorMode.Broken)
            {
                DoMove(unitId, team, -1f, world, sink);
                return;
            }

            if (DoAbility(unitId, team, world, sink))
            {
                return;
            }

            if (DoMove(unitId, team, 1f, world, sink))
            {
                return;
            }
        }

        private bool DoAbility<TSink>(EntityId unitId, byte team, ComponentWorld world, TSink sink)
            where TSink : IGameEventSink
        {
            if (world.Abilities.TryGet(unitId, out var abilities))
            {
                for (int i = 0; i < abilities.Length; ++i)
                {
                    var target = FindTarget(unitId, team, abilities[i], world);
                    if (target != EntityId.None)
                    {
                        sink.Queue(new UseAbilityEvent(unitId, 0, target));
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool DoMove<TSink>(
            EntityId unitId, byte team, float direction, ComponentWorld world, TSink sink)
            where TSink : IGameEventSink
        {
            ref var position = ref world.Positions.TryGet(unitId, out var isFound);
            if (!isFound)
            {
                return false;
            }
            var target = FindMoveTarget(unitId, position, team, world);
            if (target == EntityId.None)
            {
                return false;
            }

            var targetPosition = world.Positions.TryGet(target, out isFound);
            if (!isFound)
            {
                return false;
            }

            // TODO: Use depth limited a* for path finding
            var distance = Hexes.Distance(position, targetPosition);
            var move = Hexes.Lerp(position, targetPosition, direction / distance);
            if (world.Positions.IsOccupied(move))
            {
                return false;
            }

            sink.Queue(new MoveEvent(unitId, position, move));
            return true;
        }

        private static EntityId FindMoveTarget(
            EntityId subjectId, Hex2b position, byte subjectTeam, ComponentWorld world)
        {
            // TODO: Use quad tree find nearest
            EntityId target = EntityId.None;
            int distance = 0;
            foreach (var option in world.Units.AsSpan())
            {
                var optionId = new EntityId(option);
                var optionPosition = world.Positions.TryGet(optionId, out var isFound);
                if (!isFound)
                {
                    continue;
                }
                if (GetTargetType(subjectId, subjectTeam, optionId, world) != TargetType.Enemy)
                {
                    continue;
                }

                var d = Hexes.Distance(position, optionPosition);
                if (target == EntityId.None || d < distance)
                {
                    target = optionId;
                    distance = d;
                }
            }
            return target;
        }

        private EntityId FindTarget(EntityId subjectId, byte subjectTeam, Ability ability, ComponentWorld world)
        {
            ref var position = ref world.Positions.TryGet(subjectId, out var isFound);
            if (!isFound)
            {
                return EntityId.None;
            }
            if (ability.TargetType == TargetType.Self)
            {
                return subjectId;
            }
            var accumulator =
                new TargetAccumulator(
                    subjectId,
                    subjectTeam,
                    world,
                    ability.TargetType,
                    ability.TargetComparator,
                    _random);
            if (ability.Range < 0)
            {
                foreach (var option in world.Units.AsSpan())
                {
                    var refOption = option;
                    accumulator.Notify(0, ref refOption);
                }
            }
            else
            {
                world.Positions.QueryRange(position, (byte)ability.Range, ref accumulator);
            }
            return accumulator.GetTarget();
        }

        private static TargetType GetTargetType(
            EntityId subjectId, byte subjectTeam, EntityId targetId, ComponentWorld world)
        {
            if (subjectId == targetId)
            {
                return TargetType.Self;
            }
            var targetTeam = world.Teams.TryGet(targetId, out bool isFound);
            if (!isFound)
            {
                return TargetType.None;
            }
            return subjectTeam == targetTeam ? TargetType.Friend : TargetType.Enemy;
        }
    }
}
