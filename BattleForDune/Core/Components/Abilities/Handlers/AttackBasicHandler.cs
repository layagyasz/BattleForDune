using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;
using BattleForDune.Core.Schema;

namespace BattleForDune.Core.Components.Abilities.Handlers
{
    public record class AttackBasicHandler(
        DamageType DamageType,
        AbilityVector Vector,
        StatType Stat,
        Die Damage, 
        bool IsPrimary,
        sbyte AntiStructure) 
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
            ref var stats = ref world.Stats.TryGet(@event.SubjectId, out var isFound);
            if (!isFound)
            {
                return;
            }
            ref var targetStats = ref world.Stats.TryGet(@event.TargetId, out isFound);
            if (!isFound)
            {
                return;
            }

            var damage =
                DamageEvent.Transitive(
                    @event.SubjectId,
                    @event.AbilityId,
                    @event.TargetId,
                    IsPrimary, 
                    @event.IsCritical,
                    DamageType, 
                    Vector, 
                    Stat, 
                    Damage,
                    (sbyte)(targetStats[StatType.Structure].Value - AntiStructure));
            sink.Queue(in damage);
        }
    }
}
