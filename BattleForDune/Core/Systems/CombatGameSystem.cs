using BattleForDune.Core.Components;
using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;

namespace BattleForDune.Core.Systems
{
    public class CombatGameSystem(ComponentWorld world) : IGameEventProcessor
    {
        private readonly ComponentWorld _world = world;

        private void ApplyDamage<TSink>(in DamageEvent @event, TSink sink) where TSink : IGameEventSink
        {
            ref var stats = ref _world.TryGetStats(@event.TargetId, out bool isFound);
            if (isFound)
            {
                stats.Hitpoints.Value = (sbyte)(stats.Hitpoints.Value - @event.Damage);
            }
        }

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

        private void Process<TSink>(ref AttackEvent @event, TSink sink) where TSink : IGameEventSink
        {
            if (_world.TryGetAbility(@event.AbilityId, out var ability))
            {
                var handler = (IGameEventHandler<AttackEvent>)ability;
                handler.Intercept(ref @event, _world);
                _world.Intercept(ref @event);
                @event.Roll0 = _world.Roll(@event.Die0);
                @event.Roll1 = _world.Roll(@event.Die1);
                @event.Roll2 = _world.Roll(@event.Die2);
                handler.Handle(in @event, _world, sink);
                _world.Handle(in @event, sink);
            }
        }

        private void Process<TSink>(ref DamageEvent @event, TSink sink) where TSink : IGameEventSink
        {
            if (_world.TryGetAbility(@event.AbilityId, out var ability))
            {
                var handler = (IGameEventHandler<DamageEvent>)ability;
                handler.Intercept(ref @event, _world);
                _world.Intercept(ref @event);
                ApplyDamage(in @event, sink);
                handler.Handle(in @event, _world, sink);
                _world.Handle(in @event, sink);
            }
        }

        private void Process<TSink>(ref UseAbilityEvent @event, TSink sink) where TSink : IGameEventSink
        {
            if (_world.TryGetAbility(@event.AbilityId, out var ability))
            {
                var handler = (IGameEventHandler<UseAbilityEvent>)ability;
                handler.Intercept(ref @event, _world);
                _world.Intercept(ref @event);
                @event.InstanceId = _world.GetId();
                handler.Handle(in @event, _world, sink);
                _world.Handle(in @event, sink);
            }
        }
    }
}
