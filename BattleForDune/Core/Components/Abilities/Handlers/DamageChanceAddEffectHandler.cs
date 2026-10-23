using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;
using BattleForDune.Core.Schema;

namespace BattleForDune.Core.Components.Abilities.Handlers
{
    public record class DamageChanceAddEffectHandler(
        RegistryKey EffectKey, AbilityVector Vector, sbyte Difficulty, sbyte Duration)
        : IAbilityHandler<DamageEvent>
    {
        public void Intercept(ref DamageEvent @event, ComponentWorld world) { }

        public void Handle<TSink>(
            in DamageEvent @event, ComponentWorld world, in TSink sink) where TSink : IGameEventSink
        {
            if (@event.IsCanceled || !@event.IsPrimary)
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
                    power: Math.Max((byte)1, @event.Die.Count),
                    Duration,
                    EffectKey, 
                    skill: 0, 
                    Difficulty));
        }
    }
}
