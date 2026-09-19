using BattleForDune.Core;
using BattleForDune.Core.Components;
using BattleForDune.Core.Components.Abilities;
using BattleForDune.Core.Components.Abilities.Attack;
using BattleForDune.Core.Components.Abilities.Damage;
using BattleForDune.Core.Components.Abilities.UseAbility;
using BattleForDune.Core.Components.Effects;
using BattleForDune.Core.Components.Effects.Types;
using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;
using BattleForDune.Core.Prototypes;
using BattleForDune.Core.Schema;
using BattleForDune.Core.Systems;

namespace BattleForDune
{
    public class Program
    {
        static void Main()
        {
            var effects =
                new IGenericEffect.IEffectPrototype[]
                {
                    new DamageOverTimeEffect.Prototype(new(0), DamageType.Heat, DamageVector.Indirect),
                    new UpdateStatEffect.Prototype(new(1), StatType.Initiative, new(0, 0, 0, 1))
                };
            var abilities =
                new Ability.Prototype[]
                {
                    new(
                        new(0),
                        [
                            new BasicUseAbilityHandler.Prototype(),
                            new BasicAttackHandler.Prototype(0, 1, DamageType.Physical, new(1, 6, 0, 0), 0, 0),
                            new CriticalMultiplierHandler.Prototype(2f),
                            new ChanceApplyEffectHandler.Prototype(new(0), 0, 3)
                        ])
                };
            var unitType =
                new UnitPrototype(
                    Key: new(0),
                    Name: LocalizationKey.None,
                    new StatBlockPrototype(
                        Attack: 1,
                        Defense: 1,
                        Armor: 0,
                        Shield: 0,
                        Initiative: 0,
                        Hitpoints: 10,
                        Morale: 10,
                        Threat: 0),
                    [new(1)],
                    [new(0)]);
            var random = new Random();
            var world = new ComponentWorld(
                new ConfigurationRegistry(abilities, effects, [], [unitType]),
                new((uint)(4 * random.Next() + 1), (uint)(2 * random.Next() + 1), random.Next()), 
                random);

            var processor = new LocalEventProcessor(world);
            var events = new EventPump<LocalEventProcessor>(processor);

            for (int i = 0; i < 50; ++i)
            {
                events.Queue(new AddUnitEvent(new(0), new(0), new(0), new()));
            }
            events.Flush();

            var initiative = new InitiativeSystem();
            initiative.Run(world, in events);
            foreach (var id in world.GetInitiative().AsSpan())
            {
                Console.WriteLine(id.Value);
            }
        }
    }
}
