using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;
using BattleForDune.Core.Schema;

namespace BattleForDune.Core.Components.Effects.Handlers
{
    public record class AddEffectHandler(
        EffectDirection Direction,
        AbilityVector Vector,
        sbyte Skill,
        sbyte Difficulty,
        sbyte Stacks,
        sbyte Power,
        sbyte Duration) : IEffectHandler<AddEffectEvent>
    {
        public void Apply<TSink>(Effect parent, ComponentWorld world, in TSink sink)
            where TSink : IGameEventSink
        { }

        public void Remove<TSink>(Effect parent, ComponentWorld world, in TSink sink)
            where TSink : IGameEventSink
        { }

        public void Intercept(Effect parent, ref AddEffectEvent @event, ComponentWorld world)
        {
            if (!IGenericEffectHandler.HasDirection(in @event, Direction, parent.OwnerId))
            {
                return;
            }
            if (!Vector.HasFlag(@event.Vector))
            {
                return;
            }

            @event.Skill += Skill;
            @event.Difficulty += Difficulty;
            @event.Stacks = (byte)Math.Max(0, @event.Stacks + Stacks);
            @event.Power = (byte)Math.Max(0, @event.Power + Power);
            if (@event.Duration >= 0)
            {
                @event.Duration = (sbyte)Math.Max(0, @event.Duration + Duration);
            }
        }

        public void Handle<TSink>(Effect parent, in AddEffectEvent @event, ComponentWorld world, in TSink sink)
            where TSink : IGameEventSink
        { }
    }
}
