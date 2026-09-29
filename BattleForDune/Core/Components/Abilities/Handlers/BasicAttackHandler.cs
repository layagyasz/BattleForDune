using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;
using BattleForDune.Core.Schema;
using static BattleForDune.Core.Components.Abilities.IGenericAbilityHandler;

namespace BattleForDune.Core.Components.Abilities.Handlers
{
    public record class BasicAttackHandler(
        DamageType DamageType,
        DamageVector DamageVector,
        Die Damage, 
        sbyte ArmorPiercing) 
        : IAbilityHandler<AttackEvent>
    {
        public record class Prototype(
            DamageType DamageType,
            DamageVector DamageVector,
            Die Damage,
            sbyte ArmorPiercing) : IHandlerPrototype
        {
            public IGenericAbilityHandler Materialize()
            {
                return new BasicAttackHandler(DamageType, DamageVector, Damage, ArmorPiercing);
            }
        }

        public void Intercept(ref AttackEvent @event, ComponentWorld world) { }

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
