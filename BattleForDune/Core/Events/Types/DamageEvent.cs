using BattleForDune.Core.Schema;
using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public struct DamageEvent(
        EntityId subjectId, 
        EntityId abilityId,
        EntityId targetId, 
        bool isCritical, 
        DamageType damageType,
        Die die,
        sbyte armorPiercing,
        sbyte armor,
        sbyte shieldPiercing,
        sbyte shield) : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.Damage;
        public bool IsCanceled { get; set; }
        public bool IsCritical { get; set; } = isCritical;
        public EntityId SubjectId { get; set; } = subjectId;
        public EntityId AbilityId { get; set; } = abilityId;
        public EntityId TargetId { get; set; } = targetId;
        public DamageType DamageType { get; set; } = damageType;
        public Die Die { get; set; } = die;
        public sbyte ArmorPiercing { get; set; } = armorPiercing;
        public sbyte Armor { get; set; } = armor;
        public sbyte ShieldPiercing { get; set; } = shieldPiercing;
        public sbyte Shield { get; set; } = shield;
        public byte Roll { get; set; }
        public byte Damage { get; set; }
    }
}
