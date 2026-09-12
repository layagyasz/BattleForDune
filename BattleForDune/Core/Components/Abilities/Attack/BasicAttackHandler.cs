using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;
using BattleForDune.Core.Schema;

namespace BattleForDune.Core.Components.Abilities.Attack
{
    public record class BasicAttackHandler(sbyte Attack, byte CritRange, DamageType DamageType, Die Damage) 
        : IGameEventHandler<AttackEvent>, IGenericHandler
    {
        public void Intercept(ref AttackEvent @event, ComponentWorld world)
        {
            ref var stats = ref world.TryGetStats(@event.SubjectId, out bool isFound);
            if (!isFound)
            {
                return;
            }
            ref var targetStats = ref world.TryGetStats(@event.TargetId, out isFound);
            if (!isFound)
            {
                return;
            }
            if (@event.Roll != 20 
                && (@event.Roll + stats.Attack.Value < targetStats.Defense.Value || @event.Roll == 1))
            {
                @event.IsCanceled = true;
            }
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
            ref var targetStats = ref world.TryGetStats(@event.TargetId, out var isFound);
            if (!isFound)
            {
                return;
            }
            var damage =
                new DamageEvent(
                    @event.AbilityId,
                    @event.TargetId,
                    @event.IsCritical,
                    DamageType,
                    Damage.WithModifier((sbyte)-targetStats.Armor.Value));
            sink.Queue(in damage);
        }
    }
}
