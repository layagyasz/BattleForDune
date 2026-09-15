using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;
using BattleForDune.Core.Schema;

namespace BattleForDune.Core.Components.Abilities.Attack
{
    public record class BasicAttackHandler(
        sbyte Attack,
        byte CritRange, 
        DamageType DamageType, 
        Die Damage, 
        sbyte ArmorPiercing,
        sbyte ShieldPiercing) 
        : IGameEventHandler<AttackEvent>, IGenericHandler
    {
        public void Intercept(ref AttackEvent @event, ComponentWorld world)
        {
            if (@event.Roll > 20 - CritRange)
            {
                @event.IsCritical = true;
            }
        }

        public void Handle<TSink>(
            in AttackEvent @event, ComponentWorld world, in TSink sink) where TSink : IGameEventSink
        {
            if (@event.IsCanceled)
            {
                return;
            }
            ref var stats = ref world.GetStats().TryGet(@event.TargetId, out var isFound);
            if (!isFound)
            {
                return;
            }
            var damage =
                new DamageEvent(
                    @event.SubjectId,
                    @event.AbilityId,
                    @event.TargetId,
                    isPrimary: true,
                    @event.IsCritical,
                    DamageType,
                    Damage,
                    ArmorPiercing,
                    stats.Armor.Value,
                    ShieldPiercing,
                    stats.Shield.Value);
            sink.Queue(in damage);
        }
    }
}
