using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public struct AttackEvent(
        EntityId subjectId,
        EntityId abilityId, 
        EntityId targetId, 
        sbyte skill,
        sbyte difficulty,
        sbyte advantage) : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.Attack;
        public bool IsCanceled { get; set; }
        public bool IsCritical { get; set; }
        public EntityId SubjectId { get; set; } = subjectId;
        public EntityId AbilityId { get; set; } = abilityId;
        public EntityId TargetId { get; set; } = targetId;
        public sbyte Skill { get; set; } = skill;
        public sbyte Difficulty { get; set; } = difficulty;
        public sbyte Advantage { get; set; } = advantage;
        public byte Roll { get; set; }
    }
}
