namespace BattleForDune.Core.Events
{
    public interface IGameEventHandler<T> where T : struct, IGameEvent
    {
        void Intercept(ref T @event);
        void Handle<TSink>(in T @event, in TSink sink) where TSink : IGameEventSink;
    }
}
