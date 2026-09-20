using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;
using static BattleForDune.Core.Components.Abilities.IGenericHandler;

namespace BattleForDune.Core.Components.Abilities.Damage
{
    public record class ChanceApplyEffectHandler(RegistryKey EffectKey, sbyte Difficulty, byte Duration)
        : IGameEventHandler<DamageEvent>, IGenericHandler
    {
        public record class Prototype(RegistryKey EffectKey, sbyte Difficulty, byte Duration)
            : IHandlerPrototype
        {
            public IGenericHandler Materialize()
            {
                return new ChanceApplyEffectHandler(EffectKey, Difficulty, Duration);
            }
        }

        public void Intercept(ref DamageEvent @event, ComponentWorld world) { }

        public void Handle<TSink>(
            in DamageEvent @event, ComponentWorld world, in TSink sink) where TSink : IGameEventSink
        {
            if (@event.IsCanceled)
            {
                return;
            }
            sink.Queue(
                new AddEffectEvent(
                    @event.SubjectId, 
                    @event.AbilityId,
                    @event.TargetId,
                    stacks: 1, 
                    power: 1,
                    Duration, 
                    EffectKey,
                    skill: 0,
                    Difficulty));
        }
    }
}
