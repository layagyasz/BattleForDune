using BattleForDune.Core.Schema;
using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public struct DamageEvent(
        EntityId abilityId, EntityId targetId, DamageType damageType, byte damage) : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.Damage;
        public bool IsCanceled { get; set; }
        public EntityId AbilityId { get; set; } = abilityId;
        public EntityId TargetId { get; set; } = targetId;
        public DamageType DamageType { get; set; } = damageType;
        public byte Damage { get; set; } = damage;
    }
}
