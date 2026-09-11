using BattleForDune.Core.Events.Types;

namespace BattleForDune.Core.Events
{
    public interface IGameEventProcessor
    {
        void Process<TSink>(ref UseAbilityEvent @event, TSink sink) where TSink : IGameEventSink;
        void Process<TSink>(ref AttackEvent @event, TSink sink) where TSink : IGameEventSink;
    }
}
