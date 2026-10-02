using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;
using BattleForDune.Core.Schema;

namespace BattleForDune.Core.Components.Abilities.Handlers
{
    public record class UseAbilityAttackHandler(sbyte Attack, byte CritRange, sbyte ShieldPiercing) 
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
                new AttackEvent(
                    @event.SubjectId,
                    @event.AbilityId,
                    @event.TargetId,
                    skill: (sbyte)(Attack + stats[StatType.Attack].Total),
                    difficulty: (sbyte)(10 + stats[StatType.Defense].Total 
                        + Math.Max(0, targetStats[StatType.Shield].Total - ShieldPiercing)),
                    critRange: CritRange,
                    advantage: (sbyte)(stats[StatType.Attack].Advantage 
                        - targetStats[StatType.Defense].Advantage));
            sink.Queue(in attack);
        }
    }
}
