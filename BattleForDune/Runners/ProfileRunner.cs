using BattleForDune.Core;
using BattleForDune.Core.Components;
using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;
using BattleForDune.Core.Schema;
using BattleForDune.Geometry;
using Cardamom.Utils.Suppliers.Ids;
using System.Diagnostics;

namespace BattleForDune.Runners
{
    public record class ProfileRunner : IRunner
    {
        private const byte TURN_LIMIT = 60;

        public record struct TeamSetup(RegistryKey Unit, byte Level, int Count);

        private readonly TeamSetup _teamA;
        private readonly TeamSetup _teamB;
        private readonly int _width;
        private readonly int _rounds;

        public ProfileRunner(TeamSetup teamA, TeamSetup teamB, int width, int rounds)
        {
            _teamA = teamA;
            _teamB = teamB;
            _width = width;
            _rounds = rounds;
        }

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
            for (int i=0; i<_rounds; ++i)
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
            var world = new LocalWorld(components, new(processor), TURN_LIMIT, 0, random);

            for (int i = 0; i < _teamA.Count; ++i)
            {
                world.AddUnit(
                    _teamA.Unit, 
                    _teamA.Level,
                    new(0), 
                    0,
                    Geometry.Projection.OffsetToAxial(new((64 - _width) / 2 + (i % _width), 31 - i / _width)));
            }
            for (int i = 0; i < _teamB.Count; ++i)
            {
                world.AddUnit(
                    _teamB.Unit, 
                    _teamB.Level, 
                    new(1),
                    1, 
                    Geometry.Projection.OffsetToAxial(new((64 - _width) / 2 + (i % _width), 32 + i / _width)));
            }
            world.Resolve();
            return collector.WinningTeam;
        }
    }
}
