using BattleForDune.Core.Components;
using BattleForDune.Core.Components.Abilities;
using BattleForDune.Core.Components.Behaviors;
using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;
using BattleForDune.Core.Schema;

namespace BattleForDune.Core
{
    public class LocalEventProcessor(ComponentWorld world, IGameEventSink[] listeners) : IGameEventProcessor
    {
        private readonly ComponentWorld _world = world;
        private readonly IGameEventSink[] _listeners = listeners;

        public void Process<TSink>(ref GameEvent @event, TSink sink) where TSink : IGameEventSink
        {
            switch (@event.Type)
            {
                case GameEventType.AddEffect:
                    var effect = @event.ReadPayload<AddEffectEvent>();
                    Process(ref effect, sink);
                    Notify(in effect);
                    return;
                case GameEventType.AddUnit:
                    var spawn = @event.ReadPayload<AddUnitEvent>();
                    Process(ref spawn, sink);
                    Notify(in spawn);
                    return;
                case GameEventType.Attack:
                    var attack = @event.ReadPayload<AttackEvent>();
                    Process(ref attack, sink);
                    Notify(in attack);
                    return;
                case GameEventType.Break:
                    var @break = @event.ReadPayload<BreakEvent>();
                    Process(ref @break, sink);
                    Notify(in @break);
                    return;
                case GameEventType.Damage:
                    var damage = @event.ReadPayload<DamageEvent>();
                    Process(ref damage, sink);
                    Notify(in damage);
                    return;
                case GameEventType.FinalizeInitiative:
                    var finalize = @event.ReadPayload<FinalizeInitiativeEvent>();
                    Process(ref finalize, sink);
                    Notify(in finalize);
                    return;
                case GameEventType.Initiative:
                    var initiative = @event.ReadPayload<InitiativeEvent>();
                    Process(ref initiative, sink);
                    Notify(in initiative);
                    return;
                case GameEventType.Kill:
                    var kill = @event.ReadPayload<KillEvent>();
                    Process(ref kill, sink);
                    Notify(in kill);
                    return;
                case GameEventType.Move:
                    var move = @event.ReadPayload<MoveEvent>();
                    Process(ref move, sink);
                    Notify(in move);
                    return;
                case GameEventType.RemoveEffect:
                    var removeEffect = @event.ReadPayload<RemoveEffectEvent>();
                    Process(ref removeEffect, sink);
                    Notify(in removeEffect);
                    return;
                case GameEventType.Tick:
                    var tick = @event.ReadPayload<TickEvent>();
                    Process(ref tick, sink);
                    Notify(in tick);
                    return;
                case GameEventType.UpdateStat:
                    var stat = @event.ReadPayload<UpdateStatEvent>();
                    Process(ref stat, sink);
                    Notify(in stat);
                    return;
                case GameEventType.UseAbility:
                    var use = @event.ReadPayload<UseAbilityEvent>();
                    Process(ref use, sink);
                    Notify(in use);
                    return;
            }
        }

        private void ApplyAddEffect<TSink>(in AddEffectEvent @event, TSink sink) where TSink : IGameEventSink
        {
            if (@event.IsCanceled)
            {
                return;
            }

            // Find an existing effect to merge with.
            if (_world.Registry.GetEffectPrototype(@event.EffectKey).IsMergeable)
            {
                foreach (var effect in _world.Effects.ByOwner(@event.TargetId))
                {
                    if (effect.EffectKey != @event.EffectKey)
                    {
                        continue;
                    }

                    effect.Remove(_world, in sink);
                    effect.MergeStacks(@event.Stacks);
                    effect.MergePower(@event.Power);
                    effect.Apply(_world, in sink);

                    if (@event.Duration >= 0)
                    {
                        byte newExpiration = (byte)(_world.Turn + @event.Duration);
                        if (_world.Timers.TryGet(effect.Id, out var currentExpiration)
                            && newExpiration > currentExpiration)
                        {
                            _world.Timers.Remove(effect.Id);
                            _world.Timers.Add(effect.Id, newExpiration);
                        }
                    }
                    else
                    {
                        _world.Timers.Remove(effect.Id);
                    }
                    return;
                }
            }

            // If no matching effects exist, add a new one.
            var newEffect = 
                _world.Registry.GetEffectPrototype(@event.EffectKey)
                    .Materialize(@event.InstanceId, @event.TargetId, @event.SubjectId, @event.AbilityId);
            newEffect.MergeStacks(@event.Stacks);
            newEffect.MergePower(@event.Power);
            _world.Effects.Add(newEffect);
            newEffect.Apply(_world, in sink);
            if (@event.Duration >= 0)
            {
                _world.Timers.Add(newEffect.Id, (byte)(_world.Turn + @event.Duration));
            }
        }

        private void ApplyAddUnit<TSink>(in AddUnitEvent @event, TSink sink) where TSink : IGameEventSink
        {
            if (@event.IsCanceled)
            {
                return;
            }
            var prototype = _world.Registry.GetUnitPrototype(@event.UnitPrototypeKey);
            _world.Units.Add(@event.UnitId.Value);
            _world.Stats.Add(@event.UnitId, prototype.Stats.Materialize());
            _world.Positions.Add(@event.UnitId, @event.Position);
            _world.Behaviors.Add(@event.UnitId, new());
            _world.Teams.Add(@event.UnitId, @event.TeamId);
            _world.Factions.Add(@event.UnitId, @event.FactionKey);

            var abilities = new Ability[prototype.Abilities.Length];
            for (int i = 0; i < abilities.Length; ++i)
            {
                abilities[i] =
                    _world.Registry.GetAbilityPrototype(prototype.Abilities[i]).Materialize(@event.UnitId);
            }
            _world.Abilities.Add(@event.UnitId, abilities);

            foreach (var passive in prototype.Passives)
            {
                sink.Queue(
                    new AddEffectEvent(@event.UnitId, 0xFF, @event.UnitId, 0, 0, -1, passive, 0, 0));
            }
        }

        private void ApplyBreak<TSink>(in BreakEvent @event, TSink sink) where TSink : IGameEventSink
        {
            if (@event.IsCanceled)
            {
                return;
            }

            ref var behavior = ref _world.Behaviors.TryGet(@event.TargetId, out bool isFound);
            if (!isFound)
            {
                return;
            }

            behavior.Mode = BehaviorMode.Broken;
        }

        private void ApplyDamage<TSink>(in DamageEvent @event, TSink sink) where TSink : IGameEventSink
        {
            if (@event.IsCanceled)
            {
                return;
            }

            ref var stats = ref _world.Stats.TryGet(@event.TargetId, out bool isFound);
            if (!isFound)
            {
                return;
            }

            byte damage = @event.Damage;
            ref var entry = ref stats[(int)@event.Stat];
            if (entry.Temporary >= damage)
            {
                entry.Temporary = (sbyte)(entry.Temporary - damage);
                damage = 0;
            }
            else
            {
                damage = (byte)(damage - entry.Temporary);
                entry.Temporary = 0;
            }
            entry.Value = (sbyte)(entry.Value - damage);
            if (entry.Value <= 0)
            {
                if (@event.Stat == StatType.Hitpoints)
                {
                    sink.Queue(new KillEvent(@event.SubjectId, @event.AbilityId, @event.TargetId));
                }
                else if (@event.Stat == StatType.Morale)
                {
                    sink.Queue(new BreakEvent(@event.SubjectId, @event.AbilityId, @event.TargetId));
                }
            }

        }

        private void ApplyFinalizeInitiative<TSink>(in FinalizeInitiativeEvent @event, TSink sink) 
            where TSink : IGameEventSink
        {
            if (@event.IsCanceled)
            {
                return;
            }
            _world.Inititiative.Sort();
        }

        private void ApplyInitiative<TSink>(in InitiativeEvent @event, TSink sink) where TSink : IGameEventSink
        {
            if (@event.IsCanceled)
            {
                return;
            }
            _world.Inititiative.Add(@event.SubjectId, @event.Initiative);
        }

        private void ApplyKill<TSink>(in KillEvent @event, TSink sink) where TSink : IGameEventSink
        {
            if (@event.IsCanceled)
            {
                return;
            }
            _world.Units.Remove(@event.TargetId.Value);
            _world.Stats.Remove(@event.TargetId);
            _world.Positions.Remove(@event.TargetId);
            _world.Behaviors.Remove(@event.TargetId);
            _world.Teams.Remove(@event.TargetId);
            _world.Factions.Remove(@event.TargetId);
            _world.Abilities.Remove(@event.TargetId);

            foreach (var effect in _world.Effects.ByOwner(@event.TargetId))
            {
                sink.Queue(new RemoveEffectEvent(EntityId.None, 0, effect.Id));
            }
        }

        private void ApplyMove<TSink>(in MoveEvent @event, TSink sink) where TSink : IGameEventSink
        {
            if (@event.IsCanceled)
            {
                return;
            }
            _world.Positions.Remove(@event.SubjectId);
            _world.Positions.Add(@event.SubjectId, @event.Destination);
        }

        private void ApplyRemoveEffect<TSink>(in RemoveEffectEvent @event, TSink sink) 
            where TSink : IGameEventSink
        {
            if (@event.IsCanceled)
            {
                return;
            }
            _world.Effects.Remove(@event.InstanceId);
            _world.Timers.Remove(@event.InstanceId);
        }

        private void ApplyTick<TSink>(in TickEvent @event, TSink sink) where TSink : IGameEventSink
        {
            if (@event.IsCanceled)
            {
                return;
            }
            _world.Inititiative.Clear();
            _world.IncrementTurn();
        }

        private void ApplyUpdateStat<TSink>(in UpdateStatEvent @event, TSink sink) where TSink : IGameEventSink
        {
            if (@event.IsCanceled)
            {
                return;
            }

            ref var stats = ref _world.Stats.TryGet(@event.TargetId, out bool isFound);
            if (isFound)
            {
                ref var entry = ref stats[(int)@event.Stat];
                if (@event.IsReversal)
                {
                    entry.Remove(@event.Delta);
                }
                else
                {
                    entry.Combine(@event.Delta);
                }
            }
        }

        private void Process<TSink>(ref AddEffectEvent @event, TSink sink) where TSink : IGameEventSink
        {
            @event.InstanceId = _world.NextId();
            @event.Roll = _world.Roll(new(1, 20, 0, 0));

            bool hasAbility = _world.Abilities.TryGet(@event.SubjectId, @event.AbilityId, out var ability);

            if (hasAbility)
            {
                ability!.Intercept(ref @event, _world);
            }
            _world.Intercept(ref @event);

            if (@event.Roll + @event.Skill < @event.Difficulty)
            {
                @event.IsCanceled = true;
            }

            if (hasAbility)
            {
                ability!.Handle(in @event, sink, _world);
            }
            _world.Handle(in @event, sink);
            ApplyAddEffect(in @event, sink);
        }

        private void Process<TSink>(ref AddUnitEvent @event, TSink sink) where TSink : IGameEventSink
        {
            @event.UnitId = _world.NextId();
            _world.Intercept(ref @event);
            ApplyAddUnit(in @event, sink);
            _world.Handle(in @event, sink);
        }

        private void Process<TSink>(ref AttackEvent @event, TSink sink) where TSink : IGameEventSink
        {
            @event.Roll = _world.Roll(new(1, 20, 0, @event.Advantage));

            bool hasAbility = _world.Abilities.TryGet(@event.SubjectId, @event.AbilityId, out var ability);

            if (hasAbility)
            {
                ability!.Intercept(ref @event, _world);
            }
            _world.Intercept(ref @event);

            if (@event.Difficulty > 0 
                && @event.Roll != 20 
                && (@event.Roll + @event.Skill < @event.Difficulty || @event.Roll == 1))
            {
                @event.IsCanceled = true;
            }
            if (@event.Roll > 20 - @event.CritRange)
            {
                @event.IsCritical = true;
            }

            if (hasAbility)
            {
                ability!.Handle(in @event, sink, _world);
            }
            _world.Handle(in @event, sink);
        }

        private void Process<TSink>(ref BreakEvent @event, TSink sink) where TSink : IGameEventSink
        {
            bool hasAbility = _world.Abilities.TryGet(@event.SubjectId, @event.AbilityId, out var ability);
            if (hasAbility)
            {
                ability!.Intercept(ref @event, _world);
            }
            _world.Intercept(ref @event);

            if (hasAbility)
            {
                ability!.Handle(in @event, sink, _world);
            }
            _world.Handle(in @event, sink);
            ApplyBreak(in @event, sink);
        }

        private void Process<TSink>(ref DamageEvent @event, TSink sink) where TSink : IGameEventSink
        {
            @event.Roll = _world.Roll(@event.Die);

            bool hasAbility = _world.Abilities.TryGet(@event.SubjectId, @event.AbilityId, out var ability);
            if (hasAbility)
            {
                ability!.Intercept(ref @event, _world);
            }
            _world.Intercept(ref @event);

            @event.Damage = (byte)Math.Max(0, @event.Roll - @event.Reduction);
            if (@event.Damage == 0)
            {
                @event.IsCanceled = true;
            }

            if (hasAbility)
            {
                ability!.Handle(in @event, sink, _world);
            }
            _world.Handle(in @event, sink);
            ApplyDamage(in @event, sink);
        }

        private void Process<TSink>(ref FinalizeInitiativeEvent @event, TSink sink) where TSink : IGameEventSink
        {
            _world.Intercept(ref @event);
            _world.Handle(in @event, sink);
            ApplyFinalizeInitiative(in @event, sink);
        }

        private void Process<TSink>(ref InitiativeEvent @event, TSink sink) where TSink : IGameEventSink
        {
            @event.Roll = _world.Roll(new(1, 20, 0, @event.Advantage));
            _world.Intercept(ref @event);
            @event.Initiative = @event.Skill + @event.Roll + _world.Random.NextSingle();
            _world.Handle(in @event, sink);
            ApplyInitiative(in @event, sink);
        }

        private void Process<TSink>(ref KillEvent @event, TSink sink) where TSink : IGameEventSink
        {
            bool hasAbility = _world.Abilities.TryGet(@event.SubjectId, @event.AbilityId, out var ability);
            if (hasAbility)
            {
                ability!.Intercept(ref @event, _world);
            }
            _world.Intercept(ref @event);

            if (hasAbility)
            {
                ability!.Handle(in @event, sink, _world);
            }
            _world.Handle(in @event, sink);
            ApplyKill(in @event, sink);
        }

        private void Process<TSink>(ref MoveEvent @event, TSink sink) where TSink : IGameEventSink
        {
            _world.Intercept(ref @event);
            _world.Handle(in @event, sink);
            ApplyMove(in @event, sink);
        }

        private void Process<TSink>(ref RemoveEffectEvent @event, TSink sink) where TSink : IGameEventSink
        {
            bool hasAbility = _world.Abilities.TryGet(@event.SubjectId, @event.AbilityId, out var ability);
            if (hasAbility)
            {
                ability!.Intercept(ref @event, _world);
            }
            _world.Intercept(ref @event);

            if (hasAbility)
            {
                ability!.Handle(in @event, sink, _world);
            }
            _world.Handle(in @event, sink);
            ApplyRemoveEffect(in @event, sink);
        }

        private void Process<TSink>(ref TickEvent @event, TSink sink) where TSink : IGameEventSink
        {
            _world.Intercept(ref @event);
            _world.Handle(in @event, sink);
            ApplyTick(in @event, sink);
        }

        private void Process<TSink>(ref UpdateStatEvent @event, TSink sink) where TSink : IGameEventSink
        {
            bool hasAbility = _world.Abilities.TryGet(@event.SubjectId, @event.AbilityId, out var ability);
            if (hasAbility)
            {
                ability!.Intercept(ref @event, _world);
            }
            _world.Intercept(ref @event);

            if (hasAbility)
            {
                ability!.Handle(in @event, sink, _world);
            }
            _world.Handle(in @event, sink);
            ApplyUpdateStat(in @event, sink);
        }

        private void Process<TSink>(ref UseAbilityEvent @event, TSink sink) where TSink : IGameEventSink
        {
            if (_world.Abilities.TryGet(@event.SubjectId, @event.AbilityId, out var ability))
            {
                ability!.Intercept(ref @event, _world);
                _world.Intercept(ref @event);

                ability!.Handle(in @event, sink, _world);
                _world.Handle(in @event, sink);
            }
        }

        private void Notify<T>(in T @event) where T : struct, IGameEvent
        {
            if (@event.IsCanceled)
            {
                return;
            }
            foreach (var listener in _listeners)
            {
                listener.Queue(@event);
            }
        }
    }
}
