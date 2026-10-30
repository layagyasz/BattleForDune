namespace BattleForDune.Core.Events.Types
{
    public record struct RemoveAbilityEvent(bool IsCanceled, EntityId AbilityId) : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.RemoveAbility;
        public readonly EntityId SubjectId => EntityId.None;
        // TODO: Include owner id so passives can fire?
        public readonly EntityId TargetId => EntityId.None;

        public static RemoveAbilityEvent Intransitive(EntityId abilityId)
        {
            return new(false, abilityId);
        }
    }
}
