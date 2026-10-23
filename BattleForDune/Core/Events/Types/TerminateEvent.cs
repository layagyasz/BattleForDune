using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public struct TerminateEvent(byte winningTeam) : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.Terminate;
        public bool IsCanceled { get; set; }
        public readonly EntityId SubjectId => EntityId.None;
        public readonly EntityId TargetId => EntityId.None;
        public byte WinningTeam = winningTeam;
    }
}
