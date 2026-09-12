using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;
using BattleForDune.Core.Schema;

namespace BattleForDune.Core.Components.Abilities
{
    public record class AttackAbility(
        EntityId Id,
        EntityId OwnerId,
        sbyte Attack,
        Die Damage,
        DamageType DamageType,
        byte CritRange, 
        IGameEventHandler<DamageEvent> OnCrit)
        : 
        IAbility, 
        IGameEventHandler<UseAbilityEvent>, 
        IGameEventHandler<AttackEvent>, 
        IGameEventHandler<DamageEvent>
    {
        public void Trigger<TSink>(in TSink sink, EntityId target) where TSink : IGameEventSink
        {
            var @event = new UseAbilityEvent(Id, target);
            sink.Queue(in @event);
        }

        public void Intercept(ref UseAbilityEvent @event, ComponentWorld world) { }

        public void Handle<TSink>(
            in UseAbilityEvent @event, ComponentWorld world, in TSink sink) where TSink : IGameEventSink
        {
            if (@event.IsCanceled)
            {
                return;
            }
            ref var stats = ref world.TryGetStats(OwnerId, out bool isFound);
            if (!isFound)
            {
                return;
            }
            ref var targetStats = ref world.TryGetStats(@event.TargetId, out isFound);
            if (!isFound)
            {
                return;
            }
            var attack =
                new AttackEvent(
                    @event.AbilityId,
                    @event.TargetId,
                    @event.InstanceId,
                    (sbyte)(Attack + stats.Attack.Value),
                    targetStats.Defense.Value)
                {
                    Die0 = new(1, 20, 0),
                    Die1 = Damage
                };
            sink.Queue(in attack);
        }

        public void Intercept(ref AttackEvent @event, ComponentWorld world)
        {
            if (@event.Roll0 + @event.Skill < @event.Target)
            {
                @event.IsCanceled = true;
            }
        }

        public void Handle<TSink>(
            in AttackEvent @event, ComponentWorld world, in TSink sink) where TSink : IGameEventSink
        {
            if (@event.IsCanceled)
            {
                return;
            }
            var damage = new DamageEvent(@event.AbilityId, @event.TargetId, DamageType, @event.Roll1);
            if (@event.Roll0 >= CritRange)
            {
                OnCrit.Intercept(ref damage, world);
                OnCrit.Handle(in damage, world, sink);
            }
            sink.Queue(in damage);
        }

        public void Intercept(ref DamageEvent @event, ComponentWorld world) { }

        public void Handle<TSink>(
            in DamageEvent @event, ComponentWorld world, in TSink sink) where TSink : IGameEventSink
        { }
    }
}
