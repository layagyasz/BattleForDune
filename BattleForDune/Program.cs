using BattleForDune.Controller;
using BattleForDune.Core;
using BattleForDune.Core.Components;
using BattleForDune.Core.Components.Abilities;
using BattleForDune.Core.Components.Abilities.Handlers;
using BattleForDune.Core.Components.Effects;
using BattleForDune.Core.Components.Effects.Types;
using BattleForDune.Core.Events;
using BattleForDune.Core.Prototypes;
using BattleForDune.Core.Schema;
using BattleForDune.View;
using Cardamom.Graphics;
using Cardamom.Ui;
using Cardamom.Utils.Suppliers.Ids;
using Cardamom.Window;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Diagnostics;

namespace BattleForDune
{
    public class Program
    {
        static void Main()
        {
            var monitor = Monitors.GetPrimaryMonitor();
            var window =
                new RenderWindow(
                    "Battle for Dune",
                    new Vector2i(monitor.HorizontalResolution, monitor.VerticalResolution),
                    false);

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
            var registry = new ConfigurationRegistry(abilities, effects, [], [unitType]);

            var uiRegistry = new ViewConfigurationRegistry(["resources/textures/units/hark_light.png"]);

            var shader = 
                new RenderShader.Builder()
                    .SetFragment("resources/shaders/default.frag")
                    .SetVertex("resources/shaders/default.vert")
                    .Build();
            var font = new Font("resources/fonts/ExpandedHerbertarian.ttf");
            var sceneFactory = new SceneFactory(uiRegistry, shader, font);

            var ui = new UiWindow(window);
            ui.Bind(new MouseListener());
            ui.Bind(
                new KeyboardListener(SimpleKeyMapper.Us, [Keys.Left, Keys.Right, Keys.Up, Keys.Down]));

            var uiProcessor = new UiEventProcessor();
            var uiEventPump = new EventPump<UiEventProcessor>(uiProcessor);
            var controller = new BattleScreenController(uiEventPump, uiProcessor);
            var screen = new BattleScreen(controller, sceneFactory.CreateBattle(128));
            ui.SetRoot(screen);

            var random = new Random();
            var components =
                new ComponentWorld(
                    registry,
                    new LinearCongruentialIdSupplier(0x9E3779B1, (uint)(2 * random.Next() + 1), random.Next()),
                    random);
            var processor = new LocalEventProcessor(components, [uiEventPump]);
            var world = new LocalWorld(components, new(processor), random);

            var thread = new Thread(TestThread(world, random));
            thread.Start();
            ui.Start();
        }

        private static ParameterizedThreadStart TestThread(LocalWorld world, Random random)
        {
            return _ =>
            {
                for (int i = 0; i < 5; ++i)
                {
                    world.AddUnit(new(0), new(0), 0, new(random.Next(64), random.Next(64)));
                    Thread.Sleep(100);
                }
                for (int i = 0; i < 50; ++i)
                {
                    world.AddUnit(new(0), new(1), 1, new(random.Next(64), random.Next(64)));
                    Thread.Sleep(100);
                }
                world.Step();
            };
        }
    }
}
