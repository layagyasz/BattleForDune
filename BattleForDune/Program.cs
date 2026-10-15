using BattleForDune.Runners;

namespace BattleForDune
{
    public class Program
    {
        static void Main()
        {
            var runner = new ProfileRunner(new(new(0), 10, 50), new(new(0), 10, 50), 50, 100);
            runner.Run();
        }
    }
}
