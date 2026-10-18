using BattleForDune.Runners;
using BattleForDune.Runners.Setup;

namespace BattleForDune
{
    public class Program
    {
        static void Main()
        {
            var teamA = new TeamSetup(0, new(0), new(0), 0, 20, 4);
            var teamB = new TeamSetup(1, new(1), new(1), 10, 4, 4);
            var runner = new TestBattleRunner(new(60, teamA, teamB));
            runner.Run();
        }
    }
}
