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
            var abilities =
                new AbilityPrototype[]
                {
                    new(
                        new(0),
                        [
                            new BasicUseAbilityPrototype(),
                            new BasicAttackPrototype(0, 1, DamageType.Physical, new(1, 6, 0), 0, 0),
                            new CriticalMultiplierPrototype(2f),
                            new ChanceApplyEffectPrototype(EffectType.Burn, 0, 3)
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
                    [new(0)]);
            var random = new Random();
            var world = new ComponentWorld(
                new ConfigurationRegistry([], abilities, [unitType]),
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
