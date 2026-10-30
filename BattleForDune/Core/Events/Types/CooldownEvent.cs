namespace BattleForDune.Core.Events.Types
{
    public record struct CooldownEvent(bool IsCanceled, EntityId AbilityId) : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.Cooldown;
        public readonly EntityId SubjectId => EntityId.None;
        // TODO: Include owner id so passives can fire?
        public readonly EntityId TargetId => EntityId.None;

        public static CooldownEvent Intransitive(EntityId abilityId)
        {
            return new(false, abilityId);
        }
    }
}
