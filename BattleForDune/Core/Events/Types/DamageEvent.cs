using BattleForDune.Core.Schema;
using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public struct DamageEvent(
        EntityId subjectId, 
        byte abilityId,
        EntityId targetId,
        bool isPrimary,
        bool isCritical, 
        DamageType damageType,
        AbilityVector vector,
        StatType stat,
        Die die) : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.Damage;
        public bool IsCanceled { get; set; }
        public bool IsPrimary = isPrimary;
        public bool IsCritical = isCritical;
        public EntityId SubjectId { get; set; } = subjectId;
        public byte AbilityId = abilityId;
        public EntityId TargetId { get; set; } = targetId;
        public DamageType DamageType = damageType;
        public AbilityVector Vector = vector;
        public StatType Stat = stat;
        public Die Die = die;
        public byte Roll;
        public byte Damage;
    }
}
