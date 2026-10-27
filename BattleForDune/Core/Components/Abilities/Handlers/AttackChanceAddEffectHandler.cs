using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;
using BattleForDune.Core.Schema;

namespace BattleForDune.Core.Components.Abilities.Handlers
{
    public record class AttackChanceAddEffectHandler(
        RegistryKey EffectKey, AbilityVector Vector, sbyte Difficulty, sbyte Duration)
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
            sink.Queue(
                AddEffectEvent.Transitive(
                    @event.SubjectId,
                    @event.AbilityId,
                    @event.TargetId,
                    Vector,
                    stacks: 1,
                    power: 1,
                    Duration,
                    EffectKey,
                    skill: 0,
                    Difficulty));
        }
    }
}
