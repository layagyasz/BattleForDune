using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public record struct TerminateEvent(byte WinningTeam) : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.Terminate;
        public readonly bool IsCanceled => false;
        public readonly EntityId SubjectId => EntityId.None;
        public readonly EntityId TargetId => EntityId.None;

        public static TerminateEvent Global(byte winningTeam) => new(winningTeam);
    }
}
