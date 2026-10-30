using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public record struct UpdateActionsEvent(
        bool IsCanceled,
        EntityId SubjectId,
        EntityId AbilityId, 
        EntityId TargetId,
        sbyte Attacks, 
        sbyte Moves) : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.UpdateActions;

        public static UpdateActionsEvent Transitive(
            EntityId subjectId, EntityId abilityId, EntityId targetId, sbyte attacks,  sbyte moves)
        {
            return new(false, subjectId, abilityId, targetId, attacks, moves);
        }
    }
}
