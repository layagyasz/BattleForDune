using BattleForDune.Runners;
using BattleForDune.Runners.Setup;

namespace BattleForDune
{
    public class Program
    {
        static void Main()
        {
            var teamA = new TeamSetup(0, new(0), new(0), 10, 50, 50);
            var teamB = new TeamSetup(1, new(1), new(3), 10, 50, 50);
            var runner = new ProfileRunner(new(60, teamA, teamB), 1000);
            runner.Run();
        }
    }
}
