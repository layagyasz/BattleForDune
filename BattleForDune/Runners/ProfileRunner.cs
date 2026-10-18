using BattleForDune.Core;
using BattleForDune.Core.Components;
using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;
using BattleForDune.Core.Schema;
using BattleForDune.Geometry;
using BattleForDune.Runners.Setup;
using Cardamom.Utils.Suppliers.Ids;
using System.Diagnostics;

namespace BattleForDune.Runners
{
    public record class ProfileRunner(BattleSetup Setup, int Rounds) : IRunner
    {
        private class TerminationCollector : IGameEventSink
        {
            public byte WinningTeam { get; private set; }

            public void Queue<T>(in T @event) where T : struct, IGameEvent
            {
                if (@event is TerminateEvent terminate)
                {
                    WinningTeam = terminate.WinningTeam;
                }
            }

            public void Flush() { }
        }


        public void Run()
        {
            var registry = ConfigurationRegistry.FromFile("resources/registry.json");
            var random = new Random();
            var map = new DenseHexGrid<Tile>(new(64, 64));

            var stopwatch = Stopwatch.StartNew();
            Span<int> wins = stackalloc int[2];
            for (int i=0; i<Rounds; ++i)
            {
                var winner = DoBattle(registry, map, random);
                if (winner < 2)
                {
                    wins[winner]++;
                }
            }
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
            Console.WriteLine($"{wins[0]} {wins[1]}");
        }

        private byte DoBattle(ConfigurationRegistry registry, DenseHexGrid<Tile> map, Random random)
        {
            var components =
                new ComponentWorld(
                    new LinearCongruentialIdSupplier(0x9E3779B1, (uint)(2 * random.Next() + 1), random.Next()),
                    registry,
                    map,
                    random);
            var collector = new TerminationCollector();
            var processor = new LocalEventProcessor(components, [collector]);
            var pump = new EventPump<LocalEventProcessor>(processor);
            var world = new LocalWorld(components, pump, Setup.TurnLimit, 0, random);
            Setup.Setup(map.Size, pump);
            world.Resolve();
            return collector.WinningTeam;
        }
    }
}
