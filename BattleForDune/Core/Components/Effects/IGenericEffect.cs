using BattleForDune.Core.Events;

namespace BattleForDune.Core.Components.Effects
{
    public interface IGenericEffect
    {
        ReadOnlySpan<GameEventType> SupportedEvents { get; }
        EntityId Id { get; }
    }
}
