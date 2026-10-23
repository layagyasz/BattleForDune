using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public struct TickEvent : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.Tick;
        public bool IsCanceled { get; set; }
        public readonly EntityId SubjectId => EntityId.None;
        public readonly EntityId TargetId => EntityId.None;
    }
}
