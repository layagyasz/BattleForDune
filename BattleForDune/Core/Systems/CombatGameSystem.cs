using BattleForDune.Core.Events;
using BattleForDune.Core.Events.Types;

namespace BattleForDune.Core.Systems
{
    public class CombatGameSystem : IGameEventProcessor
    {
        public void Process<TSink>(in AttackEvent attack, TSink sink) where TSink : IGameEventSink
        {
            Console.WriteLine($"Process attack");
        }
    }
}
