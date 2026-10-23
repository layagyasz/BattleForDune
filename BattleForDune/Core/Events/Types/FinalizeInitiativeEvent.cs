using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public record struct FinalizeInitiativeEvent() : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.FinalizeInitiative;
        public readonly bool IsCanceled => false;
        public readonly EntityId SubjectId => EntityId.None;
        public readonly EntityId TargetId => EntityId.None;

        public static FinalizeInitiativeEvent Global() => new();
    }
}
