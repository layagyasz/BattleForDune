using BattleForDune.Core.Components;

namespace BattleForDune.Core.Events
{
    public interface IGameEventHandler
    {
        void Intercept<T>(ref T @event, ComponentWorld world) where T : struct, IGameEvent;
        void Handle<T, TSink>(in T @event, ComponentWorld world, in TSink sink)
            where T : struct, IGameEvent
            where TSink : IGameEventSink;
    }
}
