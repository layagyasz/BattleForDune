using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public record struct LeaveAuraEvent(EntityId SubjectId, EntityId AuraId) : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.LeaveAura;
        public readonly bool IsCanceled => false;
        public readonly EntityId TargetId => EntityId.None;

        public static LeaveAuraEvent Intransitive(EntityId subjectId, EntityId auraId)
        {
            return new(subjectId, auraId);
        }
    }
}
