using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public record struct ResetActionsEvent(
        bool IsCanceled, EntityId SubjectId, byte Attacks, byte Moves) : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.ResetActions;
        public readonly EntityId TargetId => EntityId.None;

        public static ResetActionsEvent Intransitive(EntityId subjectId, byte attacks, byte moves)
        {
            return new(false, subjectId, attacks, moves);
        }
    }
}
