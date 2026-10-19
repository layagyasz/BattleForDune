using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public struct UpdateActionsEvent(
        EntityId subjectId, byte abilityId, EntityId targetId, sbyte attacks, sbyte moves) : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.UpdateActions;
        public bool IsCanceled { get; set; }
        public EntityId SubjectId = subjectId;
        public byte AbilityId = abilityId;
        public EntityId TargetId = targetId;
        public sbyte Attacks = attacks;
        public sbyte Moves = moves;
    }
}
