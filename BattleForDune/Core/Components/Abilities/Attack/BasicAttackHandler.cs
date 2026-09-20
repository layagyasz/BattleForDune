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
        DamageVector DamageVector,
        Die Damage, 
        sbyte ArmorPiercing,
        sbyte ShieldPiercing) 
        : IGameEventHandler<AttackEvent>, IGenericHandler
    {
        public record class Prototype(
            sbyte Attack,
            byte CritRange,
            DamageType DamageType,
            DamageVector DamageVector,
            Die Damage,
            sbyte ArmorPiercing,
            sbyte ShieldPiercing) : IHandlerPrototype
        {
            public IGenericHandler Materialize()
            {
                return new BasicAttackHandler(
                    Attack, CritRange, DamageType, DamageVector, Damage, ArmorPiercing, ShieldPiercing);
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

            @event.Skill = (sbyte)(Attack + stats[StatType.Attack].Total);
            @event.Difficulty = 
                (sbyte)(10 + stats[StatType.Defense].Total
                    + Math.Max(0, targetStats[StatType.Shield].Total - ShieldPiercing));
            @event.Advantage = 
                (sbyte)(stats[StatType.Attack].Advantage - targetStats[StatType.Defense].Advantage);
            if (@event.Roll > 20 - CritRange)
            {
                @event.IsCritical = true;
            }

            Console.WriteLine($"intercept {@event.Skill}");
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
                    DamageVector,
                    StatType.Hitpoints,
                    Damage,
                    (byte)Math.Max(0, stats[StatType.Armor].Value - ArmorPiercing));
            sink.Queue(in damage);
        }
    }
}
