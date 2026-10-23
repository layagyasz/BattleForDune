using System.Runtime.InteropServices;

namespace BattleForDune.Core.Events.Types
{
    [StructLayout(LayoutKind.Sequential)]
    public struct KillEvent(EntityId subjectId, byte abilityId, EntityId targetId, bool isPrimaryDamage)
        : IGameEvent
    {
        public readonly GameEventType Type => GameEventType.Kill;
        public bool IsCanceled { get; set; }
        public EntityId SubjectId { get; set; } = subjectId;
        public byte AbilityId = abilityId;
        public EntityId TargetId { get; set; } = targetId;
        public bool IsPrimaryDamage = isPrimaryDamage;
    }
}
