using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public record struct InitiativeEvent(
        bool IsCanceled, EntityId SubjectId, sbyte Skill, sbyte Roll, float Initiative) : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.Initiative;
        public readonly EntityId TargetId => EntityId.None;

        public static InitiativeEvent Intransitive(EntityId subjectId, sbyte skill)
        {
            return new(false, subjectId, skill, 0, 0);
        }
    }
}
