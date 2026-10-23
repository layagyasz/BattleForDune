using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ResetActionsEvent(EntityId subjectId, byte attacks, byte moves) : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.ResetActions;
        public bool IsCanceled { get; set; }
        public EntityId SubjectId { get; set; } = subjectId;
        public byte Attacks = attacks;
        public readonly EntityId TargetId => EntityId.None;
        public byte Moves = moves;
    }
}
