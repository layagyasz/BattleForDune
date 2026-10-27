using BattleForDune.Core.Schema;
using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public record struct DamageEvent(
        bool IsCanceled,
        bool IsPrimary,
        bool IsCritical,
        EntityId SubjectId,
        byte AbilityId,
        EntityId TargetId,
        DamageType DamageType,
        AbilityVector Vector,
        StatType Stat,
        Die Die,
        sbyte Structure,
        sbyte Roll,
        byte Damage) : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.Damage;

        public static DamageEvent Transitive(
            EntityId subjectId,
            byte abilityId,
            EntityId targetId,
            bool isPrimary,
            bool isCritical,
            DamageType damageType,
            AbilityVector vector,
            StatType stat,
            Die die,
            sbyte structure)
        {
            return new(
                false, 
                isPrimary,
                isCritical, 
                subjectId, 
                abilityId, 
                targetId, 
                damageType,
                vector,
                stat,
                die,
                structure,
                0, 
                0);
        }
    }
}
