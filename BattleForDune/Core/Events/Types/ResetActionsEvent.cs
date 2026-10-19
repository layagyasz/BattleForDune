using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ResetActionsEvent(EntityId subjectId, byte attacks, byte moves) : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.ResetActions;
        public bool IsCanceled { get; set; }
        public EntityId SubjectId = subjectId;
        public byte Attacks = attacks;
        public byte Moves = moves;
    }
}
