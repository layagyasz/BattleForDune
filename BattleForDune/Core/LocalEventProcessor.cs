using BattleForDune.Core.Components;
using BattleForDune.Core.Components.Abilities;
using BattleForDune.Core.Components.Actions;
using BattleForDune.Core.Components.Auras;
using BattleForDune.Core.Components.Behaviors;
using BattleForDune.Core.Components.Effects;
using BattleForDune.Core.Components.Timers;
using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;
using BattleForDune.Core.Schema;
using BattleForDune.FastLookup;
using BattleForDune.Geometry;

namespace BattleForDune.Core
{
    public class LocalEventProcessor(ComponentWorld world, IGameEventSink[] listeners) : IGameEventProcessor
    {
        private readonly ref struct AddAuraEnterCallback<TSink>(
            EntityId auraId, ComponentWorld world, TSink sink) : ICallback<int> where TSink : IGameEventSink
        {
            private readonly EntityId _auraId = auraId;
            private readonly ComponentWorld _world = world;
            private readonly TSink _sink = sink;

            public readonly void Notify(in int id)
            {
                _sink.Queue(EnterAuraEvent.Intransitive(new(id), _auraId));
            }
        }

        private readonly ref struct EffectRemoveCallback<TSink>(
            ComponentWorld world, TSink sink) : IStorageCallback<Effect> where TSink : IGameEventSink
        {
            private readonly ComponentWorld _world = world;
            private readonly TSink _sink = sink;

            public readonly void Notify(int handle, ref Effect element)
            {
                element.Remove(_world, _sink);
            }
        }

        private readonly ref struct EnterAuraCallback<TSink>(
            EntityId subjectId, Hex2b origin, TSink sink) : ICallback<Aura> where TSink : IGameEventSink
        {
            private readonly EntityId _subjectId = subjectId;
            private readonly Hex2b _origin = origin;
            private readonly TSink _sink = sink;

            public readonly void Notify(in Aura aura)
            {
                if (Hexes.Distance(_origin, aura.Position) > aura.Range)
                {
                    _sink.Queue(EnterAuraEvent.Intransitive(_subjectId, aura.Id));
                }
            }
        }

        private readonly ref struct LeaveAuraCallback<TSink>(
            EntityId subjectId, Hex2b destination, TSink sink) : ICallback<Aura> where TSink : IGameEventSink
        {
            private readonly EntityId _subjectId = subjectId;
            private readonly Hex2b _destination = destination;
            private readonly TSink _sink = sink;

            public readonly void Notify(in Aura aura)
            {
                if (Hexes.Distance(_destination, aura.Position) > aura.Range)
                {
                    _sink.Queue(LeaveAuraEvent.Intransitive(_subjectId, aura.Id));
                }
            }
        }

        private readonly ref struct RemoveAuraCallback<TSink>(
            ComponentWorld world, TSink sink) : ICallback<Aura> where TSink : IGameEventSink
        {
            private readonly ComponentWorld _world = world;
            private readonly TSink _sink = sink;

            public readonly void Notify(in Aura aura)
            {
                var callback = new RemoveAuraLeaveCallback<TSink>(aura.Id, _world, _sink);
                _world.Positions.QueryRange(aura.Position, aura.Range, ref callback);
            }
        }

        private readonly ref struct RemoveAuraLeaveCallback<TSink>(
            EntityId auraId, ComponentWorld world, TSink sink) : ICallback<int> where TSink : IGameEventSink
        {
            private readonly EntityId _auraId = auraId;
            private readonly ComponentWorld _world = world;
            private readonly TSink _sink = sink;

            public readonly void Notify(in int id)
            {
                _sink.Queue(LeaveAuraEvent.Intransitive(new(id), _auraId));
            }
        }

        private readonly ComponentWorld _world = world;
        private readonly IGameEventSink[] _listeners = listeners;

        public void Process<TSink>(ref GameEvent @event, TSink sink) where TSink : IGameEventSink
        {
            switch (@event.Type)
            {
                case GameEventType.AddAura:
                    var aura = @event.ReadPayload<AddAuraEvent>();
                    Process(ref aura, sink);
                    Notify(in aura);
                    return;
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
                case GameEventType.EnterAura:
                    var enter = @event.ReadPayload<EnterAuraEvent>();
                    Process(ref enter, sink);
                    Notify(in enter);
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
                case GameEventType.LeaveAura:
                    var leave = @event.ReadPayload<LeaveAuraEvent>();
                    Process(ref leave, sink);
                    Notify(in leave);
                    return;
                case GameEventType.Move:
                    var move = @event.ReadPayload<MoveEvent>();
                    Process(ref move, sink);
                    Notify(in move);
                    return;
                case GameEventType.OverrideAllegiance:
                    var allegiance = @event.ReadPayload<OverrideAllegianceEvent>();
                    Process(ref allegiance, sink);
                    Notify(in allegiance);
                    return;
                case GameEventType.RemoveAura:
                    var removeAura = @event.ReadPayload<RemoveAuraEvent>();
                    Process(ref removeAura, sink);
                    Notify(in removeAura);
                    return;
                case GameEventType.RemoveEffect:
                    var removeEffect = @event.ReadPayload<RemoveEffectEvent>();
                    Process(ref removeEffect, sink);
                    Notify(in removeEffect);
                    return;
                case GameEventType.ResetActions:
                    var reset = @event.ReadPayload<ResetActionsEvent>();
                    Process(ref reset, sink);
                    Notify(in reset);
                    return;
                case GameEventType.Terminate:
                    var terminate = @event.ReadPayload<TerminateEvent>();
                    Process(ref terminate, sink);
                    Notify(in terminate);
                    return;
                case GameEventType.Tick:
                    var tick = @event.ReadPayload<TickEvent>();
                    Process(ref tick, sink);
                    Notify(in tick);
                    return;
                case GameEventType.UpdateActions:
                    var update = @event.ReadPayload<UpdateActionsEvent>();
                    Process(ref update, sink);
                    Notify(in update);
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

        private void ApplyAddAura<TSink>(in AddAuraEvent @event, TSink sink) where TSink : IGameEventSink
        {
            if (@event.IsCanceled)
            {
                return;
            }

            _world.Auras.Add(
                @event.AuraId,
                new Aura(
                    @event.AuraId,
                    @event.SubjectId,
                    @event.AbilityId,
                    @event.Position,
                    @event.Vector,
                    @event.EffectKey,
                    @event.Power,
                    @event.Range));
            var callback = new AddAuraEnterCallback<TSink>(@event.AuraId, _world, sink);
            _world.Positions.QueryRange(@event.Position, @event.Range, ref callback);
            if (@event.Duration >= 0)
            {
                _world.Timers.Add(@event.AuraId, GameTimerType.Aura, (byte)(_world.Turn + @event.Duration));
            }
        }

        private void ApplyAddEffect<TSink>(in AddEffectEvent @event, TSink sink) where TSink : IGameEventSink
        {
            if (@event.IsCanceled)
            {
                return;
            }

            // Find an existing effect to merge with.
            var mergeBehavior = _world.Registry.GetEffectPrototype(@event.EffectKey).MergeBehavior;
            if (mergeBehavior != MergeBehavior.None)
            {
                foreach (var effect in _world.Effects.ByOwner(@event.TargetId))
                {
                    if (effect.EffectKey != @event.EffectKey)
                    {
                        continue;
                    }

                    if (mergeBehavior.HasFlag(MergeBehavior.Power) 
                        || mergeBehavior.HasFlag(MergeBehavior.Stacks))
                    {
                        effect.Remove(_world, in sink);
                        if (mergeBehavior.HasFlag(MergeBehavior.Stacks))
                        {
                            effect.MergeStacks(@event.Stacks);
                        }
                        if (mergeBehavior.HasFlag(MergeBehavior.Power))
                        {
                            effect.MergePower(@event.Power);
                        }
                        effect.Apply(_world, in sink);
                    }

                    if (mergeBehavior.HasFlag(MergeBehavior.Timer))
                    {
                        if (@event.Duration >= 0)
                        {
                            byte newExpiration = (byte)(_world.Turn + @event.Duration);
                            if (_world.Timers.TryGet(effect.Id, out var currentTimer)
                                && newExpiration > currentTimer.Turn)
                            {
                                _world.Timers.Remove(effect.Id);
                                _world.Timers.Add(effect.Id, GameTimerType.Effect, newExpiration);
                            }
                        }
                        else
                        {
                            _world.Timers.Remove(effect.Id);
                        }
                    }
                    return;
                }
            }

            // If no matching effects exist, add a new one.
            var newEffect = 
                _world.Registry.GetEffectPrototype(@event.EffectKey)
                    .Materialize(@event.EffectId, @event.TargetId, @event.SubjectId, @event.AbilityId);
            newEffect.MergeStacks(@event.Stacks);
            newEffect.MergePower(@event.Power);
            _world.Effects.Add(newEffect);
            newEffect.Apply(_world, in sink);
            if (@event.Duration >= 0)
            {
                _world.Timers.Add(newEffect.Id, GameTimerType.Effect, (byte)(_world.Turn + @event.Duration));
            }
        }

        private void ApplyAddUnit<TSink>(in AddUnitEvent @event, TSink sink) where TSink : IGameEventSink
        {
            if (@event.IsCanceled)
            {
                return;
            }
            var prototype = _world.Registry.GetUnitPrototype(@event.UnitPrototypeKey);
            _world.Units.Add(@event.TargetId.Value);
            _world.UnitMetadatas.Add(@event.TargetId, new(@event.UnitPrototypeKey, @event.Level));
            _world.Stats.Add(@event.TargetId, prototype.Stats.Materialize(@event.Level));
            _world.Positions.Add(@event.TargetId, @event.Position);
            _world.Actions.Add(@event.TargetId, ActionPoints.Create());
            _world.Behaviors.Add(@event.TargetId, new());
            _world.Allegiances.Add(@event.TargetId, new(@event.FactionKey, @event.TeamId));

            var abilities = new Ability[prototype.Abilities.Length];
            for (int i = 0; i < abilities.Length; ++i)
            {
                abilities[i] =
                    _world.Registry.GetAbilityPrototype(prototype.Abilities[i]).Materialize();
            }
            _world.Abilities.Add(@event.TargetId, abilities);

            foreach (var passive in prototype.Passives)
            {
                sink.Queue(
                    AddEffectEvent.Intransitive(
                        @event.TargetId, AbilityVector.None, stacks: 0, power: 0, duration: -1, passive));
            }
        }

        private void ApplyBreak(in BreakEvent @event)
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
                    sink.Queue(
                        KillEvent.Transitive(
                            @event.SubjectId, @event.AbilityId, @event.TargetId, @event.IsPrimary));
                }
                else if (@event.Stat == StatType.Morale)
                {
                    ref var behavior = ref _world.Behaviors.TryGet(@event.TargetId, out isFound);
                    if (isFound && behavior.Mode != BehaviorMode.Broken)
                    {
                        sink.Queue(BreakEvent.Transitive(@event.SubjectId, @event.AbilityId, @event.TargetId));
                    }
                }
            }
        }

        private void ApplyEnterAura<TSink>(in EnterAuraEvent @event, TSink sink) where TSink : IGameEventSink
        {
            if (@event.IsCanceled)
            {
                return;
            }
            if (!_world.Auras.TryGet(@event.AuraId, out var aura))
            {
                return;
            }

            foreach (var effect in _world.Effects.ByOwner(@event.TargetId))
            {
                if (effect.EffectKey != aura.EffectKey)
                {
                    continue;
                }

                effect.Remove(_world, in sink);
                effect.Increment();
                effect.MergePower(aura.Power);
                effect.Apply(_world, in sink);

                return;
            }

            var newEffect =
                _world.Registry.GetEffectPrototype(aura.EffectKey)
                    .Materialize(@event.EffectId, @event.SubjectId, aura.SourceId, aura.AbilityId);
            newEffect.Increment();
            newEffect.MergePower(aura.Power);
            _world.Effects.Add(newEffect);
            newEffect.Apply(_world, in sink);
        }

        private void ApplyFinalizeInitiative(in FinalizeInitiativeEvent @event)
        {
            if (@event.IsCanceled)
            {
                return;
            }
            _world.Inititiative.Sort();
        }

        private void ApplyInitiative(in InitiativeEvent @event)
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
            _world.UnitMetadatas.Remove(@event.TargetId);
            _world.Stats.Remove(@event.TargetId);
            _world.Positions.Remove(@event.TargetId);
            _world.Actions.Remove(@event.TargetId);
            _world.Behaviors.Remove(@event.TargetId);
            _world.Allegiances.Remove(@event.TargetId);
            _world.Abilities.Remove(@event.TargetId);

            foreach (var effect in _world.Effects.ByOwner(@event.TargetId))
            {
                sink.Queue(RemoveEffectEvent.Intransitive(effect.Id));
            }
        }

        private void ApplyLeaveAura<TSink>(in LeaveAuraEvent @event, TSink sink) where TSink : IGameEventSink
        {
            if (@event.IsCanceled)
            {
                return;
            }
            if (!_world.Auras.TryGet(@event.AuraId, out var aura))
            {
                return;
            }

            foreach (var effect in _world.Effects.ByOwner(@event.TargetId))
            {
                if (effect.EffectKey != aura.EffectKey)
                {
                    continue;
                }

                if (effect.Stacks < 2)
                {
                    var callback = new EffectRemoveCallback<TSink>(_world, sink);
                    _world.Effects.Remove(effect.Id, ref callback);
                }
                else
                {
                    effect.Remove(_world, in sink);
                    effect.Decrement();
                    effect.MergePower(aura.Power);
                    effect.Apply(_world, in sink);
                }

                return;
            }
        }

        private void ApplyMove<TSink>(in MoveEvent @event, TSink sink) where TSink : IGameEventSink
        {
            if (@event.IsCanceled)
            {
                return;
            }
            ref var actions = ref _world.Actions.TryGet(@event.SubjectId, out var isFound);
            if (isFound)
            {
                actions.MovePoints -= 1;
            }
            _world.Positions.Move(@event.SubjectId, @event.Destination);

            var leaveCallback = new LeaveAuraCallback<TSink>(@event.SubjectId, @event.Destination, sink);
            _world.Auras.QueryRange(@event.Origin, 0, ref leaveCallback);
            var enterCallback = new EnterAuraCallback<TSink>(@event.SubjectId, @event.Origin, sink);
            _world.Auras.QueryRange(@event.Destination, 0, ref enterCallback);
        }

        private void ApplyOverrideAllegiance(in OverrideAllegianceEvent @event)
        {
            if (@event.IsCanceled)
            {
                return;
            }
            ref var allegiance = ref _world.Allegiances.TryGet(@event.TargetId, out var isFound);
            if (!isFound)
            {
                return;
            }
            allegiance.Team = @event.TeamId;
            allegiance.Faction = @event.FactionKey;
        }

        private void ApplyRemoveAura<TSink>(in RemoveAuraEvent @event, TSink sink) where TSink : IGameEventSink
        {
            if (@event.IsCanceled)
            {
                return;
            }

            var callback = new RemoveAuraCallback<TSink>(_world, sink);
            _world.Auras.Remove(@event.AuraId, ref callback);
        }

        private void ApplyRemoveEffect<TSink>(in RemoveEffectEvent @event, TSink sink) 
            where TSink : IGameEventSink
        {
            if (@event.IsCanceled)
            {
                return;
            }
            var callback = new EffectRemoveCallback<TSink>(_world, sink);
            _world.Effects.Remove(@event.EffectId, ref callback);
            _world.Timers.Remove(@event.EffectId);
        }

        private void ApplyResetActions(in ResetActionsEvent @event)
        {
            if (@event.IsCanceled)
            {
                return;
            }
            ref var actions = ref _world.Actions.TryGet(@event.SubjectId, out var isFound);
            if (isFound)
            {
                actions.AttackPoints = @event.Attacks;
                actions.MovePoints = @event.Moves;
            }
        }

        private void ApplyTerminate(in TerminateEvent @event)
        {
            if (@event.IsCanceled)
            {
                return;
            }
            _world.Terminate();
        }

        private void ApplyTick(in TickEvent @event)
        {
            if (@event.IsCanceled)
            {
                return;
            }
            _world.Inititiative.Clear();
            _world.IncrementTurn();
        }

        private void ApplyUpdateActions(in UpdateActionsEvent @event)
        {
            if (@event.IsCanceled)
            {
                return;
            }
            ref var actions = ref _world.Actions.TryGet(@event.TargetId, out bool isFound);
            if (isFound)
            {
                actions.AttackPoints = (byte)Math.Max(0, actions.AttackPoints + @event.Attacks);
                actions.MovePoints = (byte)Math.Max(0, actions.MovePoints + @event.Moves);
            }
        }

        private void ApplyUpdateStat(in UpdateStatEvent @event)
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

        private void ApplyUseAbility(in UseAbilityEvent @event)
        {
            if (@event.IsCanceled)
            {
                return;
            }
            ref var actions = ref _world.Actions.TryGet(@event.SubjectId, out var isFound);
            if (isFound)
            {
                actions.AttackPoints -= 1;
            }
        }

        private void Process<TSink>(ref AddAuraEvent @event, TSink sink) where TSink : IGameEventSink
        {
            @event.AuraId = _world.NextId();

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
            ApplyAddAura(in @event, sink);
        }

        private void Process<TSink>(ref AddEffectEvent @event, TSink sink) where TSink : IGameEventSink
        {
            @event.EffectId = _world.NextId();
            @event.Roll = _world.Roll(new(1, 20, 0));

            bool hasAbility = _world.Abilities.TryGet(@event.SubjectId, @event.AbilityId, out var ability);

            if (hasAbility)
            {
                ability!.Intercept(ref @event, _world);
            }
            _world.Intercept(ref @event);

            if (!IsSuccess(@event.Skill, @event.Difficulty, @event.Roll))
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
            @event.TargetId = _world.NextId();
            _world.Intercept(ref @event);
            ApplyAddUnit(in @event, sink);
            _world.Handle(in @event, sink);
        }

        private void Process<TSink>(ref AttackEvent @event, TSink sink) where TSink : IGameEventSink
        {
            @event.Roll = _world.Roll(new(1, 20, 0));

            bool hasAbility = _world.Abilities.TryGet(@event.SubjectId, @event.AbilityId, out var ability);
            if (hasAbility)
            {
                ability!.Intercept(ref @event, _world);
            }
            _world.Intercept(ref @event);

            var defense = 
                10
                + @event.DefenseEvasion 
                + Math.Max((sbyte) 0, @event.DefenseArmor)
                + Math.Max((sbyte) 0, @event.DefenseShield);
            if (!IsSuccess(@event.Skill, defense, @event.Roll))
            {
                @event.IsCanceled = true;
            }
            else if (@event.Roll > 20 - @event.CriticalRange)
            {
                var confirmRoll = _world.Roll(new(1, 20, 0));
                if (IsSuccess(@event.Skill, defense, confirmRoll))
                {
                    @event.IsCritical = true;
                }
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
            ApplyBreak(in @event);
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

            @event.Damage = (byte)Math.Max(0, @event.Roll - @event.Structure);
            if (@event.Damage <= 0)
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

        private void Process<TSink>(ref EnterAuraEvent @event, TSink sink) where TSink : IGameEventSink
        {
            _world.Intercept(ref @event);
            _world.Handle(in @event, sink);
            ApplyEnterAura(in @event, sink);
        }

        private void Process<TSink>(ref FinalizeInitiativeEvent @event, TSink sink) where TSink : IGameEventSink
        {
            _world.Intercept(ref @event);
            _world.Handle(in @event, sink);
            ApplyFinalizeInitiative(in @event);
        }

        private void Process<TSink>(ref InitiativeEvent @event, TSink sink) where TSink : IGameEventSink
        {
            @event.Roll = _world.Roll(new(1, 20, 0));
            _world.Intercept(ref @event);
            @event.Initiative = @event.Skill + @event.Roll + _world.Random.NextSingle();
            _world.Handle(in @event, sink);
            ApplyInitiative(in @event);
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

        private void Process<TSink>(ref LeaveAuraEvent @event, TSink sink) where TSink : IGameEventSink
        {
            _world.Intercept(ref @event);
            _world.Handle(in @event, sink);
            ApplyLeaveAura(in @event, sink);
        }

        private void Process<TSink>(ref MoveEvent @event, TSink sink) where TSink : IGameEventSink
        {
            _world.Intercept(ref @event);
            _world.Handle(in @event, sink);
            ApplyMove(in @event, sink);
        }

        private void Process<TSink>(ref OverrideAllegianceEvent @event, TSink sink) where TSink : IGameEventSink
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
            ApplyOverrideAllegiance(in @event);
        }

        private void Process<TSink>(ref RemoveAuraEvent @event, TSink sink) where TSink : IGameEventSink
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
            ApplyRemoveAura(in @event, sink);
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

        private void Process<TSink>(ref ResetActionsEvent @event, TSink sink) where TSink : IGameEventSink
        {
            _world.Intercept(ref @event);
            _world.Handle(in @event, sink);
            ApplyResetActions(in @event);
        }

        private void Process<TSink>(ref TerminateEvent @event, TSink sink) where TSink : IGameEventSink
        {
            _world.Intercept(ref @event);
            _world.Handle(in @event, sink);
            ApplyTerminate(in @event);
        }

        private void Process<TSink>(ref TickEvent @event, TSink sink) where TSink : IGameEventSink
        {
            _world.Intercept(ref @event);
            _world.Handle(in @event, sink);
            ApplyTick(in @event);
        }

        private void Process<TSink>(ref UpdateActionsEvent @event, TSink sink) where TSink : IGameEventSink
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
            ApplyUpdateActions(in @event);
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
            ApplyUpdateStat(in @event);
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
            ApplyUseAbility(in @event);
        }

        private void Notify<T>(in T @event) where T : struct, IGameEvent
        {
            foreach (var listener in _listeners)
            {
                listener.Queue(@event);
            }
        }

        private static bool IsSuccess(int skill, int difficulty, int roll)
        {
            if (roll == 1)
            {
                return false;
            }
            else if (roll == 20)
            {
                return true;
            }
            else
            {
                return skill + roll >= difficulty;
            }
        }
    }
}
