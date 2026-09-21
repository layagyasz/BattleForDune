using BattleForDune.Core;
using BattleForDune.Core.Components.Abilities;
using BattleForDune.Core.Components.Abilities.Handlers;
using BattleForDune.Core.Components.Effects;
using BattleForDune.Core.Components.Effects.Types;
using BattleForDune.Core.Prototypes;
using BattleForDune.Core.Schema;

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
                        TargetType.Enemy,
                        new ITargetComparator.DefaultComparator(),
                        [
                            new AttackUseAbilityHandler.Prototype(0, 1, 0),
                            new BasicAttackHandler.Prototype(
                                DamageType.Physical, DamageVector.Direct, new(1, 6, 0, 0), 0),
                            new CriticalMultiplierDamageHandler.Prototype(2f),
                            new ChanceApplyEffectDamageHandler.Prototype(new(0), 0, 3)
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
            var registry = new ConfigurationRegistry(abilities, effects, [], [unitType]);
            var world = new LocalWorld(registry, random);

            for (int i = 0; i < 50; ++i)
            {
                world.AddUnit(new(0), new(0), 0, new());
            }
            for (int i = 0; i < 50; ++i)
            {
                world.AddUnit(new(0), new(1), 1, new());
            }
            world.Step();
        }
    }
}
