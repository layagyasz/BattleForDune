namespace BattleForDune.Core.Events.Types
{
    public record struct AddAbilityEvent(
        bool IsCanceled, EntityId SubjectId, EntityId AbilityId, RegistryKey AbilityKey) : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.AddAbility;
        public readonly EntityId TargetId => EntityId.None;

        public static AddAbilityEvent Intransitive(EntityId subjectId, RegistryKey abilityKey)
        {
            return new(false, subjectId, EntityId.None, abilityKey);
        }
    }
}
