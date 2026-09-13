using BattleForDune.Core;
using BattleForDune.Core.Components;
using BattleForDune.Core.Components.Effects;
using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;
using BattleForDune.Core.Prototypes;
using BattleForDune.Core.Prototypes.Abilities;
using BattleForDune.Core.Prototypes.Abilities.Attack;
using BattleForDune.Core.Prototypes.Abilities.Damage;
using BattleForDune.Core.Prototypes.Abilities.UseAbility;
using BattleForDune.Core.Schema;
using BattleForDune.Core.Systems;

namespace BattleForDune
{
    public class Program
    {
        static void Main()
        {
            var random = new Random();
            var world = new ComponentWorld(
                new((uint)(4 * random.Next() + 1), (uint)(2 * random.Next() + 1), random.Next()), random);

            var processor = new CombatGameSystem(world);
            var events = new EventPump<CombatGameSystem>(processor);

            var unitType =
                new UnitPrototype(
                    new StatBlockPrototype(
                        Attack: 1,
                        Defense: 1,
                        Armor: 0,
                        Shield: 0,
                        Initiative: 0,
                        Hitpoints: 10,
                        Morale: 10,
                        Threat: 0),
                    [
                        new BasicAttackAbilityPrototype(
                            [
                                new BasicUseAbilityPrototype(), 
                                new BasicAttackPrototype(0, 1, DamageType.Physical, new(1, 6, 0), 0, 0), 
                                new CriticalMultiplierPrototype(2f),
                                new ChanceApplyEffectPrototype(EffectType.Burn, 0, 3)
                            ])
                    ]);
            world.AddUnit(unitType, new());

            var unitId = new EntityId(world.GetUnitIds()[0]);
            foreach (var ability in world.GetAbilities(unitId))
            {
                events.Queue(new UseAbilityEvent(unitId, ability.Id, unitId));
                events.Flush();
            }
        }
    }
}
