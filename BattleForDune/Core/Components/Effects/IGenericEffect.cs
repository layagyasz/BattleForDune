using BattleForDune.Core.Events;

namespace BattleForDune.Core.Components.Effects
{
    public interface IGenericEffect
    {
        ReadOnlySpan<GameEventType> SupportedEvents { get; }
        EffectType EffectType { get; }
        EntityId Id { get; }
        EntityId OwnerId { get; }
        EntityId SourceId { get; }
        EntityId AbilityId { get; }
        byte Stacks { get; }
        byte Power { get; }
    }
}
