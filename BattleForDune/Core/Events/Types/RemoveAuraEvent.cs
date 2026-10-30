using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public record struct RemoveAuraEvent(
        bool IsCanceled, EntityId SubjectId, EntityId AbilityId, EntityId AuraId) : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.RemoveAura;
        public readonly EntityId TargetId => EntityId.None;

        public static RemoveAuraEvent Intransitive(EntityId auraId)
        {
            return new(false, EntityId.None, EntityId.None, auraId);
        }

        public static RemoveAuraEvent Transitive(EntityId subjectId, EntityId abilityId, EntityId auraId)
        {
            return new(false, subjectId, abilityId, auraId);
        }
    }
}
