using BattleForDune.Core.Components;
using BattleForDune.Core.Components.Abilities;
using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;
using BattleForDune.Core.Schema;

namespace BattleForDune.Core
{
    public class LocalEventProcessor(ComponentWorld world) : IGameEventProcessor
    {
        private readonly ComponentWorld _world = world;

        public void Process<TSink>(ref GameEvent @event, TSink sink) where TSink : IGameEventSink
        {
            Console.WriteLine(@event.Type);
            switch (@event.Type)
            {
                case GameEventType.AddAbility:
                    var ability = @event.ReadPayload<AddAbilityEvent>();
                    Process(ref ability, sink);
                    return;
                case GameEventType.AddEffect:
                    var effect = @event.ReadPayload<AddEffectEvent>();
                    Process(ref effect, sink);
                    return;
                case GameEventType.AddTeam:
                    var team = @event.ReadPayload<AddTeamEvent>();
                    Process(ref team, sink);
                    return;
                case GameEventType.Attack:
                    var attack = @event.ReadPayload<AttackEvent>();
                    Process(ref attack, sink);
                    return;
                case GameEventType.Damage:
                    var damage = @event.ReadPayload<DamageEvent>();
                    Process(ref damage, sink);
                    return;
                case GameEventType.FinalizeInitiative:
                    var finalize = @event.ReadPayload<FinalizeInitiativeEvent>();
                    Process(ref finalize, sink);
                    return;
                case GameEventType.Initiative:
                    var initiative = @event.ReadPayload<InitiativeEvent>();
                    Process(ref initiative, sink);
                    return;
                case GameEventType.AddUnit:
                    var spawn = @event.ReadPayload<AddUnitEvent>();
                    Process(ref spawn, sink);
                    return;
                case GameEventType.Tick:
                    var tick = @event.ReadPayload<TickEvent>();
                    Process(ref tick, sink);
                    return;
                case GameEventType.UseAbility:
                    var use = @event.ReadPayload<UseAbilityEvent>();
                    Process(ref use, sink);
                    return;
            }
        }

        private void ApplyAddAbility<TSink>(in AddAbilityEvent @event, TSink sink) where TSink : IGameEventSink
        {
            if (@event.IsCanceled)
            {
                return;
            }

            var prototype = _world.GetRegistry().GetAbilityPrototype(@event.AbilityKey);
            _world.GetAbilities().Add(prototype.Materialize(@event.AbilityId, @event.OwnerId));
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
                    effect.Remove(_world, in sink);
                    effect.MergeStacks(@event.Stacks);
                    effect.MergePower(@event.Power);
                    effect.Apply(_world, in sink);
                    // TODO: Reschedule timeout.
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
            // TODO: Schedule timeout when duration > 0
        }

        private void ApplyAddTeam<TSink>(in AddTeamEvent @event, TSink sink) where TSink : IGameEventSink
        {
            if (@event.IsCanceled)
            {
                return;
            }
            _world.GetFactions().Add(@event.TeamId, @event.FactionKey);
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
            foreach (var ability in prototype.Abilities)
            {
                sink.Queue(new AddAbilityEvent(@event.UnitId, ability));
            }
        }

        private void ApplyDamage<TSink>(in DamageEvent @event, TSink sink) where TSink : IGameEventSink
        {
            if (@event.IsCanceled)
            {
                return;
            }

            ref var stats = ref _world.GetStats().TryGet(@event.TargetId, out bool isFound);
            if (isFound)
            {
                stats[(int)StatType.Hitpoints].Value = (sbyte)(stats.Get(StatType.Hitpoints) - @event.Damage);
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

        private void ApplyTick<TSink>(in TickEvent @event, TSink sink) where TSink : IGameEventSink
        {
            if (@event.IsCanceled)
            {
                return;
            }
            _world.GetInitiative().Clear();
        }

        private void Process<TSink>(ref AddAbilityEvent @event, TSink sink) where TSink : IGameEventSink
        {
            @event.AbilityId = _world.NextId();
            _world.Intercept(ref @event);
            ApplyAddAbility(in @event, sink);
            _world.Handle(in @event, sink);
        }

        private void Process<TSink>(ref AddEffectEvent @event, TSink sink) where TSink : IGameEventSink
        {
            @event.InstanceId = _world.NextId();
            @event.Roll = _world.Roll(new(1, 20, 0));

            if (_world.GetAbilities().TryGet(@event.AbilityId, out var ability))
            {

                AbilityIntercept(ability, ref @event);
                _world.Intercept(ref @event);

                if (@event.Roll + @event.Skill < @event.Difficulty)
                {
                    @event.IsCanceled = true;
                }

                AbilityHandle(ability, in @event, sink);
                _world.Handle(in @event, sink);
                ApplyAddEffect(in @event, sink);
            }
        }

        private void Process<TSink>(ref AddTeamEvent @event, TSink sink) where TSink : IGameEventSink
        {
            @event.TeamId = _world.NextId();
            _world.Intercept(ref @event);
            ApplyAddTeam(in @event, sink);
            _world.Handle(in @event, sink);
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
            @event.Roll = _world.Roll(new(1, 20, 0));

            if (_world.GetAbilities().TryGet(@event.AbilityId, out var ability))
            {

                AbilityIntercept(ability, ref @event);
                _world.Intercept(ref @event);

                if (@event.Difficulty > 0 
                    && @event.Roll != 20 
                    && (@event.Roll + @event.Skill < @event.Difficulty || @event.Roll == 1))
                {
                    @event.IsCanceled = true;
                }

                AbilityHandle(ability, in @event, sink);
                _world.Handle(in @event, sink);
            }
        }

        private void Process<TSink>(ref DamageEvent @event, TSink sink) where TSink : IGameEventSink
        {
            @event.Roll = _world.Roll(@event.Die);

            if (_world.GetAbilities().TryGet(@event.AbilityId, out var ability))
            {
                AbilityIntercept(ability, ref @event);
                _world.Intercept(ref @event);

                var shield = Math.Max(0, @event.Shield - @event.ShieldPiercing);
                var armor = Math.Max(0, @event.Armor - @event.ArmorPiercing);
                var damage = shield == 0 ? @event.Roll : Math.Min(@event.Roll, 6 - shield);
                @event.Damage = (byte)Math.Max(0, damage - armor);
                if (@event.Damage == 0)
                {
                    @event.IsCanceled = true;
                }

                AbilityHandle(ability, in @event, sink);
                _world.Handle(in @event, sink);
                ApplyDamage(in @event, sink);
            }
        }

        private void Process<TSink>(ref FinalizeInitiativeEvent @event, TSink sink) where TSink : IGameEventSink
        {
            _world.Intercept(ref @event);
            _world.Handle(in @event, sink);
            ApplyFinalizeInitiative(in @event, sink);
        }

        private void Process<TSink>(ref InitiativeEvent @event, TSink sink) where TSink : IGameEventSink
        {
            @event.Roll = _world.Roll(new(1, 20, 0));
            _world.Intercept(ref @event);
            @event.Initiative = (sbyte)(@event.Skill + @event.Roll);
            _world.Handle(in @event, sink);
            ApplyInitiative(in @event, sink);
        }

        private void Process<TSink>(ref TickEvent @event, TSink sink) where TSink : IGameEventSink
        {
            _world.Intercept(ref @event);
            _world.Handle(in @event, sink);
            ApplyTick(in @event, sink);
        }

        private void Process<TSink>(ref UseAbilityEvent @event, TSink sink) where TSink : IGameEventSink
        {
            if (_world.GetAbilities().TryGet(@event.AbilityId, out var ability))
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
    }
}
