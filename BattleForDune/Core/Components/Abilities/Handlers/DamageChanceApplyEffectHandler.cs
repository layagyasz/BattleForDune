using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;
using static BattleForDune.Core.Components.Abilities.IGenericAbilityHandler;

namespace BattleForDune.Core.Components.Abilities.Handlers
{
    public record class DamageChanceApplyEffectHandler(RegistryKey EffectKey, sbyte Difficulty, sbyte Duration)
        : IAbilityHandler<DamageEvent>
    {
        public record class Prototype(RegistryKey EffectKey, sbyte Difficulty, sbyte Duration)
            : IHandlerPrototype
        {
            public IGenericAbilityHandler Materialize()
            {
                return new DamageChanceApplyEffectHandler(EffectKey, Difficulty, Duration);
            }
        }

        public void Intercept(ref DamageEvent @event, ComponentWorld world) { }

        public void Handle<TSink>(
            in DamageEvent @event, ComponentWorld world, in TSink sink) where TSink : IGameEventSink
        {
            if (@event.IsCanceled || !@event.IsPrimary)
            {
                return;
            }
            sink.Queue(
                new AddEffectEvent(
                    @event.SubjectId, 
                    @event.AbilityId,
                    @event.TargetId,
                    stacks: 1, 
                    power: Math.Max((byte)1, @event.Die.Count),
                    Duration, 
                    EffectKey,
                    skill: 0,
                    Difficulty));
        }
    }
}
