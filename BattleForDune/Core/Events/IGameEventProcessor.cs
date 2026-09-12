using BattleForDune.Core.Events.Types;

namespace BattleForDune.Core.Events
{
    public interface IGameEventProcessor
    {
        void Process<TSink>(ref GameEvent @event, TSink sink) where TSink : IGameEventSink;
    }
}
