using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public struct BreakEvent(EntityId subjectId, byte abilityId, EntityId targetId) : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.Break;
        public bool IsCanceled { get; set; }
        public EntityId SubjectId = subjectId;
        public byte AbilityId = abilityId;
        public EntityId TargetId = targetId;
    }
}
