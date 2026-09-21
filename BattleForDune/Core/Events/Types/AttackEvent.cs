using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public struct AttackEvent(
        EntityId subjectId,
        byte abilityId, 
        EntityId targetId, 
        sbyte skill,
        byte critRange,
        sbyte difficulty,
        sbyte advantage) : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.Attack;
        public bool IsCanceled { get; set; }
        public bool IsCritical;
        public EntityId SubjectId = subjectId;
        public byte AbilityId = abilityId;
        public EntityId TargetId = targetId;
        public sbyte Skill = skill;
        public byte CritRange = critRange;
        public sbyte Difficulty = difficulty;
        public sbyte Advantage = advantage;
        public byte Roll;
    }
}
