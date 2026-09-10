namespace BattleForDune.Core.Events
{
    public interface IGameEventSink
    {
        void Queue<T>(in T @event) where T : struct, IGameEvent;
        void Flush();
    }
}
