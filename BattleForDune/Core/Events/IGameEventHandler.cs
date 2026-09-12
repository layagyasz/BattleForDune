using BattleForDune.Core.Components;

namespace BattleForDune.Core.Events
{
    public interface IGameEventHandler<T> where T : struct, IGameEvent
    {
        void Intercept(ref T @event, ComponentWorld world);
        void Handle<TSink>(in T @event, ComponentWorld world, in TSink sink) where TSink : IGameEventSink;
    }
}
