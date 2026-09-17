using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;
using BattleForDune.Core.Schema;
using static BattleForDune.Core.Components.Abilities.IGenericHandler;

namespace BattleForDune.Core.Components.Abilities.Attack
{
    public record class BasicAttackHandler(
        sbyte Attack,
        byte CritRange, 
        DamageType DamageType, 
        Die Damage, 
        sbyte ArmorPiercing,
        sbyte ShieldPiercing) 
        : IGameEventHandler<AttackEvent>, IGenericHandler
    {
        public record class Prototype(
            sbyte Attack,
            byte CritRange,
            DamageType DamageType,
            Die Damage,
            sbyte ArmorPiercing,
            sbyte ShieldPiercing) : IHandlerPrototype
        {
            public IGenericHandler Materialize()
            {
                return new BasicAttackHandler(
                    Attack, CritRange, DamageType, Damage, ArmorPiercing, ShieldPiercing);
            }
        }

        public void Intercept(ref AttackEvent @event, ComponentWorld world)
        {
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

            @event.Skill = (sbyte)(Attack + stats.Get(StatType.Attack));
            @event.Difficulty = (sbyte)(10 + stats.Get(StatType.Defense));
            if (@event.Roll > 20 - CritRange)
            {
                @event.IsCritical = true;
            }
        }

        public void Handle<TSink>(
            in AttackEvent @event, ComponentWorld world, in TSink sink) where TSink : IGameEventSink
        {
            if (@event.IsCanceled)
            {
                return;
            }
            ref var stats = ref world.GetStats().TryGet(@event.TargetId, out var isFound);
            if (!isFound)
            {
                return;
            }
            var damage =
                new DamageEvent(
                    @event.SubjectId,
                    @event.AbilityId,
                    @event.TargetId,
                    isPrimary: true,
                    @event.IsCritical,
                    DamageType,
                    Damage,
                    ArmorPiercing,
                    stats.Get(StatType.Armor),
                    ShieldPiercing,
                    stats.Get(StatType.Shield));
            sink.Queue(in damage);
        }
    }
}
