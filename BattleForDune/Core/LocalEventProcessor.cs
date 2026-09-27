using BattleForDune.Core.Components;
using BattleForDune.Core.Components.Abilities;
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
            foreach (var effect in _world.GetEffects().ByOwner(@event.TargetId))
            {
                if (effect.EffectKey == @event.EffectKey)
                {
                    if (!effect.IsMergeable)
                    {
                        break;
                    }

                    effect.Remove(_world, in sink);
                    effect.MergeStacks(@event.Stacks);
                    effect.MergePower(@event.Power);
                    effect.Apply(_world, in sink);

                    if (@event.Duration >= 0)
                    {
                        byte newExpiration = (byte)(_world.GetTurn() + @event.Duration);
                        if (_world.GetTimers().TryGet(effect.Id, out var currentExpiration)
                            && newExpiration > currentExpiration)
                        {
                            _world.GetTimers().Remove(effect.Id);
                            _world.GetTimers().Add(effect.Id, newExpiration);
                        }
                    }
                    else
                    {
                        _world.GetTimers().Remove(effect.Id);
                    }
                    return;
                }
            }

            // If no matching effects exist, add a new one.
            var newEffect = 
                _world.GetRegistry().GetEffectPrototype(@event.EffectKey)
                    .Materialize(@event.InstanceId, @event.TargetId, @event.SubjectId, @event.AbilityId);
            newEffect.MergeStacks(@event.Stacks);
            newEffect.MergePower(@event.Power);
            _world.GetEffects().Add(newEffect);
            newEffect.Apply(_world, in sink);
            if (@event.Duration >= 0)
            {
                _world.GetTimers().Add(newEffect.Id, (byte)(_world.GetTurn() + @event.Duration));
            }
        }

        private void ApplyAddUnit<TSink>(in AddUnitEvent @event, TSink sink) where TSink : IGameEventSink
        {
            if (@event.IsCanceled)
            {
                return;
            }
            var prototype = _world.GetRegistry().GetUnitPrototype(@event.UnitPrototypeKey);
            _world.GetUnits().Add(@event.UnitId.Value);
            _world.GetStats().Add(@event.UnitId, prototype.Stats.Materialize());
            _world.GetPositions().Add(@event.UnitId, @event.Position);
            _world.GetTeams().Add(@event.UnitId, @event.TeamId);
            _world.GetFactions().Add(@event.UnitId, @event.FactionKey);

            var abilities = new Ability[prototype.Abilities.Length];
            for (int i = 0; i < abilities.Length; ++i)
            {
                abilities[i] =
                    _world.GetRegistry().GetAbilityPrototype(prototype.Abilities[i]).Materialize(@event.UnitId);
            }
            _world.GetAbilities().Add(@event.UnitId, abilities);

            foreach (var passive in prototype.Passives)
            {
                sink.Queue(
                    new AddEffectEvent(@event.UnitId, 0xFF, @event.UnitId, 0, 0, -1, passive, 0, 0));
            }
        }

        private void ApplyDamage<TSink>(in DamageEvent @event, TSink sink) where TSink : IGameEventSink
        {
            if (@event.IsCanceled)
            {
                return;
            }

            ref var stats = ref _world.GetStats().TryGet(@event.TargetId, out bool isFound);
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
            if (entry.Value <= 0 && @event.Stat == StatType.Hitpoints)
            {
                sink.Queue(new KillEvent(@event.SubjectId, @event.AbilityId, @event.TargetId));
            }
        }

        private void ApplyFinalizeInitiative<TSink>(in FinalizeInitiativeEvent @event, TSink sink) 
            where TSink : IGameEventSink
        {
            if (@event.IsCanceled)
            {
                return;
            }
            _world.GetInitiative().Sort();
        }

        private void ApplyInitiative<TSink>(in InitiativeEvent @event, TSink sink) where TSink : IGameEventSink
        {
            if (@event.IsCanceled)
            {
                return;
            }
            _world.GetInitiative().Add(@event.SubjectId, @event.Initiative);
        }

        private void ApplyKill<TSink>(in KillEvent @event, TSink sink) where TSink : IGameEventSink
        {
            if (@event.IsCanceled)
            {
                return;
            }
            _world.GetUnits().Remove(@event.TargetId.Value);
            _world.GetStats().Remove(@event.TargetId);
            _world.GetPositions().Remove(@event.TargetId);
            _world.GetTeams().Remove(@event.TargetId);
            _world.GetFactions().Remove(@event.TargetId);
            _world.GetAbilities().Remove(@event.TargetId);

            foreach (var effect in _world.GetEffects().ByOwner(@event.TargetId))
            {
                sink.Queue(new RemoveEffectEvent(EntityId.None, 0, effect.Id));
            }
        }

        private void ApplyRemoveEffect<TSink>(in RemoveEffectEvent @event, TSink sink) 
            where TSink : IGameEventSink
        {
            if (@event.IsCanceled)
            {
                return;
            }
            _world.GetEffects().Remove(@event.InstanceId);
            _world.GetTimers().Remove(@event.InstanceId);
        }

        private void ApplyTick<TSink>(in TickEvent @event, TSink sink) where TSink : IGameEventSink
        {
            if (@event.IsCanceled)
            {
                return;
            }
            _world.GetInitiative().Clear();
            _world.IncrementTurn();
        }

        private void ApplyUpdateStat<TSink>(in UpdateStatEvent @event, TSink sink) where TSink : IGameEventSink
        {
            if (@event.IsCanceled)
            {
                return;
            }

            ref var stats = ref _world.GetStats().TryGet(@event.TargetId, out bool isFound);
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

            bool hasAbility = _world.GetAbilities().TryGet(@event.SubjectId, @event.AbilityId, out var ability);

            if (hasAbility)
            {
                AbilityIntercept(ability, ref @event);
            }
            _world.Intercept(ref @event);

            if (@event.Roll + @event.Skill < @event.Difficulty)
            {
                @event.IsCanceled = true;
            }

            if (hasAbility)
            {
                AbilityHandle(ability, in @event, sink);
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

            bool hasAbility = _world.GetAbilities().TryGet(@event.SubjectId, @event.AbilityId, out var ability);

            if (hasAbility)
            {
                AbilityIntercept(ability, ref @event);
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
                AbilityHandle(ability, in @event, sink);
            }
            _world.Handle(in @event, sink);
        }

        private void Process<TSink>(ref DamageEvent @event, TSink sink) where TSink : IGameEventSink
        {
            @event.Roll = _world.Roll(@event.Die);

            bool hasAbility = _world.GetAbilities().TryGet(@event.SubjectId, @event.AbilityId, out var ability);
            if (hasAbility)
            {
                AbilityIntercept(ability, ref @event);
            }
            _world.Intercept(ref @event);

            @event.Damage = (byte)Math.Max(0, @event.Roll - @event.Reduction);
            if (@event.Damage == 0)
            {
                @event.IsCanceled = true;
            }

            if (hasAbility)
            {
                AbilityHandle(ability, in @event, sink);
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
            @event.Initiative = @event.Skill + @event.Roll + _world.GetRandom().NextSingle();
            _world.Handle(in @event, sink);
            ApplyInitiative(in @event, sink);
        }

        private void Process<TSink>(ref KillEvent @event, TSink sink) where TSink : IGameEventSink
        {
            bool hasAbility = _world.GetAbilities().TryGet(@event.SubjectId, @event.AbilityId, out var ability);
            if (hasAbility)
            {
                AbilityIntercept(ability, ref @event);
            }
            _world.Intercept(ref @event);

            if (hasAbility)
            {
                AbilityHandle(ability, in @event, sink);
            }
            _world.Handle(in @event, sink);
            ApplyKill(in @event, sink);
        }

        private void Process<TSink>(ref RemoveEffectEvent @event, TSink sink) where TSink : IGameEventSink
        {
            bool hasAbility = _world.GetAbilities().TryGet(@event.SubjectId, @event.AbilityId, out var ability);
            if (hasAbility)
            {
                AbilityIntercept(ability, ref @event);
            }
            _world.Intercept(ref @event);

            if (hasAbility)
            {
                AbilityHandle(ability, in @event, sink);
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
            bool hasAbility = _world.GetAbilities().TryGet(@event.SubjectId, @event.AbilityId, out var ability);
            if (hasAbility)
            {
                AbilityIntercept(ability, ref @event);
            }
            _world.Intercept(ref @event);

            if (hasAbility)
            {
                AbilityHandle(ability, in @event, sink);
            }
            _world.Handle(in @event, sink);
            ApplyUpdateStat(in @event, sink);
        }

        private void Process<TSink>(ref UseAbilityEvent @event, TSink sink) where TSink : IGameEventSink
        {
            if (_world.GetAbilities().TryGet(@event.SubjectId, @event.AbilityId, out var ability))
            {
                AbilityIntercept(ability, ref @event);
                _world.Intercept(ref @event);

                AbilityHandle(ability, in @event, sink);
                _world.Handle(in @event, sink);
            }
        }

        private void AbilityIntercept<T>(Ability ability, ref T @event) where T : struct, IGameEvent
        {
            for (int i=0; i<ability.Handlers.Length; ++i)
            {
                var handler = ability.Handlers[i];
                if (handler is IGameEventHandler<T> specificHandler)
                {
                    specificHandler.Intercept(ref @event, _world);
                }
            }
        }

        private void AbilityHandle<T, TSink>(Ability ability, in T @event, TSink sink) 
            where T : struct, IGameEvent
            where TSink : IGameEventSink
        {
            for (int i = 0; i < ability.Handlers.Length; ++i)
            {
                var handler = ability.Handlers[i];
                if (handler is IGameEventHandler<T> specificHandler)
                {
                    specificHandler.Handle(in @event, _world, in sink);
                }
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
