using BattleForDune.Core.Schema;
using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public struct DamageEvent(
        EntityId subjectId, 
        EntityId abilityId,
        EntityId targetId,
        bool isPrimary,
        bool isCritical, 
        DamageType damageType,
        StatType stat,
        Die die,
        byte reduction) : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.Damage;
        public bool IsCanceled { get; set; }
        public bool IsPrimary = isPrimary;
        public bool IsCritical = isCritical;
        public EntityId SubjectId = subjectId;
        public EntityId AbilityId = abilityId;
        public EntityId TargetId = targetId;
        public DamageType DamageType = damageType;
        public StatType Stat = stat;
        public Die Die = die;
        public byte Reduction = reduction;
        public byte Roll;
        public byte Damage;
    }
}
