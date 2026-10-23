using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public record struct BreakEvent(
        bool IsCanceled, EntityId SubjectId, byte AbilityId, EntityId TargetId) : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.Break;

        public static BreakEvent Transitive(EntityId SubjectId, byte AbilityId, EntityId TargetId)
        {
            return new(false, SubjectId, AbilityId, TargetId);
        }
    }
}
