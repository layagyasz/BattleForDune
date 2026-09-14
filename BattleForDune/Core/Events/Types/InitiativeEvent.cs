using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public struct InitiativeEvent(EntityId subjectId, sbyte skill) : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.Initiative;
        public bool IsCanceled { get; set; }
        public EntityId SubjectId { get; set; } = subjectId;
        public sbyte Skill { get; set; } = skill;
        public byte Roll { get; set; }
        public sbyte Initiative { get; set; }
    }
}
