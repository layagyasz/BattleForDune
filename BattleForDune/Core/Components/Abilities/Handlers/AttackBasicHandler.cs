using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;
using BattleForDune.Core.Schema;

namespace BattleForDune.Core.Components.Abilities.Handlers
{
    public record class AttackBasicHandler(
        DamageType DamageType,
        DamageVector DamageVector,
        StatType Stat,
        Die Damage, 
        sbyte ArmorPiercing,
        bool IsPrimary) 
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
            ref var stats = ref world.Stats.TryGet(@event.TargetId, out var isFound);
            if (!isFound)
            {
                return;
            }
            byte reduction = 
                Stat == StatType.Hitpoints 
                    ? (byte)Math.Max(0, stats[StatType.Armor].Value - ArmorPiercing) : (byte)0;
            var damage =
                new DamageEvent(
                    @event.SubjectId,
                    @event.AbilityId,
                    @event.TargetId,
                    IsPrimary,
                    @event.IsCritical,
                    DamageType,
                    DamageVector,
                    Stat,
                    Damage,
                    reduction);
            sink.Queue(in damage);
        }
    }
}
