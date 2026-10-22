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

        private readonly ref struct TargetPredicate(
            EntityId subjectId, byte subjectTeam, ComponentWorld world, TargetType targetType) : IPredicate<int>
        {
            private readonly EntityId _subjectId = subjectId;
            private readonly byte _subjectTeam = subjectTeam;
            private readonly ComponentWorld _world = world;
            private readonly TargetType _targetType = targetType;

            public readonly bool Matches(in int targetId)
            {
                return GetTargetType(_subjectId, _subjectTeam, new(targetId), _world) == _targetType;
            }
        }

        private readonly int _delay = delay;
        private readonly Random _random = random;

        public readonly void Run<TSink>(ComponentWorld world, in TSink sink) where TSink : IGameEventSink
        {
            foreach (var unitId in world.Inititiative.AsSpan())
            {
                DoTurn(unitId, world, sink);
                Thread.Sleep(_delay);
            }
        }

        private void DoTurn<TSink>(EntityId unitId, ComponentWorld world, TSink sink) 
            where TSink : IGameEventSink
        {
            ref var allegiance = ref world.Allegiances.TryGet(unitId, out bool isFound);
            if (!isFound)
            {
                return;
            }
            var team = allegiance.Team;

            var behavior = world.Behaviors.TryGet(unitId, out isFound);
            if (!isFound)
            {
                return;
            }

            ref var actions = ref world.Actions.TryGet(unitId, out isFound);
            if (!isFound)
            {
                return;
            }

            if (behavior.Mode == BehaviorMode.Broken)
            {
                while (actions.MovePoints > 0 && DoMove(unitId, team, -1f, world, sink))
                {
                    sink.Flush();
                }
            }
            else
            {
                while ((actions.AttackPoints > 0 && DoAbility(unitId, team, world, sink))
                    || (actions.AttackPoints > 0 
                        && actions.MovePoints > 0 
                        && DoMove(unitId, team, 1f, world, sink)))
                {
                    sink.Flush();
                }
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
            if (!world.Map.Contains(move) || world.Positions.IsOccupied(move))
            {
                return false;
            }

            sink.Queue(new MoveEvent(unitId, position, move));
            return true;
        }

        private static EntityId FindMoveTarget(
            EntityId subjectId, Hex2b position, byte subjectTeam, ComponentWorld world)
        {
            var predicate = new TargetPredicate(subjectId, subjectTeam, world, TargetType.Enemy);
            world.Positions.QueryNearest(position, ref predicate, out var target);
            return new(target);
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
            ref var targetAllegiance = ref world.Allegiances.TryGet(targetId, out bool isFound);
            if (!isFound)
            {
                return TargetType.None;
            }
            return subjectTeam == targetAllegiance.Team ? TargetType.Friend : TargetType.Enemy;
        }
    }
}
