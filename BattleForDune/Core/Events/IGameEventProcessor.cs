using BattleForDune.Core.Events.Types;

namespace BattleForDune.Core.Events
{
    public interface IGameEventProcessor
    {
        void Process<TSink>(in AttackEvent attack, TSink sink) where TSink : IGameEventSink;
    }
}
