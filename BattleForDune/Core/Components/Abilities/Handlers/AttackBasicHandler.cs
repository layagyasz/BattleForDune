using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;
using BattleForDune.Core.Schema;

namespace BattleForDune.Core.Components.Abilities.Handlers
{
    public record class AttackBasicHandler(
        DamageType DamageType,
        DamageVector DamageVector,
        Die Damage, 
        sbyte ArmorPiercing) 
        : IAbilityHandler<AttackEvent>
    {
        public void Intercept(ref AttackEvent @event, ComponentWorld world) { }

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
                    DamageVector,
                    StatType.Hitpoints,
                    Damage,
                    (byte)Math.Max(0, stats[StatType.Armor].Value - ArmorPiercing));
            sink.Queue(in damage);
        }
    }
}
