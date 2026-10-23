using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public struct InitiativeEvent(EntityId subjectId, sbyte skill) : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.Initiative;
        public bool IsCanceled { get; set; }
        public EntityId SubjectId { get; set; } = subjectId;
        public readonly EntityId TargetId => EntityId.None;
        public sbyte Skill = skill;
        public byte Roll;
        public float Initiative;
    }
}
