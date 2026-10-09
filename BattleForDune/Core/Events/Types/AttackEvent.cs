using BattleForDune.Core.Schema;
using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public struct AttackEvent(
        EntityId subjectId,
        byte abilityId, 
        EntityId targetId,
        AbilityVector vector,
        sbyte skill,
        sbyte critRange,
        sbyte defenseEvasion,
        sbyte defenseArmor,
        sbyte defenseShield) : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.Attack;
        public bool IsCanceled { get; set; }
        public bool IsCritical;
        public EntityId SubjectId = subjectId;
        public byte AbilityId = abilityId;
        public EntityId TargetId = targetId;
        public AbilityVector Vector = vector;
        public sbyte Skill = skill;
        public sbyte CritRange = critRange;
        public sbyte DefenseEvasion = defenseEvasion;
        public sbyte DefenseArmor = defenseArmor;
        public sbyte DefenseShield = defenseShield;
        public byte Roll;
    }
}
