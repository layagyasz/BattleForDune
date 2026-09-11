namespace BattleForDune.Core.Events.Types
{
    public struct UseAbilityEvent(EntityId abilityId) : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.UseAbility;
        public bool IsCanceled { get; set; }
        public EntityId AbilityId { get; set; } = abilityId;
        public EntityId InstanceId { get; set; }
    }
}
