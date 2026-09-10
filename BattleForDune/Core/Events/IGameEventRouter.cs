namespace BattleForDune.Core.Events
{
    public interface IGameEventRouter
    {
        void Intercept<T>(ref T @event) where T : struct, IGameEvent;
        void Handle<T>(in T @event, IGameEventSink sink) where T : struct, IGameEvent;
    }
}
