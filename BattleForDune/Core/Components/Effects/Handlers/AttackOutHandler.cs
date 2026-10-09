using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;
using BattleForDune.Core.Schema;

namespace BattleForDune.Core.Components.Effects.Handlers
{
    public record class AttackOutHandler(
        AbilityVector Vector,
        BooleanFlag TargetShielded,
        sbyte AttackSkill,
        sbyte CritRange,
        sbyte DefenseEvasion, 
        sbyte DefenseArmor, 
        sbyte DefenseShield)
        : IEffectHandler<AttackEvent>
    {
        public void Apply<TSink>(Effect parent, ComponentWorld world, in TSink sink)
            where TSink : IGameEventSink
        { }

        public void Remove<TSink>(Effect parent, ComponentWorld world, in TSink sink)
            where TSink : IGameEventSink
        { }

        public void Intercept(Effect parent, ref AttackEvent @event, ComponentWorld world)
        {
            if (@event.SubjectId != parent.OwnerId)
            {
                return;
            }
            if (!Vector.HasFlag(@event.Vector))
            {
                return;
            }
            var targetStats = world.Stats.TryGet(@event.TargetId, out var isFound);
            if (!isFound || !TargetShielded.HasFlag(targetStats[StatType.Shield].Total > 0))
            {
                return;
            }

            @event.Skill += AttackSkill;
            @event.CritRange += CritRange;
            @event.DefenseEvasion += DefenseEvasion;
            @event.DefenseArmor += DefenseArmor;
            @event.DefenseShield += DefenseShield;
        }

        public void Handle<TSink>(Effect parent, in AttackEvent @event, ComponentWorld world, in TSink sink)
            where TSink : IGameEventSink
        { }
    }
}
