using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public record struct TickEvent : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.Tick;
        public readonly bool IsCanceled => false;
        public readonly EntityId SubjectId => EntityId.None;
        public readonly EntityId TargetId => EntityId.None;

        public static TickEvent Global() => new();
    }
}
