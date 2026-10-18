using BattleForDune.Controller;
using BattleForDune.Core;
using BattleForDune.Core.Components;
using BattleForDune.Core.Events;
using BattleForDune.Core.Schema;
using BattleForDune.Geometry;
using BattleForDune.Runners.Setup;
using BattleForDune.View;
using BattleForDune.View.Config;
using Cardamom.Graphics;
using Cardamom.Ui;
using Cardamom.Utils.Suppliers.Ids;
using Cardamom.Window;
using MathNet.Numerics.Random;
using OpenTK.Mathematics;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace BattleForDune.Runners
{
    public class TestBattleRunner(BattleSetup Setup) : IRunner
    {
        public void Run()
        {
            var monitor = Monitors.GetPrimaryMonitor();
            var window =
                new RenderWindow(
                    "Battle for Dune",
                    new Vector2i(monitor.HorizontalResolution, monitor.VerticalResolution),
                    false);
            var registry = ConfigurationRegistry.FromFile("resources/registry.json");

            var random = new Random();
            var map = CreateMap(new(64, 64), random);
            var components =
                new ComponentWorld(
                    new LinearCongruentialIdSupplier(0x9E3779B1, (uint)(2 * random.Next() + 1), random.Next()),
                    registry,
                    map,
                    random);

            var uiRegistry = ViewConfigurationRegistry.FromFile("resources/view/registry.json");

            var shader =
                new RenderShader.Builder()
                    .SetFragment("resources/shaders/default.frag")
                    .SetVertex("resources/shaders/default.vert")
                    .Build();
            var gridShader =
                new RenderShader.Builder()
                    .SetFragment("resources/shaders/default_no_tex.frag")
                    .SetVertex("resources/shaders/instanced.vert")
                    .Build();
            var textParticleShader =
                new RenderShader.Builder()
                    .SetFragment("resources/shaders/default.frag")
                    .SetVertex("resources/shaders/text_particle.vert")
                    .Build();
            var font = new Font("resources/fonts/ExpandedHerbertarian.ttf");
            var sceneFactory = new SceneFactory(uiRegistry, shader, gridShader, textParticleShader, font);

            var ui = new UiWindow(window);
            ui.Bind(new MouseListener());
            ui.Bind(
                new KeyboardListener(SimpleKeyMapper.Us, [Keys.Left, Keys.Right, Keys.Up, Keys.Down]));

            var uiProcessor = new UiEventProcessor(uiRegistry, components);
            var uiEventPump = new EventPump<UiEventProcessor>(uiProcessor);
            var controller = new BattleScreenController(uiEventPump, uiProcessor);
            var screen = new BattleScreen(controller, sceneFactory.CreateBattle(map, 128));
            ui.SetRoot(screen);

            var processor = new LocalEventProcessor(components, [uiEventPump]);
            var eventPump = new EventPump<LocalEventProcessor>(processor);
            var world = new LocalWorld(components, eventPump, Setup.TurnLimit, 1, random);
            Setup.Setup(map.Size, eventPump);

            var thread = new Thread(world.Resolve);
            thread.Start();
            ui.Start();
        }

        private static DenseHexGrid<Tile> CreateMap(Vector2i size, Random random)
        {
            var map = new DenseHexGrid<Tile>(size);
            for (int i = 0; i < map.Height; ++i)
            {
                for (int j = 0; j < map.Width; ++j)
                {
                    map.Set(
                        j,
                        i,
                        new Tile(
                            new((ushort)random.Next(24, 56)), new(random.NextBoolean(), (byte)random.Next(6))));
                }
            }
            return map;
        }
    }
}
