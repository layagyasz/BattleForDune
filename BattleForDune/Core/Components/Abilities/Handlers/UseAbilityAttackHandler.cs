using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;
using BattleForDune.Core.Schema;

namespace BattleForDune.Core.Components.Abilities.Handlers
{
    public record class UseAbilityAttackHandler(
        AbilityVector Vector, sbyte Attack, sbyte CriticalRange, sbyte AntiArmor, sbyte AntiShield) 
        : IAbilityHandler<UseAbilityEvent>
    {
        public void Intercept(ref UseAbilityEvent @event, ComponentWorld world) { }

        public void Handle<TSink>(
            in UseAbilityEvent @event, ComponentWorld world, in TSink sink) where TSink : IGameEventSink
        {
            if (@event.IsCanceled)
            {
                return;
            }

            ref var stats = ref world.Stats.TryGet(@event.SubjectId, out bool isFound);
            if (!isFound)
            {
                return;
            }
            ref var targetStats = ref world.Stats.TryGet(@event.TargetId, out isFound);
            if (!isFound)
            {
                return;
            }

            var attack =
                AttackEvent.Transitive(
                    @event.SubjectId,
                    @event.AbilityId,
                    @event.TargetId,
                    Vector,
                    skill: (sbyte)(Attack + stats[StatType.Attack].Total),
                    defenseEvasion: targetStats[StatType.Evasion].Total,
                    defenseArmor: (sbyte)(targetStats[StatType.Armor].Total - AntiArmor),
                    defenseShield: (sbyte)(targetStats[StatType.Shield].Total - AntiShield),
                    criticalRange: CriticalRange);
            sink.Queue(in attack);
        }
    }
}
