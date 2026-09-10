using BattleForDune.Core.Events;

namespace BattleForDune.Core.Components.Effects
{
    public interface IEffect<T> : IGenericEffect where T : struct, IGameEvent
    {
        void Intercept(ref T @event);
        void Handle(in T @event, IGameEventSink sink);
    }
}
