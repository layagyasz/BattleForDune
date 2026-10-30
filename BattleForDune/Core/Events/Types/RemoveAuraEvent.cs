using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public record struct RemoveAuraEvent(
        bool IsCanceled, EntityId SubjectId, byte AbilityId, EntityId AuraId) : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.RemoveAura;
        public readonly EntityId TargetId => EntityId.None;

        public static RemoveAuraEvent Intransitive(EntityId auraId)
        {
            return new(false, EntityId.None, 0, auraId);
        }

        public static RemoveAuraEvent Transitive(EntityId subjectId, byte abilityId, EntityId auraId)
        {
            return new(false, subjectId, abilityId, auraId);
        }
    }
}
