using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public struct InitiativeEvent(EntityId subjectId, sbyte skill, sbyte advantage) : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.Initiative;
        public bool IsCanceled { get; set; }
        public EntityId SubjectId = subjectId;
        public sbyte Skill = skill;
        public sbyte Advantage = advantage;
        public byte Roll;
        public float Initiative;
    }
}
