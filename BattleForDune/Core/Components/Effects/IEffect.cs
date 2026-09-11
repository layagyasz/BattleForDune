using BattleForDune.Core.Events;

namespace BattleForDune.Core.Components.Effects
{
    public interface IEffect<T> : IGameEventHandler<T>, IGenericEffect where T : struct, IGameEvent { }
}
