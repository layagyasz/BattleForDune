using BattleForDune.Core.Components;
using BattleForDune.Schema;

namespace BattleForDune
{
    public class Program
    {
        static void Main()
        {
            var stats = new StatComponent();
            var testStats = new StatBlock(1, 2, 3, 4, 5, 6, 7, 9);
            stats.Add(new(1), testStats);

            Console.WriteLine(stats.TryGet(new(1), out var _));
        }
    }
}
