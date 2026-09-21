using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;
using BattleForDune.Core.Schema;
using static BattleForDune.Core.Components.Abilities.IGenericHandler;

namespace BattleForDune.Core.Components.Abilities.UseAbility
{
    public class UseAttackAbilityHandler(sbyte Attack, byte CritRange, sbyte ShieldPiercing) 
        : IGameEventHandler<UseAbilityEvent>, IGenericHandler
    {
        public record class Prototype(sbyte Attack, byte CritRange, sbyte ShieldPiercing) : IHandlerPrototype
        {
            public IGenericHandler Materialize()
            {
                return new UseAttackAbilityHandler(Attack, CritRange, ShieldPiercing);
            }
        }

        public void Intercept(ref UseAbilityEvent @event, ComponentWorld world) { }

        public void Handle<TSink>(
            in UseAbilityEvent @event, ComponentWorld world, in TSink sink) where TSink : IGameEventSink
        {
            if (@event.IsCanceled)
            {
                return;
            }

            ref var stats = ref world.GetStats().TryGet(@event.SubjectId, out bool isFound);
            if (!isFound)
            {
                return;
            }
            ref var targetStats = ref world.GetStats().TryGet(@event.TargetId, out isFound);
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
