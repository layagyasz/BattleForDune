using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;
using BattleForDune.Core.Schema;

namespace BattleForDune.Core.Components.Effects.Handlers
{
    public record class DamageOutChanceAddEffectHandler(
        AbilityVector Vector, RegistryKey EffectKey, sbyte Difficulty, sbyte Duration)
        : IEffectHandler<DamageEvent>
    {
        public void Apply<TSink>(Effect parent, ComponentWorld world, in TSink sink)
            where TSink : IGameEventSink
        { }

        public void Remove<TSink>(Effect parent, ComponentWorld world, in TSink sink)
            where TSink : IGameEventSink
        { }

        public void Intercept(Effect parent, ref DamageEvent @event, ComponentWorld world) { }

        public void Handle<TSink>(Effect parent, in DamageEvent @event, ComponentWorld world, in TSink sink) 
            where TSink : IGameEventSink
        {
            if (@event.IsCanceled)
            {
                return;
            }
            if (!@event.IsPrimary)
            {
                return;
            }
            if (@event.SubjectId != parent.OwnerId)
            {
                return;
            }
            if (!Vector.HasFlag(@event.Vector))
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
