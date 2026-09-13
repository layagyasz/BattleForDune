using BattleForDune.Core.Components;
using BattleForDune.Core.Components.Abilities;
using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;

namespace BattleForDune.Core.Systems
{
    public class CombatGameSystem(ComponentWorld world) : IGameEventProcessor
    {
        private readonly ComponentWorld _world = world;

        public void Process<TSink>(ref GameEvent @event, TSink sink) where TSink : IGameEventSink
        {
            switch (@event.Type)
            {
                case GameEventType.Attack:
                    var attack = @event.ReadPayload<AttackEvent>();
                    Process(ref attack, sink);
                    return;
                case GameEventType.Damage:
                    var damage = @event.ReadPayload<DamageEvent>();
                    Process(ref damage, sink);
                    return;
                case GameEventType.UseAbility:
                    var ability = @event.ReadPayload<UseAbilityEvent>();
                    Process(ref ability, sink);
                    return;
            }
        }

        private void ApplyDamage<TSink>(in DamageEvent @event, TSink sink) where TSink : IGameEventSink
        {
            if (@event.IsCanceled)
            {
                return;
            }

            ref var stats = ref _world.TryGetStats(@event.TargetId, out bool isFound);
            if (isFound)
            {
                stats.Hitpoints.Value = (sbyte)(stats.Hitpoints.Value - @event.Damage);
            }
        }

        private void Process<TSink>(ref AttackEvent @event, TSink sink) where TSink : IGameEventSink
        {
            @event.Roll = _world.Roll(new(1, 20, 0));

            if (_world.TryGetAbility(@event.AbilityId, out var ability))
            {

                AbilityIntercept(ability, ref @event);
                _world.Intercept(ref @event);

                if (@event.Roll != 20 && (@event.Roll + @event.Skill < @event.Difficulty || @event.Roll == 1))
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

            if (_world.TryGetAbility(@event.AbilityId, out var ability))
            {
                AbilityIntercept(ability, ref @event);
                _world.Intercept(ref @event);

                var shield = Math.Max(0, @event.Shield - @event.ShieldPiercing);
                var armor = Math.Max(0, @event.Armor - @event.ArmorPiercing);
                var damage = shield == 0 ? @event.Roll : Math.Min(@event.Roll, 6 - shield);
                @event.Damage = (byte)Math.Max(0, damage - armor);

                ApplyDamage(in @event, sink);
                AbilityHandle(ability, in @event, sink);
                _world.Handle(in @event, sink);
            }
        }

        private void Process<TSink>(ref UseAbilityEvent @event, TSink sink) where TSink : IGameEventSink
        {
            if (_world.TryGetAbility(@event.AbilityId, out var ability))
            {
                AbilityIntercept(ability, ref @event);
                _world.Intercept(ref @event);
                @event.InstanceId = _world.GetId();
                AbilityHandle(ability, in @event, sink);
                _world.Handle(in @event, sink);
            }
        }

        private void AbilityIntercept<T>(IAbility ability, ref T @event) where T : struct, IGameEvent
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

        private void AbilityHandle<T, TSink>(IAbility ability, in T @event, TSink sink) 
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
