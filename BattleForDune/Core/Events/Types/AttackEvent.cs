using BattleForDune.Core.Schema;
using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public record struct AttackEvent(
        bool IsCanceled,
        bool IsCritical,
        EntityId SubjectId,
        byte AbilityId, 
        EntityId TargetId,
        AbilityVector Vector,
        sbyte Skill,
        sbyte CriticalRange,
        sbyte DefenseEvasion,
        sbyte DefenseArmor,
        sbyte DefenseShield,
        sbyte Roll) : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.Attack;

        public static AttackEvent Transitive(
            EntityId subjectId,
            byte abilityId,
            EntityId targetId,
            AbilityVector vector,
            sbyte skill,
            sbyte criticalRange,
            sbyte defenseEvasion,
            sbyte defenseArmor,
            sbyte defenseShield)
        {
            return new(
                false, 
                false, 
                subjectId,
                abilityId, 
                targetId, 
                vector, 
                skill,
                criticalRange, 
                defenseEvasion, 
                defenseArmor,
                defenseShield,
                0);
        }
    }
}
