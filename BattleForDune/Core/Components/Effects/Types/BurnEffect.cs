using BattleForDune.Core.Events;

namespace BattleForDune.Core.Components.Effects.Types
{
    public record class BurnEffect(
        EntityId Id, 
        EntityId OwnerId, 
        EntityId SourceId,
        EntityId AbilityId,
        byte Stacks,
        byte Power)
        : IGenericEffect
    {
        public EffectType EffectType => EffectType.Burn;
        public ReadOnlySpan<GameEventType> SupportedEvents => [];
    }
}
